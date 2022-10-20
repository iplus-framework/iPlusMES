using System;
using System.Collections.Generic;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;

namespace gip.mes.facility
{
    public class MsgBooking : MsgWithDetails
    {
        public enum eResultCodes : short
        {
            Suceeded = 0,

            // Warning  (10000 - 19999)
            NoFacilityFoundForRetrotragePosting = 10000,

            // Failure  (20000 - 29999)

            // Error    (30000 - 39999)
            NoBookingType = 30000,
            NoBookingTypeInfo = 30001,
            RequiredParamsNotSet = 30002,
            DependingParamsNotSet = 30003,
            WrongParameterCombinations = 30004,
            EmptyParameterCouldNotBeDerived = 30005,
            DependingParamsHasWrongValue = 30006,
            ProhibitedBooking = 30007,
            WrongConfigurationInMaterialManagement = 30008,
            WrongStateOfEntity = 30009,
            QuantityConversionError = 30010,
            EntityPropertyNotSet = 30011,
            WrongImplementation = 30012,
            TransactionError = 31000,
        }

        public MsgBooking()
        {
            Source = "BookingResultMessage";
        }

        public void AddBookingMessage(eResultCodes messageNo, string message)
        {
            AddDetailMessage(new Msg { Source = Source, MessageLevel = GetMessageLevel(messageNo), ACIdentifier = messageNo.ToString(), Message = message });
            SetMessageLevelOfHead();
        }

        public void AddBookingMessage(eResultCodes messageNo, string message, string xmlData)
        {
            AddDetailMessage(new Msg { Source = Source, MessageLevel = GetMessageLevel(messageNo), ACIdentifier = messageNo.ToString(), Message = message, XMLConfig = xmlData });
            SetMessageLevelOfHead();
        }

        public void Merge(MsgWithDetails msgToIntegrate)
        {
            if (msgToIntegrate == null)
                return;
            if (msgToIntegrate.MsgDetails == null)
                return;
            foreach (Msg msg in msgToIntegrate.MsgDetails)
            {
                AddDetailMessage(msg);
            }
            SetMessageLevelOfHead();
        }

        private eMsgLevel GetMessageLevel(eResultCodes messageNo)
        {
            eMsgLevel messageLevel = eMsgLevel.Info;
            if ( (int)messageNo >= 10000 && (int)messageNo <= 19999)
            {
                messageLevel = eMsgLevel.Warning;
            }
            else if ( (int)messageNo >= 20000 && (int)messageNo <= 29999)
            {
                messageLevel = eMsgLevel.Failure;
            } 
            else if ( (int)messageNo >= 30000 && (int)messageNo <= 39999)
            {
                messageLevel = eMsgLevel.Error;
            }
            return messageLevel;
        }

        private void SetMessageLevelOfHead()
        {
            eMsgLevel levelLocal = eMsgLevel.Default;
            foreach (Msg msg in MsgDetails)
            {
                if (msg.MessageLevel == eMsgLevel.Exception)
                {
                    levelLocal = msg.MessageLevel;
                    break;
                }
                else if (msg.MessageLevel == eMsgLevel.Error)
                {
                    if ((levelLocal == eMsgLevel.Failure)
                        || (levelLocal == eMsgLevel.Warning)
                        || (levelLocal == eMsgLevel.Info)
                        || (levelLocal == eMsgLevel.Debug)
                        || (levelLocal == eMsgLevel.Default))
                    {
                        levelLocal = msg.MessageLevel;
                    }
                }
                else if (msg.MessageLevel == eMsgLevel.Failure)
                {
                    if ((levelLocal == eMsgLevel.Warning)
                        || (levelLocal == eMsgLevel.Info)
                        || (levelLocal == eMsgLevel.Debug)
                        || (levelLocal == eMsgLevel.Default))
                    {
                        levelLocal = msg.MessageLevel;
                    }
                }
                else if (msg.MessageLevel == eMsgLevel.Warning)
                {
                    if ((levelLocal == eMsgLevel.Info)
                        || (levelLocal == eMsgLevel.Debug)
                        || (levelLocal == eMsgLevel.Default))
                    {
                        levelLocal = msg.MessageLevel;
                    }
                }
                else if (msg.MessageLevel == eMsgLevel.Info)
                {
                    if ((levelLocal == eMsgLevel.Debug)
                        || (levelLocal == eMsgLevel.Default))
                    {
                        levelLocal = msg.MessageLevel;
                    }
                }
            }
            MessageLevel = levelLocal;
        }
    }
}
