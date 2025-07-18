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
    public partial class OutOfferPosEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.OutOfferPos",
                typeof(OutOfferPos),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(OutOfferPos)),
                propertyCount: 30,
                navigationCount: 11,
                servicePropertyCount: 1,
                foreignKeyCount: 9,
                unnamedIndexCount: 9,
                keyCount: 1);

            var outOfferPosID = runtimeEntityType.AddProperty(
                "OutOfferPosID",
                typeof(Guid),
                propertyInfo: typeof(OutOfferPos).GetProperty("OutOfferPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_OutOfferPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            outOfferPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(OutOfferPos).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment2 = runtimeEntityType.AddProperty(
                "Comment2",
                typeof(string),
                propertyInfo: typeof(OutOfferPos).GetProperty("Comment2", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_Comment2", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment2.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var groupOutOfferPosID = runtimeEntityType.AddProperty(
                "GroupOutOfferPosID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferPos).GetProperty("GroupOutOfferPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_GroupOutOfferPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            groupOutOfferPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var groupSum = runtimeEntityType.AddProperty(
                "GroupSum",
                typeof(bool),
                propertyInfo: typeof(OutOfferPos).GetProperty("GroupSum", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_GroupSum", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            groupSum.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(OutOfferPos).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(OutOfferPos).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDCountrySalesTaxID = runtimeEntityType.AddProperty(
                "MDCountrySalesTaxID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDCountrySalesTaxID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDCountrySalesTaxID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDCountrySalesTaxID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDCountrySalesTaxMDMaterialGroupID = runtimeEntityType.AddProperty(
                "MDCountrySalesTaxMDMaterialGroupID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDCountrySalesTaxMDMaterialGroupID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDCountrySalesTaxMDMaterialGroupID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDCountrySalesTaxMDMaterialGroupID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDCountrySalesTaxMaterialID = runtimeEntityType.AddProperty(
                "MDCountrySalesTaxMaterialID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDCountrySalesTaxMaterialID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDCountrySalesTaxMaterialID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDCountrySalesTaxMaterialID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDTimeRangeID = runtimeEntityType.AddProperty(
                "MDTimeRangeID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDTimeRangeID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDTimeRangeID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDTimeRangeID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDUnitID = runtimeEntityType.AddProperty(
                "MDUnitID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDUnitID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDUnitID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDUnitID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialID = runtimeEntityType.AddProperty(
                "MaterialID",
                typeof(Guid),
                propertyInfo: typeof(OutOfferPos).GetProperty("MaterialID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MaterialID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            materialID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialPosTypeIndex = runtimeEntityType.AddProperty(
                "MaterialPosTypeIndex",
                typeof(short),
                propertyInfo: typeof(OutOfferPos).GetProperty("MaterialPosTypeIndex", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MaterialPosTypeIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (short)0);
            materialPosTypeIndex.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var outOfferID = runtimeEntityType.AddProperty(
                "OutOfferID",
                typeof(Guid),
                propertyInfo: typeof(OutOfferPos).GetProperty("OutOfferID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_OutOfferID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            outOfferID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var parentOutOfferPosID = runtimeEntityType.AddProperty(
                "ParentOutOfferPosID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferPos).GetProperty("ParentOutOfferPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_ParentOutOfferPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            parentOutOfferPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var priceGross = runtimeEntityType.AddProperty(
                "PriceGross",
                typeof(decimal),
                propertyInfo: typeof(OutOfferPos).GetProperty("PriceGross", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_PriceGross", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0m);
            priceGross.AddAnnotation("Relational:ColumnType", "money");
            priceGross.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var priceNet = runtimeEntityType.AddProperty(
                "PriceNet",
                typeof(decimal),
                propertyInfo: typeof(OutOfferPos).GetProperty("PriceNet", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_PriceNet", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0m);
            priceNet.AddAnnotation("Relational:ColumnType", "money");
            priceNet.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var salesTax = runtimeEntityType.AddProperty(
                "SalesTax",
                typeof(decimal),
                propertyInfo: typeof(OutOfferPos).GetProperty("SalesTax", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_SalesTax", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0m);
            salesTax.AddAnnotation("Relational:ColumnType", "money");
            salesTax.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sequence = runtimeEntityType.AddProperty(
                "Sequence",
                typeof(int),
                propertyInfo: typeof(OutOfferPos).GetProperty("Sequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_Sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            sequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetDeliveryDate = runtimeEntityType.AddProperty(
                "TargetDeliveryDate",
                typeof(DateTime),
                propertyInfo: typeof(OutOfferPos).GetProperty("TargetDeliveryDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_TargetDeliveryDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            targetDeliveryDate.AddAnnotation("Relational:ColumnType", "datetime");
            targetDeliveryDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetDeliveryMaxDate = runtimeEntityType.AddProperty(
                "TargetDeliveryMaxDate",
                typeof(DateTime?),
                propertyInfo: typeof(OutOfferPos).GetProperty("TargetDeliveryMaxDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_TargetDeliveryMaxDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            targetDeliveryMaxDate.AddAnnotation("Relational:ColumnType", "datetime");
            targetDeliveryMaxDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetDeliveryPriority = runtimeEntityType.AddProperty(
                "TargetDeliveryPriority",
                typeof(short),
                propertyInfo: typeof(OutOfferPos).GetProperty("TargetDeliveryPriority", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_TargetDeliveryPriority", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (short)0);
            targetDeliveryPriority.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetQuantity = runtimeEntityType.AddProperty(
                "TargetQuantity",
                typeof(double),
                propertyInfo: typeof(OutOfferPos).GetProperty("TargetQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_TargetQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetQuantityUOM = runtimeEntityType.AddProperty(
                "TargetQuantityUOM",
                typeof(double),
                propertyInfo: typeof(OutOfferPos).GetProperty("TargetQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_TargetQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetWeight = runtimeEntityType.AddProperty(
                "TargetWeight",
                typeof(double),
                propertyInfo: typeof(OutOfferPos).GetProperty("TargetWeight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_TargetWeight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetWeight.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(OutOfferPos).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(OutOfferPos).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLDesign = runtimeEntityType.AddProperty(
                "XMLDesign",
                typeof(string),
                propertyInfo: typeof(OutOfferPos).GetProperty("XMLDesign", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_XMLDesign", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLDesign.AddAnnotation("Relational:ColumnType", "text");
            xMLDesign.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(OutOfferPos).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { outOfferPosID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { groupOutOfferPosID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { mDCountrySalesTaxID });

            var index1 = runtimeEntityType.AddIndex(
                new[] { mDCountrySalesTaxMDMaterialGroupID });

            var index2 = runtimeEntityType.AddIndex(
                new[] { mDCountrySalesTaxMaterialID });

            var index3 = runtimeEntityType.AddIndex(
                new[] { mDTimeRangeID });

            var index4 = runtimeEntityType.AddIndex(
                new[] { mDUnitID });

            var index5 = runtimeEntityType.AddIndex(
                new[] { materialID });

            var index6 = runtimeEntityType.AddIndex(
                new[] { outOfferID });

            var index7 = runtimeEntityType.AddIndex(
                new[] { parentOutOfferPosID });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("GroupOutOfferPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("OutOfferPosID") }),
                principalEntityType);

            var outOfferPos1_GroupOutOfferPos = declaringEntityType.AddNavigation("OutOfferPos1_GroupOutOfferPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(OutOfferPos),
                propertyInfo: typeof(OutOfferPos).GetProperty("OutOfferPos1_GroupOutOfferPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_OutOfferPos1_GroupOutOfferPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_GroupOutOfferPos = principalEntityType.AddNavigation("OutOfferPos_GroupOutOfferPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(OutOfferPos).GetProperty("OutOfferPos_GroupOutOfferPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_OutOfferPos_GroupOutOfferPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_GroupOutOfferPosID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDCountrySalesTaxID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDCountrySalesTaxID") }),
                principalEntityType);

            var mDCountrySalesTax = declaringEntityType.AddNavigation("MDCountrySalesTax",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDCountrySalesTax),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDCountrySalesTax", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDCountrySalesTax", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_MDCountrySalesTax = principalEntityType.AddNavigation("OutOfferPos_MDCountrySalesTax",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(MDCountrySalesTax).GetProperty("OutOfferPos_MDCountrySalesTax", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDCountrySalesTax).GetField("_OutOfferPos_MDCountrySalesTax", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_MDCountrySalesTaxID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDCountrySalesTaxMDMaterialGroupID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDCountrySalesTaxMDMaterialGroupID") }),
                principalEntityType);

            var mDCountrySalesTaxMDMaterialGroup = declaringEntityType.AddNavigation("MDCountrySalesTaxMDMaterialGroup",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDCountrySalesTaxMDMaterialGroup),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDCountrySalesTaxMDMaterialGroup", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDCountrySalesTaxMDMaterialGroup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_MDCountrySalesTaxMDMaterialGroup = principalEntityType.AddNavigation("OutOfferPos_MDCountrySalesTaxMDMaterialGroup",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(MDCountrySalesTaxMDMaterialGroup).GetProperty("OutOfferPos_MDCountrySalesTaxMDMaterialGroup", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDCountrySalesTaxMDMaterialGroup).GetField("_OutOfferPos_MDCountrySalesTaxMDMaterialGroup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_MDCountrySalesTaxMDMaterialGroupID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDCountrySalesTaxMaterialID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDCountrySalesTaxMaterialID") }),
                principalEntityType);

            var mDCountrySalesTaxMaterial = declaringEntityType.AddNavigation("MDCountrySalesTaxMaterial",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDCountrySalesTaxMaterial),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDCountrySalesTaxMaterial", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDCountrySalesTaxMaterial", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_MDCountrySalesTaxMaterial = principalEntityType.AddNavigation("OutOfferPos_MDCountrySalesTaxMaterial",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(MDCountrySalesTaxMaterial).GetProperty("OutOfferPos_MDCountrySalesTaxMaterial", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDCountrySalesTaxMaterial).GetField("_OutOfferPos_MDCountrySalesTaxMaterial", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_MDCountrySalesTaxMaterialID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey5(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDTimeRangeID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDTimeRangeID") }),
                principalEntityType);

            var mDTimeRange = declaringEntityType.AddNavigation("MDTimeRange",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDTimeRange),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDTimeRange", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDTimeRange", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_MDTimeRange = principalEntityType.AddNavigation("OutOfferPos_MDTimeRange",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(MDTimeRange).GetProperty("OutOfferPos_MDTimeRange", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDTimeRange).GetField("_OutOfferPos_MDTimeRange", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_MDTimeRangeID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDUnitID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDUnitID") }),
                principalEntityType);

            var mDUnit = declaringEntityType.AddNavigation("MDUnit",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDUnit),
                propertyInfo: typeof(OutOfferPos).GetProperty("MDUnit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_MDUnit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_MDUnit = principalEntityType.AddNavigation("OutOfferPos_MDUnit",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(MDUnit).GetProperty("OutOfferPos_MDUnit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDUnit).GetField("_OutOfferPos_MDUnit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_MDUnitID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey7(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaterialID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaterialID") }),
                principalEntityType,
                required: true);

            var material = declaringEntityType.AddNavigation("Material",
                runtimeForeignKey,
                onDependent: true,
                typeof(Material),
                propertyInfo: typeof(OutOfferPos).GetProperty("Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_Material = principalEntityType.AddNavigation("OutOfferPos_Material",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(Material).GetProperty("OutOfferPos_Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Material).GetField("_OutOfferPos_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_MaterialID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey8(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("OutOfferID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("OutOfferID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var outOffer = declaringEntityType.AddNavigation("OutOffer",
                runtimeForeignKey,
                onDependent: true,
                typeof(OutOffer),
                propertyInfo: typeof(OutOfferPos).GetProperty("OutOffer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_OutOffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_OutOffer = principalEntityType.AddNavigation("OutOfferPos_OutOffer",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(OutOffer).GetProperty("OutOfferPos_OutOffer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOffer).GetField("_OutOfferPos_OutOffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_OutOfferID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey9(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ParentOutOfferPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("OutOfferPosID") }),
                principalEntityType);

            var outOfferPos1_ParentOutOfferPos = declaringEntityType.AddNavigation("OutOfferPos1_ParentOutOfferPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(OutOfferPos),
                propertyInfo: typeof(OutOfferPos).GetProperty("OutOfferPos1_ParentOutOfferPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_OutOfferPos1_ParentOutOfferPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferPos_ParentOutOfferPos = principalEntityType.AddNavigation("OutOfferPos_ParentOutOfferPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferPos>),
                propertyInfo: typeof(OutOfferPos).GetProperty("OutOfferPos_ParentOutOfferPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferPos).GetField("_OutOfferPos_ParentOutOfferPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferPos_ParentOutOfferPosID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "OutOfferPos");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
