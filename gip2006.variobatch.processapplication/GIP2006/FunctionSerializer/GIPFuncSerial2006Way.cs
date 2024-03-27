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
using System.Xml.XPath;

namespace gip2006.variobatch.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Serializer for Way'}de{'Serialisierer für Way'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.NotStorable, false, false)]
    public class GIPFuncSerial2006Way : ACSessionObjSerializer
    {
        public GIPFuncSerial2006Way(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();



            return result;
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            return false;
        }

        public SafeList<Tuple<short,string, string>> RouteItemIDs
        {
            get;
            set;
        }

        #region Read route

        public override object ReadObject(object complexObj, int dbNo, int offset, int? routeOffset, object miscParams)
        {
            try
            {

                if (!routeOffset.HasValue)
                    return null;

                S7TCPSession s7Session = ParentACComponent as S7TCPSession;

                ACMethod request = complexObj as ACMethod;
                Route route = request?.ParameterValueList.Where(c => c.ACIdentifier == nameof(Route)).FirstOrDefault()?.Value as Route;

                List<Tuple<short, string>> routeItemIDs = route?.Select(c => new Tuple<short, string>(GetRouteItemID(c.SourceACComponent), c.SourceACComponent.ACUrl)).ToList();
                IACComponent targetComp = route?.GetRouteTarget().TargetACComponent;
                if (targetComp != null)
                    routeItemIDs.Add(new Tuple<short, string>(GetRouteItemID(targetComp), targetComp.ACUrl));

                if (routeItemIDs == null)
                    routeItemIDs = new List<Tuple<short, string>>();

                int maxPackageSize = 200;
                int maxRouteItems = 49;

                int headerSize = GetHeaderSize();
                int routeItemSize = GetRouteItemSize();

                int totalSize = headerSize + (routeItemSize * maxRouteItems);
                int remainingSize = totalSize;

                int myOffset = routeOffset.Value;
                int iOffset = headerSize;

                List<byte[]> result = new List<byte[]>();
                RouteItemHeaderModel header = new RouteItemHeaderModel();
                List<RouteItemModel> routeItems = new List<RouteItemModel>();
                bool readHeader = true;

                while (remainingSize > 0)
                {
                    int availableSize = maxPackageSize - iOffset;
                    if (availableSize > 0)
                        iOffset += (availableSize / routeItemSize) * routeItemSize;
                    else
                    {
                        //TODO:Error
                        return null;
                    }

                    if (iOffset > remainingSize)
                        iOffset = remainingSize;

                    byte[] readPackage = new byte[iOffset];

                    PLC.Result errCode = s7Session.PLCConn.ReadBytes(DataTypeEnum.DataBlock, dbNo, myOffset, iOffset, out readPackage);
                    if (errCode != null && !errCode.IsSucceeded)
                        return null;

                    result.Add(readPackage);

                    RouteItemHeaderModel headerTemp = null;
                    if (readHeader)
                        headerTemp = header;

                    ReadRoute(readPackage, headerTemp, routeItems, routeItemSize, routeItemIDs);

                    bool isLastElement = IsLastElement(header, routeItems);
                    if (isLastElement)
                        break;

                    myOffset += iOffset;
                    remainingSize -= iOffset;
                    iOffset = 0;
                    readHeader = false;
                }

                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in routeItems)
                {
                    stringBuilder.AppendLine(item.ToString());
                }
                this.Messages.LogDebug(this.GetACUrl(), nameof(ReadObject), stringBuilder.ToString());
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), nameof(ReadObject), e);
            }

            return null;
        }

        private void ReadRoute(byte[] readPackage, RouteItemHeaderModel header, List<RouteItemModel> routeItems, int routeItemSize, List<Tuple<short, string>> routeItemIDs)
        {
            int iOffset = 0;

            int packageLenght = readPackage.Length;

            if (header != null)
            {
                header.Command = gip.core.communication.ISOonTCP.Types.Word.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Word.Length;

                //Destination change component
                header.DestinationChangeAggrNo = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Source
                header.SourceAggrNo = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Target
                header.TargetAggrNo = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //SecondTarget
                header.SecondTargetAggrNo = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Third Target
                header.ThirdTargetAggrNo = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Fourth Target
                header.FourthTargetAggrNo = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Unused = 0
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Unused = 0
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Unused = 0
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Unused = 0
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Unused = 1
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //TargetFull
                header.DestinationChangeAggrNo = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Unused = 0
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Unused = 0
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                packageLenght = packageLenght - iOffset;
            }

            int packageItemCount = packageLenght / routeItemSize;

            for (int i=0; i < packageItemCount; i++)
            {
                RouteItemModel routeItem = new RouteItemModel();

                routeItem.AggrNo = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Aggregat-Typ
                routeItem.AggrType = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Source
                routeItem.Source = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Target
                routeItem.Target = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //First group
                iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length;

                //Second group
                iOffset += gip.core.communication.ISOonTCP.Types.Byte.Length;

                //TurnOn delay
                routeItem.TurnOnDelay = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //TurnOff delay
                routeItem.TurnOffDelay = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Second target
                routeItem.SecondTarget = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para1
                //routeItem.Para1 = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para2
                //routeItem.Para2 = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para3
                //routeItem.Para3 = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para4
                //routeItem.Para4 = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Para5
                //routeItem.Para5 = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Second source
                routeItem.SecondSource = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                //Third source
                routeItem.ThirdSource = gip.core.communication.ISOonTCP.Types.Int.FromByteArray(readPackage, iOffset);
                iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

                
                if (routeItemIDs != null)
                {
                    var item = routeItemIDs.FirstOrDefault(c => c.Item1 == routeItem.AggrNo);
                    if (item != null)
                        routeItem.SourceT = item.Item2;
                    else
                        routeItem.SourceT = GetRouteItemIDFromCache(routeItem.AggrNo);

                    item = routeItemIDs.FirstOrDefault(c => c.Item1 == routeItem.Target);
                    if (item != null)
                        routeItem.TargetT = item.Item2;
                    else
                        routeItem.TargetT = GetRouteItemIDFromCache(routeItem.Target);

                    item = routeItemIDs.FirstOrDefault(c => c.Item1 == routeItem.SecondSource);
                    if (item != null)
                        routeItem.Source2T = item.Item2;
                    else
                        routeItem.Source2T = GetRouteItemIDFromCache(routeItem.SecondSource);

                    item = routeItemIDs.FirstOrDefault(c => c.Item1 == routeItem.ThirdSource);
                    if (item != null)
                        routeItem.Source3T = item.Item2;
                    else
                        routeItem.Source3T = GetRouteItemIDFromCache(routeItem.ThirdSource);

                    item = routeItemIDs.FirstOrDefault(c => c.Item1 == routeItem.SecondTarget);
                    if (item != null)
                        routeItem.Target2T = item.Item2;
                    else
                        routeItem.Target2T = GetRouteItemIDFromCache(routeItem.SecondTarget);
                }


                routeItems.Add(routeItem);
            }
        }

        private string GetRouteItemIDFromCache(short routeItemID)
        {
            if (RouteItemIDs == null)
                FillRouteItemIDs();

            string routeID = routeItemID.ToString();

            string sessionACUrl = this.ParentACComponent.ACUrl;

            var tempList = RouteItemIDs.Where(c => c.Item1 == routeItemID && c.Item3 == sessionACUrl).ToList();

            if (tempList.Count == 1)
                routeID = tempList.FirstOrDefault().Item2;

            return routeID;
        }

        private bool IsLastElement(RouteItemHeaderModel header, List<RouteItemModel> routeItems)
        {
            if (header == null || routeItems == null || !routeItems.Any())
                return false;

            return routeItems.Any(c => (c.Target == header.TargetAggrNo || c.SecondTarget == header.TargetAggrNo)
                                   && (header.SecondTargetAggrNo <= 0 || (c.Target == header.SecondTargetAggrNo || c.SecondTarget == header.SecondTargetAggrNo))
                                   && (header.ThirdTargetAggrNo <= 0 || (c.Target == header.ThirdTargetAggrNo || c.SecondTarget == header.ThirdTargetAggrNo))
                                   && (header.FourthTargetAggrNo <= 0 || (c.Target == header.FourthTargetAggrNo || c.SecondTarget == header.FourthTargetAggrNo)));
        }

        private void FillRouteItemIDs()
        {
            var converters = Root.FindChildComponents<GIPConv2006Base>(c => c is GIPConv2006Base && c.ACOperationMode == ACOperationModes.Live).ToList();

            RouteItemIDs = new SafeList<Tuple<short, string, string>>();

            foreach (GIPConv2006Base conv in converters)
            {
                short aggrNo = conv.AggrNo.ValueT;

                if (conv.Session == null || conv.ParentACComponent == conv.Session)
                    continue;

                if (aggrNo != 0)
                    RouteItemIDs.Add(new Tuple<short, string, string>(aggrNo, conv.ParentACComponent.ACUrl, conv.Session.ACUrl));
            }
        }

        #endregion


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

            IEnumerable<Route> splittedRoutes = Route.SplitRoute(route);
            Route routeForSending = Route.MergeRoutesWithoutDuplicates(splittedRoutes);

            IACComponent destinationChangeComponent = null;
            Route previousRoute = null;
            ACMethod previousRequest = prevComplexObj as ACMethod;
            if (previousRequest != null)
            {
                previousRoute = previousRequest.ParameterValueList.Where(c => c.ACIdentifier == nameof(Route)).FirstOrDefault()?.Value as Route;
                if (previousRoute != null)
                {
                    // If one Element is negative, then there is a forced change (e.g. Scales, that must be emptied first)
                    RouteItem forcedItemForChange = routeForSending.Where(c => GetRouteItemID(c.SourceACComponent) < -1).FirstOrDefault();
                    if (forcedItemForChange != null)
                    {
                        RouteItem prevForcedItemForChange = previousRoute.Where(c => GetRouteItemID(c.SourceACComponent) < -1).FirstOrDefault();
                        if (prevForcedItemForChange != null && prevForcedItemForChange.SourceACComponent == forcedItemForChange.SourceACComponent)
                        {
                            destinationChangeComponent = prevForcedItemForChange.SourceACComponent;
                        }
                    }

                    if (destinationChangeComponent == null)
                    {
                        IEnumerable<Route> prevSplittedRoutes = Route.SplitRoute(previousRoute);
                        Route previousRouteForCompare = Route.MergeRoutesWithoutDuplicates(prevSplittedRoutes);
                        routeForSending.Compare(previousRouteForCompare, out destinationChangeComponent);
                    }
                }
            }

            int maxPackageSize = 200;
            int maxRouteItems = 49;
            string errorMsg = null;
            List<byte[]> sendPackages = CreateSendPackages(routeForSending, maxRouteItems, maxPackageSize, out errorMsg, destinationChangeComponent);
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
            int headerSize = GetHeaderSize();
            int routeItemSize = GetRouteItemSize(); 
            int definedMaxPackSize = maxPackageSize;
            //errorMessage = null;

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
            if (sourceItemID == -1 || sourceItemID == 0)
                sb.AppendLine(String.Format("RouteItemID {1} of source {0} is invalid", sourceItem.SourceACComponent.GetACUrl(), sourceItemID));
            sourceItemID = Math.Abs(sourceItemID);

            short[] targetItemIDs = new short[] { 0, 0, 0, 0 };
            short i = 0;
            foreach (RouteItem rItem in targetItems)
            {
                short itemID = GetRouteItemID(rItem.TargetACComponent);
                if (itemID == -1 || itemID == 0)
                    sb.AppendLine(String.Format("RouteItemID {1} of target {0} is invalid", rItem.TargetACComponent.GetACUrl(), itemID));

                if (targetItemIDs.Where(c => c == itemID).Any())
                    continue;
                targetItemIDs[i] = Math.Abs(itemID);
                if (i > 3)
                    break;
                i++;
            }

            short destinationChangeAggrNo = 0;
            if (destinationChangeComponent != null)
            {
                destinationChangeAggrNo = GetRouteItemID(destinationChangeComponent);
                if (destinationChangeAggrNo == -1 || destinationChangeAggrNo == 0)
                    sb.AppendLine(String.Format("RouteItemID {1} of destination change component {0} is invalid", destinationChangeComponent.GetACUrl(), destinationChangeAggrNo));
                destinationChangeAggrNo = Math.Abs(destinationChangeAggrNo);
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
            TextCopy.ClipboardService.SetText(testResult);
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
                {
                    controlModuleBase = (routableModule as PAETransport).Motor;
                    if (controlModuleBase != null)
                    {
                        turnOffDelay = (short)(routableModule as PAETransport).DepletingTime.ValueT.TotalSeconds;
                        turnOnDelay = (short)controlModuleBase.TurnOnDelay.ValueT.TotalSeconds;
                    }
                }
                else if (controlModuleBase != null)
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

            IEnumerable<IGrouping<IACComponent,RouteItem>> itemsGroupedByTargetComp = GroupRouteByTargetComp(route, routeSource, routeTargets); //route.GroupBy(x => x.TargetACComponent);

            foreach (var itemGroup in itemsGroupedByTargetComp)
            {
                IACComponent currentComponent = itemGroup.Key;

                if (routeTargets.Any(c => c.TargetACComponent.ACUrl == currentComponent.ACUrl))
                    continue;

                string currentACUrl = currentComponent.ACUrl;
                short turnOnDelay = 0, turnOffDelay = 0;
                short currentItemID = GetRouteItemData(currentComponent, out turnOnDelay, out turnOffDelay);
                if (currentItemID == -1 || currentItemID == 0)
                {
                    errorStringBuilder.AppendLine(String.Format("A: RouteItemID {1} of {0} is invalid", currentComponent.GetACUrl(), currentItemID));
                    modelValid = false;
                }
                currentItemID = Math.Abs(currentItemID);

                IACComponent sourceComponent = itemGroup.FirstOrDefault().SourceACComponent;
                string sourceACUrl = sourceComponent.GetACUrl();
                short sourceItemID = GetRouteItemID(sourceComponent);
                if (sourceItemID == -1 || sourceItemID == 0)
                {
                    errorStringBuilder.AppendLine(String.Format("B: RouteItemID {1} of source {0} is invalid at {2}", sourceACUrl, sourceItemID, currentACUrl));
                    modelValid = false;
                }
                sourceItemID = Math.Abs(sourceItemID);

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
                    if (source2ItemID == -1 || source2ItemID == 0)
                    {
                        errorStringBuilder.AppendLine(String.Format("D: RouteItemID {1} of second source {0} is invalid at {2}", source2ACUrl, source2ItemID, currentACUrl));
                        modelValid = false;
                    }
                    source2ItemID = Math.Abs(source2ItemID);
                    if (sourceItemsCount > 2)
                    {
                        source3ACUrl = itemGroup.ToArray()[2].SourceACComponent.ACUrl; ;
                        source3ItemID = GetRouteItemID(itemGroup.ToArray()[2].SourceACComponent);
                        if (source3ItemID == -1 || source3ItemID == 0)
                        {
                            errorStringBuilder.AppendLine(String.Format("E: RouteItemID {1} of third source {0} is invalid at {2}", source3ACUrl, source3ItemID, currentACUrl));
                            modelValid = false;
                        }
                        source3ItemID = Math.Abs(source3ItemID);
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
                if (targetItemID == -1 || targetItemID == 0)
                {
                    errorStringBuilder.AppendLine(String.Format("G: RouteItemID {1} of target {0} is invalid at {2}", targetACUrl, targetItemID, currentACUrl));
                    modelValid = false;
                }
                targetItemID = Math.Abs(targetItemID);

                short target2ItemID = 0;
                if (nextRouteItems.Length > 1)
                {
                    target2ACUrl = nextRouteItems[1].TargetACComponent.ACUrl;
                    target2ItemID = GetRouteItemID(nextRouteItems[1].TargetACComponent);
                    if (target2ItemID == -1 || target2ItemID == 0)
                    {
                        errorStringBuilder.AppendLine(String.Format("H: RouteItemID {1} of second target {0} is invalid at {2}", target2ACUrl, target2ItemID, currentACUrl));
                        modelValid = false;
                    }
                    target2ItemID = Math.Abs(target2ItemID);
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

        public virtual IEnumerable<IGrouping<IACComponent, RouteItem>> GroupRouteByTargetComp (Route route, RouteItem routeSource, IEnumerable<RouteItem> routeTargets)
        {
            var groupedByRouteNo = route.GroupBy(c => c.RouteNo);
            var result = route.GroupBy(x => x.TargetACComponent).ToList();

            if (groupedByRouteNo.Count() > 1)
            {
                var multiPointRouteItems = result.Where(c => c.Count() > 1).ToList();
                if (multiPointRouteItems.Any())
                {
                    List<IGrouping<IACComponent, RouteItem>> newResult = new List<IGrouping<IACComponent, RouteItem>>();

                    while(result.Any())
                    {
                        while(multiPointRouteItems.Any())
                        {
                            IGrouping<IACComponent, RouteItem> multiPointItem = multiPointRouteItems.FirstOrDefault();
                            if (multiPointItem != null)
                            {
                                foreach (RouteItem rItem in multiPointItem)
                                {
                                    List<IGrouping<IACComponent,RouteItem>> partResult = result.Where(c => c.Any(x => x.RouteNo == rItem.RouteNo)).ToList();

                                    SortRouteByTargetComp(partResult);

                                    foreach (var partItem in partResult)
                                    {
                                        if (partItem == multiPointItem)
                                            break;

                                        newResult.Add(partItem);
                                        result.Remove(partItem);
                                    }
                                }

                                newResult.Add(multiPointItem);
                                multiPointRouteItems.Remove(multiPointItem);
                                result.Remove(multiPointItem);
                            }
                        }

                        if (result.Any())
                        {
                            newResult.AddRange(result);
                            result.Clear();
                        }
                    }

                    return newResult;
                }
            }

            return result;
        }

        private void SortRouteByTargetComp(List<IGrouping<IACComponent, RouteItem>> items)
        {
            IGrouping<IACComponent, RouteItem> firstMultiPointItem = items.FirstOrDefault(c => c.Count() > 1);
            if (firstMultiPointItem != null)
            {
                foreach (RouteItem routeItem in firstMultiPointItem)
                {
                    IGrouping<IACComponent, RouteItem> source = items.FirstOrDefault(c => c.Key == routeItem.SourceACComponent);
                    if (source != null)
                    {
                        
                        int indexMultiPointItem = items.IndexOf(firstMultiPointItem);
                        int indexNew = items.IndexOf(source);

                        if (indexNew > indexMultiPointItem)
                        {
                            items.RemoveAt(indexMultiPointItem);
                            indexNew = items.IndexOf(source);
                            items.Insert(indexNew + 1, firstMultiPointItem);
                        }

                        break;
                    }
                }
            }
        }

        private int GetHeaderSize()
        {
            int headerSize = 0;

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

            return headerSize;
        }

        private int GetRouteItemSize()
        {
            int routeItemSize = 0;

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

            return routeItemSize;
        }

        class RouteItemHeaderModel
        {
            public ushort Command
            {
                get;set;
            }

            public short DestinationChangeAggrNo
            {
                get;set;
            }

            public short SourceAggrNo
            {
                get;set;
            }

            public short TargetAggrNo
            {
                get;set;
            }

            public short SecondTargetAggrNo
            {
                get;set;
            }

            public short ThirdTargetAggrNo
            {
                get;set;
            }

            public short FourthTargetAggrNo
            {
                get;set;
            }
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
