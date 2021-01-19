using gip.core.datamodel;
/*
TandT_ItemType
    ItemTypeID
*/
namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'ItemType'}de{'ItemType'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(9999, "ACCaptionTranslation", "en{'Translation'}de{'Übersetzung'}", "", "", true)]
    public partial class TandTv2ItemType
    {
        public const string ClassName = "TandTv2ItemType";
        public const string ItemName = "TandTv2ItemType";


        #region IACTypeMember
        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(2, "ACCaption", "en{'Description'}de{'Bezeichnung'}")]
        public override string ACCaption
        {
            get
            {
                return Translator.GetTranslation(ACIdentifier, ACCaptionTranslation);
            }
            set
            {
                ACCaptionTranslation = Translator.SetTranslation(ACCaptionTranslation, value);
                OnPropertyChanged(Const.ACCaptionPrefix);
            }
        }

        #endregion
    }
}
