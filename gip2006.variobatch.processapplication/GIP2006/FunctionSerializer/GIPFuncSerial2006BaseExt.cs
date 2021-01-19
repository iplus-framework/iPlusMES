using gip.core.communication;
using gip.core.communication.ISOonTCP;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ErrorCode errCode = s7Session.PLCConn.WriteBytes(DataType.DataBlock, dbNo, offset, ref sendPackage1);
            if (errCode != ErrorCode.NoError)
                return false;

            if (s7Session.HashCodeValidation == HashCodeValidationEnum.End_WithRead || s7Session.HashCodeValidation == HashCodeValidationEnum.Head_WithRead)
            {
                int offsetOfHash = 0;
                if (s7Session.HashCodeValidation == HashCodeValidationEnum.End_WithRead)
                    offsetOfHash = offset + iOffset - gip.core.communication.ISOonTCP.Types.DInt.Length;
                else
                    offsetOfHash = offset;
                object validationResult = s7Session.PLCConn.Read(DataType.DataBlock, dbNo, offsetOfHash, VarType.DInt, 1);
                if (validationResult == null)
                    return false;
                int hashCodeValidation = (int)validationResult;
                if (hashCodeValidation != hashCode)
                {
                    s7Session.Messages.LogError(sender.GetACUrl(), "SendObjectToPLC(1)", String.Format("Different hashcodes {0} <> {1}", hashCode, hashCodeValidation));
                    return false;
                }
            }
            return errCode == ErrorCode.NoError;
        }
    }
}
