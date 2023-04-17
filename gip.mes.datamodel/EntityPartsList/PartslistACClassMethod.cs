using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Partslist 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'PartslistACClassMethod'}de{'PartslistACClassMethod'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPartslist")]
    [ACPropertyEntity(1, MaterialWFACClassMethod.ClassName, "en{'MaterialWFACClassMethod'}de{'MaterialWFACClassMethod'}", Const.ContextDatabase + "\\" + MaterialWFACClassMethod.ClassName, "", true)]
    [ACPropertyEntity(2, Partslist.ClassName, "en{'Bill of Materials'}de{'Stückliste'}", Const.ContextDatabase + "\\" + Partslist.ClassName, "", true)]
    [ACPropertyEntity(3, "UsedInPhaseIndex", "en{'Valid for Phase'}de{'Gültig für Phase'}", typeof(PartslistACClassMethod.Phase), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + PartslistACClassMethod.ClassName, "en{'MaterialWF-Workflow-Mapping'}de{'MaterialWF-Workflow-Mapping'}", typeof(PartslistACClassMethod), PartslistACClassMethod.ClassName, "", "PartslistACClassMethodID")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<PartslistACClassMethod>) })]
    public partial class PartslistACClassMethod
    {
        public const string ClassName = "PartslistACClassMethod";
        public const string NoColumnName = "LotNo";
        public const string FormatNewNo = "PLAC{0}";

        #region New / Delete
        public static PartslistACClassMethod NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            PartslistACClassMethod entity = new PartslistACClassMethod();
            entity.PartslistACClassMethodID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is Partslist)
                entity.Partslist = parentACObject as Partslist;
            entity.UsedInPhase = Phase.Default;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region enums
        public enum Phase : short
        {
            Default = 0,
            Start = 1,
            End = 2
        }
        #endregion

        #region IACConfigStore

        public string ACConfigKeyACUrl
        {
            get
            {
                return null; // GetKey();
            }
        }

        static public string KeyACIdentifier
        {
            get
            {
                return "Partslist\\ACIdentifier,MaterialWFACClassMethod\\ACIdentifier";
            }
        }

        public string GetKey()
        {
            return this.Partslist.GetACUrl() + ACUrlHelper.Delimiter_Relationship + this.MaterialWFACClassMethod.GetACUrl();
        }

        #endregion

        #region convention members

        public override string ToString()
        {
            if (this.MaterialWFACClassMethod == null || this.Partslist == null)
                return null;
            return this.Partslist.ACIdentifier + "<=>" + this.MaterialWFACClassMethod.ACIdentifier;
        }

        #endregion

        #region Partial Properties
        public Phase UsedInPhase
        {
            get
            {
                return (Phase)this.UsedInPhaseIndex;
            }
            set
            {
                UsedInPhaseIndex = (Int16)value;
            }
        }        
        #endregion

    }
}
