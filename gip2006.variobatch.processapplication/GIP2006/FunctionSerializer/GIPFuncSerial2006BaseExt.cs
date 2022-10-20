using gip.core.communication;
using gip.core.communication.ISOonTCP;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gip2006.variobatch.processapplication
{
    public static class GIPFuncSerial2006BaseExt
    {
        public static bool SendObjectToPLC(this ACSessionObjSerializer sender, S7TCPSession s7Session, ACMethod request, byte[] sendPackage1, int dbNo, int offset, int iOffset)
        {
            int hashCode = 0;
            if (s7Session.HashCodeValidation != HashCodeValidationEnum.Off)
            {
                hashCode = request.GetHashCode();
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.End || s7Session.HashCodeValidation == HashCodeValidationEnum.End_WithRead)
                {
                    Array.Copy(gip.core.communication.ISOonTCP.Types.DInt.ToByteArray(hashCode),
                        0, sendPackage1, iOffset, gip.core.communication.ISOonTCP.Types.DInt.Length);
                    iOffset += gip.core.communication.ISOonTCP.Types.DInt.Length;
                }
                else if (s7Session.HashCodeValidation == HashCodeValidationEnum.Head || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
                {
                    Array.Copy(gip.core.communication.ISOonTCP.Types.DInt.ToByteArray(hashCode),
                        0, sendPackage1, 0, gip.core.communication.ISOonTCP.Types.DInt.Length);
                }
            }

            PLC.Result plcError = null;
            PLC.Result firstPLCError = null;
            for (int tries = 0; tries < 3; tries++)
            {
                if (tries > 0)
                    Thread.Sleep(50);
                plcError = s7Session.PLCConn.WriteBytes(DataTypeEnum.DataBlock, dbNo, offset, ref sendPackage1);
                if (plcError == null || plcError.IsSucceeded)
                    break;
                if (!plcError.IsPLCError)
                    break;
                else if (firstPLCError == null)
                    firstPLCError = plcError;
            }

            if (firstPLCError != null)
                sender.Messages.LogFailure(sender.GetACUrl(), "S7TCPSubscr.SendObjectToPLC(10)", "PLC-Error: " + firstPLCError.ToString());

            if (plcError != null && !plcError.IsSucceeded)
            {
                sender.Messages.LogFailure(sender.GetACUrl(), "S7TCPSubscr.SendObjectToPLC(20)", "PLC-Error: " + plcError.ToString());
                return false;
            }

            if (s7Session.HashCodeValidation == HashCodeValidationEnum.End_WithRead || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
            {
                int offsetOfHash = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.End_WithRead)
                    offsetOfHash = offset + iOffset - gip.core.communication.ISOonTCP.Types.DInt.Length;
                else
                    offsetOfHash = offset;
                object validationResult = null;
                plcError = s7Session.PLCConn.Read(DataTypeEnum.DataBlock, dbNo, offsetOfHash, VarTypeEnum.DInt, 1, out validationResult);
                if (plcError != null && !plcError.IsSucceeded)
                {
                    sender.Messages.LogFailure(sender.GetACUrl(), "S7TCPSubscr.SendObjectToPLC(30)", "PLC-Error while reading: " + plcError.ToString());
                    return false;
                }
                if (validationResult == null)
                    return false;
                int hashCodeValidation = (int)validationResult;
                if (hashCodeValidation != hashCode)
                {
                    s7Session.Messages.LogError(sender.GetACUrl(), "SendObjectToPLC(40)", String.Format("Different hashcodes {0} <> {1}", hashCode, hashCodeValidation));
                    return false;
                }
            }
            return plcError == null || plcError.IsSucceeded;
        }
    }
}
