using gip.mes.datamodel;
using iplusContext = gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.facility
{
    public class DoItem<T>:IDoItem where T : iplusContext.IACObjectEntity
    {

        #region ctor's 
        public DoItem(DatabaseApp databaseApp, TandTv2Result result, T item, TandTv2Job jobFilter)
        {
            Item = item;
            StepItem = Factory_StepItem(result, result.LastStep);
            result.StepItems.Add(StepItem);
            result.LastStep.TandTv2StepItem_TandTv2Step.Add(StepItem);
            TandTv2StepLot tandT_StepLot = Factory_StepLot(result.LastStep);
            if (tandT_StepLot != null)
            {
                if(!result.StepLots.Select(c=>c.LotNo).Contains(tandT_StepLot.LotNo))
                {
                    result.StepLots.Add(tandT_StepLot);
                    result.LastStep.TandTv2StepLot_TandTv2Step.Add(tandT_StepLot);
                }
            }

            List<TandTv2TempPos> tempPositions = Factory_TmpPos(result);
            if (tempPositions != null && tempPositions.Any())
            {
                result.TempPositions.AddRange(tempPositions);
                foreach (var tmpPosItem in tempPositions)
                    databaseApp.TandTv2TempPos.AddObject(tmpPosItem);
            }

            Console.WriteLine(@"T&T [{0}]: Constructed DoItem: {1}", DateTime.Now, Factory_StepItem_ACIdentifier());
        }
        
        #endregion


        #region properties

        public TandTv2StepItem StepItem { get; set; }

        public T Item { get; set; }


        #endregion


        #region methods

        public virtual TandTv2StepItem Factory_StepItem(TandTv2Result result, TandTv2Step step)
        {
            TandTv2StepItem stepItem = new TandTv2StepItem();
            stepItem.TandTv2StepItemID = Guid.NewGuid();
            stepItem.TandTv2Step = step;
            stepItem.SubStepNo = 0;
            stepItem.TandT_OperationEnum = TandTv2OperationEnum.BW_FB_START;
            stepItem.ACIdentifier = Factory_StepItem_ACIdentifier();
            stepItem.ACCaptionTranslation = Factory_StepItem_ACCaptionTranslation();
            return stepItem;
        }

        public virtual string Factory_StepItem_ACIdentifier()
        {
            return null;
        }
        public virtual string Factory_StepItem_ACCaptionTranslation()
        {
            return null;
        }

        public virtual TandTv2StepLot Factory_StepLot(TandTv2Step step)
        {
            return null;
        }

        public virtual List<TandTv2TempPos> Factory_TmpPos(TandTv2Result tandTv2Result)
        {
            return null;
        }

        public virtual List<IDoItem> ProcessRelatedSameStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            return null;
        }

        public virtual List<IDoItem> SearchRelatedNextStep(DatabaseApp databaseApp, TandTv2Result result, TandTv2Job jobFilter)
        {
            return null;
        }

        public virtual void BuildRelations(TandTv2Result result, TandTv2StepItem stepItem, List<TandTv2StepItem> related)
        {
            List<TandTv2StepItemRelation> relations =
                related.Select(c =>
                new TandTv2StepItemRelation()
                {
                    TandTv2StepItemRelationID = Guid.NewGuid(),
                    TandTv2RelationTypeEnum = TandTv2RelationTypeEnum.TrackingFlow,
                    TargetTandTv2StepItem = c
                }).ToList();
            foreach (var item in relations)
            {
                stepItem.TandTv2StepItemRelation_SourceTandTv2StepItem.Add(item);
                result.StepItemRelations.Add(item);
            }
        }


        public override string ToString()
        {
            if (Item == null) return null;
            return Item.ToString();
        }

        #endregion
    }
}
