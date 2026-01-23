using gip.core.datamodel;
using gip.core.webservices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using gip.core.autocomponent;

namespace gip.mes.webservices
{
    //[ACClassInfo(Const.PackName_VarioSystem, "en{'Json Host VB Listener'}de{'Json Host VB Listener'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.Required, false, false)]
    //public class PAJsonServiceHostVBListener : PAJsonServiceHostListener
    public partial class PAJsonServiceHostVB
    {
        #region c´tors
        //public PAJsonServiceHostVBListener(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
        //    : base(acType, content, parentACObject, parameter, acIdentifier)
        //{
        //}
        #endregion

        #region Implementation

        /// <summary>
        /// Configures JSON serialization for specific operations.
        /// Equivalent to OnAddKnownTypesToOperationContract in WCF version.
        /// </summary>
        protected override void OnConfigureJsonSerialization(string operationName, JsonSerializerSettings settings)
        {
            // Enable extended serialization for specific operations
            if (operationName == "InvokeBarcodeSequence" || 
                operationName == "FinishPickingOrdersByMaterial")
            {
                // Enable TypeHandling for complex types
                settings.TypeNameHandling = TypeNameHandling.Auto;
                
                // Use ACConvert.MyDataContractResolver if available
                // (corresponds to WCF version)
                if (ACConvert.MyDataContractResolver != null)
                {
                    // For JSON we need to use a custom SerializationBinder
                    settings.SerializationBinder = new ACSerializationBinder();
                }
                
                // Extended settings for complex object graphs
                settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                settings.MaxDepth = 32; // Prevents stack overflow with deep hierarchies
            }
            
            base.OnConfigureJsonSerialization(operationName, settings);
        }

        #endregion
    }

    /// <summary>
    /// Custom SerializationBinder for ACKnownTypes.
    /// Corresponds to DataContractResolver in WCF.
    /// </summary>
    public class ACSerializationBinder : ISerializationBinder
    {
        public Type BindToType(string assemblyName, string typeName)
        {
            // Try to load type from known types
            var knownTypes = ACKnownTypes.GetKnownType();
            foreach (var knownType in knownTypes)
            {
                if (knownType.FullName == typeName || knownType.Name == typeName)
                    return knownType;
            }

            // Fallback: standard type resolution
            if (!string.IsNullOrEmpty(assemblyName))
            {
                return Type.GetType($"{typeName}, {assemblyName}");
            }
            return Type.GetType(typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
        }
    }
}


