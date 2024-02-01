using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.communication;
using gip.core.communication.ISOonTCP;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using System.Windows;
using gip.mes.processapplication;
using System.Security.Policy;

namespace gip2006.variobatch.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Serializer for Way'}de{'Serialisierer für Way'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPFuncSerial2006Way : ACSessionObjSerializer
    {
        public GIPFuncSerial2006Way(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return false;
        }

        public override object ReadObject(object complexObj, int dbNo, int offset, object miscParams)
        {
            throw new NotImplementedException();
        }

        [ACMethodInfo("", "", 999)]
        public override bool SendObject(object complexObj, object prevComplexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            string message = null;
            return SendRoute(null, complexObj, prevComplexObj, dbNo, offset, routeOffset, miscParams, out message);
        }

        public virtual bool SendRoute(ACSessionObjSerializer invoker, object complexObj, object prevComplexObj, int dbNo, int offset, int? routeOffset, object miscParams, out string message)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            //if (s7Session == null || complexObj == null)
            //    return false;
            //if (!s7Session.PLCConn.IsConnected)
            //    return false;
            ACMethod request = complexObj as ACMethod;
            if (request == null)
            {
                message = "ACMethod is null";
                return false;
            }

            Route route = request.ParameterValueList.Where(c => c.ACIdentifier == nameof(Route)).FirstOrDefault()?.Value as Route;
            if (route == null || !route.Any())
            {
                message = "route is empty";
                return false;
            }


            IACComponent destinationChangeComponent = null;
            Route previousRoute = null;
            ACMethod previousRequest = prevComplexObj as ACMethod;
            if (previousRequest != null)
            {
                previousRoute = previousRequest.ParameterValueList.Where(c => c.ACIdentifier == nameof(Route)).FirstOrDefault()?.Value as Route;
                if (previousRoute != null)
                {
                    route.Compare(previousRoute, out destinationChangeComponent);
                }
            }


            int maxPackageSize = 200;
            int maxRouteItems = 49;
            string errorMsg = null;
            List<byte[]> sendPackages = CreateSendPackages(route, maxRouteItems, maxPackageSize, out errorMsg, destinationChangeComponent);
            if (sendPackages == null)
            {
                if (invoker == null && errorMsg != null)
                    Messages.LogError(this.GetACUrl(), "SendRoute(10)", errorMsg);
                message = errorMsg;
                return false;
            }


            if (s7Session.IsConnectionLocalSim)
            {
                message = null;
                return true;
            }

            foreach (byte[] package in sendPackages)
            {
                byte[] pack = package;
                PLC.Result errCode = s7Session.PLCConn.WriteBytes(DataTypeEnum.DataBlock, dbNo, offset, ref pack);
                offset += pack.Length;
                if (errCode != null && !errCode.IsSucceeded)
                {
                    message = String.Format("PLC Write Error {0}", errCode);
                    return false;
                }
            }

            message = null;
            return true;
        }

        private List<byte[]> CreateSendPackages(Route route, int maxRouteItems, int maxPackageSize, out string errorMessage, IACComponent destinationChangeComponent = null)
        {
            int headerSize = 0, routeItemSize = 0, definedMaxPackSize = maxPackageSize;
            //errorMessage = null;

            #region Header size
            headerSize += gip.core.communication.ISOonTCP.Types.Word.Length; //VARIOBATCH Command
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Destination change component
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Source
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Target
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //SecondTarget
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Third Target
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Fourth Target
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Unused = 0
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Unused = 0
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Unused = 0
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Unused = 0
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Unused = 1
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //TargetFull
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Unused = 0
            headerSize += gip.core.communication.ISOonTCP.Types.Int.Length; //Unused = 0
            #endregion

            #region Route item size
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//GIPConv2006Base.AggrNo from Converter-Child-Instance.
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Aggregat-Typ
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Source
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Target
            routeItemSize += gip.core.communication.ISOonTCP.Types.Byte.Length;//First group
            routeItemSize += gip.core.communication.ISOonTCP.Types.Byte.Length;//Second group
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//TurnOn delay
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//TurnOff delay
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Second target
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Para1
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Para2
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Para3
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Para4
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Para5
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Second source
            routeItemSize += gip.core.communication.ISOonTCP.Types.Int.Length;//Third source
            #endregion

            if (maxPackageSize < headerSize || maxPackageSize < routeItemSize)
            {
                errorMessage = String.Format("Maximum package size {0} is lower than headerSize {1} or routeItemSize {2}", maxPackageSize, headerSize, routeItemSize);
                return null;
            }

            int remaingSize = maxPackageSize - headerSize;
            if (remaingSize < routeItemSize)
                maxPackageSize = headerSize;
            else
                maxPackageSize = headerSize + routeItemSize * (remaingSize / routeItemSize);

            List<byte[]> sendPackages = new List<byte[]>();
            byte[] sendPackage = new byte[maxPackageSize];
            sendPackages.Add(sendPackage);

            RouteItem sourceItem = route.GetRouteSource();
            IEnumerable<RouteItem> targetItems = route.GetRouteTargets();
            if (targetItems.Count() > 4)
            {
                errorMessage = String.Format("Route has too much targets {0}", targetItems.Count());
                return null;
            }

            StringBuilder sb = new StringBuilder();
            short sourceItemID = GetRouteItemID(sourceItem.SourceACComponent);
            if (sourceItemID <= 0)
                sb.AppendLine(String.Format("RouteItemID {1} of source {0} is invalid", sourceItem.SourceACComponent.GetACUrl(), sourceItemID));

            short[] targetItemIDs = new short[] { 0, 0, 0, 0 };
            short i = 0;
            foreach (RouteItem rItem in targetItems)
            {
                short itemID = GetRouteItemID(rItem.TargetACComponent);
                if (itemID <= 0)
                    sb.AppendLine(String.Format("RouteItemID {1} of target {0} is invalid", rItem.TargetACComponent.GetACUrl(), itemID));

                if (targetItemIDs.Where(c => c == itemID).Any())
                    continue;
                targetItemIDs[i] = itemID;
                if (i > 3)
                    break;
                i++;
            }

            short destinationChangeAggrNo = 0;
            if (destinationChangeComponent != null)
            {
                destinationChangeAggrNo = GetRouteItemID(destinationChangeComponent);
                if (destinationChangeAggrNo <= 0)
                    sb.AppendLine(String.Format("RouteItemID {1} of destination change component {0} is invalid", destinationChangeComponent.GetACUrl(), destinationChangeAggrNo));
            }

            
            List<RouteItemModel> routeItemsModel = null;
            if (!route.AreACUrlInfosSet)
            {
                using (Database db = new gip.core.datamodel.Database())
                {
                    route.AttachTo(db);
                    route.Detach(true);
                }
            }
            
            bool validModel = GenerateRouteItemsModel(route, sourceItem, targetItems, sb, out routeItemsModel);
            if (!validModel || sb.Length > 0)
            {
                errorMessage = sb.ToString();
                return null;
            }

            int iOffset = 0;

            #region Header
            //VARIOBATCH Command
            iOffset += gip.core.communication.ISOonTCP.Types.Word.Length;

            //Destination change component
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(destinationChangeAggrNo), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Source
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(sourceItemID), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Target
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(targetItemIDs[0]), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //SecondTarget
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(targetItemIDs[1]), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Third Target
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(targetItemIDs[2]), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Fourth Target
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(targetItemIDs[3]), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Unused = 0
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Unused = 0
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Unused = 0
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Unused = 0
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Unused = 1
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(1), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //TargetFull
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray((short)routeItemsModel.Count), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Unused = 0
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Unused = 0
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            #endregion

            //ROUTE ITEMS

            int emptyRItemModelCount = maxRouteItems - routeItemsModel.Count();
            if (emptyRItemModelCount < 0)
            {
                errorMessage = String.Format("Too much items: {2}. Maximum item size is {1}. Oversize is {0}.", emptyRItemModelCount, maxRouteItems, routeItemsModel.Count());
                return null;
            }

            RouteItemModel emptyModel = new RouteItemModel();
            for (int j = 0; j < emptyRItemModelCount; j++)
                routeItemsModel.Add(emptyModel);

            int totalOffset = 0;
            bool recalcMaxPackSize = true;

            foreach (RouteItemModel routeItemModel in routeItemsModel)
            {
                if (iOffset >= maxPackageSize)
                {
                    if (recalcMaxPackSize)
                    {
                        maxPackageSize = routeItemSize * (definedMaxPackSize / routeItemSize);
                        recalcMaxPackSize = false;
                    }
                    totalOffset += iOffset;
                    int remaingPackageSize = routeItemSize * maxRouteItems - totalOffset + headerSize;
                    if (remaingPackageSize > maxPackageSize)
                        sendPackage = new byte[maxPackageSize];
                    else
                        sendPackage = new byte[remaingPackageSize];
                    sendPackages.Add(sendPackage);
                    iOffset = 0;
                }

                #region Routing item
                //GIPConv2006Base.AggrNo from Converter-Child-Instance.
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(routeItemModel.AggrNo), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Aggregat-Typ
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Source
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(routeItemModel.Source), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Target
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(routeItemModel.Target), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //First group
                Array.Copy(gip.core.communication.ISOonTCP.Types.Byte.ToByteArray(routeItemModel.FirstBooleanGroup), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Byte.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length;

                //Second group
                Array.Copy(gip.core.communication.ISOonTCP.Types.Byte.ToByteArray(routeItemModel.SecondBooleanGroup), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Byte.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length;

                //TurnOn delay
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(routeItemModel.TurnOnDelay), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //TurnOff delay
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(routeItemModel.TurnOffDelay), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Second target
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(routeItemModel.SecondTarget), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para1
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para2
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para3
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para4
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para5
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(0), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Second source
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(routeItemModel.SecondSource), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Third source
                Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(routeItemModel.ThirdSource), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;
                #endregion  
            }

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in routeItemsModel)
                {
                    stringBuilder.AppendLine(item.ToString());
                }
                this.Messages.LogDebug(this.GetACUrl(), nameof(CreateSendPackages), stringBuilder.ToString());
                //Clipboard.SetText(testResult);
            }
#endif

            errorMessage = null;
            return sendPackages;
        }

        private short GetRouteItemID(IACComponent component)
        {
            if (component == null)
                return 0;

            try
            {
                IRouteItemIDProvider routableModule = component as IRouteItemIDProvider;
                return routableModule != null ? (short)routableModule.RouteItemIDAsNum : (short)0;
            }
            catch (Exception ex)
            {
                this.Messages.LogException(this.GetACUrl(), nameof(GetRouteItemData), ex);
                return 0;
            }

            //PAEControlModuleBase moduleBase = null;
            //PAETransport paeTransport = component as PAETransport;
            //if (paeTransport != null)
            //    moduleBase = paeTransport.Motor;
            //if (moduleBase == null)
            //    moduleBase = component as PAEControlModuleBase;
            //if (moduleBase != null)
            //{
            //    GIPConv2006Base conv = component.FindChildComponents<GIPConv2006Base>(c => c is GIPConv2006Base).FirstOrDefault();
            //    if (conv != null && conv.AggrNo != null)
            //        return conv.AggrNo.ValueT;
            //}
            //else if (component is PAProcessModule)
            //    return (short)(component as PAProcessModule).RouteItemIDAsNum;
            //return 0;
        }

        private short GetRouteItemData(IACComponent component, out short turnOnDelay, out short turnOffDelay)
        {
            turnOnDelay = 0;
            turnOffDelay = 0;

            if (component == null)
                return 0;

            try
            {
                IRoutableModule routableModule = component as IRoutableModule;
                PAEControlModuleBase controlModuleBase = routableModule as PAEControlModuleBase;
                if (controlModuleBase == null && routableModule is PAETransport)
                    controlModuleBase = (routableModule as PAETransport).Motor;
                if (controlModuleBase != null)
                {
                    turnOffDelay = (short)controlModuleBase.TurnOffDelay.ValueT.TotalSeconds;
                    turnOnDelay = (short)controlModuleBase.TurnOnDelay.ValueT.TotalSeconds;
                }
                return routableModule != null ? (short)routableModule.RouteItemIDAsNum : (short)0;
            }
            catch (Exception ex) 
            {
                this.Messages.LogException(this.GetACUrl(), nameof(GetRouteItemData), ex);
                return 0;
            }

            //PAEControlModuleBase moduleBase = null;
            //PAETransport paeTransport = component as PAETransport;
            //if (paeTransport != null)
            //    moduleBase = paeTransport.Motor;
            //if (moduleBase == null)
            //    moduleBase = component as PAEControlModuleBase;
            //if (moduleBase != null)
            //{
            //    //turnOnDelay = (short)moduleBase.TurnOnDelay.ValueT.TotalSeconds;
            //    //turnOffDelay = (short)moduleBase.TurnOffDelay.ValueT.TotalSeconds;

            //    GIPConv2006Base conv = component.FindChildComponents<GIPConv2006Base>(c => c is GIPConv2006Base).FirstOrDefault();
            //    if (conv != null && conv.AggrNo != null)
            //        return conv.AggrNo.ValueT;
            //}
            //return 0;
        }

        private bool GenerateRouteItemsModel(Route route, RouteItem routeSource, IEnumerable<RouteItem> routeTargets, StringBuilder errorStringBuilder, out List<RouteItemModel> result)
        {
            bool modelValid = true;
            result = new List<RouteItemModel>();

            IEnumerable<IGrouping<IACComponent,RouteItem>> itemsGroupedByTargetComp = route.GroupBy(x => x.TargetACComponent);

            foreach (var itemGroup in itemsGroupedByTargetComp)
            {
                IACComponent currentComponent = itemGroup.Key;

                if (routeTargets.Any(c => c.TargetACComponent.ACUrl == currentComponent.ACUrl))
                    continue;

                string currentACUrl = currentComponent.ACUrl;
                short turnOnDelay = 0, turnOffDelay = 0;
                short currentItemID = GetRouteItemData(currentComponent, out turnOnDelay, out turnOffDelay);
                if (currentItemID <= 0)
                {
                    errorStringBuilder.AppendLine(String.Format("A: RouteItemID {1} of {0} is invalid", currentComponent.GetACUrl(), currentItemID));
                    modelValid = false;
                }

                IACComponent sourceComponent = itemGroup.FirstOrDefault().SourceACComponent;
                string sourceACUrl = sourceComponent.GetACUrl();
                short sourceItemID = GetRouteItemID(sourceComponent);
                if (sourceItemID <= 0)
                {
                    errorStringBuilder.AppendLine(String.Format("B: RouteItemID {1} of source {0} is invalid at {2}", sourceACUrl, sourceItemID, currentACUrl));
                    modelValid = false;
                }

                int sourceItemsCount = itemGroup.Count();
                if (sourceItemsCount > 3)
                {
                    // 3 sources is maximum
                    errorStringBuilder.AppendLine(String.Format("C: Source-Count {1} > 3 at {0}", currentACUrl, sourceItemsCount));
                    modelValid = false;
                }

                short source2ItemID = 0, source3ItemID = 0;
                string source2ACUrl = "", source3ACUrl = "";
                if (sourceItemsCount > 1)
                {
                    source2ACUrl = itemGroup.ToArray()[1].SourceACComponent.ACUrl;
                    source2ItemID = GetRouteItemID(itemGroup.ToArray()[1].SourceACComponent);
                    if (source2ItemID <= 0)
                    {
                        errorStringBuilder.AppendLine(String.Format("D: RouteItemID {1} of second source {0} is invalid at {2}", source2ACUrl, source2ItemID, currentACUrl));
                        modelValid = false;
                    }
                    if (sourceItemsCount > 2)
                    {
                        source3ACUrl = itemGroup.ToArray()[2].SourceACComponent.ACUrl; ;
                        source3ItemID = GetRouteItemID(itemGroup.ToArray()[2].SourceACComponent);
                        if (source3ItemID <= 0)
                        {
                            errorStringBuilder.AppendLine(String.Format("E: RouteItemID {1} of third source {0} is invalid at {2}", source3ACUrl, source3ItemID, currentACUrl));
                            modelValid = false;
                        }
                    }
                }

                var nextRouteItems = itemsGroupedByTargetComp.SelectMany(x => x)
                                                             .Where(c => c.SourceACComponent.ACUrl == currentComponent.ACUrl)
                                                             .ToArray();
                if (nextRouteItems == null || nextRouteItems.Length == 0)
                    continue;

                if (nextRouteItems.Length > 2)
                {
                    errorStringBuilder.AppendLine(String.Format("F: Item {0} has {1} targets (Max: 2)", currentACUrl, nextRouteItems.Length));
                    modelValid = false;
                }

                string targetACUrl, target2ACUrl = "";
                targetACUrl = nextRouteItems[0].TargetACComponent.ACUrl;
                short targetItemID = GetRouteItemID(nextRouteItems[0].TargetACComponent);
                if (targetItemID <= 0)
                {
                    errorStringBuilder.AppendLine(String.Format("G: RouteItemID {1} of target {0} is invalid at {2}", targetACUrl, targetItemID, currentACUrl));
                    modelValid = false;
                }

                short target2ItemID = 0;
                if (nextRouteItems.Length > 1)
                {
                    target2ACUrl = nextRouteItems[1].TargetACComponent.ACUrl;
                    target2ItemID = GetRouteItemID(nextRouteItems[1].TargetACComponent);
                    if (target2ItemID <= 0)
                    {
                        errorStringBuilder.AppendLine(String.Format("H: RouteItemID {1} of second target {0} is invalid at {2}", target2ACUrl, target2ItemID, currentACUrl));
                        modelValid = false;
                    }
                }

                bool isFirstControlModuleInRoute = false, isLastControlModuleInRoute = false;
                if (itemGroup.Any(c => c.SourceACComponent.ACUrl == routeSource.SourceACComponent.ACUrl) && currentComponent is IRoutableModule)
                    isFirstControlModuleInRoute = true;

                if (routeTargets.Any(c => c.SourceACComponent.ACUrl == currentComponent.ACUrl && c.SourceACComponent is IRoutableModule))
                    isLastControlModuleInRoute = true;

                RouteItemModel routeItemModel = new RouteItemModel()
                {
                    AggrNo = currentItemID,
                    AggrType = 1,
                    Source = sourceItemID,
                    SecondSource = source2ItemID,
                    ThirdSource = source3ItemID,
                    Target = targetItemID,
                    SecondTarget = target2ItemID,
                    IsFirstControlModuleInRouteItemList = isFirstControlModuleInRoute,
                    IsLastControlModuleInRouteItemList = isLastControlModuleInRoute,
                    TurnOnDelay = turnOnDelay,
                    TurnOffDelay = turnOffDelay,
                    Me = currentACUrl,
                    SourceT = sourceACUrl,
                    Source2T = source2ACUrl,
                    Source3T = source3ACUrl,
                    TargetT = targetACUrl,
                    Target2T = target2ACUrl
                };
                result.Add(routeItemModel);
            }

            return modelValid;
        }

        class RouteItemModel
        {
            public short AggrNo
            {
                get;
                set;
            }

            public short AggrType
            {
                get;
                set;
            }

            public short Source
            {
                get;
                set;
            }

            public short Target
            {
                get;
                set;
            }

            public bool IsFirstControlModuleInRouteItemList
            {
                get;
                set;
            }

            public bool IsLastControlModuleInRouteItemList
            {
                get;
                set;
            }

            public byte FirstBooleanGroup
            {
                get
                {
                    return new BitAccessForByte() { Bit00 = IsFirstControlModuleInRouteItemList, Bit01 = IsLastControlModuleInRouteItemList }.ValueT;
                }
            }

            public byte SecondBooleanGroup
            {
                get
                {
                    return 0;
                }
            }

            public short TurnOnDelay
            {
                get;
                set;
            }

            public short TurnOffDelay
            {
                get;
                set;
            }

            public short SecondTarget
            {
                get;
                set;
            }

            public short Para1
            {
                get
                {
                    return 0;
                }
            }

            public short Para2
            {
                get
                {
                    return 0;
                }
            }

            public short Para3
            {
                get
                {
                    return 0;
                }
            }

            public short Para4
            {
                get
                {
                    return 0;
                }
            }

            public short Para5
            {
                get
                {
                    return 0;
                }
            }

            public short SecondSource
            {
                get;
                set;
            }

            public short ThirdSource
            {
                get;
                set;
            }

            //for test
            public string Me;
            public string SourceT;
            public string TargetT;
            public string Source2T;
            public string Source3T;
            public string Target2T;

            public override string ToString()
            {
                string result = string.Format("AggrNo:{1}{0}Source:{2}{0}Target:{3}{0}Source2:{4}{0}Source3{5}{0}Target2:{6}{0}IsFirst:{7}{0}IsLast:{8}{0}",
                                               System.Environment.NewLine, Me, SourceT, TargetT, Source2T, Source3T, Target2T,
                                               IsFirstControlModuleInRouteItemList, IsLastControlModuleInRouteItemList);

                return result;
            }
        }
    }
}
