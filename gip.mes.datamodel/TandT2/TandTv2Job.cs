using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Job'}de{'Job'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, TandTv2TrackingStyle.ItemName, "en{'TrackingStyle'}de{'TrackingStyle'}", Const.ContextDatabase + "\\" + TandTv2TrackingStyle.ClassName, "", true)]
    [ACPropertyEntity(2, TandTv2ItemType.ItemName, "en{'ItemType'}de{'ItemType'}", Const.ContextDatabase + "\\" + TandTv2ItemType.ClassName, "", true)]
    [ACPropertyEntity(3, "JobNo", "en{'JobNo'}de{'JobNo'}", "", "", true)]
    [ACPropertyEntity(4, "StartTime", "en{'Started'}de{'Started'}", "", "", true)]
    [ACPropertyEntity(5, "EndTime", "en{'Finished'}de{'Beendet'}", "", "", true)]
    [ACPropertyEntity(6, "FilterDateFrom", "en{'Start time'}de{'Startzeit'}", "", "", true)]
    [ACPropertyEntity(7, "FilterDateTo", "en{'End Time'}de{'Endzeit'}", "", "", true)]
    [ACPropertyEntity(8, "ItemSystemNo", "en{'ItemSystemNo'}de{'ItemSystemNo'}", "", "", true)]
    [ACPropertyEntity(9, "PrimaryKeyID", "en{'PrimaryKeyID'}de{'PrimaryKeyID'}", "", "", true)]
    [ACPropertyEntity(10, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + TandTv2Job.ClassName, "en{'Job'}de{'Job'}", typeof(TandTv2Job), TandTv2Job.ClassName, "ItemSystemNo", "ItemSystemNo")]
    public partial class TandTv2Job
    {
        public const string ClassName = "TandTv2Job";
        public const string ItemName = "TandTv2Job";

        #region Enum

        /// <summary>
        /// 
        /// </summary>
        public TandTv2TrackingStyleEnum TrackingStyleEnum
        {
            get
            {
                return (TandTv2TrackingStyleEnum)Enum.Parse(typeof(TandTv2TrackingStyleEnum), TandTv2TrackingStyleID);
            }
            set
            {
                TandTv2TrackingStyleID = value.ToString();
            }
        }


        public TandTv2ItemTypeEnum ItemTypeEnum
        {
            get
            {
                return (TandTv2ItemTypeEnum)Enum.Parse(typeof(TandTv2ItemTypeEnum), TandTv2ItemTypeID);
            }
            set
            {
                TandTv2ItemTypeID = value.ToString();
            }
        }

        private string _TrackingStyleACCaption;
        [ACPropertyInfo(999, "ACCaption", "en{'Direction'}de{'Richtung'}")]
        public string TrackingStyleACCaption
        {
            get
            {
                if (string.IsNullOrEmpty(_TrackingStyleACCaption))
                {
                    string acCaptionTranslation = "";
                    switch (TrackingStyleEnum)
                    {
                        case TandTv2TrackingStyleEnum.Backward:
                            acCaptionTranslation = "en{'Trace back'}de{'Rückwärtsverfolgung'}";
                            break;
                        case TandTv2TrackingStyleEnum.Forward:
                            acCaptionTranslation = "en{'Forward Track and Trace'}de{'Vorwärtsverfolgung'}";
                            break;
                    }
                    _TrackingStyleACCaption = Translator.GetTranslation(ACIdentifier, acCaptionTranslation);
                }
                return _TrackingStyleACCaption;
            }
        }
        #endregion


        #region Additional members

        [ACPropertyInfo(9999, "MaterialNOs", "en{'Query Materials'}de{'Abfragematerialen'}")]
        public string MaterialNOs
        {
           get
            {
                string materialNOs = "";
                if (this.TandTv2JobMaterial_TandTv2Job.Any())
                {
                    foreach (var mt in this.TandTv2JobMaterial_TandTv2Job)
                        materialNOs = mt.Material.MaterialNo + ", ";
                    materialNOs = materialNOs.TrimEnd(", ".ToCharArray());
                }
                return materialNOs;
            }
        }

        public List<Guid> MaterialIDs { get; set; }


        /// <summary>
        /// If job is found - old job is deleted, new created, calculated again
        /// option calling from production wehre order items changed countinous
        /// </summary>
        public bool RecalcAgain { get; set; }

        /// <summary>
        /// When is parameter forwarded as report
        /// </summary>
        public bool IsReport { get; set; }

        /// <summary>
        /// If no need result to be saved
        /// </summary>
        public bool IsDynamic { get; set; }

        #endregion
    }
}
