using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'WorkTaskOnHoldItem'}de{'WorkTaskOnHoldItem'}", Global.ACKinds.TACSimpleClass)]
    public class WorkTaskOnHoldItem : EntityBase, IACObject
    {
        public WorkTaskOnHoldItem()
        {

        }

        private string _POProgramNo;
        [ACPropertyInfo(9999)]
        public string POProgramNo
        {
            get => _POProgramNo;
            set => SetProperty(ref _POProgramNo, value);
        }

        private DateTime? _StartDate;
        [ACPropertyInfo(9999)]
        public DateTime? StartDate
        {
            get => _StartDate;
            set => SetProperty(ref _StartDate, value);
        }

        private string _PartslistNo;
        [ACPropertyInfo(9999)]
        public string PartslistNo
        {
            get => _PartslistNo;
            set => SetProperty(ref _PartslistNo, value);
        }

        private string _PartslistName;
        [ACPropertyInfo(9999)]
        public string PartslistName
        {
            get => _PartslistName;
            set => SetProperty(ref _PartslistName, value);
        }

        private string _IntermediateMaterial;
        [ACPropertyInfo(9999)]
        public string IntermediateMaterial
        {
            get => _IntermediateMaterial;
            set => SetProperty(ref _IntermediateMaterial, value);
        }

        private string _Sequence;
        [ACPropertyInfo(9999)]
        public string Sequence
        {
            get => _Sequence;
            set => SetProperty(ref _Sequence, value);
        }

        private bool _ForRelease;
        [ACPropertyInfo(9999)]
        public bool ForRelease
        {
            get => _ForRelease;
            set => SetProperty(ref _ForRelease, value);
        }

        private string _FinalMaterialNo;
        [ACPropertyInfo(9999)]
        public string FinalMaterialNo
        {
            get => _FinalMaterialNo;
            set => SetProperty(ref _FinalMaterialNo, value);
        }

        private string _FinalMaterialName;
        [ACPropertyInfo(9999)]
        public string FinalMaterialName
        {
            get => _FinalMaterialName;
            set => SetProperty(ref _FinalMaterialName, value);
        }

        private string _FinalPartslistNo;
        [ACPropertyInfo(9999)]
        public string FinalPartslistNo
        {
            get => _FinalPartslistNo;
            set => SetProperty(ref _FinalPartslistNo, value);
        }

        private string _FinalPartslistName;
        [ACPropertyInfo(9999)]
        public string FinalPartslistName
        {
            get => _FinalPartslistName;
            set => SetProperty(ref _FinalPartslistName, value);
        }

        public string WFACUrl
        {
            get;
            set;
        }


        #region IACObject

        public IACObject ParentACObject => null;

        public IACType ACType => this.ReflectACType();

        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        public string ACIdentifier => this.ReflectGetACIdentifier();

        public string ACCaption => POProgramNo;

        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        #endregion
    }
}
