﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    internal partial class ProdOrderPartslistPosFacilityLotEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.ProdOrderPartslistPosFacilityLot",
                typeof(ProdOrderPartslistPosFacilityLot),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(ProdOrderPartslistPosFacilityLot)));

            var prodOrderPartslistPosFacilityLotID = runtimeEntityType.AddProperty(
                "ProdOrderPartslistPosFacilityLotID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("ProdOrderPartslistPosFacilityLotID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_ProdOrderPartslistPosFacilityLotID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            prodOrderPartslistPosFacilityLotID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var facilityLotID = runtimeEntityType.AddProperty(
                "FacilityLotID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("FacilityLotID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_FacilityLotID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            facilityLotID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isActive = runtimeEntityType.AddProperty(
                "IsActive",
                typeof(bool?),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("IsActive", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_IsActive", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            isActive.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodOrderPartslistPosID = runtimeEntityType.AddProperty(
                "ProdOrderPartslistPosID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("ProdOrderPartslistPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_ProdOrderPartslistPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            prodOrderPartslistPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { prodOrderPartslistPosFacilityLotID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { facilityLotID });

            var uXPosLotUnique = runtimeEntityType.AddIndex(
                new[] { prodOrderPartslistPosID, facilityLotID },
                name: "UXPosLotUnique",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("FacilityLotID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("FacilityLotID") }),
                principalEntityType,
                required: true);

            var facilityLot = declaringEntityType.AddNavigation("FacilityLot",
                runtimeForeignKey,
                onDependent: true,
                typeof(FacilityLot),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("FacilityLot", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_FacilityLot", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslistPosFacilityLotFacilityLot = principalEntityType.AddNavigation("ProdOrderPartslistPosFacilityLot_FacilityLot",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslistPosFacilityLot>),
                propertyInfo: typeof(FacilityLot).GetProperty("ProdOrderPartslistPosFacilityLot_FacilityLot", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(FacilityLot).GetField("_ProdOrderPartslistPosFacilityLot_FacilityLot", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslistPosFacilityLot_FacilityLot");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ProdOrderPartslistPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ProdOrderPartslistPosID") }),
                principalEntityType,
                required: true);

            var prodOrderPartslistPos = declaringEntityType.AddNavigation("ProdOrderPartslistPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(ProdOrderPartslistPos),
                propertyInfo: typeof(ProdOrderPartslistPosFacilityLot).GetProperty("ProdOrderPartslistPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosFacilityLot).GetField("_ProdOrderPartslistPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslistPosFacilityLotProdOrderPartslistPos = principalEntityType.AddNavigation("ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslistPosFacilityLot>),
                propertyInfo: typeof(ProdOrderPartslistPos).GetProperty("ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPos).GetField("_ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "ProdOrderPartslistPosFacilityLot");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}