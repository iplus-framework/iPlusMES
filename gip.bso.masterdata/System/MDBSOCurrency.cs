// ***********************************************************************
// Assembly         : gip.bso.masterdata
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : APinter
// Last Modified On : 01-15-2018
// ***********************************************************************
// <copyright file="MDBSOCurrency.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using gip.core.autocomponent;
using System.Collections.Generic;
using System.Linq;
using gip.mes.datamodel;
using gip.core.datamodel;
using System.Data;
using gip.mes.autocomponent;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace gip.bso.masterdata
{
    public class HNBResult
    {
        public string broj_tecajnice { get; set; }
        public string datum_primjene { get; set; }
        public string drzava { get; set; }
        public string drzava_iso { get; set; }
        public string sifra_valute { get; set; }
        public string valuta { get; set; }
        public int jedinica { get; set; }
        public string kupovni_tecaj { get; set; }
        public string srednji_tecaj { get; set; }
        public string prodajni_tecaj { get; set; }

        public float ExchangeRate
        {
            get
            {
                if (String.IsNullOrEmpty(kupovni_tecaj))
                    return 1;
                return float.Parse(srednji_tecaj, System.Globalization.NumberStyles.AllowDecimalPoint);
            }
        }
    }

    /// <summary>
    /// Allgemeine Stammdatenmaske für MDCurrency
    /// Bei den einfachen MD-Tabellen wird bewußt auf die Managerklassen verzichtet.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioSystem, "en{'Currency'}de{'Währung'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + MDCurrency.ClassName)]
    public class MDBSOCurrency : ACBSOvbNav
    {
        #region c´tors
        /// <summary>
        /// Initializes a new instance of the <see cref="MDBSOCurrency"/> class.
        /// </summary>
        /// <param name="acType">Type of the ac.</param>
        /// <param name="content">The content.</param>
        /// <param name="parentACObject">The parent AC object.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="acIdentifier">The ac identifier.</param>
        public MDBSOCurrency(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
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
            Search();
            return true;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            this._AccessCurrencyExchange = null;
            this._CurrentCurrencyExchange = null;
            this._CurrentNewCurrencyExchange = null;
            var b = base.ACDeInit(deleteACClassTask);
            if (_AccessCurrencyExchange != null)
            {
                _AccessCurrencyExchange.ACDeInit(false);
                _AccessCurrencyExchange = null;
            }
            if (_AccessPrimary != null)
            {
                _AccessPrimary.ACDeInit(false);
                _AccessPrimary = null;
            }
            return b;
        }

        #endregion

        #region BSO->ACProperty
        #region 1. MDCurrency
        public override IAccessNav AccessNav { get { return AccessPrimary; } }
        /// <summary>
        /// The _ access primary
        /// </summary>
        ACAccessNav<MDCurrency> _AccessPrimary;
        /// <summary>
        /// Gets the access primary.
        /// </summary>
        /// <value>The access primary.</value>
        [ACPropertyAccessPrimary(9999, MDCurrency.ClassName)]
        public ACAccessNav<MDCurrency> AccessPrimary
        {
            get
            {
                if (_AccessPrimary == null && ACType != null)
                {
                    ACQueryDefinition navACQueryDefinition = Root.Queries.CreateQueryByClass(null, PrimaryNavigationquery(), ACType.ACIdentifier);
                    _AccessPrimary = navACQueryDefinition.NewAccessNav<MDCurrency>(MDCurrency.ClassName, this);
                }
                return _AccessPrimary;
            }
        }

        /// <summary>
        /// Gets or sets the selected currency.
        /// </summary>
        /// <value>The selected currency.</value>
        [ACPropertySelected(9999, MDCurrency.ClassName)]
        public MDCurrency SelectedCurrency
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Selected;
            }
            set
            {
                if (AccessPrimary == null) return; AccessPrimary.Selected = value;
                OnPropertyChanged("SelectedCurrency");
            }
        }

        /// <summary>
        /// Gets or sets the current currency.
        /// </summary>
        /// <value>The current currency.</value>
        [ACPropertyCurrent(9999, MDCurrency.ClassName)]
        public MDCurrency CurrentCurrency
        {
            get
            {
                if (AccessPrimary == null) return null; return AccessPrimary.Current;
            }
            set
            {
                if (AccessPrimary.Current != value)
                {
                    if (AccessPrimary == null) return; AccessPrimary.Current = value;
                    OnPropertyChanged("CurrentCurrency");
                    OnPropertyChanged("CurrencyExchangeList");
                }
            }
        }

        /// <summary>
        /// Gets the currency list.
        /// </summary>
        /// <value>The currency list.</value>
        [ACPropertyList(9999, MDCurrency.ClassName)]
        public IEnumerable<MDCurrency> CurrencyList
        {
            get
            {
                return AccessPrimary.NavList;
            }
        }
        #endregion

        #region 1.1 MDCurrencyExchange
        /// <summary>
        /// The _ access currency exchange
        /// </summary>
        ACAccess<MDCurrencyExchange> _AccessCurrencyExchange;
        /// <summary>
        /// Gets the access currency exchange.
        /// </summary>
        /// <value>The access currency exchange.</value>
        [ACPropertyAccess(9999, "MDCurrencyExchange")]
        public ACAccess<MDCurrencyExchange> AccessCurrencyExchange
        {
            get
            {
                if (_AccessCurrencyExchange == null && ACType != null)
                {
                    ACQueryDefinition acQueryDefinition = AccessPrimary.NavACQueryDefinition.ACUrlCommand(Const.QueryPrefix + MDCurrencyExchange.ClassName) as ACQueryDefinition;
                    _AccessCurrencyExchange = acQueryDefinition.NewAccess<MDCurrencyExchange>("MDCurrencyExchange", this);
                }
                return _AccessCurrencyExchange;
            }
        }

        /// <summary>
        /// The _ current currency exchange
        /// </summary>
        MDCurrencyExchange _CurrentCurrencyExchange;
        /// <summary>
        /// Gets or sets the current currency exchange.
        /// </summary>
        /// <value>The current currency exchange.</value>
        [ACPropertyCurrent(9999, "MDCurrencyExchange")]
        public MDCurrencyExchange CurrentCurrencyExchange
        {
            get
            {
                return _CurrentCurrencyExchange;
            }
            set
            {
                _CurrentCurrencyExchange = value;
                OnPropertyChanged("CurrentCurrencyExchange");
            }
        }

        //List<MDCurrencyExchange> _MDCurrencyExchangeList = null;
        /// <summary>
        /// Gets the currency exchange list.
        /// </summary>
        /// <value>The currency exchange list.</value>
        [ACPropertyList(9999, "MDCurrencyExchange")]
        public IEnumerable<MDCurrencyExchange> CurrencyExchangeList
        {
            get
            {
                if (CurrentCurrency == null)
                    return null;
                //if (_MDCurrencyExchangeList == null)
                //    _MDCurrencyExchangeList = Database.MDCurrencyExchange.ToList();
                //return _MDCurrencyExchangeList;
                return CurrentCurrency.MDCurrencyExchange_MDCurrency.OrderByDescending(c => c.InsertDate).ToList();

            }
        }

        /// <summary>
        /// The _ current new currency exchange
        /// </summary>
        MDCurrencyExchange _CurrentNewCurrencyExchange;
        /// <summary>
        /// Gets or sets the current new currency exchange.
        /// </summary>
        /// <value>The current new currency exchange.</value>
        [ACPropertyCurrent(9999, "NewMDCurrencyExchange")]
        public MDCurrencyExchange CurrentNewCurrencyExchange
        {
            get
            {
                return _CurrentNewCurrencyExchange;
            }
            set
            {
                _CurrentNewCurrencyExchange = value;
                OnPropertyChanged("CurrentNewCurrencyExchange");
            }
        }
        #endregion
        #endregion

        #region BSO->ACMethod
        #region 1. MDCurrency
        /// <summary>
        /// Saves this instance.
        /// </summary>
        [ACMethodCommand(MDCurrency.ClassName, "en{'Save'}de{'Speichern'}", (short)MISort.Save, false, Global.ACKinds.MSMethodPrePost)]
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
        [ACMethodCommand(MDCurrency.ClassName, "en{'Undo'}de{'Nicht speichern'}", (short)MISort.UndoSave, false, Global.ACKinds.MSMethodPrePost)]
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
        /// Loads this instance.
        /// </summary>
        [ACMethodInteraction(MDCurrency.ClassName, "en{'Load'}de{'Laden'}", (short)MISort.Load, false, "SelectedCurrency", Global.ACKinds.MSMethodPrePost)]
        public void Load(bool requery = false)
        {
            if (!PreExecute("Load"))
                return;
            LoadEntity<MDCurrency>(requery, () => SelectedCurrency, () => CurrentCurrency, c => CurrentCurrency = c,
                        DatabaseApp.MDCurrency
                        .Where(c => c.MDCurrencyID == SelectedCurrency.MDCurrencyID));
            PostExecute("Load");
        }

        /// <summary>
        /// Determines whether [is enabled load].
        /// </summary>
        /// <returns><c>true</c> if [is enabled load]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledLoad()
        {
            return SelectedCurrency != null;
        }

        /// <summary>
        /// News this instance.
        /// </summary>
        [ACMethodInteraction(MDCurrency.ClassName, "en{'New'}de{'Neu'}", (short)MISort.New, true, "SelectedCurrency", Global.ACKinds.MSMethodPrePost)]
        public void New()
        {
            if (!PreExecute("New")) return;
            CurrentCurrency = MDCurrency.NewACObject(DatabaseApp, null);
            DatabaseApp.MDCurrency.AddObject(CurrentCurrency);
            ACState = Const.SMNew;
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
        [ACMethodInteraction(MDCurrency.ClassName, "en{'Delete'}de{'Löschen'}", (short)MISort.Delete, true, "CurrentCurrency", Global.ACKinds.MSMethodPrePost)]
        public void Delete()
        {
            if (!PreExecute("Delete")) return;
            Msg msg = CurrentCurrency.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }
            if (AccessPrimary == null) return; AccessPrimary.NavList.Remove(CurrentCurrency);
            SelectedCurrency = AccessPrimary.NavList.FirstOrDefault();
            Load();
            PostExecute("Delete");
        }

        /// <summary>
        /// Determines whether [is enabled delete].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDelete()
        {
            return CurrentCurrency != null;
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        [ACMethodCommand(MDCurrency.ClassName, "en{'Search'}de{'Suchen'}", (short)MISort.Search)]
        public void Search()
        {
            if (AccessPrimary == null) return; AccessPrimary.NavSearch(DatabaseApp);
            OnPropertyChanged("CurrencyList");
        }
        #endregion

        #region 1.1 MDCurrencyExchange
        /// <summary>
        /// News the currency exchange.
        /// </summary>
        [ACMethodInteraction("MDCurrencyExchange", "en{'New Exchange rate'}de{'Neuer Wechselkurs'}", (short)MISort.New, true, "CurrentCurrencyExchange", Global.ACKinds.MSMethodPrePost)]
        public void NewCurrencyExchange()
        {
            if (!PreExecute("NewCurrencyExchange")) return;
            CurrentNewCurrencyExchange = MDCurrencyExchange.NewACObject(DatabaseApp, CurrentCurrency);
            ShowDialog(this, "CurrencyExchangeNew");

            PostExecute("NewCurrencyExchange");
        }

        /// <summary>
        /// Determines whether [is enabled new currency exchange].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new currency exchange]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewCurrencyExchange()
        {
            return true;
        }

        /// <summary>
        /// Deletes the currency exchange.
        /// </summary>
        [ACMethodInteraction("MDCurrencyExchange", "en{'Delete Exchange rate'}de{'Wechselkurs löschen'}", (short)MISort.Delete, true, "CurrentCurrencyExchange", Global.ACKinds.MSMethodPrePost)]
        public void DeleteCurrencyExchange()
        {
            if (!PreExecute("DeleteCurrencyExchange")) return;
            Msg msg = CurrentCurrencyExchange.DeleteACObject(DatabaseApp, true);
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            PostExecute("DeleteCurrencyExchange");
        }

        /// <summary>
        /// Determines whether [is enabled delete currency exchange].
        /// </summary>
        /// <returns><c>true</c> if [is enabled delete currency exchange]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledDeleteCurrencyExchange()
        {
            return CurrentCurrency != null && CurrentCurrencyExchange != null;
        }

        /// <summary>
        /// News the currency exchange OK.
        /// </summary>
        [ACMethodCommand("NewMDCurrencyExchange", "en{'OK'}de{'OK'}", (short)MISort.Okay)]
        public void NewCurrencyExchangeOK()
        {
            CloseTopDialog();
            CurrentCurrencyExchange = CurrentNewCurrencyExchange;
            DatabaseApp.MDCurrencyExchange.AddObject(CurrentCurrencyExchange);
            CurrentCurrencyExchange = CurrentNewCurrencyExchange;
            OnPropertyChanged("CurrencyExchangeList");
        }

        /// <summary>
        /// Determines whether [is enabled new currency exchange OK].
        /// </summary>
        /// <returns><c>true</c> if [is enabled new currency exchange OK]; otherwise, <c>false</c>.</returns>
        public bool IsEnabledNewCurrencyExchangeOK()
        {
            if (CurrentNewCurrencyExchange == null || CurrentNewCurrencyExchange.ToMDCurrency == null)
                return false;
            return true;
        }

        /// <summary>
        /// News the currency exchange cancel.
        /// </summary>
        [ACMethodCommand("NewMDCurrencyExchange", "en{'Cancel'}de{'Abbrechen'}", (short)MISort.Cancel)]
        public void NewCurrencyExchangeCancel()
        {
            CloseTopDialog();
            if (CurrentNewCurrencyExchange != null)
            {
                Msg msg = CurrentNewCurrencyExchange.DeleteACObject(DatabaseApp, true);
                if (msg != null)
                {
                    Messages.Msg(msg);
                    return;
                }

            }
            CurrentNewCurrencyExchange = null;
        }


        [ACMethodCommand("GetCurrencyFromWebApi", "en{'Read Online-Exchangerate'}de{'Lese Online-Wechselkurs'}", 500)]
        public async void GetExchangeRateFromWebApi()
        {
            if (CurrentCurrencyExchange == null
                || CurrentCurrency == null
                || CurrentCurrencyExchange.ToMDCurrency == null)
                return;
            try
            {
                if (CurrentCurrency.MDCurrencyShortname.ToLower() == "kn")
                {
                    HttpClient httpClient = new HttpClient();
                    string request = String.Format("https://api.hnb.hr/tecajn/v2?valuta={0}&datum-primjene={1:yyyy-MM-dd}",
                        CurrentCurrencyExchange.ToMDCurrency.MDCurrencyShortname,
                        CurrentCurrencyExchange.InsertDate);
                    HttpResponseMessage response = await httpClient.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var result = await Task.Run(() => JsonConvert.DeserializeObject<HNBResult[]>(json));
                        var excRes = result.FirstOrDefault();
                        CurrentCurrencyExchange.ExchangeRate = excRes.ExchangeRate;
                        CurrentCurrencyExchange.ExchangeNo = excRes.broj_tecajnice;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Messages.LogException(this.GetACUrl(), "GetExchangeRateFromWebApi()", ex);
                this.Messages.Exception(this, ex.Message, true);
            }

            //CurrentNewCurrencyExchange
        }

        #endregion
        #endregion

        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(Save):
                    Save();
                    return true;
                case nameof(IsEnabledSave):
                    result = IsEnabledSave();
                    return true;
                case nameof(UndoSave):
                    UndoSave();
                    return true;
                case nameof(IsEnabledUndoSave):
                    result = IsEnabledUndoSave();
                    return true;
                case nameof(Load):
                    Load(acParameter.Count() == 1 ? (System.Boolean)acParameter[0] : false);
                    return true;
                case nameof(IsEnabledLoad):
                    result = IsEnabledLoad();
                    return true;
                case nameof(New):
                    New();
                    return true;
                case nameof(IsEnabledNew):
                    result = IsEnabledNew();
                    return true;
                case nameof(Delete):
                    Delete();
                    return true;
                case nameof(IsEnabledDelete):
                    result = IsEnabledDelete();
                    return true;
                case nameof(Search):
                    Search();
                    return true;
                case nameof(NewCurrencyExchange):
                    NewCurrencyExchange();
                    return true;
                case nameof(IsEnabledNewCurrencyExchange):
                    result = IsEnabledNewCurrencyExchange();
                    return true;
                case nameof(DeleteCurrencyExchange):
                    DeleteCurrencyExchange();
                    return true;
                case nameof(IsEnabledDeleteCurrencyExchange):
                    result = IsEnabledDeleteCurrencyExchange();
                    return true;
                case nameof(NewCurrencyExchangeOK):
                    NewCurrencyExchangeOK();
                    return true;
                case nameof(IsEnabledNewCurrencyExchangeOK):
                    result = IsEnabledNewCurrencyExchangeOK();
                    return true;
                case nameof(NewCurrencyExchangeCancel):
                    NewCurrencyExchangeCancel();
                    return true;
                case nameof(GetExchangeRateFromWebApi):
                    GetExchangeRateFromWebApi();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}