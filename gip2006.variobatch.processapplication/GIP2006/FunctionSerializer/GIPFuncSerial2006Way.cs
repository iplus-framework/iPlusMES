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

namespace gip2006.variobatch.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Serializer for Way'}de{'Serialisierer für Way'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.Required, false, false)]
    public class GIPFuncSerial2006Way : ACSessionObjSerializer
    {
        public GIPFuncSerial2006Way(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool IsSerializerFor(string typeOrACMethodName)
        {
            throw new NotImplementedException();
        }

        public override object ReadObject(object complexObj, int dbNo, int offset, object miscParams)
        {
            throw new NotImplementedException();
        }

        [ACMethodInfo("", "", 999)]
        public override bool SendObject(object complexObj, int dbNo, int offset, object miscParams)
        {
            S7TCPSession s7Session = ParentACComponent as S7TCPSession;
            //if (s7Session == null || complexObj == null)
            //    return false;
            //if (!s7Session.PLCConn.IsConnected)
            //    return false;
            ACMethod request = complexObj as ACMethod;
            if (request == null)
                return false;

            Route route = request.ParameterValueList.FirstOrDefault().Value as Route;
            if (route == null || !route.Any())
                return false;

            int maxPackageSize = 200;
            int maxRouteItems = 49;

            List<byte[]> sendPackages = CreateSendPackages(route, maxRouteItems, maxPackageSize);
            if (sendPackages == null)
                return false;

            if (s7Session.IsConnectionLocalSim)
                return true;

            foreach (byte[] package in sendPackages)
            {
                byte[] pack = package;
                ErrorCode errCode = s7Session.PLCConn.WriteBytes(DataType.DataBlock, dbNo, offset, ref pack);
                offset += pack.Length;
                if (errCode != ErrorCode.NoError)
                    return false;
            }

            return true;
        }

        private List<byte[]> CreateSendPackages(Route route, int maxRouteItems, int maxPackageSize, IACComponent destinationChangeComponent = null)
        {
            int headerSize = 0, routeItemSize = 0, definedMaxPackSize = maxPackageSize;

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
                return null;

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
                return null;

            short sourceID = GetComponentID(sourceItem.SourceACComponent);
            if (sourceID == 0)
                return null;

            short[] targetsID = new short[] { 0, 0, 0, 0 };
            short tI = 0;
            foreach (RouteItem rItem in targetItems)
            {
                targetsID[tI] = GetComponentID(rItem.TargetACComponent);
                if (tI > 3)
                    break;
                tI++;
            }

            if (targetsID[0] == 0)
                return null;

            short destinationChangeAggrNo = 0;
            if (destinationChangeComponent != null)
                destinationChangeAggrNo = GetComponentID(destinationChangeComponent);


            List<RouteItemModel> routeItemsModel = null;
            if (route.IsAttached)
            {
                routeItemsModel = GenerateRouteItemsModel(route, sourceItem, targetItems);
            }
            else
            {
                using (Database db = new gip.core.datamodel.Database())
                {
                    route.AttachTo(db);
                    routeItemsModel = GenerateRouteItemsModel(route, sourceItem, targetItems);
                }
            }

            if (routeItemsModel == null)
                return null;

            int iOffset = 0;

            #region Header
            //VARIOBATCH Command
            iOffset += gip.core.communication.ISOonTCP.Types.Word.Length;

            //Destination change component
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(destinationChangeAggrNo), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Source
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(sourceID), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Target
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(targetsID[0]), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //SecondTarget
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(targetsID[1]), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Third Target
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(targetsID[2]), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
            iOffset += gip.core.communication.ISOonTCP.Types.Int.Length;

            //Fourth Target
            Array.Copy(gip.core.communication.ISOonTCP.Types.Int.ToByteArray(targetsID[3]), 0, sendPackage, iOffset, gip.core.communication.ISOonTCP.Types.Int.Length);
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
                return null;

            RouteItemModel emptyModel = new RouteItemModel();
            for (int i = 0; i < emptyRItemModelCount; i++)
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

            //for test
            string testResult = "";
            foreach (var item in routeItemsModel)
            {
                testResult += item.ToString() + System.Environment.NewLine;
            }
            Clipboard.SetText(testResult);

            return sendPackages;
        }

        private short GetComponentID(IACComponent component)
        {
            if (component == null)
                return 0;

            if (component is PAEControlModuleBase)
            {
                GIPConv2006Base conv = component.FindChildComponents<GIPConv2006Base>(c => c is GIPConv2006Base).FirstOrDefault();
                if (conv != null && conv.AggrNo != null)
                    return conv.AggrNo.ValueT;
            }
            else if (component is PAProcessModule)
                return (short)(component as PAProcessModule).RouteItemIDAsNum;

            return 0;
        }

        private short GetAggrNo(IACComponent component, out short turnOnDelay, out short turnOffDelay)
        {
            turnOnDelay = 0;
            turnOffDelay = 0;

            if (component == null)
                return 0;

            if (component is PAEControlModuleBase)
            {
                PAEControlModuleBase moduleBase = component as PAEControlModuleBase;
                turnOnDelay = (short)moduleBase.TurnOnDelay.ValueT.TotalSeconds;
                turnOffDelay = (short)moduleBase.TurnOffDelay.ValueT.TotalSeconds;

                GIPConv2006Base conv = component.FindChildComponents<GIPConv2006Base>(c => c is GIPConv2006Base).FirstOrDefault();
                if (conv != null && conv.AggrNo != null)
                    return conv.AggrNo.ValueT;
            }
            return 0;
        }

        private List<RouteItemModel> GenerateRouteItemsModel(Route route, RouteItem routeSource, IEnumerable<RouteItem> routeTargets)
        {
            List<RouteItemModel> result = new List<RouteItemModel>();

            var groupedRoutes = route.GroupBy(x => x.TargetACComponent);

            foreach (var rItem in groupedRoutes)
            {
                IACComponent currentComponent = rItem.Key;

                if (routeTargets.Any(c => c.TargetACComponent.ACUrl == currentComponent.ACUrl))
                    continue;

                string me = currentComponent.ACUrl;
                short turnOnDelay = 0, turnOffDelay = 0;
                short aggrNo = GetAggrNo(currentComponent, out turnOnDelay, out turnOffDelay);
                if (aggrNo == 0)
                    return null; //error message

                string sourceT = rItem.FirstOrDefault().SourceACComponent.ACUrl;
                short source = GetComponentID(rItem.FirstOrDefault().SourceACComponent);
                if (source == 0)
                    return null; //error message

                int rItemCount = rItem.Count();

                if (rItemCount > 3)
                    return null; //error message - 3 sources is maximum

                short source2 = 0, source3 = 0;
                string source2T = "", source3T = "";
                if (rItemCount > 1)
                {
                    source2T = rItem.ToArray()[1].SourceACComponent.ACUrl;
                    source2 = GetComponentID(rItem.ToArray()[1].SourceACComponent);
                    if (rItemCount > 2)
                    {
                        source3T = rItem.ToArray()[2].SourceACComponent.ACUrl; ;
                        source3 = GetComponentID(rItem.ToArray()[2].SourceACComponent);
                    }
                }

                var nextRouteItems = groupedRoutes.SelectMany(x => x).Where(c => c.SourceACComponent.ACUrl == currentComponent.ACUrl).ToArray();
                if (nextRouteItems == null || nextRouteItems.Length == 0)
                    continue;

                if (nextRouteItems.Length > 2)
                    return null; // error message

                string targetT, target2T = "";
                targetT = nextRouteItems[0].TargetACComponent.ACUrl;
                short target = GetComponentID(nextRouteItems[0].TargetACComponent);
                if (target == 0)
                    return null; //error message

                short target2 = 0;
                if (nextRouteItems.Length > 1)
                {
                    target2T = nextRouteItems[1].TargetACComponent.ACUrl;
                    target2 = GetComponentID(nextRouteItems[1].TargetACComponent);
                }

                bool isFirstControlModuleInRoute = false, isLastControlModuleInRoute = false;
                if (rItem.Any(c => c.SourceACComponent.ACUrl == routeSource.SourceACComponent.ACUrl) && currentComponent is PAEControlModuleBase)
                    isFirstControlModuleInRoute = true;

                if (routeTargets.Any(c => c.SourceACComponent.ACUrl == currentComponent.ACUrl && c.SourceACComponent is PAEControlModuleBase))
                    isLastControlModuleInRoute = true;

                RouteItemModel routeItemModel = new RouteItemModel()
                {
                    AggrNo = aggrNo,
                    AggrType = 1,
                    Source = source,
                    SecondSource = source2,
                    ThirdSource = source3,
                    Target = target,
                    SecondTarget = target2,
                    IsFirstControlModuleInRouteItemList = isFirstControlModuleInRoute,
                    IsLastControlModuleInRouteItemList = isLastControlModuleInRoute,
                    TurnOnDelay = turnOnDelay,
                    TurnOffDelay = turnOffDelay,
                    Me = me,
                    SourceT = sourceT,
                    Source2T = source2T,
                    Source3T = source3T,
                    TargetT = targetT,
                    Target2T = target2T
                };
                result.Add(routeItemModel);
            }

            return result;
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
