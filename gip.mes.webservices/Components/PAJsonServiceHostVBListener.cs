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
        /// Konfiguriert JSON-Serialisierung für spezifische Operationen.
        /// Äquivalent zu OnAddKnownTypesToOperationContract in WCF-Version.
        /// </summary>
        protected override void OnConfigureJsonSerialization(string operationName, JsonSerializerSettings settings)
        {
            // Für bestimmte Operationen erweiterte Serialisierung aktivieren
            if (operationName == "InvokeBarcodeSequence" || 
                operationName == "FinishPickingOrdersByMaterial")
            {
                // Aktiviere TypeHandling für komplexe Typen
                settings.TypeNameHandling = TypeNameHandling.Auto;
                
                // Verwende ACConvert.MyDataContractResolver wenn verfügbar
                // (entspricht der WCF-Version)
                if (ACConvert.MyDataContractResolver != null)
                {
                    // Für JSON müssen wir eine Custom-SerializationBinder verwenden
                    settings.SerializationBinder = new ACSerializationBinder();
                }
                
                // Erweiterte Einstellungen für komplexe Objektgraphen
                settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                settings.MaxDepth = 32; // Verhindert Stack-Overflow bei tiefen Hierarchien
            }
            
            base.OnConfigureJsonSerialization(operationName, settings);
        }

        #endregion
    }

    /// <summary>
    /// Custom SerializationBinder für ACKnownTypes.
    /// Entspricht DataContractResolver in WCF.
    /// </summary>
    public class ACSerializationBinder : ISerializationBinder
    {
        public Type BindToType(string assemblyName, string typeName)
        {
            // Versuche Typ aus bekannten Typen zu laden
            var knownTypes = ACKnownTypes.GetKnownType();
            foreach (var knownType in knownTypes)
            {
                if (knownType.FullName == typeName || knownType.Name == typeName)
                    return knownType;
            }

            // Fallback: Standard-Typ-Auflösung
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


