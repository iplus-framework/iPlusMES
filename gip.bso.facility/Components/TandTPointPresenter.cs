using gip.core.datamodel;
using gip.core.autocomponent;
using System.Collections.Generic;
using System;
using System.Linq;
using vbModel = gip.mes.datamodel;
using gip.mes.processapplication;
using gip.mes.facility;

namespace gip.bso.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTPointPresenter'}de{'TandTPointPresenter'}", Global.ACKinds.TACObject, Global.ACStorableTypes.NotStorable, true, true)]
    public class TandTPointPresenter : IACInteractiveObject, IACObject, IACMenuBuilder
    {
        #region c´tors

        public TandTPointPresenter(IACObject content, IACObject parentComponent)
        {
            ItemType = content.GetType().Name;
            JobIds = new List<Guid>();
            MixPointIds = new List<Guid>();
            Content = content;
            ParentACObject = parentComponent;
        }

        #endregion

        #region Connection with data model
        public string ItemType { get; set; }

        public Guid ACObjectID { get; set; }
        public string ACObjectNo { get; set; }
        public List<Guid> MixPointIds { get; set; }


        [ACPropertyInfo(100, "Content", "en{'Content'}de{'Content'}")]
        public IACObject Content
        {
            get;
            set;
        }

        public Guid? RepresentMixPointID { get; set; }

        #endregion

        #region IACType

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public string ACCaption
        {
            get
            {
                return Content.ACCaption;
            }
        }
        #endregion

        #region Points

        public List<Guid> JobIds { get; set; }

        /// <summary>By setting a ACUrl in XAML, the Control resolves it by calling the IACObject.ACUrlBinding()-Method. 
        /// The ACUrlBinding()-Method returns a Source and a Path which the Control use to create a WPF-Binding to bind the right value and set the WPF-DataContext.
        /// ACUrl's can be either absolute or relative to the DataContext of the parent WPFControl (or the ContextACObject of the parent IACInteractiveObject)</summary>
        /// <value>Relative or absolute ACUrl</value>
        public string VBContent
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// ContextACObject is used by WPF-Controls and mostly it equals to the FrameworkElement.DataContext-Property.
        /// IACInteractiveObject-Childs in the logical WPF-tree resolves relative ACUrl's to this ContextACObject-Property.
        /// </summary>
        /// <value>The Data-Context as IACObject</value>
        public IACObject ContextACObject
        {
            get
            {
                return null;
            }
        }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType => this.ReflectACType();

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj != null)
            {
                TandTPointPresenter second = obj as TandTPointPresenter;
                if (second != null)
                {
                    isEqual = ItemType == second.ItemType && second.ACObjectID == ACObjectID;
                }
            }
            return isEqual;
        }

        public override string ToString()
        {
            return ItemType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public void ACAction(ACActionArgs actionArgs)
        {

            ACActionMenuArgs menuArgs = actionArgs as ACActionMenuArgs;
            if (menuArgs != null)
            {
                ACMenuItemList result = this.GetMenu("", "");
                var menuList = menuArgs.ACMenuItemList.ToList();
                menuList.AddRange(result);
                menuArgs.ACMenuItemList = menuList.ToArray();
                return;
            }

            if (actionArgs.ElementAction == Global.ElementActionType.ACCommand)
            {

                ACCommand acCommand = actionArgs.DropObject.ACContentList.Where(c => c is ACCommand).FirstOrDefault() as ACCommand;
                if (acCommand != null)
                {
                    if (Enum.GetNames(typeof(PresenterMenuItems)).Contains(acCommand.ACUrl))
                    {
                        using (vbModel.DatabaseApp databaseApp = new vbModel.DatabaseApp())
                        {
                            PresenterMenuItems menuItemType = (PresenterMenuItems)Enum.Parse(typeof(PresenterMenuItems), acCommand.ACUrl);
                            BSOTandTv3 parentBSO = ParentACObject as BSOTandTv3;
                            PAShowDlgManagerVB manager = PAShowDlgManagerVB.GetServiceInstance((ParentACObject as BSOTandTv3).Root as ACComponent) as PAShowDlgManagerVB;
                            if (manager == null) return;
                            PAOrderInfoEntry pAOrderInfoEntry = null;
                            switch (menuItemType)
                            {
                                case PresenterMenuItems.ShowDetails:
                                    (ParentACObject as BSOTandTv3).ShowDetails(this);
                                    break;
                                case PresenterMenuItems.FacilityBookCell:
                                    manager.ShowFacilityBookCellDialog((Content as vbModel.Facility).FacilityNo);
                                    break;
                                case PresenterMenuItems.FacilityOverview:
                                    string facilityNo = (Content as FacilityPreview).FacilityNo;
                                    DateTime? searchFrom = null;
                                    DateTime? searchTo = null;
                                    if (parentBSO.Result.MixPoints != null)
                                    {
                                        var mandatoryBookings =
                                            parentBSO.Result.MixPoints.SelectMany(c => c.InwardBookings)
                                            .Union(parentBSO.Result.MixPoints.SelectMany(c => c.OutwardBookings))
                                            .Where(c => c.FacilityNo == facilityNo);
                                        if (mandatoryBookings.Any())
                                        {
                                            searchFrom = mandatoryBookings.Min(c => c.InsertDate);
                                            searchTo = mandatoryBookings.Max(c => c.InsertDate);
                                            searchFrom = searchFrom.Value.Date;
                                            searchTo = searchTo.Value.Date.AddDays(1);
                                        }
                                    }
                                    manager.ShowFacilityOverviewDialog(facilityNo, searchFrom, searchTo);
                                    break;
                                case PresenterMenuItems.ProdOrderPartslist:
                                    pAOrderInfoEntry = null;
                                    if (Content is vbModel.ProdOrder)
                                    {
                                        vbModel.ProdOrder prodOrder = Content as vbModel.ProdOrder;
                                        Guid prodOrderPartslistID =
                                           databaseApp
                                            .ProdOrderPartslist
                                            .Where(c => c.ProdOrderID == prodOrder.ProdOrderID)
                                            .OrderBy(c => c.Sequence)
                                            .Select(c => c.ProdOrderPartslistID)
                                            .FirstOrDefault();
                                        pAOrderInfoEntry = new PAOrderInfoEntry()
                                        {
                                            EntityID = prodOrderPartslistID,
                                            EntityName = vbModel.ProdOrderPartslist.ClassName
                                        };
                                    }
                                    // TandTv3Point v TandTv3PointPosGrouped
                                    if (Content is TandTv3Point)
                                    {
                                        TandTv3Point tandTv3Point = Content as TandTv3Point;
                                        if (tandTv3Point.ProductionPositions != null && tandTv3Point.ProductionPositions.Any())
                                            pAOrderInfoEntry = new PAOrderInfoEntry()
                                            {
                                                EntityID = tandTv3Point.ProductionPositions.FirstOrDefault().ProdOrderBatchID ?? Guid.Empty,
                                                EntityName = vbModel.ProdOrderBatch.ClassName
                                            };
                                        else if (tandTv3Point.ProdOrder != null)
                                            pAOrderInfoEntry = new PAOrderInfoEntry()
                                            {
                                                EntityID = tandTv3Point.ProdOrder.ProdOrderPartslist_ProdOrder.FirstOrDefault().ProdOrderPartslistID,
                                                EntityName = vbModel.ProdOrderPartslist.ClassName
                                            };
                                    }

                                    break;
                                case PresenterMenuItems.DeliveryNotePos:
                                    pAOrderInfoEntry = null;
                                    if (Content is TandTv3PointDN)
                                    {
                                        TandTv3Point tandTv3Point = Content as TandTv3Point;
                                        if (tandTv3Point.IsInputPoint && tandTv3Point.InOrderPositions != null)
                                        {
                                            Guid[] inOrderPosiDs = tandTv3Point.InOrderPositions.Select(c => c.InOrderPosID).ToArray();
                                            vbModel.DeliveryNotePos dns = databaseApp.DeliveryNotePos.Where(c => inOrderPosiDs.Contains(c.InOrderPosID ?? Guid.Empty)).FirstOrDefault();
                                            if (dns != null)
                                                pAOrderInfoEntry = new PAOrderInfoEntry()
                                                {
                                                    EntityID = dns.DeliveryNotePosID,
                                                    EntityName = vbModel.DeliveryNotePos.ClassName
                                                };
                                        }
                                    }
                                    break;
                                case PresenterMenuItems.InOrderPos:
                                    if (Content is vbModel.InOrderPosPreview)
                                    {
                                        vbModel.InOrderPosPreview inOrderPos = Content as vbModel.InOrderPosPreview;
                                        manager.ShowInOrderDialog(inOrderPos.InOrderNo, inOrderPos.ID);
                                    }
                                    break;
                                case PresenterMenuItems.OutOrderPos:
                                    break;
                                case PresenterMenuItems.PickingPos:
                                    pAOrderInfoEntry = null;
                                    if (Content is TandTv3PointDN)
                                    {
                                        TandTv3PointDN tandTv3Point = Content as TandTv3PointDN;
                                        if (tandTv3Point.IsInputPoint && tandTv3Point.PickingPosPreviews != null && tandTv3Point.PickingPosPreviews.Any())
                                        {
                                            Guid[] pickingPosIDs = tandTv3Point.PickingPosPreviews.SelectMany(c => c.PickingPosIDs).ToArray();
                                            //vbModel.pir dns = databaseApp.Where(c => pickingPosIDs.Contains(c.InOrderPosID ?? Guid.Empty)).FirstOrDefault();
                                            pAOrderInfoEntry = new PAOrderInfoEntry()
                                            {
                                                EntityID = pickingPosIDs.FirstOrDefault(),
                                                EntityName = vbModel.PickingPos.ClassName
                                            };
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            if (pAOrderInfoEntry == null)
                                return;
                            PAOrderInfo pAOrderInfo = new PAOrderInfo();
                            pAOrderInfo.Entities.Add(pAOrderInfoEntry);
                            manager.ShowDialogOrder(ParentACObject as ACComponent, pAOrderInfo);
                        }
                    }
                    else
                        ACUrlCommand(acCommand.ACUrl, acCommand.ParameterList.ToValueArray());
                }

            }
        }

        /// <summary>
        /// It's called at the Target-IACInteractiveObject to inform the Source-IACInteractiveObject that ACAction ist allowed to be invoked.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        /// <returns><c>true</c> if ACAction can be invoked otherwise, <c>false</c>.</returns>
        public bool IsEnabledACAction(ACActionArgs actionArgs)
        {
            return true;
        }

        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ACIdentifier;
        }

        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        public ACMenuItemList GetMenu(string vbContent, string vbControl)
        {
            ACMenuItemList result = new ACMenuItemList();

            IEnumerable<ACClassMethod> methods;
            using (ACMonitor.Lock(Database.GlobalDatabase.QueryLock_1X000))
                methods = ACType.ValueTypeACClass.ACClassMethod_ACClass.Where(c => c.IsInteraction).OrderBy(c => c.SortIndex).ThenBy(x => x.ACCaption).ToArray();

            foreach (var method in methods)
            {
                ACMenuItem item;
                if (this.Root().CurrentInvokingUser != null)
                    item = new ACMenuItem(method.GetTranslation(this.Root().CurrentInvokingUser.VBLanguage.VBLanguageCode), "!" + method.ACIdentifier, method.SortIndex, null, "", false, this);
                else
                    item = new ACMenuItem(method.ACCaption, "!" + method.ACIdentifier, method.SortIndex, null, "", false, this);

                item.IconACUrl = method.GetIconACUrl();
                result.Add(item);
            }

            vbModel.MDTrackingStartItemTypeEnum contentItemType = (vbModel.MDTrackingStartItemTypeEnum)Enum.Parse(typeof(vbModel.MDTrackingStartItemTypeEnum), ItemType);
            bool validEnviroment = ParentACObject != null && Content != null;

            if (validEnviroment)
            {
                vbModel.MDTrackingStartItemTypeEnum[] mixPointTypes = new vbModel.MDTrackingStartItemTypeEnum[]
                {
                    vbModel.MDTrackingStartItemTypeEnum.TandTv3Point, vbModel.MDTrackingStartItemTypeEnum.TandTv3PointPosGrouped
                };
                if (mixPointTypes.Contains(contentItemType))
                {
                    result.Add(new ACMenuItem("en{'Show details'}de{'Details anzeigen'}", PresenterMenuItems.ShowDetails.ToString(), 250, null));
                    TandTv3Point tandTv3Point = Content as TandTv3Point;
                    result.Add(new ACMenuItem("en{'View order'}de{'Auftrag anschauen'}", PresenterMenuItems.ProdOrderPartslist.ToString(), 250, null));
                }

                if (contentItemType == vbModel.MDTrackingStartItemTypeEnum.TandTv3PointDN)
                {
                    TandTv3Point tandTv3Point = Content as TandTv3Point;
                    result.Add(new ACMenuItem("en{'Dialog delivery note'}de{'Dialog Lieferschein'}", PresenterMenuItems.DeliveryNotePos.ToString(), 250, null));
                }

                if (ItemType == vbModel.MDTrackingStartItemTypeEnum.InOrderPosPreview.ToString())
                {
                    TandTv3Point tandTv3Point = Content as TandTv3Point;
                    result.Add(new ACMenuItem("en{'Dialog Purchase Order'}de{'Dialog Bestellung'}", PresenterMenuItems.InOrderPos.ToString(), 250, null));
                }

                if (ItemType == vbModel.MDTrackingStartItemTypeEnum.OutOrderPosPreview.ToString())
                {
                    TandTv3Point tandTv3Point = Content as TandTv3Point;
                    result.Add(new ACMenuItem("en{'Dialog Sales Order'}de{'Dialog Kundenauftrag'}", PresenterMenuItems.OutOrderPos.ToString(), 250, null));
                }

                if (contentItemType == vbModel.MDTrackingStartItemTypeEnum.PickingPosPreview)
                {
                    TandTv3Point tandTv3Point = Content as TandTv3Point;
                    result.Add(new ACMenuItem("en{'Dialog Picking Order'}de{'Dialog Kommissionierauftrag'}", PresenterMenuItems.PickingPos.ToString(), 250, null));
                }

                if (contentItemType == vbModel.MDTrackingStartItemTypeEnum.Facility || contentItemType == vbModel.MDTrackingStartItemTypeEnum.FacilityPreview)
                {
                    //result.Add(new ACMenuItem("en{'Manage stock'}de{'Bestand verwalten'}", PresenterMenuItems.FacilityBookCell.ToString(), 250, null));
                    result.Add(new ACMenuItem("en{'Stockhistory'}de{'Bestandshistorie'}", PresenterMenuItems.FacilityOverview.ToString(), 250, null));
                }

                if (contentItemType == vbModel.MDTrackingStartItemTypeEnum.ProdOrder)
                {
                    result.Add(new ACMenuItem("en{'View order'}de{'Auftrag anschauen'}", PresenterMenuItems.ProdOrderPartslist.ToString(), 250, null));
                }
            }

            return result;
        }

        #endregion

    }

    public class TandTEdge : IACEdge, IACObject
    {
        public IACPointBase Source => throw new NotImplementedException();

        public IACPointBase Target => throw new NotImplementedException();

        public IACObject SourceParent { get; set; }

        public IACObject TargetParent { get; set; }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier { get; }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public string ACCaption { get; }

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType { get; }

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return false;
        }

        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return null;
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return ACIdentifier;
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return false;
        }
    }

    public class TandTPath : List<TandTEdge>
    {

    }

    public enum PresenterMenuItems
    {
        ShowDetails,
        FacilityBookCell,
        FacilityOverview,
        ProdOrderPartslist,
        DeliveryNotePos,
        InOrderPos,
        OutOrderPos,
        PickingPos
    }
}
