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
    internal partial class MDGMPMaterialGroupPosEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.MDGMPMaterialGroupPos",
                typeof(MDGMPMaterialGroupPos),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(MDGMPMaterialGroupPos)));

            var mDGMPMaterialGroupPosID = runtimeEntityType.AddProperty(
                "MDGMPMaterialGroupPosID",
                typeof(Guid),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("MDGMPMaterialGroupPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_MDGMPMaterialGroupPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            mDGMPMaterialGroupPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDGMPAdditiveID = runtimeEntityType.AddProperty(
                "MDGMPAdditiveID",
                typeof(Guid),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("MDGMPAdditiveID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_MDGMPAdditiveID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            mDGMPAdditiveID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDGMPMaterialGroupID = runtimeEntityType.AddProperty(
                "MDGMPMaterialGroupID",
                typeof(Guid),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("MDGMPMaterialGroupID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_MDGMPMaterialGroupID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            mDGMPMaterialGroupID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maxConcentration = runtimeEntityType.AddProperty(
                "MaxConcentration",
                typeof(float),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("MaxConcentration", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_MaxConcentration", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            maxConcentration.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sequence = runtimeEntityType.AddProperty(
                "Sequence",
                typeof(int),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("Sequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_Sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            sequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { mDGMPMaterialGroupPosID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { mDGMPMaterialGroupID });

            var nCIFKMDGMPMaterialGroupPosMDGMPAdditiveID = runtimeEntityType.AddIndex(
                new[] { mDGMPAdditiveID },
                name: "NCI_FK_MDGMPMaterialGroupPos_MDGMPAdditiveID");

            var uIXMDGMPMaterialGroupPos = runtimeEntityType.AddIndex(
                new[] { mDGMPMaterialGroupPosID, sequence },
                name: "UIX_MDGMPMaterialGroupPos",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDGMPAdditiveID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDGMPAdditiveID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var mDGMPAdditive = declaringEntityType.AddNavigation("MDGMPAdditive",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDGMPAdditive),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("MDGMPAdditive", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_MDGMPAdditive", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var mDGMPMaterialGroupPosMDGMPAdditive = principalEntityType.AddNavigation("MDGMPMaterialGroupPos_MDGMPAdditive",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MDGMPMaterialGroupPos>),
                propertyInfo: typeof(MDGMPAdditive).GetProperty("MDGMPMaterialGroupPos_MDGMPAdditive", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPAdditive).GetField("_MDGMPMaterialGroupPos_MDGMPAdditive", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MDGMPMaterialGroupPos_MDGMPAdditiveID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDGMPMaterialGroupID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDGMPMaterialGroupID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var mDGMPMaterialGroup = declaringEntityType.AddNavigation("MDGMPMaterialGroup",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDGMPMaterialGroup),
                propertyInfo: typeof(MDGMPMaterialGroupPos).GetProperty("MDGMPMaterialGroup", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroupPos).GetField("_MDGMPMaterialGroup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var mDGMPMaterialGroupPosMDGMPMaterialGroup = principalEntityType.AddNavigation("MDGMPMaterialGroupPos_MDGMPMaterialGroup",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MDGMPMaterialGroupPos>),
                propertyInfo: typeof(MDGMPMaterialGroup).GetProperty("MDGMPMaterialGroupPos_MDGMPMaterialGroup", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDGMPMaterialGroup).GetField("_MDGMPMaterialGroupPos_MDGMPMaterialGroup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MDGMPMaterialGroupPos_MDGMPMaterialGroupID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "MDGMPMaterialGroupPos");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}