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
    internal partial class MaintOrderEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.MaintOrder",
                typeof(MaintOrder),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(MaintOrder)));

            var maintOrderID = runtimeEntityType.AddProperty(
                "MaintOrderID",
                typeof(Guid),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintOrderID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintOrderID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            maintOrderID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(MaintOrder).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var dirPicture = runtimeEntityType.AddProperty(
                "DirPicture",
                typeof(string),
                propertyInfo: typeof(MaintOrder).GetProperty("DirPicture", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_DirPicture", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 200,
                unicode: false);
            dirPicture.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var dirText = runtimeEntityType.AddProperty(
                "DirText",
                typeof(string),
                propertyInfo: typeof(MaintOrder).GetProperty("DirText", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_DirText", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 200,
                unicode: false);
            dirText.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(MaintOrder).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(MaintOrder).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDMaintModeID = runtimeEntityType.AddProperty(
                "MDMaintModeID",
                typeof(Guid?),
                propertyInfo: typeof(MaintOrder).GetProperty("MDMaintModeID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MDMaintModeID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDMaintModeID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDMaintOrderStateID = runtimeEntityType.AddProperty(
                "MDMaintOrderStateID",
                typeof(Guid?),
                propertyInfo: typeof(MaintOrder).GetProperty("MDMaintOrderStateID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MDMaintOrderStateID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            mDMaintOrderStateID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintACClassID = runtimeEntityType.AddProperty(
                "MaintACClassID",
                typeof(Guid),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            maintACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintActDuration = runtimeEntityType.AddProperty(
                "MaintActDuration",
                typeof(int),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintActDuration", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintActDuration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            maintActDuration.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintActEndDate = runtimeEntityType.AddProperty(
                "MaintActEndDate",
                typeof(DateTime?),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintActEndDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintActEndDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            maintActEndDate.AddAnnotation("Relational:ColumnType", "datetime");
            maintActEndDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintActStartDate = runtimeEntityType.AddProperty(
                "MaintActStartDate",
                typeof(DateTime?),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintActStartDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintActStartDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            maintActStartDate.AddAnnotation("Relational:ColumnType", "datetime");
            maintActStartDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintOrderNo = runtimeEntityType.AddProperty(
                "MaintOrderNo",
                typeof(string),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintOrderNo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintOrderNo", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            maintOrderNo.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintSetDate = runtimeEntityType.AddProperty(
                "MaintSetDate",
                typeof(DateTime?),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintSetDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintSetDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            maintSetDate.AddAnnotation("Relational:ColumnType", "datetime");
            maintSetDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintSetDuration = runtimeEntityType.AddProperty(
                "MaintSetDuration",
                typeof(int),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintSetDuration", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintSetDuration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            maintSetDuration.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(MaintOrder).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(MaintOrder).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiPAACClassID = runtimeEntityType.AddProperty(
                "VBiPAACClassID",
                typeof(Guid),
                propertyInfo: typeof(MaintOrder).GetProperty("VBiPAACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_VBiPAACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            vBiPAACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(MaintOrder).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { maintOrderID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { mDMaintModeID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { maintACClassID });

            var nCIFKMaintOrderMDMaintOrderStateID = runtimeEntityType.AddIndex(
                new[] { mDMaintOrderStateID },
                name: "NCI_FK_MaintOrder_MDMaintOrderStateID");

            var nCIFKMaintOrderVBiPAACClassID = runtimeEntityType.AddIndex(
                new[] { vBiPAACClassID },
                name: "NCI_FK_MaintOrder_VBiPAACClassID");

            var uIXMaintOrderMaintOrderNo = runtimeEntityType.AddIndex(
                new[] { maintOrderNo },
                name: "UIX_MaintOrder_MaintOrderNo",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDMaintModeID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDMaintModeID") }),
                principalEntityType);

            var mDMaintMode = declaringEntityType.AddNavigation("MDMaintMode",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDMaintMode),
                propertyInfo: typeof(MaintOrder).GetProperty("MDMaintMode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MDMaintMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var maintOrderMDMaintMode = principalEntityType.AddNavigation("MaintOrder_MDMaintMode",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MaintOrder>),
                propertyInfo: typeof(MDMaintMode).GetProperty("MaintOrder_MDMaintMode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDMaintMode).GetField("_MaintOrder_MDMaintMode", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MaintOrder_MDMaintModeID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDMaintOrderStateID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDMaintOrderStateID") }),
                principalEntityType);

            var mDMaintOrderState = declaringEntityType.AddNavigation("MDMaintOrderState",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDMaintOrderState),
                propertyInfo: typeof(MaintOrder).GetProperty("MDMaintOrderState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MDMaintOrderState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var maintOrderMDMaintOrderState = principalEntityType.AddNavigation("MaintOrder_MDMaintOrderState",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MaintOrder>),
                propertyInfo: typeof(MDMaintOrderState).GetProperty("MaintOrder_MDMaintOrderState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDMaintOrderState).GetField("_MaintOrder_MDMaintOrderState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MaintOrder_MDMaintOrderStateID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaintACClassID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaintACClassID") }),
                principalEntityType,
                required: true);

            var maintACClass = declaringEntityType.AddNavigation("MaintACClass",
                runtimeForeignKey,
                onDependent: true,
                typeof(MaintACClass),
                propertyInfo: typeof(MaintOrder).GetProperty("MaintACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_MaintACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var maintOrderMaintACClass = principalEntityType.AddNavigation("MaintOrder_MaintACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MaintOrder>),
                propertyInfo: typeof(MaintACClass).GetProperty("MaintOrder_MaintACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClass).GetField("_MaintOrder_MaintACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MaintOrder_MaintACClassID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBiPAACClassID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var vBiPAACClass = declaringEntityType.AddNavigation("VBiPAACClass",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClass),
                propertyInfo: typeof(MaintOrder).GetProperty("VBiPAACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintOrder).GetField("_VBiPAACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var maintOrderVBiPAACClass = principalEntityType.AddNavigation("MaintOrder_VBiPAACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MaintOrder>),
                propertyInfo: typeof(ACClass).GetProperty("MaintOrder_VBiPAACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_MaintOrder_VBiPAACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MaintOrder_PAACClassID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "MaintOrder");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}