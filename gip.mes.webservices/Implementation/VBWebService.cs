// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.webservices;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.reporthandler;

namespace gip.mes.webservices
{
    public partial class VBWebService : CoreWebService, IVBWebService
    {
        public WSResponse<BarcodeEntity> GetBarcodeEntity(string barcodeID)
        {
            if (string.IsNullOrEmpty(barcodeID))
                return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "barcodeID is empty"));

            var result01 = GetFacilityChargeByBarcode(barcodeID);
            if (result01.Suceeded && result01.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { FacilityCharge = result01.Data });

            var result02 = GetFacilityLotByBarcode(barcodeID);
            if (result02.Suceeded && result02.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { FacilityLot = result02.Data });

            var result03 = GetMaterialByBarcode(barcodeID);
            if (result03.Suceeded && result03.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { Material = result03.Data });

            var result04 = GetFacilityByBarcode(barcodeID);
            if (result04.Suceeded && result04.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { Facility = result04.Data });

            var result05 = GetACClassByBarcode(barcodeID);
            if (result05.Suceeded && result05.Data != null)
                return new WSResponse<BarcodeEntity>(new BarcodeEntity() { ACClass = result05.Data });

            return new WSResponse<BarcodeEntity>(null, new Msg(eMsgLevel.Error, "Unknown barcode"));
        }

        public WSResponse<BarcodeSequence> InvokeBarcodeSequence(BarcodeSequence sequence)
        {
            if (String.IsNullOrEmpty(sequence.CurrentBarcode) && sequence.State < BarcodeSequence.ActionState.Question)
                return new WSResponse<BarcodeSequence>(sequence, new Msg(eMsgLevel.Error, "barcodeID is empty"));

            if (   sequence.State == BarcodeSequence.ActionState.Completed
                || sequence.State == BarcodeSequence.ActionState.Cancelled
                || sequence.Sequence == null)
            {
                sequence.Sequence = new List<BarcodeEntity>();
            }
            if (sequence.State < BarcodeSequence.ActionState.Question || sequence.State == BarcodeSequenceBase.ActionState.SelectionScanAgain)
            {
                WSResponse<BarcodeEntity> nextEntity = GetBarcodeEntity(sequence.CurrentBarcode);

                if (!nextEntity.Suceeded)
                {
                    sequence.State = BarcodeSequence.ActionState.Cancelled;
                    sequence.Message = nextEntity.Message;
                    return new WSResponse<BarcodeSequence>(sequence, nextEntity.Message);
                }

                if (sequence.State == BarcodeSequenceBase.ActionState.SelectionScanAgain && nextEntity != null && nextEntity.Data.ACClass != null)
                {
                    if (sequence.Sequence.Any(c => c.ACClass != null))
                    {
                        sequence.Sequence.Clear();
                    }
                }

                sequence.AddSequence(nextEntity.Data);
            }
            else if (sequence.State == datamodel.BarcodeSequenceBase.ActionState.Selection
                    || sequence.State == BarcodeSequenceBase.ActionState.FastSelection)
            {
                BarcodeEntity lastEntity = sequence.Sequence.LastOrDefault();
                if (lastEntity.SelectedOrderWF == null && lastEntity.WFMethod == null)
                {
                    sequence.Sequence.Clear();
                    WSResponse<BarcodeEntity> nextEntity = GetBarcodeEntity(sequence.CurrentBarcode);

                    if (!nextEntity.Suceeded)
                    {
                        sequence.State = BarcodeSequence.ActionState.Cancelled;
                        sequence.Message = nextEntity.Message;
                        return new WSResponse<BarcodeSequence>(sequence, nextEntity.Message);
                    }

                    sequence.AddSequence(nextEntity.Data);

                    //return new WSResponse<BarcodeSequence>(sequence, new Msg(eMsgLevel.Error, "SelectedOrderWF is empty"));
                }
            }

            if (sequence.Sequence != null)
            {
                foreach (var entry in sequence.Sequence)
                {
                    if (entry.WFMethod != null)
                        entry.WFMethod.FullSerialization = true;
                    if (entry.SelectedOrderWF != null && entry.SelectedOrderWF.WFMethod != null)
                        entry.SelectedOrderWF.WFMethod.FullSerialization = true;
                    if (entry.OrderWFInfos != null && entry.OrderWFInfos.Any())
                    {
                        foreach (var orderWFInfo in entry.OrderWFInfos)
                        {
                            if (orderWFInfo.WFMethod != null)
                                orderWFInfo.WFMethod.FullSerialization = true;
                        }
                    }
                }
            }

            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            if (myServiceHost == null)
                return new WSResponse<BarcodeSequence>(sequence, new Msg(eMsgLevel.Error, "PAJsonServiceHostVB not found"));

            PAEScannerDecoderWS decoder = myServiceHost.FindChildComponents<PAEScannerDecoderWS>(c => c is PAEScannerDecoderWS).FirstOrDefault();
            if (decoder == null)
                return new WSResponse<BarcodeSequence>(sequence, new Msg(eMsgLevel.Error, "PAEScannerDecoderWS not found"));

            return decoder.OnHandleNextBarcodeSequence(sequence);
        }

        public WSResponse<bool> Print(PrintEntity printEntity)
        {
            if (printEntity.Sequence == null || !printEntity.Sequence.Any())
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "No elements in Barcode Entity sequence!"));
            PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
            ACPrintManager printManager = ACPrintManager.GetServiceInstance(myServiceHost);
            if (printManager == null)
                return new WSResponse<bool>(false, new Msg(eMsgLevel.Error, "PrintManager instance is null!"));
            PAOrderInfo pAOrderInfo = GetPAOrderInfo(printEntity.Sequence.ToArray());

            string vbUserName = null;
            Guid? currentSessionID = WSRestAuthorizationManager.CurrentSessionID;
            if (currentSessionID.HasValue && myServiceHost != null)
            {
                VBUserRights userRights = myServiceHost.GetRightsForSession(currentSessionID.Value);
                if (userRights == null)
                {
                    return new WSResponse<bool>(false, WSResponse<bool>.LoginAgainMessage);
                }
                else
                {
                    vbUserName = userRights.UserName;
                }
            }

            if (printEntity.CopyCount <= 0)
                return new WSResponse<bool>(true);

            Msg msg = printManager.Print(pAOrderInfo, printEntity.CopyCount, vbUserName, printEntity.MaxPrintJobsInSpooler) as Msg;
            if (msg != null && msg.MessageLevel != eMsgLevel.Info)
            {
                msg.RedirectToOtherSource(myServiceHost);
                myServiceHost.IsServiceAlarm.ValueT = PANotifyState.AlarmOrFault; 
                if (myServiceHost.IsAlarmActive(myServiceHost.IsServiceAlarm, msg.Message) == null)
                    myServiceHost.Messages.LogMessageMsg(msg);
                myServiceHost.OnNewAlarmOccurred(myServiceHost.IsServiceAlarm, msg, true);
            }

            return new WSResponse<bool>(msg == null) { Message = msg };
        }

        private PAOrderInfo GetPAOrderInfo(BarcodeEntity[] entities)
        {
            PAOrderInfo pAOrderInfo = new PAOrderInfo();
            foreach(BarcodeEntity barcodeEntity in entities)
                if (barcodeEntity.FacilityCharge != null)
                    pAOrderInfo.Add(datamodel.FacilityCharge.ClassName, barcodeEntity.FacilityCharge.FacilityChargeID);
            return pAOrderInfo;
        }

        protected override void OnGetKnownTypes4Translation(ref List<Tuple<Type, Type>> knownTypes)
        {
            base.OnGetKnownTypes4Translation(ref knownTypes);
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.Facility), typeof(webservices.Facility)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.FacilityCharge), typeof(webservices.FacilityCharge)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.FacilityLot), typeof(webservices.FacilityLot)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.FacilityLotStock), typeof(webservices.FacilityLotStock)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.FacilityStock), typeof(webservices.FacilityStock)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.Material), typeof(webservices.Material)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.MaterialStock), typeof(webservices.MaterialStock)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.MDFacilityType), typeof(webservices.MDFacilityType)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.MDReleaseState), typeof(webservices.MDReleaseState)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.MDMovementReason), typeof(webservices.MDMovementReason)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.Picking), typeof(webservices.Picking)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.PickingPos), typeof(webservices.PickingPos)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.MDUnit), typeof(webservices.MDUnit)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(facility.ACMethodBooking), typeof(webservices.ACMethodBooking)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(FacilityChargeSumMaterialHelper), typeof(FacilityChargeSumMaterialHelper)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(FacilityChargeSumLotHelper), typeof(FacilityChargeSumLotHelper)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(FacilityChargeSumLocationHelper), typeof(FacilityChargeSumLocationHelper)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(FacilityChargeSumFacilityHelper), typeof(FacilityChargeSumFacilityHelper)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(FacilityBookingOverview), typeof(FacilityBookingOverview)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(FacilityBookingChargeOverview), typeof(FacilityBookingChargeOverview)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.Partslist), typeof(webservices.Partslist)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.ProdOrderPartslist), typeof(webservices.ProdOrderPartslist)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.ProdOrderPartslistPos), typeof(webservices.ProdOrderPartslistPos)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.ProdOrder), typeof(webservices.ProdOrder)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.ProdOrderBatch), typeof(webservices.ProdOrderBatch)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.ProdOrderPartslistPosRelation), typeof(webservices.ProdOrderPartslistPosRelation)));

            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.FacilityInventory), typeof(webservices.FacilityInventory)));
            knownTypes.Add(new Tuple<Type, Type>(typeof(datamodel.FacilityInventoryPos), typeof(webservices.FacilityInventoryPos)));
        }

        public WSResponse<string> GetAssignedPrinter()
        {
            WSResponse<string> response = new WSResponse<string>();

            try
            {
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
                Guid? currentSessionID = WSRestAuthorizationManager.CurrentSessionID;
                if (currentSessionID.HasValue && myServiceHost != null)
                {
                    VBUserRights userRights = myServiceHost.GetRightsForSession(currentSessionID.Value);
                    if (userRights != null)
                    {
                        using (Database db = new Database())
                        {
                            core.datamodel.VBUser user = db.VBUser.FirstOrDefault(c => c.VBUserName == userRights.UserName);
                            if (user != null)
                            {
                                ACPrintManager printManager = ACPrintManager.GetServiceInstance(myServiceHost);
                                if (printManager != null)
                                {
                                    var configuredPrinters = ACPrintManager.GetConfiguredPrinters(db, printManager.ComponentClass.ACClassID, false);
                                    PrinterInfo info = configuredPrinters.FirstOrDefault(c => c.VBUserID == user.VBUserID);
                                    if (info != null)
                                    {
                                        if (string.IsNullOrEmpty(info.PrinterName))
                                            response.Data = info.PrinterACUrl;
                                        else
                                            response.Data = info.PrinterName;
                                    }
                                }
                                else
                                {
                                    response.Message = new Msg(eMsgLevel.Error, "printManager is null.");
                                }
                            }
                            else
                            {
                                response.Message = new Msg(eMsgLevel.Error, "myServiceHost or currentSessionID is null.");
                            }
                        }
                    }
                    else
                    {
                        response.Message = new Msg(eMsgLevel.Error, "userRights for session cannot be found.");
                    }
                }
                else
                {
                    response.Message = new Msg(eMsgLevel.Error, "myServiceHost or currentSessionID is null.");
                }
            }
            catch (Exception e)
            {
                response.Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            return response;
        }

        public WSResponse<string> GetScannedPrinter(string printerID)
        {
            if (string.IsNullOrEmpty(printerID))
            {
                return new WSResponse<string>(new Msg(eMsgLevel.Error, "parameter printerID is empty!"));
            }

            WSResponse<string> response = new WSResponse<string>();

            try
            {
                ACRoot root = gip.core.datamodel.Database.Root as ACRoot;

                List<PrinterInfo> windowsPrinters = new List<PrinterInfo>();
                var printers = root?.WPFServices?.VBMediaControllerService?.GetWindowsPrinters();
                if (printers != null)
                    windowsPrinters = ACPrintManager.GetPrinters(printers);
                if (windowsPrinters != null)
                {
                    PrinterInfo pInfo = windowsPrinters.FirstOrDefault(c => c.PrinterName == printerID);
                    if (pInfo != null)
                    {
                        response.Data = pInfo.PrinterName;
                    }
                    else
                    {
                        //TODO: translation
                        response.Message = new Msg(eMsgLevel.Error, "Scanned printer cannot be found!");
                    }
                }
            }
            catch (Exception e)
            {
                response.Message = new Msg(eMsgLevel.Exception, e.Message);
            }

            return response;
        }

        public WSResponse<string> AssignPrinter(string printerID)
        {
            WSResponse<string> response = new WSResponse<string>();

            if (string.IsNullOrEmpty(printerID))
            {
                response.Message = new Msg(eMsgLevel.Error, "Parameter printerID is null.");
                return response;
            }

            try
            {
                PAJsonServiceHostVB myServiceHost = PAWebServiceBase.FindPAWebService<PAJsonServiceHostVB>(WSRestAuthorizationManager.ServicePort);
                Guid? currentSessionID = WSRestAuthorizationManager.CurrentSessionID;
                if (currentSessionID.HasValue && myServiceHost != null)
                {
                    VBUserRights userRights = myServiceHost.GetRightsForSession(currentSessionID.Value);
                    if (userRights != null)
                    {
                        using (Database db = new Database())
                        {
                            Guid printerACClassID;
                            core.datamodel.ACClass printerACClass = null;
                            if (Guid.TryParse(printerID, out printerACClassID))
                            {
                                printerACClass = db.ACClass.FirstOrDefault(c => c.ACClassID == printerACClassID);
                                if (printerACClass == null)
                                {
                                    response.Message = new Msg(eMsgLevel.Error, "ACClass for scanned printer cannot be found!");
                                    return response;
                                }
                            }

                            core.datamodel.VBUser user = db.VBUser.FirstOrDefault(c => c.VBUserName == userRights.UserName);
                            if (user != null)
                            {
                                ACPrintManager printManager = ACPrintManager.GetServiceInstance(myServiceHost);
                                if (printManager != null)
                                {
                                    var configuredPrinters = ACPrintManager.GetConfiguredPrinters(db, printManager.ComponentClass.ACClassID, false);
                                    PrinterInfo info = configuredPrinters.FirstOrDefault(c => c.VBUserID == user.VBUserID);

                                    if (info != null)
                                    {
                                        if (info.PrinterName == printerID || (printerACClass != null && info.PrinterACUrl == printerACClass.ACUrlComponent))
                                        {
                                            if (printerACClass != null)
                                                response.Data = printerACClass.ACUrlComponent;
                                            else
                                                response.Data = printerID;
                                        }
                                        else
                                        {
                                            Msg msg = printManager.UnAssignPrinter(db, info);
                                            if (msg == null)
                                            {
                                                string printer = printerID;

                                                PrinterInfo printerInfo = new PrinterInfo();
                                                printerInfo.VBUserID = user.VBUserID;

                                                if (printerACClass != null)
                                                {
                                                    printer = printerACClass.ACUrlComponent;
                                                    printerInfo.PrinterACUrl = printer;
                                                }
                                                else
                                                    printerInfo.PrinterName = printer;

                                                msg = printManager.AssignPrinter(db, printerInfo);
                                                response.Message = msg;
                                                response.Data = printer;
                                            }
                                            else
                                            {
                                                response.Message = msg;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string printer = printerID;

                                        PrinterInfo printerInfo = new PrinterInfo();
                                        printerInfo.VBUserID = user.VBUserID;

                                        if (printerACClass != null)
                                        {
                                            printer = printerACClass.ACUrlComponent;
                                            printerInfo.PrinterACUrl = printer;
                                        }
                                        else
                                            printerInfo.PrinterName = printer;

                                        Msg msg = printManager.AssignPrinter(db, printerInfo);
                                        response.Message = msg;
                                        response.Data = printer;
                                    }
                                }
                                else
                                {
                                    response.Message = new Msg(eMsgLevel.Error, "printManager is null.");
                                }
                            }
                            else
                            {
                                response.Message = new Msg(eMsgLevel.Error, "myServiceHost or currentSessionID is null.");
                            }
                        }
                    }
                    else
                    {
                        response.Message = new Msg(eMsgLevel.Error, "userRights for session cannot be found.");
                    }
                }
                else
                {
                    response.Message = new Msg(eMsgLevel.Error, "myServiceHost or currentSessionID is null.");
                }
            }
            catch (Exception e)
            {
                response.Message = new Msg(eMsgLevel.Exception, e.Message);
            }
            return response;
        }

        protected WSResponse<T> SetDatabaseUserName<T>(DatabaseApp dbApp)
        {
            if (dbApp == null)
                return null;
            var userRights = InvokingUser;
            if (userRights == null)
                return new WSResponse<T>(default(T), WSResponse<MsgWithDetails>.LoginAgainMessage);
            dbApp.UserName = userRights.Initials;
            return null;
        }

        protected WSResponse<T> SetDatabaseUserName<T>(DatabaseApp dbApp, PAJsonServiceHostVB myServiceHost)
        {
            if (dbApp == null || myServiceHost == null)
                return null;
            VBUserRights userRights = GetInvokingUser(myServiceHost);
            if (userRights == null)
                return new WSResponse<T>(default(T), WSResponse<MsgWithDetails>.LoginAgainMessage);
            dbApp.UserName = userRights.Initials;
            return null;
        }
    }
}
