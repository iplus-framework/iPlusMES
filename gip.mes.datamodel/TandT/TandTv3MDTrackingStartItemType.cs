﻿using gip.core.datamodel;
/*
TandTv3_ItemType
    ItemTypeID
*/
namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTv3MDTrackingStartItemType'}de{'TandTv3MDTrackingStartItemType'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "ACCaptionTranslation", "en{'Translation'}de{'Übersetzung'}", "", "", true)]
    public partial class TandTv3MDTrackingStartItemType
    {
        public const string ClassName = "TandTv3MDTrackingStartItemType";

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