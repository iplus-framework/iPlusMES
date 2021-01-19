using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.datamodel
{
    public enum TandTv2OperationEnum
    {
        BW_FB_INPOS_TO_FBC,
        BW_DELIVERY_DELIVERYNOTEPOS_TO_DELIVERYNOTE,
        BW_DELIVERY_FBC_TO_INORDERPOS,
        BW_DELIVERY_INPOS_TO_DELIVERYNOTEPOS,
        BW_DELIVERY_INPOS_TO_INORDER,
        BW_FB_FBC_TO_FACILITY,
        BW_FB_FBC_TO_FB,
        BW_FB_FB_TO_FC,
        BW_FB_FC_TO_FL,
        BW_FB_POS_TO_FBC,
        BW_FB_REL_TO_FBC,
        BW_FB_START,
        BW_PROD_FBC_TO_POS,
        BW_PROD_FBC_TO_REL,
        BW_PROD_INWARD_POS,
        BW_PROD_REL_TO_POS
    }
}
