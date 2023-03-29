using gip.core.datamodel;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'ProdOrderPartslistExpand'}de{'ProdOrderPartslistExpand'}", Global.ACKinds.TACClass, Global.ACStorableTypes.NotStorable, false, true)]
    public class ProdOrderPartslistExpand : ExpandBase
    {

        #region ctor's

        public ProdOrderPartslistExpand(ProdOrderPartslist prodOrderPartslist, int index = 1, double treeQuantityRatio = 1, ExpandBase parent = null)
           : base(prodOrderPartslist, index, treeQuantityRatio, parent)
        {
        }

        #endregion


        #region Properties

        public ProdOrderPartslist ProdOrderPartslist
        {
            get
            {
                return Item as ProdOrderPartslist;
            }
        }

        #region Properties -> IACContainerWithItems


        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public override string ACCaption
        {
            get
            {
                Partslist partslist = ProdOrderPartslist.Partslist;
                if (partslist == null) return null;
                string name = partslist.PartslistNo + "-" + partslist.PartslistName;
                name += " [" + partslist.Material.MaterialNo + " - " + partslist.Material.MaterialName1 + "]";
                return name;
            }
        }

        #endregion

        #endregion

        #region Methods

        public override void LoadTree()
        {
            List<ProdOrderPartslist> sourcePls =
                ProdOrderPartslist
                .ProdOrderPartslistPos_ProdOrderPartslist
                .Where(c => c.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot && c.SourceProdOrderPartslist != null)
                .OrderBy(c => c.Sequence)
                .Select(c => c.SourceProdOrderPartslist)
                .ToList();
            int i = 1;
            foreach (ProdOrderPartslist sourcePl in sourcePls)
            {
                ProdOrderPartslistExpand childExpand = new ProdOrderPartslistExpand(sourcePl, i, 1, this);
                Children.Add(childExpand);
                childExpand.IsChecked = childExpand.Parent != null ? childExpand.Parent.IsChecked : true;
                i++;
            }
        }

        #endregion
    }
}
