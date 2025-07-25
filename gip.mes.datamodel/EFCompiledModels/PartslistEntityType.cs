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
    public partial class PartslistEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.Partslist",
                typeof(Partslist),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(Partslist)),
                propertyCount: 32,
                navigationCount: 19,
                servicePropertyCount: 1,
                foreignKeyCount: 4,
                namedIndexCount: 6,
                keyCount: 1,
                triggerCount: 1);

            var partslistID = runtimeEntityType.AddProperty(
                "PartslistID",
                typeof(Guid),
                propertyInfo: typeof(Partslist).GetProperty("PartslistID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_PartslistID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            partslistID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deleteDate = runtimeEntityType.AddProperty(
                "DeleteDate",
                typeof(DateTime?),
                propertyInfo: typeof(Partslist).GetProperty("DeleteDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_DeleteDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            deleteDate.AddAnnotation("Relational:ColumnType", "datetime");
            deleteDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deleteName = runtimeEntityType.AddProperty(
                "DeleteName",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("DeleteName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_DeleteName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            deleteName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var enabledFrom = runtimeEntityType.AddProperty(
                "EnabledFrom",
                typeof(DateTime?),
                propertyInfo: typeof(Partslist).GetProperty("EnabledFrom", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_EnabledFrom", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            enabledFrom.AddAnnotation("Relational:ColumnType", "datetime");
            enabledFrom.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var enabledTo = runtimeEntityType.AddProperty(
                "EnabledTo",
                typeof(DateTime?),
                propertyInfo: typeof(Partslist).GetProperty("EnabledTo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_EnabledTo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            enabledTo.AddAnnotation("Relational:ColumnType", "datetime");
            enabledTo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var grossWeight = runtimeEntityType.AddProperty(
                "GrossWeight",
                typeof(double),
                propertyInfo: typeof(Partslist).GetProperty("GrossWeight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_GrossWeight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            grossWeight.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(Partslist).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isDefault = runtimeEntityType.AddProperty(
                "IsDefault",
                typeof(bool),
                propertyInfo: typeof(Partslist).GetProperty("IsDefault", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_IsDefault", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isDefault.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isEnabled = runtimeEntityType.AddProperty(
                "IsEnabled",
                typeof(bool),
                propertyInfo: typeof(Partslist).GetProperty("IsEnabled", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_IsEnabled", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isEnabled.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isInEnabledPeriod = runtimeEntityType.AddProperty(
                "IsInEnabledPeriod",
                typeof(bool?),
                propertyInfo: typeof(Partslist).GetProperty("IsInEnabledPeriod", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_IsInEnabledPeriod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                valueGenerated: ValueGenerated.OnAddOrUpdate,
                beforeSaveBehavior: PropertySaveBehavior.Ignore,
                afterSaveBehavior: PropertySaveBehavior.Ignore);
            isInEnabledPeriod.AddAnnotation("Relational:ComputedColumnSql", "([dbo].[udf_IsTimeSpanActual]([EnabledFrom],[EnabledTo]))");
            isInEnabledPeriod.AddAnnotation("Relational:IsStored", false);
            isInEnabledPeriod.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isProductionUnit = runtimeEntityType.AddProperty(
                "IsProductionUnit",
                typeof(bool),
                propertyInfo: typeof(Partslist).GetProperty("IsProductionUnit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_IsProductionUnit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isProductionUnit.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isStandard = runtimeEntityType.AddProperty(
                "IsStandard",
                typeof(bool),
                propertyInfo: typeof(Partslist).GetProperty("IsStandard", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_IsStandard", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            isStandard.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var keyOfExtSys = runtimeEntityType.AddProperty(
                "KeyOfExtSys",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("KeyOfExtSys", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_KeyOfExtSys", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 40,
                unicode: false);
            keyOfExtSys.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lastFormulaChange = runtimeEntityType.AddProperty(
                "LastFormulaChange",
                typeof(DateTime),
                propertyInfo: typeof(Partslist).GetProperty("LastFormulaChange", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_LastFormulaChange", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            lastFormulaChange.AddAnnotation("Relational:ColumnType", "datetime");
            lastFormulaChange.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDUnitID = runtimeEntityType.AddProperty(
                "MDUnitID",
                typeof(Guid?),
                propertyInfo: typeof(Partslist).GetProperty("MDUnitID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_MDUnitID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDUnitID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialID = runtimeEntityType.AddProperty(
                "MaterialID",
                typeof(Guid),
                propertyInfo: typeof(Partslist).GetProperty("MaterialID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_MaterialID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            materialID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialWFID = runtimeEntityType.AddProperty(
                "MaterialWFID",
                typeof(Guid?),
                propertyInfo: typeof(Partslist).GetProperty("MaterialWFID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_MaterialWFID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            materialWFID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var partslistName = runtimeEntityType.AddProperty(
                "PartslistName",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("PartslistName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_PartslistName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 350);
            partslistName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var partslistNo = runtimeEntityType.AddProperty(
                "PartslistNo",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("PartslistNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_PartslistNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 40,
                unicode: false);
            partslistNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var partslistVersion = runtimeEntityType.AddProperty(
                "PartslistVersion",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("PartslistVersion", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_PartslistVersion", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            partslistVersion.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var previousPartslistID = runtimeEntityType.AddProperty(
                "PreviousPartslistID",
                typeof(Guid?),
                propertyInfo: typeof(Partslist).GetProperty("PreviousPartslistID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_PreviousPartslistID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            previousPartslistID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var productionUnits = runtimeEntityType.AddProperty(
                "ProductionUnits",
                typeof(double?),
                propertyInfo: typeof(Partslist).GetProperty("ProductionUnits", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_ProductionUnits", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            productionUnits.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var productionWeight = runtimeEntityType.AddProperty(
                "ProductionWeight",
                typeof(float),
                propertyInfo: typeof(Partslist).GetProperty("ProductionWeight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_ProductionWeight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0f);
            productionWeight.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetQuantity = runtimeEntityType.AddProperty(
                "TargetQuantity",
                typeof(double),
                propertyInfo: typeof(Partslist).GetProperty("TargetQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_TargetQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetQuantityUOM = runtimeEntityType.AddProperty(
                "TargetQuantityUOM",
                typeof(double),
                propertyInfo: typeof(Partslist).GetProperty("TargetQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_TargetQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(Partslist).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLComment = runtimeEntityType.AddProperty(
                "XMLComment",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("XMLComment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_XMLComment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLComment.AddAnnotation("Relational:ColumnType", "text");
            xMLComment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLDesign = runtimeEntityType.AddProperty(
                "XMLDesign",
                typeof(string),
                propertyInfo: typeof(Partslist).GetProperty("XMLDesign", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_XMLDesign", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLDesign.AddAnnotation("Relational:ColumnType", "text");
            xMLDesign.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(Partslist).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { partslistID });
            runtimeEntityType.SetPrimaryKey(key);

            var nCI_FK_Partslist_MDUnitID = runtimeEntityType.AddIndex(
                new[] { mDUnitID },
                name: "NCI_FK_Partslist_MDUnitID");

            var nCI_FK_Partslist_MaterialID = runtimeEntityType.AddIndex(
                new[] { materialID },
                name: "NCI_FK_Partslist_MaterialID");

            var nCI_FK_Partslist_MaterialWFID = runtimeEntityType.AddIndex(
                new[] { materialWFID },
                name: "NCI_FK_Partslist_MaterialWFID");

            var nCI_FK_Partslist_PreviousPartslistID = runtimeEntityType.AddIndex(
                new[] { previousPartslistID },
                name: "NCI_FK_Partslist_PreviousPartslistID");

            var nCI_Partslist_PartslistID_MaterialID = runtimeEntityType.AddIndex(
                new[] { partslistID, materialID },
                name: "NCI_Partslist_PartslistID_MaterialID");

            var partslistVersion_PartslistNo = runtimeEntityType.AddIndex(
                new[] { partslistNo, partslistVersion, deleteDate },
                name: "PartslistVersion_PartslistNo",
                unique: true);
            partslistVersion_PartslistNo.AddAnnotation("Relational:Filter", "[PartslistNo] IS NOT NULL AND [PartslistVersion] IS NOT NULL AND [DeleteDate] IS NOT NULL");

            var dboudf_IsTimeSpanActualEnabledFromEnabledTo = runtimeEntityType.AddTrigger(
                "([dbo].[udf_IsTimeSpanActual]([EnabledFrom],[EnabledTo]))");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDUnitID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDUnitID") }),
                principalEntityType);

            var mDUnit = declaringEntityType.AddNavigation("MDUnit",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDUnit),
                propertyInfo: typeof(Partslist).GetProperty("MDUnit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_MDUnit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var partslist_MDUnit = principalEntityType.AddNavigation("Partslist_MDUnit",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Partslist>),
                propertyInfo: typeof(MDUnit).GetProperty("Partslist_MDUnit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDUnit).GetField("_Partslist_MDUnit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Partslist_MDUnitID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaterialID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaterialID") }),
                principalEntityType,
                required: true);

            var material = declaringEntityType.AddNavigation("Material",
                runtimeForeignKey,
                onDependent: true,
                typeof(Material),
                propertyInfo: typeof(Partslist).GetProperty("Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var partslist_Material = principalEntityType.AddNavigation("Partslist_Material",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Partslist>),
                propertyInfo: typeof(Material).GetProperty("Partslist_Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Material).GetField("_Partslist_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Partslist_MaterialID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaterialWFID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaterialWFID") }),
                principalEntityType);

            var materialWF = declaringEntityType.AddNavigation("MaterialWF",
                runtimeForeignKey,
                onDependent: true,
                typeof(MaterialWF),
                propertyInfo: typeof(Partslist).GetProperty("MaterialWF", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_MaterialWF", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var partslist_MaterialWF = principalEntityType.AddNavigation("Partslist_MaterialWF",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Partslist>),
                propertyInfo: typeof(MaterialWF).GetProperty("Partslist_MaterialWF", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaterialWF).GetField("_Partslist_MaterialWF", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_Partslist_MaterialWF");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("PreviousPartslistID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("PartslistID") }),
                principalEntityType);

            var partslist1_PreviousPartslist = declaringEntityType.AddNavigation("Partslist1_PreviousPartslist",
                runtimeForeignKey,
                onDependent: true,
                typeof(Partslist),
                propertyInfo: typeof(Partslist).GetProperty("Partslist1_PreviousPartslist", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_Partslist1_PreviousPartslist", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var partslist_PreviousPartslist = principalEntityType.AddNavigation("Partslist_PreviousPartslist",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Partslist>),
                propertyInfo: typeof(Partslist).GetProperty("Partslist_PreviousPartslist", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Partslist).GetField("_Partslist_PreviousPartslist", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "PreviousPartslistID_PartslistID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Partslist");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);
            runtimeEntityType.AddAnnotation("SqlServer:UseSqlOutputClause", false);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
