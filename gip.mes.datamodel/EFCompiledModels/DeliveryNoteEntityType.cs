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
    public partial class DeliveryNoteEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.DeliveryNote",
                typeof(DeliveryNote),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(DeliveryNote)),
                propertyCount: 29,
                navigationCount: 8,
                servicePropertyCount: 1,
                foreignKeyCount: 6,
                unnamedIndexCount: 1,
                namedIndexCount: 7,
                keyCount: 1);

            var deliveryNoteID = runtimeEntityType.AddProperty(
                "DeliveryNoteID",
                typeof(Guid),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryNoteID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryNoteID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            deliveryNoteID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(DeliveryNote).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var delivery2CompanyAddressID = runtimeEntityType.AddProperty(
                "Delivery2CompanyAddressID",
                typeof(Guid?),
                propertyInfo: typeof(DeliveryNote).GetProperty("Delivery2CompanyAddressID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_Delivery2CompanyAddressID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            delivery2CompanyAddressID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryCompanyAddressID = runtimeEntityType.AddProperty(
                "DeliveryCompanyAddressID",
                typeof(Guid),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryCompanyAddressID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryCompanyAddressID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            deliveryCompanyAddressID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryDate = runtimeEntityType.AddProperty(
                "DeliveryDate",
                typeof(DateTime),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            deliveryDate.AddAnnotation("Relational:ColumnType", "datetime");
            deliveryDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryNoteNo = runtimeEntityType.AddProperty(
                "DeliveryNoteNo",
                typeof(string),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryNoteNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryNoteNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 30,
                unicode: false);
            deliveryNoteNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryNoteTypeIndex = runtimeEntityType.AddProperty(
                "DeliveryNoteTypeIndex",
                typeof(short),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryNoteTypeIndex", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryNoteTypeIndex", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: (short)0);
            deliveryNoteTypeIndex.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryWeightDeliveryIn = runtimeEntityType.AddProperty(
                "DeliveryWeightDeliveryIn",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryWeightDeliveryIn", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryWeightDeliveryIn", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            deliveryWeightDeliveryIn.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryWeightDeliveryOut = runtimeEntityType.AddProperty(
                "DeliveryWeightDeliveryOut",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryWeightDeliveryOut", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryWeightDeliveryOut", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            deliveryWeightDeliveryOut.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryWeightOrderIn = runtimeEntityType.AddProperty(
                "DeliveryWeightOrderIn",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryWeightOrderIn", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryWeightOrderIn", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            deliveryWeightOrderIn.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryWeightOrderOut = runtimeEntityType.AddProperty(
                "DeliveryWeightOrderOut",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryWeightOrderOut", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryWeightOrderOut", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            deliveryWeightOrderOut.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryWeightStockIn = runtimeEntityType.AddProperty(
                "DeliveryWeightStockIn",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryWeightStockIn", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryWeightStockIn", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            deliveryWeightStockIn.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryWeightStockOut = runtimeEntityType.AddProperty(
                "DeliveryWeightStockOut",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryWeightStockOut", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryWeightStockOut", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            deliveryWeightStockOut.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var emptyWeight = runtimeEntityType.AddProperty(
                "EmptyWeight",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("EmptyWeight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_EmptyWeight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            emptyWeight.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(DeliveryNote).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(DeliveryNote).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lossComment = runtimeEntityType.AddProperty(
                "LossComment",
                typeof(string),
                propertyInfo: typeof(DeliveryNote).GetProperty("LossComment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_LossComment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            lossComment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lossWeight = runtimeEntityType.AddProperty(
                "LossWeight",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("LossWeight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_LossWeight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            lossWeight.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDDelivNoteStateID = runtimeEntityType.AddProperty(
                "MDDelivNoteStateID",
                typeof(Guid),
                propertyInfo: typeof(DeliveryNote).GetProperty("MDDelivNoteStateID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_MDDelivNoteStateID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            mDDelivNoteStateID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var netWeight = runtimeEntityType.AddProperty(
                "NetWeight",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("NetWeight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_NetWeight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            netWeight.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var shipperCompanyAddressID = runtimeEntityType.AddProperty(
                "ShipperCompanyAddressID",
                typeof(Guid),
                propertyInfo: typeof(DeliveryNote).GetProperty("ShipperCompanyAddressID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_ShipperCompanyAddressID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            shipperCompanyAddressID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var supplierDeliveryNo = runtimeEntityType.AddProperty(
                "SupplierDeliveryNo",
                typeof(string),
                propertyInfo: typeof(DeliveryNote).GetProperty("SupplierDeliveryNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_SupplierDeliveryNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 40,
                unicode: false);
            supplierDeliveryNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var totalWeight = runtimeEntityType.AddProperty(
                "TotalWeight",
                typeof(double),
                propertyInfo: typeof(DeliveryNote).GetProperty("TotalWeight", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_TotalWeight", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            totalWeight.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var tourplanPosID = runtimeEntityType.AddProperty(
                "TourplanPosID",
                typeof(Guid?),
                propertyInfo: typeof(DeliveryNote).GetProperty("TourplanPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_TourplanPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            tourplanPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(DeliveryNote).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(DeliveryNote).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var visitorVoucherID = runtimeEntityType.AddProperty(
                "VisitorVoucherID",
                typeof(Guid?),
                propertyInfo: typeof(DeliveryNote).GetProperty("VisitorVoucherID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_VisitorVoucherID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            visitorVoucherID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var weighingID = runtimeEntityType.AddProperty(
                "WeighingID",
                typeof(Guid?),
                propertyInfo: typeof(DeliveryNote).GetProperty("WeighingID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_WeighingID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            weighingID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(DeliveryNote).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { deliveryNoteID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { delivery2CompanyAddressID });

            var nCI_FK_DeliveryNote_DeliveryCompanyAddressID = runtimeEntityType.AddIndex(
                new[] { deliveryCompanyAddressID },
                name: "NCI_FK_DeliveryNote_DeliveryCompanyAddressID");

            var nCI_FK_DeliveryNote_MDDelivNoteStateID = runtimeEntityType.AddIndex(
                new[] { mDDelivNoteStateID },
                name: "NCI_FK_DeliveryNote_MDDelivNoteStateID");

            var nCI_FK_DeliveryNote_ShipperCompanyAddressID = runtimeEntityType.AddIndex(
                new[] { shipperCompanyAddressID },
                name: "NCI_FK_DeliveryNote_ShipperCompanyAddressID");

            var nCI_FK_DeliveryNote_TourplanPosID = runtimeEntityType.AddIndex(
                new[] { tourplanPosID },
                name: "NCI_FK_DeliveryNote_TourplanPosID");

            var nCI_FK_DeliveryNote_VisitorVoucherID = runtimeEntityType.AddIndex(
                new[] { visitorVoucherID },
                name: "NCI_FK_DeliveryNote_VisitorVoucherID");

            var nCI_FK_DeliveryNote_WeighingID = runtimeEntityType.AddIndex(
                new[] { weighingID },
                name: "NCI_FK_DeliveryNote_WeighingID");

            var uIX_DeliveryNote_DeliveryNoteNo = runtimeEntityType.AddIndex(
                new[] { deliveryNoteNo },
                name: "UIX_DeliveryNote_DeliveryNoteNo",
                unique: true);
            uIX_DeliveryNote_DeliveryNoteNo.AddAnnotation("Relational:Filter", null);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("Delivery2CompanyAddressID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("CompanyAddressID") }),
                principalEntityType);

            var delivery2CompanyAddress = declaringEntityType.AddNavigation("Delivery2CompanyAddress",
                runtimeForeignKey,
                onDependent: true,
                typeof(CompanyAddress),
                propertyInfo: typeof(DeliveryNote).GetProperty("Delivery2CompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_Delivery2CompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var deliveryNote_Delivery2CompanyAddress = principalEntityType.AddNavigation("DeliveryNote_Delivery2CompanyAddress",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<DeliveryNote>),
                propertyInfo: typeof(CompanyAddress).GetProperty("DeliveryNote_Delivery2CompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyAddress).GetField("_DeliveryNote_Delivery2CompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_DeliveryNote_Delivery2CompanyAddressID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("DeliveryCompanyAddressID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("CompanyAddressID") }),
                principalEntityType,
                required: true);

            var deliveryCompanyAddress = declaringEntityType.AddNavigation("DeliveryCompanyAddress",
                runtimeForeignKey,
                onDependent: true,
                typeof(CompanyAddress),
                propertyInfo: typeof(DeliveryNote).GetProperty("DeliveryCompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_DeliveryCompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var deliveryNote_DeliveryCompanyAddress = principalEntityType.AddNavigation("DeliveryNote_DeliveryCompanyAddress",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<DeliveryNote>),
                propertyInfo: typeof(CompanyAddress).GetProperty("DeliveryNote_DeliveryCompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyAddress).GetField("_DeliveryNote_DeliveryCompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_DeliveryNote_DeliveryCompanyAddressID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDDelivNoteStateID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDDelivNoteStateID") }),
                principalEntityType,
                required: true);

            var mDDelivNoteState = declaringEntityType.AddNavigation("MDDelivNoteState",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDDelivNoteState),
                propertyInfo: typeof(DeliveryNote).GetProperty("MDDelivNoteState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_MDDelivNoteState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var deliveryNote_MDDelivNoteState = principalEntityType.AddNavigation("DeliveryNote_MDDelivNoteState",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<DeliveryNote>),
                propertyInfo: typeof(MDDelivNoteState).GetProperty("DeliveryNote_MDDelivNoteState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDDelivNoteState).GetField("_DeliveryNote_MDDelivNoteState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_DeliveryNote_MDDelivNoteStateID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ShipperCompanyAddressID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("CompanyAddressID") }),
                principalEntityType,
                required: true);

            var shipperCompanyAddress = declaringEntityType.AddNavigation("ShipperCompanyAddress",
                runtimeForeignKey,
                onDependent: true,
                typeof(CompanyAddress),
                propertyInfo: typeof(DeliveryNote).GetProperty("ShipperCompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_ShipperCompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var deliveryNote_ShipperCompanyAddress = principalEntityType.AddNavigation("DeliveryNote_ShipperCompanyAddress",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<DeliveryNote>),
                propertyInfo: typeof(CompanyAddress).GetProperty("DeliveryNote_ShipperCompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyAddress).GetField("_DeliveryNote_ShipperCompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_DeliveryNote_ShipperCompanyAddressID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey5(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("TourplanPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("TourplanPosID") }),
                principalEntityType);

            var tourplanPos = declaringEntityType.AddNavigation("TourplanPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(TourplanPos),
                propertyInfo: typeof(DeliveryNote).GetProperty("TourplanPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_TourplanPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var deliveryNote_TourplanPos = principalEntityType.AddNavigation("DeliveryNote_TourplanPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<DeliveryNote>),
                propertyInfo: typeof(TourplanPos).GetProperty("DeliveryNote_TourplanPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(TourplanPos).GetField("_DeliveryNote_TourplanPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_DeliveryNote_TourplanPosID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VisitorVoucherID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("VisitorVoucherID") }),
                principalEntityType);

            var visitorVoucher = declaringEntityType.AddNavigation("VisitorVoucher",
                runtimeForeignKey,
                onDependent: true,
                typeof(VisitorVoucher),
                propertyInfo: typeof(DeliveryNote).GetProperty("VisitorVoucher", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(DeliveryNote).GetField("_VisitorVoucher", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var deliveryNote_VisitorVoucher = principalEntityType.AddNavigation("DeliveryNote_VisitorVoucher",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<DeliveryNote>),
                propertyInfo: typeof(VisitorVoucher).GetProperty("DeliveryNote_VisitorVoucher", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VisitorVoucher).GetField("_DeliveryNote_VisitorVoucher", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_DeliveryNote_VisitorVoucherID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "DeliveryNote");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
