﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using gip.core.datamodel;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    [EntityFrameworkInternal]
    public partial class ProdOrderBatchPlanEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.ProdOrderBatchPlan",
                typeof(ProdOrderBatchPlan),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(ProdOrderBatchPlan)),
                propertyCount: 31,
                navigationCount: 7,
                servicePropertyCount: 1,
                foreignKeyCount: 5,
                unnamedIndexCount: 2,
                namedIndexCount: 5,
                keyCount: 1);

            var prodOrderBatchPlanID = runtimeEntityType.AddProperty(
                "ProdOrderBatchPlanID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("ProdOrderBatchPlanID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_ProdOrderBatchPlanID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            prodOrderBatchPlanID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var batchActualCount = runtimeEntityType.AddProperty(
                "BatchActualCount",
                typeof(int),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("BatchActualCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_BatchActualCount", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            batchActualCount.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var batchNoFrom = runtimeEntityType.AddProperty(
                "BatchNoFrom",
                typeof(int?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("BatchNoFrom", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_BatchNoFrom", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            batchNoFrom.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var batchNoTo = runtimeEntityType.AddProperty(
                "BatchNoTo",
                typeof(int?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("BatchNoTo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_BatchNoTo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            batchNoTo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var batchSize = runtimeEntityType.AddProperty(
                "BatchSize",
                typeof(double),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("BatchSize", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_BatchSize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            batchSize.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var batchTargetCount = runtimeEntityType.AddProperty(
                "BatchTargetCount",
                typeof(int),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("BatchTargetCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_BatchTargetCount", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            batchTargetCount.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var calculatedEndDate = runtimeEntityType.AddProperty(
                "CalculatedEndDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("CalculatedEndDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_CalculatedEndDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            calculatedEndDate.AddAnnotation("Relational:ColumnType", "datetime");
            calculatedEndDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var calculatedStartDate = runtimeEntityType.AddProperty(
                "CalculatedStartDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("CalculatedStartDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_CalculatedStartDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            calculatedStartDate.AddAnnotation("Relational:ColumnType", "datetime");
            calculatedStartDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var durationSecAVG = runtimeEntityType.AddProperty(
                "DurationSecAVG",
                typeof(double?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("DurationSecAVG", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_DurationSecAVG", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            durationSecAVG.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isValidated = runtimeEntityType.AddProperty(
                "IsValidated",
                typeof(bool),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("IsValidated", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_IsValidated", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isValidated.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDBatchPlanGroupID = runtimeEntityType.AddProperty(
                "MDBatchPlanGroupID",
                typeof(Guid?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("MDBatchPlanGroupID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_MDBatchPlanGroupID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDBatchPlanGroupID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialWFACClassMethodID = runtimeEntityType.AddProperty(
                "MaterialWFACClassMethodID",
                typeof(Guid?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("MaterialWFACClassMethodID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_MaterialWFACClassMethodID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            materialWFACClassMethodID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var partialActualCount = runtimeEntityType.AddProperty(
                "PartialActualCount",
                typeof(int?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("PartialActualCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_PartialActualCount", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            partialActualCount.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var partialTargetCount = runtimeEntityType.AddProperty(
                "PartialTargetCount",
                typeof(int?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("PartialTargetCount", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_PartialTargetCount", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            partialTargetCount.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var planModeIndex = runtimeEntityType.AddProperty(
                "PlanModeIndex",
                typeof(short),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("PlanModeIndex", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_PlanModeIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (short)0);
            planModeIndex.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var planStateIndex = runtimeEntityType.AddProperty(
                "PlanStateIndex",
                typeof(short),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("PlanStateIndex", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_PlanStateIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (short)0);
            planStateIndex.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var plannedStartDate = runtimeEntityType.AddProperty(
                "PlannedStartDate",
                typeof(DateTime),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("PlannedStartDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_PlannedStartDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            plannedStartDate.AddAnnotation("Relational:ColumnType", "datetime");
            plannedStartDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodOrderPartslistID = runtimeEntityType.AddProperty(
                "ProdOrderPartslistID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("ProdOrderPartslistID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_ProdOrderPartslistID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            prodOrderPartslistID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodOrderPartslistPosID = runtimeEntityType.AddProperty(
                "ProdOrderPartslistPosID",
                typeof(Guid?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("ProdOrderPartslistPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_ProdOrderPartslistPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            prodOrderPartslistPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var scheduledEndDate = runtimeEntityType.AddProperty(
                "ScheduledEndDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("ScheduledEndDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_ScheduledEndDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            scheduledEndDate.AddAnnotation("Relational:ColumnType", "datetime");
            scheduledEndDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var scheduledOrder = runtimeEntityType.AddProperty(
                "ScheduledOrder",
                typeof(int?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("ScheduledOrder", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_ScheduledOrder", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            scheduledOrder.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var scheduledStartDate = runtimeEntityType.AddProperty(
                "ScheduledStartDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("ScheduledStartDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_ScheduledStartDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            scheduledStartDate.AddAnnotation("Relational:ColumnType", "datetime");
            scheduledStartDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sequence = runtimeEntityType.AddProperty(
                "Sequence",
                typeof(int),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("Sequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_Sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            sequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var startOffsetSecAVG = runtimeEntityType.AddProperty(
                "StartOffsetSecAVG",
                typeof(double?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("StartOffsetSecAVG", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_StartOffsetSecAVG", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            startOffsetSecAVG.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var totalSize = runtimeEntityType.AddProperty(
                "TotalSize",
                typeof(double),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("TotalSize", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_TotalSize", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            totalSize.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassWFID = runtimeEntityType.AddProperty(
                "VBiACClassWFID",
                typeof(Guid?),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("VBiACClassWFID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_VBiACClassWFID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassWFID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { prodOrderBatchPlanID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { mDBatchPlanGroupID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { materialWFACClassMethodID });

            var nCI_FK_ProdOrderBatchPlan_ProdOrderPartslistID = runtimeEntityType.AddIndex(
                new[] { prodOrderPartslistID },
                name: "NCI_FK_ProdOrderBatchPlan_ProdOrderPartslistID");

            var nCI_FK_ProdOrderBatchPlan_ProdOrderPartslistPosID = runtimeEntityType.AddIndex(
                new[] { prodOrderPartslistPosID },
                name: "NCI_FK_ProdOrderBatchPlan_ProdOrderPartslistPosID");

            var nCI_FK_ProdOrderBatchPlan_VBiACClassWFID = runtimeEntityType.AddIndex(
                new[] { vBiACClassWFID },
                name: "NCI_FK_ProdOrderBatchPlan_VBiACClassWFID");

            var nCI_ProdOrderBatchPlan_ProdOrderPartslistID_OT = runtimeEntityType.AddIndex(
                new[] { prodOrderPartslistID },
                name: "NCI_ProdOrderBatchPlan_ProdOrderPartslistID_OT");

            var nCI_ProdOrderBatchPlan_ProdOrderPartslistPosID_PlanStateIndex_ProdOrderBatchPlanID = runtimeEntityType.AddIndex(
                new[] { prodOrderPartslistPosID, planStateIndex, prodOrderBatchPlanID },
                name: "NCI_ProdOrderBatchPlan_ProdOrderPartslistPosID_PlanStateIndex_ProdOrderBatchPlanID");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDBatchPlanGroupID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDBatchPlanGroupID") }),
                principalEntityType);

            var mDBatchPlanGroup = declaringEntityType.AddNavigation("MDBatchPlanGroup",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDBatchPlanGroup),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("MDBatchPlanGroup", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_MDBatchPlanGroup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderBatchPlan_MDBatchPlanGroup = principalEntityType.AddNavigation("ProdOrderBatchPlan_MDBatchPlanGroup",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderBatchPlan>),
                propertyInfo: typeof(MDBatchPlanGroup).GetProperty("ProdOrderBatchPlan_MDBatchPlanGroup", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDBatchPlanGroup).GetField("_ProdOrderBatchPlan_MDBatchPlanGroup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderBatchPlan_MDBatchPlanGroupID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaterialWFACClassMethodID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaterialWFACClassMethodID") }),
                principalEntityType);

            var materialWFACClassMethod = declaringEntityType.AddNavigation("MaterialWFACClassMethod",
                runtimeForeignKey,
                onDependent: true,
                typeof(MaterialWFACClassMethod),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("MaterialWFACClassMethod", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_MaterialWFACClassMethod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderBatchPlan_MaterialWFACClassMethod = principalEntityType.AddNavigation("ProdOrderBatchPlan_MaterialWFACClassMethod",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderBatchPlan>),
                propertyInfo: typeof(MaterialWFACClassMethod).GetProperty("ProdOrderBatchPlan_MaterialWFACClassMethod", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaterialWFACClassMethod).GetField("_ProdOrderBatchPlan_MaterialWFACClassMethod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderBatchPlan_MaterialWFACClassMethodID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ProdOrderPartslistID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ProdOrderPartslistID") }),
                principalEntityType,
                required: true);

            var prodOrderPartslist = declaringEntityType.AddNavigation("ProdOrderPartslist",
                runtimeForeignKey,
                onDependent: true,
                typeof(ProdOrderPartslist),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("ProdOrderPartslist", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_ProdOrderPartslist", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderBatchPlan_ProdOrderPartslist = principalEntityType.AddNavigation("ProdOrderBatchPlan_ProdOrderPartslist",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderBatchPlan>),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ProdOrderBatchPlan_ProdOrderPartslist", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ProdOrderBatchPlan_ProdOrderPartslist", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderBatchPlan_ProdOrderPartslist");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ProdOrderPartslistPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ProdOrderPartslistPosID") }),
                principalEntityType);

            var prodOrderPartslistPos = declaringEntityType.AddNavigation("ProdOrderPartslistPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(ProdOrderPartslistPos),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("ProdOrderPartslistPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_ProdOrderPartslistPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderBatchPlan_ProdOrderPartslistPos = principalEntityType.AddNavigation("ProdOrderBatchPlan_ProdOrderPartslistPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderBatchPlan>),
                propertyInfo: typeof(ProdOrderPartslistPos).GetProperty("ProdOrderBatchPlan_ProdOrderPartslistPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPos).GetField("_ProdOrderBatchPlan_ProdOrderPartslistPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderBatchPlan_ProdOrderPartslistPosID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey5(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBiACClassWFID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassWFID") }),
                principalEntityType);

            var vBiACClassWF = declaringEntityType.AddNavigation("VBiACClassWF",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClassWF),
                propertyInfo: typeof(ProdOrderBatchPlan).GetProperty("VBiACClassWF", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatchPlan).GetField("_VBiACClassWF", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderBatchPlan_VBiACClassWF = principalEntityType.AddNavigation("ProdOrderBatchPlan_VBiACClassWF",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderBatchPlan>),
                propertyInfo: typeof(ACClassWF).GetProperty("ProdOrderBatchPlan_VBiACClassWF", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassWF).GetField("_ProdOrderBatchPlan_VBiACClassWF", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderBatchPlan_VBiACClassWFID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "ProdOrderBatchPlan");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
