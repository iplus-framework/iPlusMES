using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Sales Order Pos.'}de{'Auftragsposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(4, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(5, "TargetQuantityUOM", ConstApp.TargetQuantityUOM, "", "", true)]
    [ACPropertyEntity(6, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(7, "ActualQuantityUOM", ConstApp.ActualQuantityUOM, "", "", true)]
    [ACPropertyEntity(8, "ActualQuantity", ConstApp.ActualQuantity, "", "", true)]
    [ACPropertyEntity(9, "CalledUpQuantityUOM", "en{'Called up Quantity (UOM)'}de{'Abgerufene Menge (BME)'}", "", "", true)]
    [ACPropertyEntity(10, "CalledUpQuantity", "en{'Called up Quantity'}de{'Abgerufene Menge'}", "", "", true)]
    [ACPropertyEntity(11, "ExternQuantityUOM", "en{'Extern Quantity (UOM)'}de{'Externe Menge (BME)'}", "", "", true)]
    [ACPropertyEntity(12, "ExternQuantity", "en{'Extern Quantity'}de{'Externe Menge'}", "", "", true)]
    [ACPropertyEntity(13, "TargetDeliveryDate", ConstApp.TargetDeliveryDate, "", "", true)]
    [ACPropertyEntity(14, "TargetDeliveryMaxDate", ConstApp.TargetDeliveryMaxDate, "", "", true)]
    [ACPropertyEntity(15, "TargetDeliveryPriority", "en{'Delivery Priority'}de{'Lieferpriorität'}", "", "", true)]
    [ACPropertyEntity(16, "TargetDeliveryDateConfirmed", "en{'Confirmed Delivery Date'}de{'Bestätigtes Lieferdatum'}", "", "", true)]
    [ACPropertyEntity(17, MDTimeRange.ClassName, "en{'Shift'}de{'Schicht'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName, "", true)]
    [ACPropertyEntity(18, MDDelivPosState.ClassName, "en{'Delivery Status'}de{'Lieferstatus'}", Const.ContextDatabase + "\\" + MDDelivPosState.ClassName, "", true)]
    [ACPropertyEntity(19, MDOutOrderPosState.ClassName, "en{'Pos. Status'}de{'Pos. Status'}", Const.ContextDatabase + "\\" + MDOutOrderPosState.ClassName, "", true)]
    [ACPropertyEntity(20, MDDelivPosLoadState.ClassName, "en{'Loading State'}de{'Beladungszustand'}", Const.ContextDatabase + "\\" + MDDelivPosLoadState.ClassName, "", true)]
    [ACPropertyEntity(21, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(22, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(23, MDCountrySalesTax.ClassName, "en{'Sales Tax'}de{'Steuersatz'}", Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName, "", true)]
    [ACPropertyEntity(24, MDToleranceState.ClassName, "en{'Tolerance Status'}de{'Toleranz Status'}", Const.ContextDatabase + "\\" + MDToleranceState.ClassName, "", true)]
    [ACPropertyEntity(25, MDOutOrderPlanState.ClassName, "en{'Production Status'}de{'Status Produktion'}", Const.ContextDatabase + "\\" + MDOutOrderPlanState.ClassName, "", true)]
    [ACPropertyEntity(26, MDTourplanPosState.ClassName, "en{'Picking Status'}de{'Status Kommissionierung'}", Const.ContextDatabase + "\\" + MDTourplanPosState.ClassName, "", true)]
    [ACPropertyEntity(27, CompanyAddressUnloadingpoint.ClassName, "en{'Unloading Point'}de{'Abladestelle'}", Const.ContextDatabase + "\\" + CompanyAddressUnloadingpoint.ClassName, "", true)]
    [ACPropertyEntity(28, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(29, "Comment2", "en{'Comment 2'}de{'Bemerkung 2'}", "", "", true)]
    [ACPropertyEntity(30, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(31, MDTransportMode.ClassName, ConstApp.ESTransportMode, Const.ContextDatabase + "\\" + MDTransportMode.ClassName, "", true)]
    [ACPropertyEntity(9999, "PickupCompanyMaterial", "en{'Material for Pick-Up'}de{'Material für Abholung'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(9999, OutOrder.ClassName, "en{'Sales Order'}de{'Auftrag'}", Const.ContextDatabase + "\\" + OutOrder.ClassName, "", true)]
    [ACPropertyEntity(9999, "MaterialPosTypeIndex", "en{'Position Type'}de{'Positionstyp'}", typeof(GlobalApp.MaterialPosTypes), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + OutOrderPos.ClassName, "en{'Sales Order Pos.'}de{'Auftragsposition'}", typeof(OutOrderPos), OutOrderPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OutOrderPos>) })]
    public partial class OutOrderPos : IOutOrderPos
    {
        public const string ClassName = "OutOrderPos";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static OutOrderPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            OutOrderPos entity = new OutOrderPos();
            entity.OutOrderPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            OutOrder outOrder = null;
            if (parentACObject is OutOrderPos)
            {
                OutOrderPos parentOutOrderPos = parentACObject as OutOrderPos;
                entity.OutOrderPos1_ParentOutOrderPos = parentOutOrderPos;
                entity.CopyFromParent(parentOutOrderPos);
                outOrder = parentOutOrderPos.OutOrder;
                entity.MaterialPosType = GlobalApp.MaterialPosTypes.OutwardPart;
            }
            if (parentACObject is OutOrder)
            {
                outOrder = parentACObject as OutOrder;
            }
            if (outOrder != null)
            {
                if (outOrder.EntityState != System.Data.EntityState.Added
                    && outOrder.OutOrderPos_OutOrder != null
                    && outOrder.OutOrderPos_OutOrder.Select(c => c.Sequence).Any())
                    entity.Sequence = outOrder.OutOrderPos_OutOrder.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;
                entity.OutOrder = outOrder;
                entity.MaterialPosType = GlobalApp.MaterialPosTypes.OutwardRoot;
                outOrder.OutOrderPos_OutOrder.Add(entity);
            }
            entity.TargetQuantityUOM = 0;
            entity.MDOutOrderPosState = MDOutOrderPosState.DefaultMDOutOrderPosState(dbApp);
            entity.MDDelivPosLoadState = MDDelivPosLoadState.DefaultMDDelivPosLoadState(dbApp);
            entity.MDOutOrderPlanState = MDOutOrderPlanState.DefaultMDOutOrderPlanState(dbApp);
            entity.MDToleranceState = MDToleranceState.DefaultMDToleranceState(dbApp);
            entity.MDTourplanPosState = MDTourplanPosState.DefaultMDTourplanPosState(dbApp);
            entity.MDDelivPosState = MDDelivPosState.DefaultMDDelivPosState(dbApp);
            entity.TargetDeliveryDate = DateTime.Now;
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }

        /// <summary>
        /// Deletes this entity-object from the database
        /// </summary>
        /// <param name="database">Entity-Framework databasecontext</param>
        /// <param name="withCheck">If set to true, a validation happens before deleting this EF-object. If Validation fails message ís returned.</param>
        /// <param name="softDelete">If set to true a delete-Flag is set in the dabase-table instead of a physical deletion. If  a delete-Flag doesn't exit in the table the record will be deleted.</param>
        /// <returns>If a validation or deletion failed a message is returned. NULL if sucessful.</returns>
        public override MsgWithDetails DeleteACObject(IACEntityObjectContext database, bool withCheck, bool softDelete = false)
        {
            foreach (CompanyMaterialPickup pickup in CompanyMaterialPickup_OutOrderPos.ToList())
            {
                MsgWithDetails msg = pickup.DeleteACObject(database, withCheck);
                if (msg != null)
                    return msg;
            }
            if (withCheck)
            {
                MsgWithDetails msg = IsEnabledDeleteACObject(database);
                if (msg != null)
                    return msg;
            }
            int sequence = Sequence;
            OutOrder outOrder = OutOrder;
            if (outOrder.OutOrderPos_OutOrder.IsLoaded)
                outOrder.OutOrderPos_OutOrder.Remove(this);
            database.DeleteObject(this);
            OutOrderPos.RenumberSequence(outOrder, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(OutOrder outOrder, int sequence)
        {
            var elements = from c in outOrder.OutOrderPos_OutOrder where c.Sequence > sequence && c.EntityState != System.Data.EntityState.Deleted orderby c.Sequence select c;
            int sequenceCount = sequence;
            foreach (var element in elements)
            {
                element.Sequence = sequence;
                sequence++;
            }
        }

        public void CopyFromParent(OutOrderPos parentOutOrderPos)
        {
            this.MDUnit = parentOutOrderPos.MDUnit;
            this.Material = parentOutOrderPos.Material;
            this.TargetDeliveryDate = parentOutOrderPos.TargetDeliveryDate;
            this.TargetDeliveryMaxDate = parentOutOrderPos.TargetDeliveryMaxDate;
            this.TargetDeliveryPriority = parentOutOrderPos.TargetDeliveryPriority;
            this.TargetDeliveryDateConfirmed = parentOutOrderPos.TargetDeliveryDateConfirmed;
            this.MDTimeRange = parentOutOrderPos.MDTimeRange;
            this.PriceNet = parentOutOrderPos.PriceNet;
            this.PriceGross = parentOutOrderPos.PriceGross;
            this.MDCountrySalesTax = parentOutOrderPos.MDCountrySalesTax;
            this.Comment = parentOutOrderPos.Comment;
            this.Comment2 = parentOutOrderPos.Comment2;
            this.Sequence = parentOutOrderPos.Sequence;
            this.LineNumber = parentOutOrderPos.LineNumber;
            this.MDTransportMode = parentOutOrderPos.MDTransportMode;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                if (Material == null)
                    return Sequence.ToString();
                return Sequence.ToString() + " " + Material.ACCaption;
            }
        }

        /// <summary>
        /// Returns OutOrder
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to OutOrder</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return OutOrder;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == OutOrderPosUtilization.ClassName)
                return this.OutOrderPosUtilization_OutOrderPos.Where(c => c.OutOrderPosUtilizationNo == filterValues[0]).FirstOrDefault();
            return null;
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (this.Material == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "Key",
                    Message = "Key",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "Key"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence";
            }
        }
        #endregion

        #region IEntityProperty Members

        bool bRefreshConfig = false;
        partial void OnXMLConfigChanging(global::System.String value)
        {
            bRefreshConfig = false;
            if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
                bRefreshConfig = true;
        }

        partial void OnXMLConfigChanged()
        {
            if (bRefreshConfig)
                ACProperties.Refresh();
        }

        #endregion

        [ACPropertyInfo(9999)]
        public string Position
        {
            get
            {
                return "Position " + Sequence.ToString();
            }
        }

        public GlobalApp.MaterialPosTypes MaterialPosType
        {
            get
            {
                return (GlobalApp.MaterialPosTypes)MaterialPosTypeIndex;
            }
            set
            {
                MaterialPosTypeIndex = (Int16)value;
            }
        }

        [ACPropertyInfo(999, "", "en{'VAT amount'}de{'Mehrwertsteuerbetrag'}")]
        public double SalesTaxAmount
        {
            get
            {
                return (double)PriceNet * (SalesTax / 100);
            }
        }

        #region partial methods
        bool _OnCalledUpQuantityChanging = false;
        partial void OnCalledUpQuantityChanged()
        {
            if (!_OnCalledUpQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnCalledUpQuantityChanging = true;
                try
                {
                    CalledUpQuantityUOM = Material.ConvertToBaseQuantity(CalledUpQuantity, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnCalledUpQuantity", msg);
                }
                finally
                {
                    _OnCalledUpQuantityChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantity");
        }

        bool _OnCalledUpQuantityUOMChanging = false;
        partial void OnCalledUpQuantityUOMChanged()
        {
            if (!_OnCalledUpQuantityChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnCalledUpQuantityUOMChanging = true;
                try
                {
                    CalledUpQuantity = Material.ConvertQuantity(CalledUpQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnCalledUpQuantityUOM", msg);
                }
                finally
                {
                    _OnCalledUpQuantityUOMChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantityUOM");
        }

        bool _OnTargetQuantityChanging = false;
        partial void OnTargetQuantityChanged()
        {
            if (!_OnTargetQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnTargetQuantityChanging = true;
                try
                {
                    TargetQuantityUOM = Material.ConvertToBaseQuantity(TargetQuantity, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnTargetQuantityChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantity");
        }

        bool _OnTargetQuantityUOMChanging = false;
        partial void OnTargetQuantityUOMChanged()
        {
            if (!_OnTargetQuantityChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnTargetQuantityUOMChanging = true;
                try
                {
                    TargetQuantity = Material.ConvertQuantity(TargetQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnTargetQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnTargetQuantityUOMChanging = false;
                }
            }
            OnPropertyChanged("RemainingCallQuantityUOM");
        }

        bool _OnActualQuantityChanging = false;
        partial void OnActualQuantityChanged()
        {
            if (!_OnActualQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnActualQuantityChanging = true;
                try
                {
                    ActualQuantityUOM = Material.ConvertToBaseQuantity(ActualQuantity, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnActualQuantityChanged", msg);
                }
                finally
                {
                    _OnActualQuantityChanging = false;
                }
            }
        }

        bool _OnActualQuantityUOMChanging = false;
        partial void OnActualQuantityUOMChanged()
        {
            if (!_OnActualQuantityChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnActualQuantityUOMChanging = true;
                try
                {
                    ActualQuantity = Material.ConvertQuantity(ActualQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnActualQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnActualQuantityUOMChanging = false;
                }
            }
        }

        bool _OnExternQuantityChanging = false;
        partial void OnExternQuantityChanged()
        {
            if (!_OnExternQuantityUOMChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnExternQuantityChanging = true;
                try
                {
                    ExternQuantityUOM = Material.ConvertToBaseQuantity(ExternQuantity, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnExternQuantityChanged", msg);
                }
                finally
                {
                    _OnExternQuantityChanging = false;
                }
            }
        }

        bool _OnExternQuantityUOMChanging = false;
        partial void OnExternQuantityUOMChanged()
        {
            if (!_OnExternQuantityChanging && EntityState != System.Data.EntityState.Detached && Material != null && MDUnit != null)
            {
                _OnExternQuantityUOMChanging = true;
                try
                {
                    ExternQuantity = Material.ConvertQuantity(ExternQuantityUOM, Material.BaseMDUnit, MDUnit);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException(ClassName, "OnExternQuantityUOMChanged", msg);
                }
                finally
                {
                    _OnExternQuantityUOMChanging = false;
                }
            }
        }

        [ACPropertyInfo(24, "", "en{'Remaining Quantity'}de{'Restmenge'}")]
        public double RemainingCallQuantity
        {
            get
            {
                return TargetQuantity - CalledUpQuantity;
            }
        }

        [ACPropertyInfo(25, "", "en{'Remaining Quantity(UOM)'}de{'Restmenge(BME)'}")]
        public double RemainingCallQuantityUOM
        {
            get
            {
                return TargetQuantityUOM - CalledUpQuantityUOM;
            }
        }

        public OutOrderPos TopParentOutOrderPos
        {
            get
            {
                if (this.OutOrderPos1_ParentOutOrderPos != null)
                    return this.OutOrderPos1_ParentOutOrderPos.TopParentOutOrderPos;
                return this;
            }
        }


        public void RecalcActualQuantity(Nullable<MergeOption> mergeOption = null)
        {
            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.OutOrderPos_ParentOutOrderPos.Load(mergeOption.Value);
                else
                    this.OutOrderPos_ParentOutOrderPos.AutoLoad();
            }

            double sumActualQuantity = 0;
            double sumActualQuantityUOM = 0;
            foreach (OutOrderPos childPos in this.OutOrderPos_ParentOutOrderPos)
            {
                childPos.RecalcActualQuantity(mergeOption);
                sumActualQuantityUOM += childPos.ActualQuantityUOM;
                if (childPos.MDUnit == this.MDUnit)
                    sumActualQuantity += childPos.ActualQuantity;
                else if (this.Material != null)
                {
                    try
                    {
                        sumActualQuantity += Material.ConvertQuantity(childPos.ActualQuantity, childPos.MDUnit, this.MDUnit);
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        this.Root().Messages.LogException(ClassName, "RecalcActualQuantity", msg);
                    }
                }
            }

            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.FacilityBooking_OutOrderPos.Load(mergeOption.Value);
                else
                    this.FacilityBooking_OutOrderPos.AutoLoad();
            }
            foreach (FacilityBooking fb in FacilityBooking_OutOrderPos)
            {
                foreach (FacilityBookingCharge fbc in fb.FacilityBookingCharge_FacilityBooking)
                {
                    sumActualQuantityUOM += fbc.OutwardQuantityUOM;
                    try
                    {
                        sumActualQuantity += fbc.OutwardMaterial.ConvertQuantity(fbc.OutwardQuantity, fbc.MDUnit, this.MDUnit);
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        this.Root().Messages.LogException(ClassName, "RecalcActualQuantity(10)", msg);
                    }
                }
            }
            this.ActualQuantity = sumActualQuantity;
            this.ActualQuantityUOM = sumActualQuantityUOM;
        }


        //public double PreBookingQuantity
        //{
        //    get;
        //}

        //public double PreBookingQuantityUOM
        //{
        //    get;
        //}

        public double PreBookingOutwardQuantityUOM(Nullable<MergeOption> mergeOption = null)
        {
            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.OutOrderPos_ParentOutOrderPos.Load(mergeOption.Value);
                else
                    this.OutOrderPos_ParentOutOrderPos.AutoLoad();
            }
            double sumUOM = 0;
            foreach (OutOrderPos childPos in this.OutOrderPos_ParentOutOrderPos)
            {
                sumUOM += childPos.PreBookingOutwardQuantityUOM(mergeOption);
            }

            if (this.EntityState != System.Data.EntityState.Added)
            {
                if (mergeOption.HasValue)
                    this.FacilityPreBooking_OutOrderPos.Load(mergeOption.Value);
                else
                    this.FacilityPreBooking_OutOrderPos.AutoLoad();
            }
            foreach (FacilityPreBooking fb in FacilityPreBooking_OutOrderPos)
            {
                if (fb.OutwardQuantity.HasValue)
                {
                    try
                    {
                        sumUOM += Material.ConvertToBaseQuantity(fb.OutwardQuantity.Value, this.MDUnit);
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        this.Root().Messages.LogException(ClassName, "PreBookingOutwardQuantityUOM", msg);
                    }
                }
            }
            return sumUOM;
        }

        #endregion
    }
}
