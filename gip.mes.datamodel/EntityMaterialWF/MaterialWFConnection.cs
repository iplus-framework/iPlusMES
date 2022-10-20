using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'MaterialWFConection'}de{'MaterialWFConection'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaterialWF")]
    [ACPropertyEntity(1, Material.ClassName, "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(2, "MaterialWFACClassMethod", "en{'MateriaWF-ACClassMethod'}de{'MateriaWF-ACClassMethod'}", Const.ContextDatabase + "\\" + MaterialWFACClassMethod.ClassName, "", true)]
    [ACPropertyEntity(3, gip.core.datamodel.ACClassWF.ClassName, "en{'To Workflownode'}de{'Workflownode'}", Const.ContextDatabase + "\\" + gip.core.datamodel.ACClassWF.ClassName, "", true)]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaterialWFConnection>) })]
    public partial class MaterialWFConnection : IACObjectEntity
    {
        public const string ClassName = "MaterialWFConnection";

        public static MaterialWFConnection NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaterialWFConnection entity = new MaterialWFConnection();
            entity.MaterialWFConnectionID = Guid.NewGuid();
            MaterialWFACClassMethod method = parentACObject as MaterialWFACClassMethod;
            if (method != null)
            {
                entity.MaterialWFACClassMethod = method;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #region IACUrl Member
        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                if (Material != null && ACClassWF != null)
                {
                    return MaterialWFACClassMethod.ACCaption + " " + Material.MaterialNo + " " + ACClassWF.ACIdentifier;
                }
                return MaterialWFACClassMethod.ACCaption;
            }
        }

        /// <summary>
        /// Returns MaterialWFACClassMethod
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to MaterialWFACClassMethod</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return MaterialWFACClassMethod;
            }
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            base.EntityCheckAdded(user, context);
            return null;
        }

        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence";
            }
        }
        #endregion


        #region convetion implementation

        public override string ToString()
        {
            return string.Format("MaterialWFConnection MW:{0} {1} <=> {2}",
                MaterialWFACClassMethod != null ? MaterialWFACClassMethod.ACCaption : "-", Material != null ? Material.MaterialNo : "-",
                ACClassWF != null ? ACClassWF.ACIdentifier : "-");
        }
        #endregion

    }
}
