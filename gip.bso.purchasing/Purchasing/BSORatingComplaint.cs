using gip.bso.masterdata;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.purchasing
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Rating Complaint Type'}de{'Beanstandungstyp'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + RatingComplaint.ClassName)]
    [ACClassConstructorInfo(
        new object[]
        {
            new object[] {"Rating", Global.ParamOption.Optional, typeof(Rating) },
            new object[] {"CompanyID", Global.ParamOption.Optional, typeof(Guid) }
        }
    )]
    public class BSORatingComplaint : ACBSOvb
    {

        #region c´tors

        /// <summary>
        /// Initializes a new instance of the <see cref="BSOInRequestOverview"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public BSORatingComplaint(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            if (ParameterValue("Rating") != null)
            {
                FilterRating = ParameterValue("Rating") as Rating;
            }
            else if (ParameterValue("CompanyID") != null)
            {
                FilterCompany = DatabaseApp.Company.FirstOrDefault(x => x.CompanyID == new Guid(ParameterValue("CompanyID").ToString()));
            }
        }

        /// <summary>
        /// ACs the init.
        /// </summary>
        /// <param name="startChildMode">The start child mode.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            RatingStarCount = 3;
            if (this.ParentACObject != null)
            {
                BSOCompany bsoCompany = this.ParentACObject as BSOCompany;
                if (bsoCompany != null)
                    bsoCompany.PropertyChanged += bsoCompany_PropertyChanged;
            }

            if (FilterCompany != null && FilterRating != null)
                Search();
            return true;
        }

        void bsoCompany_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCompany")
            {
                BSOCompany bsoCompany = sender as BSOCompany;
                FilterRating = null;
                if (FilterCompany == null || bsoCompany.CurrentCompany == null || FilterCompany.CompanyID != bsoCompany.CurrentCompany.CompanyID)
                {
                    FilterCompany = bsoCompany.CurrentCompany;
                    ratingCompanyScoreAvg = null;
                    Search();
                }
            }
        }

        #endregion

        #region Filter

        private Company filterCompany;
        [ACPropertyInfo(503)]
        public Company FilterCompany
        {
            get
            {
                return filterCompany;
            }
            set
            {
                if (value != filterCompany)
                {
                    filterCompany = value;
                    OnPropertyChanged("FilterCompany");
                }
            }
        }

        private Rating filterRating;
        [ACPropertyInfo(504)]
        public Rating FilterRating
        {
            get
            {
                return filterRating;
            }
            set
            {
                if (value != filterRating)
                {
                    filterRating = value;
                    FilterCompany = filterRating.Company;
                    OnPropertyChanged("FilterRating");
                }
            }
        }

        [ACPropertyInfo(505)]
        public decimal FilterRatingScore
        {
            get
            {
                if (FilterRating == null) return 0;
                return FilterRating.Score;
            }
        }

        [ACPropertyInfo(506, "FilterStartTime", "en{'From'}de{'Von'}")]
        public DateTime? FilterStartTime { get; set; }

        [ACPropertyInfo(507, "FilterEndTime", "en{'To'}de{'Zum'}")]
        public DateTime? FilterEndTime { get; set; }

        #region Filter -> FilterCompiliantType

        ACValueItem _SelectedFilterCompiliantType;
        /// <summary>
        /// Gets or sets the selected facility history.
        /// </summary>
        /// <value>The selected facility history.</value>
        [ACPropertySelected(508, "FilterCompiliantType", "en{'Rating Complaint Type'}de{'Beanstandungstyp'}")]
        public ACValueItem SelectedFilterCompiliantType
        {
            get
            {
                return _SelectedFilterCompiliantType;
            }
            set
            {
                if (_SelectedFilterCompiliantType != value)
                {
                    _SelectedFilterCompiliantType = value;
                    OnPropertyChanged("SelectedFilterCompiliantType");
                }
            }
        }


        private List<ACValueItem> _FilterCompiliantTypeList;
        /// <summary>
        /// Gets or sets the facility history list.
        /// </summary>
        /// <value>The facility history list.</value>
        [ACPropertyList(509, "FilterCompiliantType")]
        public IEnumerable<ACValueItem> FilterCompiliantTypeList
        {
            get
            {
                if (_FilterCompiliantTypeList == null)
                {
                    _FilterCompiliantTypeList = DatabaseApp.MDRatingComplaintType.ToList().Select(x => new ACValueItem(x.MDRatingComplaintTypeName, (object)x.MDKey, null)).ToList();
                }
                return _FilterCompiliantTypeList;
            }
        }

        #endregion

        [ACPropertyInfo(510, "MainDesign", "en{'MainDesign'}de{'MainDesign'}")]
        public string MainDesign
        {
            get
            {
                string designName = FilterRating == null ? "MainCompanyPreview" : "MainEditing";
                gip.core.datamodel.ACClassDesign acClassDesign = ACType.GetDesign(this, Global.ACUsages.DUMain, Global.ACKinds.DSDesignLayout, designName);
                string layoutXAML = null;
                if (acClassDesign != null && acClassDesign.ACIdentifier != "UnknowMainlayout")
                {
                    layoutXAML = acClassDesign.XMLDesign;
                }
                else
                {
                    layoutXAML = "<vb:VBDockPanel><vb:VBTextBox ACCaption=\"Unknown:\" Text=\"" + designName + "\"></vb:VBTextBox></vb:VBDockPanel>";
                }
                return layoutXAML;

            }
        }

        private int _RatingStarCount;
        public int RatingStarCount
        {
            get
            {
                return _RatingStarCount;
            }
            set
            {
                if (_RatingStarCount != value)
                {
                    _RatingStarCount = value;
                    OnPropertyChanged("RatingStarCount");
                }
            }
        }
        #endregion

        /// <summary>
        ///  Rating Complaints for selected company
        /// </summary>
        #region BSO->ACProperty


        private RatingComplaint _CurrentRatingComplaint;
        /// <summary>
        /// Gets or sets the current in request.
        /// </summary>
        /// <value>The current in request.</value>
        [ACPropertyCurrent(500, "RatingComplaint")]
        public RatingComplaint CurrentRatingComplaint
        {
            get
            {
                return _CurrentRatingComplaint;
            }
            set
            {
                if (_CurrentRatingComplaint != value)
                {
                    _CurrentRatingComplaint = value;
                    SelectedRatingComplaint = value;
                    OnPropertyChanged("CurrentRatingComplaint");
                    OnPropertyChanged("CurrentRatingComplaintScore");
                }
            }
        }

        [ACPropertyInfo(999)]
        public decimal CurrentRatingComplaintScore
        {
            get
            {
                if (CurrentRatingComplaint == null) return 0;
                return CurrentRatingComplaint.Score;
            }
            set
            {
                if (CurrentRatingComplaint != null)
                    CurrentRatingComplaint.Score = value;
            }
        }
        /// <summary>
        /// This is Rating Complaint list for specific Rating (filtered) or cumulative - all complaints (for general preview, not filtered)
        /// RatingRatingComplaintList is list for specific Rating - used in Company preview
        /// </summary>
        /// <value>The in request list.</value>
        [ACPropertyList(501, "RatingComplaint")]
        public IEnumerable<RatingComplaint> RatingComplaintList
        {
            get
            {
                IEnumerable<RatingComplaint> query = null;
                if (FilterRating != null)
                {
                    query = FilterRating.RatingComplaint_Rating;
                }
                else if (FilterCompany != null)
                {
                    query = FilterCompany.Rating_Company.SelectMany(x => x.RatingComplaint_Rating);
                    // Only in company tab filter is usable
                    if (FilterStartTime.HasValue)
                        query = query.Where(x => x.InsertDate >= FilterStartTime.Value);
                    if (FilterEndTime.HasValue)
                        query = query.Where(x => x.InsertDate < FilterEndTime.Value);
                    if (SelectedFilterCompiliantType != null)
                    {
                        query = query.Where(x => x.MDRatingComplaintType.MDKey == SelectedFilterCompiliantType.Value.ToString());
                    }
                }
                if (query != null)
                    query = query.OrderBy(x => x.InsertDate);
                return query;
            }
        }


        private RatingComplaint _SelectedRatingComplaint;
        /// <summary>
        /// Gets or sets the selected in request.
        /// </summary>
        /// <value>The selected in request.</value>
        [ACPropertySelected(502, "RatingComplaint")]
        public RatingComplaint SelectedRatingComplaint
        {
            get
            {
                return _SelectedRatingComplaint;
            }
            set
            {
                if (_SelectedRatingComplaint != value)
                {
                    _SelectedRatingComplaint = value;
                    CurrentRatingComplaint = value;
                    OnPropertyChanged("SelectedRatingComplaint");
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>Called inside the GetControlModes-Method to get the Global.ControlModes from derivations.
        /// This method should be overriden in the derivations to dynmically control the presentation mode depending on the current value which is bound via VBContent</summary>
        /// <param name="vbControl">A WPF-Control that implements IVBContent</param>
        /// <returns>ControlModesInfo</returns>
        public override Global.ControlModes OnGetControlModes(IVBContent vbControl)
        {
            if (vbControl == null)
                return base.OnGetControlModes(vbControl);

            Global.ControlModes result = base.OnGetControlModes(vbControl);
            if (result < Global.ControlModes.Enabled)
                return result;
            if (!String.IsNullOrEmpty(vbControl.VBContent) && vbControl.VBContent.StartsWith("CurrentRatingComplaint\\"))
                return CurrentRatingComplaint != null ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
            //switch (vbControl.VBContent)
            //{
            //    case "CurrentRatingComplaint\\MDRatingComplaintType":
            //        return CurrentRatingComplaint != null ? Global.ControlModes.Enabled : Global.ControlModes.Disabled;
            //}

            return base.OnGetControlModes(vbControl);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand("RatingComplaint", "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
        public void Save()
        {
            OnSave();
        }

        /// <summary>
        /// Determines whether [is enabled save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledSave()
        {
            return OnIsEnabledSave();
        }

        /// <summary>
        /// Undoes the save.
        /// </summary>
        [ACMethodCommand("RatingComplaint", "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
        public void UndoSave()
        {
            OnUndoSave();
        }

        /// <summary>
        /// Determines whether [is enabled undo save].
        /// </summary>
        /// <returns><c>true</c> if [is enabled undo save]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledUndoSave()
        {
            return OnIsEnabledUndoSave();
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedRatingComplaint != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction("RatingComplaint", Const.New, (short)MISort.New, true, "SelectedRatingComplaint", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentRatingComplaint = RatingComplaint.NewACObject(DatabaseApp, null);
            if (FilterRating != null)
            {
                CurrentRatingComplaint.Rating = FilterRating;
            }
            FilterRating.RatingComplaint_Rating.Add(CurrentRatingComplaint);
            OnPropertyChanged("RatingComplaintList");
            PostExecute("New");

        }

        /// <summary>
        /// Determines whether [is enabled new].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNew()
        {
            return true;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        [ACMethodInteraction("RatingComplaint", Const.Delete, (short)MISort.Delete, true, "CurrentRatingComplaint", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentRatingComplaint.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.MsgAsync(msg);
                return;
            }
            FilterRating.RatingComplaint_Rating.Remove(CurrentRatingComplaint);
            OnPropertyChanged("RatingComplaintList");
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentRatingComplaint != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand("RatingComplaint", "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            OnPropertyChanged("RatingComplaintList");
            OnPropertyChanged("RatingComplaintAvgList");
            OnPropertyChanged("RatingList");
            OnPropertyChanged("RatingAVGList");
            OnPropertyChanged("RatingCompanyScoreAvg");
            OnPropertyChanged("RatingCompanyScoreAvgNumberPresenation");
            OnPropertyChanged("RatingCompleteAVGList");
        }


        [ACMethodInfo("RatingComplaint", "en{'Search'}de{'Suchen'}", 500)]
        public void OpenAsModal(Rating rating)
        {
            FilterRating = rating;
            Search();
            ShowDialog(this, "Mainlayout");
        }
        #endregion

        /// <summary>
        /// Accumulation of selected Rating Complaints
        /// </summary>
        #region RatingComplaintAvg
        private RatingComplaint _SelectedRatingComplaintAvg;
        /// <summary>
        /// Selected property for RatingComplaint
        /// </summary>
        /// <value>The selected RatingComplaintAvg</value>
        [ACPropertySelected(511, "RatingComplaintAvg", "en{'TODO: RatingComplaintAvg'}de{'TODO: RatingComplaintAvg'}")]
        public RatingComplaint SelectedRatingComplaintAvg
        {
            get
            {
                return _SelectedRatingComplaintAvg;
            }
            set
            {
                if (_SelectedRatingComplaintAvg != value)
                {
                    _SelectedRatingComplaintAvg = value;
                    OnPropertyChanged("SelectedRatingComplaintAvg");
                }
            }
        }

        /// <summary>
        /// List property for RatingComplaint
        /// </summary>
        /// <value>The RatingComplaintAvg list</value>
        [ACPropertyList(512, "RatingComplaintAvg")]
        public IEnumerable<RatingComplaint> RatingComplaintAvgList
        {
            get
            {
                return LoadRatingComplaintAvgList();
            }
        }

        private IEnumerable<RatingComplaint> LoadRatingComplaintAvgList()
        {
            if (RatingComplaintList == null)
            {
                return null;
            }
            return
                RatingComplaintList
                .GroupBy(x => new { x.MDRatingComplaintType.MDKey, x.MDRatingComplaintType.MDNameTrans })
                .OrderBy(x => x.Key.MDKey)
                .Select(x => new RatingComplaint()
                {
                    MDRatingComplaintType = new MDRatingComplaintType() { MDKey = x.Key.MDKey, MDNameTrans = x.Key.MDNameTrans },
                    Score = x.Average(b => b.Score)
                });
        }
        #endregion

        /// <summary>
        /// AVG of Rating Complaint Score
        /// </summary>
        #region RatingCompanyScoreAvg

        private decimal? ratingCompanyScoreAvg;
        /// <summary>
        /// AVG Rating score summary
        /// </summary>
        [ACPropertyInfo(513, "RatingScore", "en{'Score'}de{'Punktestand'}")]
        public decimal RatingCompanyScoreAvg
        {
            get
            {
                if (ratingCompanyScoreAvg == null && FilterRating == null && FilterCompany != null && DatabaseApp.Rating.Where(x => x.CompanyID == FilterCompany.CompanyID).Any())
                {
                    ratingCompanyScoreAvg = DatabaseApp.Rating.Where(x => x.CompanyID == FilterCompany.CompanyID).Average(x => x.Score);
                }
                return ratingCompanyScoreAvg ?? 0;
            }
        }

        [ACPropertyInfo(514, "RatingCompanyScoreAvgNumberPresenation", "en{'Score'}de{'Punktestand'}")]
        public decimal RatingCompanyScoreAvgNumberPresenation
        {
            get
            {
                return RatingCompanyScoreAvg * RatingStarCount;
            }
        }

        #endregion

        /// <summary>
        /// Rating list for selected Company per DeliveryNote
        /// </summary>
        #region Rating

        private Rating _SelectedRating;
        /// <summary>
        /// Selected property for Rating
        /// </summary>
        /// <value>The selected Rating</value>
        [ACPropertySelected(515, "Rating", "en{'Rating'}de{'Bewertung'}")]
        public Rating SelectedRating
        {
            get
            {
                return _SelectedRating;
            }
            set
            {
                if (_SelectedRating != value)
                {
                    _SelectedRating = value;
                    OnPropertyChanged("SelectedRating");
                    OnPropertyChanged("RatingRatingComplaintList");
                }
            }
        }

        /// <summary>
        /// List property for Rating
        /// </summary>
        /// <value>The Rating list</value>
        [ACPropertyList(516, "Rating")]
        public IEnumerable<Rating> RatingList
        {
            get
            {
                if (FilterRating == null && FilterCompany != null)
                    return FilterCompany.Rating_Company.Where(x =>
                        (FilterStartTime == null || x.InsertDate >= (FilterStartTime ?? new DateTime()))
                        &&
                        (FilterEndTime == null || x.InsertDate < (FilterEndTime ?? new DateTime()))
                    ).OrderBy(x => x.InsertDate);
                return null;
            }
        }

        #endregion

        /// <summary>
        /// on DeliveryNote based rating list grupped per score - number of every score count - 1,2,3 ... 
        /// </summary>
        #region RatingAVG
        private RatingAVGModel _SelectedRatingAVG;
        /// <summary>
        /// Selected property for RatingAVGModel
        /// </summary>
        /// <value>The selected RatingAVG</value>
        [ACPropertySelected(517, "RatingAVG", "en{'Rating Avg.'}de{'Bewertung Durchschn.'}")]
        public RatingAVGModel SelectedRatingAVG
        {
            get
            {
                return _SelectedRatingAVG;
            }
            set
            {
                if (_SelectedRatingAVG != value)
                {
                    _SelectedRatingAVG = value;
                    OnPropertyChanged("SelectedRatingAVG");
                }
            }
        }


        /// <summary>
        /// List property for RatingAVGModel
        /// </summary>
        /// <value>The RatingAVG list</value>
        [ACPropertyList(518, "RatingAVG")]
        public IEnumerable<RatingAVGModel> RatingAVGList
        {
            get
            {
                return LoadRatingAVGList();
            }
        }

        private IEnumerable<RatingAVGModel> LoadRatingAVGList()
        {
            if (RatingList == null) return null;
            return RatingList.GroupBy(r => (int)Math.Round(r.Score * RatingStarCount, 0))
            .Select(x => new RatingAVGModel() { Score = (decimal)((decimal)x.Key / (decimal)RatingStarCount), Count = x.Count() }).OrderByDescending(x => x.Score);
        }
        #endregion

        #region RatingRatingComplaint
        private RatingComplaint _SelectedRatingRatingComplaint;
        /// <summary>
        /// Selected property for RatingComplaint
        /// </summary>
        /// <value>The selected RatingRatingComplaint</value>
        [ACPropertySelected(519, "RatingRatingComplaint", "en{'TODO: RatingRatingComplaint'}de{'TODO: RatingRatingComplaint'}")]
        public RatingComplaint SelectedRatingRatingComplaint
        {
            get
            {
                return _SelectedRatingRatingComplaint;
            }
            set
            {
                if (_SelectedRatingRatingComplaint != value)
                {
                    _SelectedRatingRatingComplaint = value;
                    OnPropertyChanged("SelectedRatingRatingComplaint");
                }
            }
        }

        /// <summary>
        /// List property for RatingComplaint
        /// </summary>
        /// <value>The RatingRatingComplaint list</value>
        [ACPropertyList(520, "RatingRatingComplaint")]
        public IEnumerable<RatingComplaint> RatingRatingComplaintList
        {
            get
            {
                if (SelectedRating == null)
                    return null;
                return SelectedRating.RatingComplaint_Rating.OrderBy(x => x.InsertDate);
            }
        }

        #endregion

        /// <summary>
        ///  AVG for every rating score in company for all records - not filtered as RatingAVG
        /// </summary>
        #region RatingCompleteAVG

        private RatingAVGModel _SelectedRatingCompleteAVG;
        /// <summary>
        /// Selected property for RatingAVGModel
        /// </summary>
        /// <value>The selected RatingCompleteAVG</value>
        [ACPropertySelected(521, "RatingCompleteAVG", "en{'TODO: RatingCompleteAVG'}de{'TODO: RatingCompleteAVG'}")]
        public RatingAVGModel SelectedRatingCompleteAVG
        {
            get
            {
                return _SelectedRatingCompleteAVG;
            }
            set
            {
                if (_SelectedRatingCompleteAVG != value)
                {
                    _SelectedRatingCompleteAVG = value;
                    OnPropertyChanged("SelectedRatingCompleteAVG");
                }
            }
        }


        /// <summary>
        /// List property for RatingAVGModel
        /// </summary>
        /// <value>The RatingCompleteAVG list</value>
        [ACPropertyList(522, "RatingCompleteAVG")]
        public IEnumerable<RatingAVGModel> RatingCompleteAVGList
        {
            get
            {
                return LoadRatingCompleteAVGList();
            }
        }

        private IEnumerable<RatingAVGModel> LoadRatingCompleteAVGList()
        {
            if (FilterCompany == null) return null;
            return FilterCompany
                .Rating_Company
                .GroupBy(r => (int)Math.Round(r.Score * RatingStarCount, 0))
                .Select(x => new RatingAVGModel() { Score = (decimal)((decimal)x.Key / (decimal)RatingStarCount), Count = x.Count() })
                .OrderByDescending(x => x.Score);
        }

        #endregion


        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "Save":
                    Save();
                    return true;
                case "IsEnabledSave":
                    result = IsEnabledSave();
                    return true;
                case "UndoSave":
                    UndoSave();
                    return true;
                case "IsEnabledUndoSave":
                    result = IsEnabledUndoSave();
                    return true;
                case "IsEnabledLoad":
                    result = IsEnabledLoad();
                    return true;
                case "New":
                    New();
                    return true;
                case "IsEnabledNew":
                    result = IsEnabledNew();
                    return true;
                case "Delete":
                    Delete();
                    return true;
                case "IsEnabledDelete":
                    result = IsEnabledDelete();
                    return true;
                case "Search":
                    Search();
                    return true;
                case "OpenAsModal":
                    OpenAsModal((Rating)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }
}
