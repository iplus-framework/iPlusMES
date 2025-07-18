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
    public partial class ProdOrderPartslistEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.ProdOrderPartslist",
                typeof(ProdOrderPartslist),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(ProdOrderPartslist)),
                propertyCount: 31,
                navigationCount: 11,
                servicePropertyCount: 1,
                foreignKeyCount: 4,
                unnamedIndexCount: 1,
                namedIndexCount: 4,
                keyCount: 1);

            var prodOrderPartslistID = runtimeEntityType.AddProperty(
                "ProdOrderPartslistID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ProdOrderPartslistID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ProdOrderPartslistID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            prodOrderPartslistID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var actualQuantity = runtimeEntityType.AddProperty(
                "ActualQuantity",
                typeof(double),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ActualQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ActualQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            actualQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var actualQuantityScrapUOM = runtimeEntityType.AddProperty(
                "ActualQuantityScrapUOM",
                typeof(double),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ActualQuantityScrapUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ActualQuantityScrapUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            actualQuantityScrapUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var departmentUserDate = runtimeEntityType.AddProperty(
                "DepartmentUserDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("DepartmentUserDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_DepartmentUserDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            departmentUserDate.AddAnnotation("Relational:ColumnType", "datetime");
            departmentUserDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var departmentUserName = runtimeEntityType.AddProperty(
                "DepartmentUserName",
                typeof(string),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("DepartmentUserName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_DepartmentUserName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 40,
                unicode: false);
            departmentUserName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var endDate = runtimeEntityType.AddProperty(
                "EndDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("EndDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_EndDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            endDate.AddAnnotation("Relational:ColumnType", "datetime");
            endDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var externProdOrderNo = runtimeEntityType.AddProperty(
                "ExternProdOrderNo",
                typeof(string),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ExternProdOrderNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ExternProdOrderNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            externProdOrderNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inputQForActualOutputPer = runtimeEntityType.AddProperty(
                "InputQForActualOutputPer",
                typeof(double?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("InputQForActualOutputPer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_InputQForActualOutputPer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            inputQForActualOutputPer.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inputQForFinalActualOutputPer = runtimeEntityType.AddProperty(
                "InputQForFinalActualOutputPer",
                typeof(double?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("InputQForFinalActualOutputPer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_InputQForFinalActualOutputPer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            inputQForFinalActualOutputPer.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inputQForFinalGoodActualOutputPer = runtimeEntityType.AddProperty(
                "InputQForFinalGoodActualOutputPer",
                typeof(double?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("InputQForFinalGoodActualOutputPer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_InputQForFinalGoodActualOutputPer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            inputQForFinalGoodActualOutputPer.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inputQForFinalScrapActualOutputPer = runtimeEntityType.AddProperty(
                "InputQForFinalScrapActualOutputPer",
                typeof(double?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("InputQForFinalScrapActualOutputPer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_InputQForFinalScrapActualOutputPer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            inputQForFinalScrapActualOutputPer.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inputQForGoodActualOutputPer = runtimeEntityType.AddProperty(
                "InputQForGoodActualOutputPer",
                typeof(double?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("InputQForGoodActualOutputPer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_InputQForGoodActualOutputPer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            inputQForGoodActualOutputPer.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inputQForScrapActualOutputPer = runtimeEntityType.AddProperty(
                "InputQForScrapActualOutputPer",
                typeof(double?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("InputQForScrapActualOutputPer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_InputQForScrapActualOutputPer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            inputQForScrapActualOutputPer.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isEnabled = runtimeEntityType.AddProperty(
                "IsEnabled",
                typeof(bool),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("IsEnabled", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_IsEnabled", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isEnabled.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lastFormulaChange = runtimeEntityType.AddProperty(
                "LastFormulaChange",
                typeof(DateTime),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("LastFormulaChange", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_LastFormulaChange", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            lastFormulaChange.AddAnnotation("Relational:ColumnType", "datetime");
            lastFormulaChange.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lossComment = runtimeEntityType.AddProperty(
                "LossComment",
                typeof(string),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("LossComment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_LossComment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            lossComment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDProdOrderStateID = runtimeEntityType.AddProperty(
                "MDProdOrderStateID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("MDProdOrderStateID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_MDProdOrderStateID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            mDProdOrderStateID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var partslistID = runtimeEntityType.AddProperty(
                "PartslistID",
                typeof(Guid?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("PartslistID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_PartslistID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            partslistID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodOrderID = runtimeEntityType.AddProperty(
                "ProdOrderID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ProdOrderID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ProdOrderID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            prodOrderID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodUserEndDate = runtimeEntityType.AddProperty(
                "ProdUserEndDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ProdUserEndDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ProdUserEndDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            prodUserEndDate.AddAnnotation("Relational:ColumnType", "datetime");
            prodUserEndDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodUserEndName = runtimeEntityType.AddProperty(
                "ProdUserEndName",
                typeof(string),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ProdUserEndName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ProdUserEndName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            prodUserEndName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sequence = runtimeEntityType.AddProperty(
                "Sequence",
                typeof(int),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("Sequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_Sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            sequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var startDate = runtimeEntityType.AddProperty(
                "StartDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("StartDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_StartDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            startDate.AddAnnotation("Relational:ColumnType", "datetime");
            startDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetDeliveryDate = runtimeEntityType.AddProperty(
                "TargetDeliveryDate",
                typeof(DateTime?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("TargetDeliveryDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_TargetDeliveryDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            targetDeliveryDate.AddAnnotation("Relational:ColumnType", "datetime");
            targetDeliveryDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetQuantity = runtimeEntityType.AddProperty(
                "TargetQuantity",
                typeof(double),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("TargetQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_TargetQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACProgramID = runtimeEntityType.AddProperty(
                "VBiACProgramID",
                typeof(Guid?),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("VBiACProgramID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_VBiACProgramID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACProgramID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { prodOrderPartslistID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { vBiACProgramID });

            var nCI_FK_ProdOrderPartslist_MDProdOrderStateID = runtimeEntityType.AddIndex(
                new[] { mDProdOrderStateID },
                name: "NCI_FK_ProdOrderPartslist_MDProdOrderStateID");

            var nCI_FK_ProdOrderPartslist_PartslistID = runtimeEntityType.AddIndex(
                new[] { partslistID },
                name: "NCI_FK_ProdOrderPartslist_PartslistID");

            var nCI_FK_ProdOrderPartslist_ProdOrderID = runtimeEntityType.AddIndex(
                new[] { prodOrderID },
                name: "NCI_FK_ProdOrderPartslist_ProdOrderID");

            var nCI_ProdOrderPartslist_ProdOrderPartslistID_ProdOrderID_MDProdOrderStateID = runtimeEntityType.AddIndex(
                new[] { prodOrderPartslistID, prodOrderID, mDProdOrderStateID },
                name: "NCI_ProdOrderPartslist_ProdOrderPartslistID_ProdOrderID_MDProdOrderStateID");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDProdOrderStateID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDProdOrderStateID") }),
                principalEntityType,
                required: true);

            var mDProdOrderState = declaringEntityType.AddNavigation("MDProdOrderState",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDProdOrderState),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("MDProdOrderState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_MDProdOrderState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslist_MDProdOrderState = principalEntityType.AddNavigation("ProdOrderPartslist_MDProdOrderState",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslist>),
                propertyInfo: typeof(MDProdOrderState).GetProperty("ProdOrderPartslist_MDProdOrderState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDProdOrderState).GetField("_ProdOrderPartslist_MDProdOrderState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslist_MDProdOrderState");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("PartslistID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("PartslistID") }),
                principalEntityType);

            var partslist = declaringEntityType.AddNavigation("Partslist",
                runtimeForeignKey,
                onDependent: true,
                typeof(Partslist),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("Partslist", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_Partslist", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslist_Partslist = principalEntityType.AddNavigation("ProdOrderPartslist_Partslist",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslist>),
                propertyInfo: typeof(Partslist).GetProperty("ProdOrderPartslist_Partslist", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_ProdOrderPartslist_Partslist", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslist_PartslistID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ProdOrderID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ProdOrderID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var prodOrder = declaringEntityType.AddNavigation("ProdOrder",
                runtimeForeignKey,
                onDependent: true,
                typeof(ProdOrder),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("ProdOrder", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_ProdOrder", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslist_ProdOrder = principalEntityType.AddNavigation("ProdOrderPartslist_ProdOrder",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslist>),
                propertyInfo: typeof(ProdOrder).GetProperty("ProdOrderPartslist_ProdOrder", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrder).GetField("_ProdOrderPartslist_ProdOrder", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslist_ProdOrderID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBiACProgramID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACProgramID") }),
                principalEntityType);

            var vBiACProgram = declaringEntityType.AddNavigation("VBiACProgram",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACProgram),
                propertyInfo: typeof(ProdOrderPartslist).GetProperty("VBiACProgram", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslist).GetField("_VBiACProgram", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslist_VBiACProgram = principalEntityType.AddNavigation("ProdOrderPartslist_VBiACProgram",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslist>),
                propertyInfo: typeof(ACProgram).GetProperty("ProdOrderPartslist_VBiACProgram", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACProgram).GetField("_ProdOrderPartslist_VBiACProgram", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslist_ACProgramID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "ProdOrderPartslist");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
