using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Partslist 
    /// </summary>
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'MaterialWF'}de{'MaterialWF'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaterialWF")]
    [ACPropertyEntity(1, "MaterialWFNo", "en{'Material Workflow No.'}de{'Material-Workflow Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "Name", "en{'Name'}de{'Name'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MaterialWF.ClassName, "en{'MaterialWF'}de{'MaterialWF'}", typeof(MaterialWF), MaterialWF.ClassName, "MaterialWFNo", "MaterialWFNo", new object[]
       {
            new object[] {Const.QueryPrefix + MaterialWFRelation.ClassName, "en{'MaterialWFRelation'}de{'MaterialWFRelation'}", typeof(MaterialWFRelation), MaterialWFRelation.ClassName + "_" + MaterialWF.ClassName, "Sequence", "Sequence"}
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaterialWF>) })]
    public partial class MaterialWF : IACWorkflowDesignContext, IACWorkflowNode, IACClassDesignProvider
    {
        [NotMapped]
        public const string ClassName = "MaterialWF";
        [NotMapped]
        public const string NoColumnName = "MaterialWFNo";
        [NotMapped]
        public const string FormatNewNo = "MW{0}";

        [NotMapped]
        public readonly ACMonitorObject _11020_LockValue = new ACMonitorObject(11020);

        #region c'tors and instancing

        public static MaterialWF NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            MaterialWF entity = new MaterialWF();
            entity.MaterialWFID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MaterialWFNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACEntityProperty

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "MaterialWFNo";
            }
        }

        #endregion

        #region IACUrl members

        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            switch (className)
            {
                // MaterialWFRelation(KeyACIdentifier) "TargetMaterial\\MaterialNo,SourceMaterial\\MaterialNo,Sequence";
                case MaterialWFRelation.ClassName:
                    if (filterValues.Any())
                    {
                        string[] filterItems = filterValues[0].Split(',');
                        string targetMaterialNo = filterItems[0];
                        string sourceMaterialNo = filterItems[1];
                        int sequence = int.Parse(filterItems[2]);
                        return
                            MaterialWFRelation_MaterialWF
                            .FirstOrDefault(x =>
                                x.TargetMaterial.MaterialNo == targetMaterialNo &&
                                (x.SourceMaterial != null && x.SourceMaterial.MaterialNo == sourceMaterialNo) &&
                                x.Sequence == sequence);
                    }
                    break;
            }
            return null;
        }

        #endregion

        #region IACWorkflowDesignContext

        [NotMapped]
        public Material FromNode;
        [NotMapped]
        public Material ToNode;
        [NotMapped]
        public core.datamodel.ACClass WorkflowTypeACClass
        {
            get
            {
                return this.PWACClass;
            }
        }

        [NotMapped]
        public IACWorkflowNode RootWFNode
        {
            get
            {
                return this;
            }
        }

        [NotMapped]
        public IEnumerable<IACWorkflowEdge> AllWFEdges
        {
            get { return this.MaterialWFRelation_MaterialWF; }
        }

        public void AddNode(IACWorkflowNode vbVisualWF)
        {
        }

        public void DeleteNode(IACEntityObjectContext database, IACWorkflowNode vbVisualWF, string configACUrl)
        {
        }

        public void DeleteInnerWFs(IACEntityObjectContext database, IACWorkflowNode vbVisualWF)
        {
        }

        [NotMapped]
        public IEnumerable<IACWorkflowNode> AllWFNodes
        {
            get { return this.GetMaterials(); }
        }


        public void AddEdge(IACWorkflowEdge vbEdge)
        {
            ((MaterialWFRelation)vbEdge).MaterialWF = this;
        }

        public void DeleteEdge(IACEntityObjectContext database, IACWorkflowEdge vbEdge)
        {
        }

        public IACWorkflowEdge CreateNewEdge(IACEntityObjectContext database)
        {
            if (FromNode != null && ToNode != null)
                return this.MaterialWFRelation_MaterialWF.FirstOrDefault(c => c.SourceMaterial == FromNode && c.TargetMaterial == ToNode);
            else if (database is DatabaseApp)
                return MaterialWFRelation.NewACObject(database as DatabaseApp, this);
            return null;
        }

        /// <summary>Returns a ACClassDesign for presenting itself on the gui</summary>
        /// <param name="acUsage">Filter for selecting designs that belongs to this ACUsages-Group</param>
        /// <param name="acKind">Filter for selecting designs that belongs to this ACKinds-Group</param>
        /// <param name="vbDesignName">Optional: The concrete acIdentifier of the design</param>
        /// <returns>ACClassDesign</returns>
        public core.datamodel.ACClassDesign GetDesign(Global.ACUsages acUsage, Global.ACKinds acKind, string vbDesignName = "")
        {
            return PWACClass.ACType.GetDesign(PWACClass, acUsage, acKind, vbDesignName);
        }

        #region IACWorkflowDesignContext -> IACConfigStore

        [NotMapped]
        public string ConfigStoreName
        {
            get
            {
                ACClassInfo acClassInfo = (ACClassInfo)GetType().GetCustomAttributes(typeof(ACClassInfo), false)[0];
                string caption = Translator.GetTranslation(acClassInfo.ACCaptionTranslation);
                return caption;
            }
        }

        /// <summary>
        /// ACConfigKeyACUrl returns the relative Url to the "main table" in group a group of semantically related tables.
        /// This property is used when NewACConfig() is called. NewACConfig() creates a new IACConfig-Instance and set the IACConfig.KeyACUrl-Property with this ACConfigKeyACUrl.
        /// </summary>
        [NotMapped]
        public string ACConfigKeyACUrl
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Creates and adds a new IACConfig-Entry to ConfigItemsSource.
        /// The implementing class creates a new entity object an add it to its "own Configuration-Table".
        /// It sets automatically the IACConfig.KeyACUrl-Property with this ACConfigKeyACUrl.
        /// </summary>
        /// <param name="acObject">Optional: Reference to another Entity-Object that should be related for this new configuration entry.</param>
        /// <param name="valueTypeACClass">The iPlus-Type of the "Value"-Property.</param>
        /// <returns>IACConfig as a new entry</returns>
        public IACConfig NewACConfig(IACObjectEntity acObject = null, gip.core.datamodel.ACClass valueTypeACClass = null, string localConfigACUrl = null)
        {
            return null;
        }

        /// <summary>
        /// Deletes all IACConfig-Entries in the Database-Context as well as in ConfigurationEntries.
        /// </summary>
        public void DeleteAllConfig()
        {
        }

        [NotMapped]
        public decimal OverridingOrder { get; set; }

        /// <summary>
        /// A thread-safe and cached list of Configuration-Values of type IACConfig.
        /// </summary>
        [NotMapped]
        public IEnumerable<IACConfig> ConfigurationEntries
        {
            get
            {
                return new List<IACConfig>();
            }
        }

        /// <summary>Clears the cache of configuration entries. (ConfigurationEntries)
        /// Re-accessing the ConfigurationEntries property rereads all configuration entries from the database.</summary>
        public void ClearCacheOfConfigurationEntries()
        {
        }

        /// <summary>
        /// Checks if cached configuration entries are loaded from database successfully
        /// </summary>
        public bool ValidateConfigurationEntriesWithDB(ConfigEntriesValidationMode mode = ConfigEntriesValidationMode.AnyCheck)
        {
            return true;
        }

        /// <summary>Removes a configuration from ConfigurationEntries and the database-context.</summary>
        /// <param name="acObject">Entry as IACConfig</param>
        public void RemoveACConfig(IACConfig acObject)
        {
        }

        #endregion

        #endregion

        #region IACWorkflowNode

        /// <summary>
        /// All edges that starts from this node
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of edges</value>
        public IEnumerable<IACWorkflowEdge> GetOutgoingWFEdges(IACWorkflowContext context)
        {
            return AllWFEdges;
        }

        /// <summary>
        /// If this Node is a Workflow-Group, this property returns all outgoing-edges belonging to this node.
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of edges</value>
        public IEnumerable<IACWorkflowEdge> GetOutgoingWFEdgesInGroup(IACWorkflowContext context)
        {
            return AllWFEdges;
        }

        /// <summary>
        /// All edges that ends in this node
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of edges</value>
        public IEnumerable<IACWorkflowEdge> GetIncomingWFEdges(IACWorkflowContext context)
        {
            return AllWFEdges;
        }

        /// <summary>
        /// If this Node is a Workflow-Group, this property returns all incoming-edges belonging to this node.
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of edges</value>
        public IEnumerable<IACWorkflowEdge> GetIncomingWFEdgesInGroup(IACWorkflowContext context)
        {
            return AllWFEdges;
        }

        /// <summary>
        /// Returns true if this Node is a Workflow-Group and is the most outer node.
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value><c>true</c> if this Node is a Workflow-Group and is the most outer node; otherwise, <c>false</c>.</value>
        public bool IsRootWFNode(IACWorkflowContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns the ACClassProperty that reprensents a Connection-Point where Edges can be connected to.
        /// </summary>
        /// <param name="acPropertyName">Name of the property.</param>
        /// <returns>ACClassProperty.</returns>
        public core.datamodel.ACClassProperty GetConnector(string acPropertyName)
        {
            return PWACClass.GetPoint(acPropertyName);
        }

        /// <summary>
        /// If this Node is a Workflow-Group, this method returns all nnodes that are inside of this group.
        /// </summary>
        /// <param name="context">Workflowcontext. (Main Entity Class that saves the Workflow and the Nodes and Edges are related to)</param>
        /// <value>List of nodes</value>
        public IEnumerable<IACWorkflowNode> GetChildWFNodes(IACWorkflowContext context)
        {
            return AllWFNodes;
        }


        /// <summary>
        /// Returns a ACUrl, to be able to find this instance in the WPF-Logical-Tree.
        /// </summary>
        /// <value>ACUrl as string</value>
        [NotMapped]
        public string VisualACUrl
        {
            get { return this.ACIdentifier; }
        }


        /// <summary>
        /// The Runtime-type of the Workflow-Class that will be instantiated when the Workflow is loaded.
        /// </summary>
        /// <value>Reference to a ACClass</value>
        [NotMapped]
        public core.datamodel.ACClass PWACClass
        {
            get { return this.GetObjectContext().ContextIPlus.GetACType(typeof(PWMaterialGroup)); }
        }

        /// <summary>
        /// Unique ID of the Workflow Node.
        /// The MaterialWF itself is a WorkflowNode because it represents the root. 
        /// Therefore interface IACWorkflowNode is implemented.
        /// </summary>
        /// <value>Returns MaterialID</value>
        [NotMapped]
        public Guid WFObjectID
        {
            get { return MaterialWFID; }
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
                return this;
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
            get { return this.MaterialWFNo; }
        }

        public IACObject GetParentACObject(IACObject context)
        {
            return null;
        }
        #endregion



        #region Loading material WF content

        public static int CalculateMixingLevel(Material item, IEnumerable<Material> materials, IEnumerable<MaterialWFRelation> relations)
        {
            int mixingLevel = 0;
            Material source = item;
            while (source != null)
            {
                Guid sourceID = relations.Where(x => x.TargetMaterialID == source.MaterialID).Select(x => x.SourceMaterialID).FirstOrDefault();
                if (sourceID == item.MaterialID)
                    return 0;
                source = materials.FirstOrDefault(x => x.MaterialID == sourceID);
                mixingLevel++;
                if (mixingLevel > 100)
                    return mixingLevel;
            }
            return mixingLevel;
        }

        public IEnumerable<Material> GetMaterials()
        {
            if (MaterialWFRelation_MaterialWF.Where(x => x.SourceMaterial != null && x.SourceMaterialID != Guid.Empty).Any())
                return MaterialWFRelation_MaterialWF.Select(x => x.TargetMaterial)
                    .Union(MaterialWFRelation_MaterialWF.Select(x => x.SourceMaterial).Where(x =>
                    !MaterialWFRelation_MaterialWF.Select(a => a.TargetMaterial.MaterialID).Contains(x.MaterialID)));
            else
                return MaterialWFRelation_MaterialWF.Select(x => x.TargetMaterial);
        }

        #endregion

        #region convention implementation

        public override string ToString()
        {

            return "MaterialWF [" + (MaterialWFNo ?? "") + "] " + (Name ?? "");
        }

        #endregion

    }
}
