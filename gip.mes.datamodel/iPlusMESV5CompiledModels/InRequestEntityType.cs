﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using gip.core.datamodel;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    internal partial class InRequestEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.InRequest",
                typeof(InRequest),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(InRequest)));

            var inRequestID = runtimeEntityType.AddProperty(
                "InRequestID",
                typeof(Guid),
                propertyInfo: typeof(InRequest).GetProperty("InRequestID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_InRequestID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            inRequestID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var billingCompanyAddressID = runtimeEntityType.AddProperty(
                "BillingCompanyAddressID",
                typeof(Guid),
                propertyInfo: typeof(InRequest).GetProperty("BillingCompanyAddressID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_BillingCompanyAddressID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            billingCompanyAddressID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(InRequest).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var deliveryCompanyAddressID = runtimeEntityType.AddProperty(
                "DeliveryCompanyAddressID",
                typeof(Guid),
                propertyInfo: typeof(InRequest).GetProperty("DeliveryCompanyAddressID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_DeliveryCompanyAddressID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            deliveryCompanyAddressID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var distributorCompanyID = runtimeEntityType.AddProperty(
                "DistributorCompanyID",
                typeof(Guid),
                propertyInfo: typeof(InRequest).GetProperty("DistributorCompanyID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_DistributorCompanyID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            distributorCompanyID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var distributorOfferNo = runtimeEntityType.AddProperty(
                "DistributorOfferNo",
                typeof(string),
                propertyInfo: typeof(InRequest).GetProperty("DistributorOfferNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_DistributorOfferNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            distributorOfferNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inRequestDate = runtimeEntityType.AddProperty(
                "InRequestDate",
                typeof(DateTime),
                propertyInfo: typeof(InRequest).GetProperty("InRequestDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_InRequestDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            inRequestDate.AddAnnotation("Relational:ColumnType", "datetime");
            inRequestDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inRequestNo = runtimeEntityType.AddProperty(
                "InRequestNo",
                typeof(string),
                propertyInfo: typeof(InRequest).GetProperty("InRequestNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_InRequestNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            inRequestNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inRequestVersion = runtimeEntityType.AddProperty(
                "InRequestVersion",
                typeof(int),
                propertyInfo: typeof(InRequest).GetProperty("InRequestVersion", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_InRequestVersion", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            inRequestVersion.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(InRequest).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(InRequest).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDDelivTypeID = runtimeEntityType.AddProperty(
                "MDDelivTypeID",
                typeof(Guid),
                propertyInfo: typeof(InRequest).GetProperty("MDDelivTypeID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDDelivTypeID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            mDDelivTypeID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDInOrderTypeID = runtimeEntityType.AddProperty(
                "MDInOrderTypeID",
                typeof(Guid),
                propertyInfo: typeof(InRequest).GetProperty("MDInOrderTypeID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDInOrderTypeID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            mDInOrderTypeID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDInRequestStateID = runtimeEntityType.AddProperty(
                "MDInRequestStateID",
                typeof(Guid),
                propertyInfo: typeof(InRequest).GetProperty("MDInRequestStateID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDInRequestStateID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            mDInRequestStateID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDTermOfPaymentID = runtimeEntityType.AddProperty(
                "MDTermOfPaymentID",
                typeof(Guid?),
                propertyInfo: typeof(InRequest).GetProperty("MDTermOfPaymentID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDTermOfPaymentID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDTermOfPaymentID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDTimeRangeID = runtimeEntityType.AddProperty(
                "MDTimeRangeID",
                typeof(Guid?),
                propertyInfo: typeof(InRequest).GetProperty("MDTimeRangeID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDTimeRangeID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDTimeRangeID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var priceGross = runtimeEntityType.AddProperty(
                "PriceGross",
                typeof(decimal),
                propertyInfo: typeof(InRequest).GetProperty("PriceGross", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_PriceGross", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            priceGross.AddAnnotation("Relational:ColumnType", "money");
            priceGross.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var priceNet = runtimeEntityType.AddProperty(
                "PriceNet",
                typeof(decimal),
                propertyInfo: typeof(InRequest).GetProperty("PriceNet", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_PriceNet", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            priceNet.AddAnnotation("Relational:ColumnType", "money");
            priceNet.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetDeliveryDate = runtimeEntityType.AddProperty(
                "TargetDeliveryDate",
                typeof(DateTime),
                propertyInfo: typeof(InRequest).GetProperty("TargetDeliveryDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_TargetDeliveryDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            targetDeliveryDate.AddAnnotation("Relational:ColumnType", "datetime");
            targetDeliveryDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetDeliveryMaxDate = runtimeEntityType.AddProperty(
                "TargetDeliveryMaxDate",
                typeof(DateTime?),
                propertyInfo: typeof(InRequest).GetProperty("TargetDeliveryMaxDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_TargetDeliveryMaxDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            targetDeliveryMaxDate.AddAnnotation("Relational:ColumnType", "datetime");
            targetDeliveryMaxDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(InRequest).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(InRequest).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(InRequest).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { inRequestID });
            runtimeEntityType.SetPrimaryKey(key);

            var nCIFKInRequestBillingCompanyAddressID = runtimeEntityType.AddIndex(
                new[] { billingCompanyAddressID },
                name: "NCI_FK_InRequest_BillingCompanyAddressID");

            var nCIFKInRequestDeliveryCompanyAddressID = runtimeEntityType.AddIndex(
                new[] { deliveryCompanyAddressID },
                name: "NCI_FK_InRequest_DeliveryCompanyAddressID");

            var nCIFKInRequestDistributorCompanyID = runtimeEntityType.AddIndex(
                new[] { distributorCompanyID },
                name: "NCI_FK_InRequest_DistributorCompanyID");

            var nCIFKInRequestMDDelivTypeID = runtimeEntityType.AddIndex(
                new[] { mDDelivTypeID },
                name: "NCI_FK_InRequest_MDDelivTypeID");

            var nCIFKInRequestMDInOrderTypeID = runtimeEntityType.AddIndex(
                new[] { mDInOrderTypeID },
                name: "NCI_FK_InRequest_MDInOrderTypeID");

            var nCIFKInRequestMDInRequestStateID = runtimeEntityType.AddIndex(
                new[] { mDInRequestStateID },
                name: "NCI_FK_InRequest_MDInRequestStateID");

            var nCIFKInRequestMDTermOfPaymentID = runtimeEntityType.AddIndex(
                new[] { mDTermOfPaymentID },
                name: "NCI_FK_InRequest_MDTermOfPaymentID");

            var nCIFKInRequestMDTimeRangeID = runtimeEntityType.AddIndex(
                new[] { mDTimeRangeID },
                name: "NCI_FK_InRequest_MDTimeRangeID");

            var uIXInRequestInRequestNo = runtimeEntityType.AddIndex(
                new[] { inRequestNo },
                name: "UIX_InRequest_InRequestNo",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("BillingCompanyAddressID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("CompanyAddressID") }),
                principalEntityType,
                required: true);

            var billingCompanyAddress = declaringEntityType.AddNavigation("BillingCompanyAddress",
                runtimeForeignKey,
                onDependent: true,
                typeof(CompanyAddress),
                propertyInfo: typeof(InRequest).GetProperty("BillingCompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_BillingCompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestBillingCompanyAddress = principalEntityType.AddNavigation("InRequest_BillingCompanyAddress",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequest>),
                propertyInfo: typeof(CompanyAddress).GetProperty("InRequest_BillingCompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyAddress).GetField("_InRequest_BillingCompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequest_BillingCompanyAddressID");
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
                propertyInfo: typeof(InRequest).GetProperty("DeliveryCompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_DeliveryCompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestDeliveryCompanyAddress = principalEntityType.AddNavigation("InRequest_DeliveryCompanyAddress",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequest>),
                propertyInfo: typeof(CompanyAddress).GetProperty("InRequest_DeliveryCompanyAddress", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(CompanyAddress).GetField("_InRequest_DeliveryCompanyAddress", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequest_DeliveryCompanyAddressID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("DistributorCompanyID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("CompanyID") }),
                principalEntityType,
                required: true);

            var distributorCompany = declaringEntityType.AddNavigation("DistributorCompany",
                runtimeForeignKey,
                onDependent: true,
                typeof(Company),
                propertyInfo: typeof(InRequest).GetProperty("DistributorCompany", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_DistributorCompany", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestDistributorCompany = principalEntityType.AddNavigation("InRequest_DistributorCompany",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequest>),
                propertyInfo: typeof(Company).GetProperty("InRequest_DistributorCompany", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Company).GetField("_InRequest_DistributorCompany", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequest_CompanyID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDDelivTypeID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDDelivTypeID") }),
                principalEntityType,
                required: true);

            var mDDelivType = declaringEntityType.AddNavigation("MDDelivType",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDDelivType),
                propertyInfo: typeof(InRequest).GetProperty("MDDelivType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDDelivType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestMDDelivType = principalEntityType.AddNavigation("InRequest_MDDelivType",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequest>),
                propertyInfo: typeof(MDDelivType).GetProperty("InRequest_MDDelivType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDDelivType).GetField("_InRequest_MDDelivType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequest_MDDelivTypeID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey5(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDInOrderTypeID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDInOrderTypeID") }),
                principalEntityType,
                required: true);

            var mDInOrderType = declaringEntityType.AddNavigation("MDInOrderType",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDInOrderType),
                propertyInfo: typeof(InRequest).GetProperty("MDInOrderType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDInOrderType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestMDInOrderType = principalEntityType.AddNavigation("InRequest_MDInOrderType",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequest>),
                propertyInfo: typeof(MDInOrderType).GetProperty("InRequest_MDInOrderType", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDInOrderType).GetField("_InRequest_MDInOrderType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequest_MDInOrderTypeID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDInRequestStateID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDInRequestStateID") }),
                principalEntityType,
                required: true);

            var mDInRequestState = declaringEntityType.AddNavigation("MDInRequestState",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDInRequestState),
                propertyInfo: typeof(InRequest).GetProperty("MDInRequestState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDInRequestState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestMDInRequestState = principalEntityType.AddNavigation("InRequest_MDInRequestState",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequest>),
                propertyInfo: typeof(MDInRequestState).GetProperty("InRequest_MDInRequestState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDInRequestState).GetField("_InRequest_MDInRequestState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequest_MDInRequestStateID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey7(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDTermOfPaymentID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDTermOfPaymentID") }),
                principalEntityType);

            var mDTermOfPayment = declaringEntityType.AddNavigation("MDTermOfPayment",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDTermOfPayment),
                propertyInfo: typeof(InRequest).GetProperty("MDTermOfPayment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDTermOfPayment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestMDTermOfPayment = principalEntityType.AddNavigation("InRequest_MDTermOfPayment",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequest>),
                propertyInfo: typeof(MDTermOfPayment).GetProperty("InRequest_MDTermOfPayment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDTermOfPayment).GetField("_InRequest_MDTermOfPayment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequest_MDTermOfPaymentID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey8(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDTimeRangeID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDTimeRangeID") }),
                principalEntityType);

            var mDTimeRange = declaringEntityType.AddNavigation("MDTimeRange",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDTimeRange),
                propertyInfo: typeof(InRequest).GetProperty("MDTimeRange", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_MDTimeRange", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestMDTimeRange = principalEntityType.AddNavigation("InRequest_MDTimeRange",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequest>),
                propertyInfo: typeof(MDTimeRange).GetProperty("InRequest_MDTimeRange", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDTimeRange).GetField("_InRequest_MDTimeRange", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequest_MDTimeRangeID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "InRequest");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}