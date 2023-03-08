using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    /// <summary>
    /// 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'PartslistExpand'}de{'PartslistExpand'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    public class PartslistExpand : ExpandBase
    {

        #region ctors

        public PartslistExpand(Partslist partsList, int index = 1, double treeQuantityRatio = 1, ExpandBase parent = null)
            : base(partsList, index, treeQuantityRatio, parent)
        {
            if (Partslist != null
                && Partslist.EntityState == System.Data.EntityState.Unchanged
                && Partslist.PartslistPos_Partslist != null
                && Partslist.PartslistPos_Partslist.IsLoaded)
            {
                Partslist.PartslistPos_Partslist.AutoLoad();
                Partslist.PartslistPos_Partslist.AutoRefresh();
            }
            LoadDisplayProperties(partsList, TreeQuantityRatio);
        }

        public PartslistExpand(Partslist partsList, PartslistPos position, int index = 1, ExpandBase parent = null)
            : base(partsList, index, 1, parent)
        {
            if (partsList.TargetQuantityUOM != 0)
                TreeQuantityRatio = position.TargetQuantityUOM / partsList.TargetQuantityUOM;
            if (parent.TreeQuantityRatio > 0)
            {
                TreeQuantityRatio = parent.TreeQuantityRatio * TreeQuantityRatio;
            }
            LoadDisplayProperties(partsList, TreeQuantityRatio);
        }

        #endregion

        #region Properties

        #region Properties -> Partslist

        public Partslist Partslist
        {
            get
            {
                return Item as Partslist;
            }
        }


        [ACPropertyInfo(101, "PartslistNo", "en{'No'}de{'Nr'}")]
        public string PartslistNo { get; set; }

        [ACPropertyInfo(102, "PartslistNo", "en{'No'}de{'Nr'}")]
        public string PartslistVersion { get; set; }

        [ACPropertyInfo(103, "PartslistName", "en{'Bill of material name'}de{'Stückliste Name'}")]
        public string PartslistName { get; set; }

        [ACPropertyInfo(105, "TargetQuantity", "en{'Required quantity'}de{'Bedarfsmenge'}")]
        public double TargetQuantity { get; set; }

        [ACPropertyInfo(106, "TargetQuantityUOM", "en{'Required quantity (UOM)'}de{'Bedarfsmenge (UOM)'}")]
        public double TargetQuantityUOM { get; set; }

        [ACPropertyInfo(107, "MDUnit", "en{'MDUnit'}de{'MDUnit'}")]
        public MDUnit MDUnit { get; set; }

        #endregion

        #region Properties -> IACContainerWithItems


        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public override string ACCaption
        {

            get
            {
                if (Partslist == null) return null;
                string name = Partslist.PartslistNo + "-" + Partslist.PartslistName;
                name += " [" + Partslist.Material?.MaterialNo + " - " + Partslist.Material?.MaterialName1 + "]";
                return name;
            }
        }

        #endregion

        #endregion

        #region Tree manipulation methods

        private void CheckIsPartslistPresent(PartslistExpand item, Guid partslistID, ref bool isPartslistPresent)
        {
            isPartslistPresent = isPartslistPresent || item.Partslist.PartslistID == partslistID;
            if(isPartslistPresent)
                return;
            if (item.Children != null)
                foreach (PartslistExpand childItem in item.Children)
                    CheckIsPartslistPresent(childItem, partslistID, ref isPartslistPresent);
        }


        /// <summary>
        /// Load partslist reference tree
        /// Load all materials they can be possible product of 
        /// another partslist
        /// </summary>
        public override void LoadTree()
        {
            bool isPartslistPresent = false;
            int i = 1;
            if (   Partslist.EntityState == System.Data.EntityState.Added
                || Partslist.EntityState == System.Data.EntityState.Detached
                || Partslist.EntityState == System.Data.EntityState.Deleted
                || Partslist.PartslistPos_Partslist == null)
            {
                return;
            }
            var posItems =
                Partslist
                .PartslistPos_Partslist
                .Where(x =>
                        x.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot
                        && x.AlternativePartslistPosID == null
                        && !(x.ExplosionOff ?? false))
                .ToList();
            foreach (PartslistPos position in posItems)
            {
                PartslistExpand childExpand = null;
                if (position.ParentPartslist != null)
                {
                    if (
                        position.ParentPartslist.IsEnabled
                        && (position.ParentPartslist.IsInEnabledPeriod ?? false)
                        && position.ParentPartslist.DeleteDate == null)
                    {
                        isPartslistPresent = false;
                        CheckIsPartslistPresent((PartslistExpand)this.Root, position.ParentPartslist.PartslistID, ref isPartslistPresent);

                        if (!isPartslistPresent)
                        {
                            childExpand = new PartslistExpand(position.ParentPartslist, position, i, this);
                            childExpand.IsChecked = childExpand.Parent != null ? childExpand.Parent.IsChecked : true;
                            Children.Add(childExpand);
                            i++;
                        }
                    }
                }
                else
                {
                    if (   position.Material != null
                        && position.Material.Partslist_Material != null)
                    {
                        if (position.Material.Partslist_Material.IsLoaded)
                        {
                            position.Material.Partslist_Material.AutoLoad();
                            position.Material.Partslist_Material.AutoRefresh();
                        }
                        List<Partslist> positionPartslist =
                            position
                            .Material
                            .Partslist_Material
                            .Where(pl => pl.IsEnabled && (pl.IsInEnabledPeriod ?? false) && pl.DeleteDate == null)
                            .OrderByDescending(c => c.PartslistVersion)
                            .ToList();
                        int localI = 0;
                        foreach (Partslist partslistForPosition in positionPartslist)
                        {
                            isPartslistPresent = false;
                            CheckIsPartslistPresent((PartslistExpand)this.Root, partslistForPosition.PartslistID, ref isPartslistPresent);

                            if (!isPartslistPresent)
                            {
                                childExpand = new PartslistExpand(partslistForPosition, position, i, this);
                                childExpand.IsChecked = (childExpand.Parent != null ? childExpand.Parent.IsChecked : true) && localI <= 1;
                                Children.Add(childExpand);
                                i++;
                                localI++;
                            }
                        }
                    }
                }
            }

            foreach(ExpandBase item in Children)
            {
                item.LoadTree();
            }
        }


        public void LoadDisplayProperties(Partslist partslist, double treeQuantityRatio)
        {
            PartslistNo = partslist.PartslistNo;
            PartslistName = partslist.PartslistName;
            PartslistVersion = partslist.PartslistVersion;
            MDUnit = partslist.MDUnit;
            TargetQuantityUOM = partslist.TargetQuantityUOM * treeQuantityRatio;
            if (partslist.MDUnitID.HasValue)
                TargetQuantity = partslist.Material.ConvertQuantity(TargetQuantityUOM, partslist.Material.BaseMDUnit, partslist.MDUnit);
        }
        #endregion

    }
}
