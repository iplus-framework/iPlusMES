using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'MaterialWFRelation'}de{'MaterialWFRelation'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaterialWF")]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Reihenfolge'}","", "", true)]
    [ACPropertyEntity(2, "SourceMaterial", "en{'From Material'}de{'Von Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "TargetMaterial", "en{'To Material'}de{'Nach Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MaterialWFRelation.ClassName, "en{'MaterialWFRelation'}de{'MaterialWFRelation'}", typeof(MaterialWFRelation), MaterialWFRelation.ClassName, "", "MaterialWFRelationID")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaterialWFRelation>) })]
    public partial class MaterialWFRelation : IACWorkflowEdge
    {
        public const string ClassName = "MaterialWFRelation";

        public static MaterialWFRelation NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaterialWFRelation entity = new MaterialWFRelation();
            entity.MaterialWFRelationID = Guid.NewGuid();
            if (parentACObject is MaterialWF)
                entity.MaterialWF = parentACObject as MaterialWF;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }


        #region IACUrl member

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "TargetMaterial\\MaterialNo,SourceMaterial\\MaterialNo,Sequence";
            }
        }

        #endregion


        #region convetion implementation

        public override string ToString()
        {
            return string.Format("MaterialWFRelation #{0} MW:{1} {2} => {3}",
                Sequence, MaterialWF != null ? MaterialWF.MaterialWFNo : "-", SourceMaterial != null ? SourceMaterial.ToString() : "-",
                TargetMaterial != null ? TargetMaterial.ToString() : "-");
        }
        #endregion

        #region IACWorkflowEdge members

        /// <summary>
        /// Reference to the From-Node (Source)
        /// </summary>
        /// <value>Reference to the From-Node (Source)</value>
        [NotMapped]
        public IACWorkflowNode FromWFNode
        {
            get
            {
                return SourceMaterial;
            }
            set
            {
                SourceMaterial = value as Material;
            }
        }

        /// <summary>
        /// Reference to the To-Node (Destination)
        /// </summary>
        /// <value>Reference to the To-Node (Destination)</value>
        [NotMapped]
        public IACWorkflowNode ToWFNode
        {
            get
            {
                return TargetMaterial;
            }
            set
            {
                TargetMaterial = value as Material;
            }
        }

        [NotMapped]
        public Global.ConnectionTypes ConnectionType
        {
            get
            {
                return Global.ConnectionTypes.Connection;
            }
            set
            {
            }
        }

        /// <summary>
        /// ACIdentifier of the FromWFNode
        /// </summary>
        /// <value>ACIdentifier of the FromWFNode</value>
        [NotMapped]
        public string SourceACName
        {
            get
            {
                return SourceMaterial != null ? SourceMaterial.ACIdentifier : "";
            }
        }

        [NotMapped]
        core.datamodel.ACClassProperty _SourceACClassProperty;
        /// <summary>
        /// Connection-Point of the FromWFNode
        /// </summary>
        /// <value>Connection-Point of the FromWFNode</value>
        [NotMapped]
        public core.datamodel.ACClassProperty SourceACClassProperty
        {
            get
            {
                return _SourceACClassProperty;
            }
            set
            {
                _SourceACClassProperty = value;
            }
        }


        /// <summary>
        /// WPF-x:Name of the FromWFNode + \\ + SourceACClassProperty.ACIdentifier
        /// for indentify the connector in the Logical-Tree
        /// </summary>
        /// <value>WPF-x:Name of the FromWFNode + \\ + SourceACClassProperty.ACIdentifier</value>
        [NotMapped]
        public string SourceACConnector
        {
            get 
            {
                return MaterialWF.MaterialWFNo + "\\" + FromWFNode.XName + "\\" + SourceACClassProperty.ACIdentifier;
            }
        }

        /// <summary>
        /// ACIdentifier of the ToWFNode
        /// </summary>
        /// <value>ACIdentifier of the ToWFNode</value>
        [NotMapped]
        public string TargetACName
        {
            get
            {
                return TargetMaterial != null ? TargetMaterial.ACIdentifier : "";
            }
        }

        [NotMapped]
        core.datamodel.ACClassProperty _TargetACClassProperty;
        /// <summary>
        /// Connection-Point of the ToWFNode
        /// </summary>
        /// <value>Connection-Point of the ToWFNode</value>
        [NotMapped]
        public core.datamodel.ACClassProperty TargetACClassProperty
        {
            get
            {
                return _TargetACClassProperty;
            }
            set
            {
                _TargetACClassProperty = value;
            }
        }

        /// <summary>
        /// WPF-x:Name of the ToWFNode + \\ + TargetACClassProperty.ACIdentifier
        /// for indentify the connector in the Logical-Tree
        /// </summary>
        /// <value>WPF-x:Name of the ToWFNode + \\ + TargetACClassProperty.ACIdentifier</value>
        [NotMapped]
        public string TargetACConnector
        {
            get 
            {
                return MaterialWF.MaterialWFNo + "\\" + ToWFNode.XName + "\\" + TargetACClassProperty.ACIdentifier; 
            }
        }

        /// <summary>
        /// Unique ID of the Workflow Edge
        /// </summary>
        /// <value>returns MaterialWFRelationID</value>
        [NotMapped]
        public Guid WFObjectID
        {
            get { return MaterialWFRelationID; }
        }

        /// <summary>
        /// Reference to the parent Workflow-Node that groups more child-nodes together
        /// </summary>
        /// <value>Parent Workflow-Node (Group)</value>
        [NotMapped]
        public IACWorkflowNode WFGroup
        {
            get
            {
                return this.MaterialWF;
            }
            set
            {
            }
        }

        /// <summary>
        /// WPF's x:Name to indentify this instance in the Logical-Tree
        /// </summary>
        /// <value>x:Name (WPF)</value>
        [NotMapped]
        public string XName
        {
            get 
            {
                return ACIdentifier;
            }
        }

        /// <summary>
        /// Returns MaterialWF
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to MaterialWF</value>
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return this.MaterialWF;
            }
        }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        [NotMapped]
        public override string ACIdentifier
        {
            get
            {
                string name = "";
                if (this.SourceMaterial != null)
                    name = this.SourceMaterial.XName;
                if (this.TargetMaterial != null)
                    name = name + "_" + this.TargetMaterial.XName;
                return name;
            }
            set
            {
                base.ACIdentifier = value;
            }
        }

        //public override string GetACUrl(IACObject rootACObject = null)
        //{
        //    return ACIdentifier;
        //}

        #endregion
    }
}
