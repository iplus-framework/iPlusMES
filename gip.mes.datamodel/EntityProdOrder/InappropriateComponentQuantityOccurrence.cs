using gip.core.datamodel;
using System;

namespace gip.mes.datamodel
{
    public class InappropriateComponentQuantityOccurrence
    {

        public static bool IsForAnalyse(double oldValue, double newValue)
        {
            bool isForAnalyse = false;
            if (oldValue > double.Epsilon && newValue > double.Epsilon && Math.Abs(oldValue - newValue) > double.Epsilon)
            {
                if ((oldValue / newValue) > 5 || (newValue / oldValue) > 5)
                {
                    isForAnalyse = true;
                }
            }
            return isForAnalyse;
        }

        public static bool IsInappropriate(ProdOrderPartslistPos pos)
        {
            bool isInappropriate = false;
            if (pos.MaterialPosTypeIndex == (short)GlobalApp.MaterialPosTypes.OutwardRoot)
            {
                if (pos.ProdOrderPartslist.TargetQuantity > 0 && pos.BasedOnPartslistPos != null)
                {
                    double componentRatio = pos.TargetQuantityUOM / pos.ProdOrderPartslist.TargetQuantity;
                    double partslistComponentRatio = pos.BasedOnPartslistPos.TargetQuantityUOM / pos.BasedOnPartslistPos.Partslist.TargetQuantityUOM;

                    if (Math.Abs(componentRatio - partslistComponentRatio) > 0.1)
                        isInappropriate = true;
                }
            }
            return isInappropriate;
        }

        public static void WriteStackTrace(ProdOrderPartslistPos pos)
        {
            string partslistUnitName = pos.ProdOrderPartslist.Partslist.MDUnit != null ? pos.ProdOrderPartslist.Partslist.MDUnit.MDUnitName :
                pos.ProdOrderPartslist.Partslist.Material.BaseMDUnit.MDUnitName;

            string message = "When changing the order size, an error occurred during the conversion of the required material input. Please check whether the component quantities match the order size before starting the order.";

            string message1 = "";
            if (
                    pos.ProdOrderPartslist != null
                    && pos.ProdOrderPartslist.ProdOrder != null
                    && pos.ProdOrderPartslist.Partslist != null
                    && pos.ProdOrderPartslist.Partslist.Material != null)
            {
                message1 = $"ProgramNo: "
                + $" {pos.ProdOrderPartslist.ProdOrder.ProgramNo} "
                + System.Environment.NewLine
                + $"Recipe: #{pos.ProdOrderPartslist.Sequence} "
                + $"{pos.ProdOrderPartslist.Partslist.Material.MaterialNo} "
                + $"{pos.ProdOrderPartslist.Partslist.Material.MaterialName1} "
                + System.Environment.NewLine
                + $"Recipe quantity: {pos.ProdOrderPartslist.TargetQuantity} {partslistUnitName}";
            }

            string stackTrace = System.Environment.StackTrace.ToString();

            IACEntityObjectContext context = pos.GetObjectContext();
            if (context != null)
            {
                IRoot root = context.Root();
                if (root != null && root.Messages != null)
                {
                    root.Messages.LogWarning(pos.GetACUrl(), pos.ACIdentifier, message);
                    root.Messages.LogWarning(pos.GetACUrl(), pos.ACIdentifier, message1);
                    root.Messages.LogWarning(pos.GetACUrl(), pos.ACIdentifier, stackTrace);
                }
            }
        }
    }

}
