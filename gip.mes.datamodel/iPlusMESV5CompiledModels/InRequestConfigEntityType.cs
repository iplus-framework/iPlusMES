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
    internal partial class InRequestConfigEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.InRequestConfig",
                typeof(InRequestConfig),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(InRequestConfig)));

            var inRequestConfigID = runtimeEntityType.AddProperty(
                "InRequestConfigID",
                typeof(Guid),
                propertyInfo: typeof(InRequestConfig).GetProperty("InRequestConfigID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_InRequestConfigID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            inRequestConfigID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(InRequestConfig).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var expression = runtimeEntityType.AddProperty(
                "Expression",
                typeof(string),
                propertyInfo: typeof(InRequestConfig).GetProperty("Expression", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_Expression", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            expression.AddAnnotation("Relational:ColumnType", "text");
            expression.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var inRequestID = runtimeEntityType.AddProperty(
                "InRequestID",
                typeof(Guid),
                propertyInfo: typeof(InRequestConfig).GetProperty("InRequestID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_InRequestID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            inRequestID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(InRequestConfig).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(InRequestConfig).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var keyACUrl = runtimeEntityType.AddProperty(
                "KeyACUrl",
                typeof(string),
                propertyInfo: typeof(InRequestConfig).GetProperty("KeyACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_KeyACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            keyACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var localConfigACUrl = runtimeEntityType.AddProperty(
                "LocalConfigACUrl",
                typeof(string),
                propertyInfo: typeof(InRequestConfig).GetProperty("LocalConfigACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_LocalConfigACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            localConfigACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialID = runtimeEntityType.AddProperty(
                "MaterialID",
                typeof(Guid?),
                propertyInfo: typeof(InRequestConfig).GetProperty("MaterialID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_MaterialID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            materialID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var parentInRequestConfigID = runtimeEntityType.AddProperty(
                "ParentInRequestConfigID",
                typeof(Guid?),
                propertyInfo: typeof(InRequestConfig).GetProperty("ParentInRequestConfigID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_ParentInRequestConfigID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            parentInRequestConfigID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var preConfigACUrl = runtimeEntityType.AddProperty(
                "PreConfigACUrl",
                typeof(string),
                propertyInfo: typeof(InRequestConfig).GetProperty("PreConfigACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_PreConfigACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            preConfigACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(InRequestConfig).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(InRequestConfig).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassID = runtimeEntityType.AddProperty(
                "VBiACClassID",
                typeof(Guid?),
                propertyInfo: typeof(InRequestConfig).GetProperty("VBiACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_VBiACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassPropertyRelationID = runtimeEntityType.AddProperty(
                "VBiACClassPropertyRelationID",
                typeof(Guid?),
                propertyInfo: typeof(InRequestConfig).GetProperty("VBiACClassPropertyRelationID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_VBiACClassPropertyRelationID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassPropertyRelationID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiValueTypeACClassID = runtimeEntityType.AddProperty(
                "VBiValueTypeACClassID",
                typeof(Guid),
                propertyInfo: typeof(InRequestConfig).GetProperty("VBiValueTypeACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_VBiValueTypeACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            vBiValueTypeACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(InRequestConfig).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { inRequestConfigID });
            runtimeEntityType.SetPrimaryKey(key);

            var nCIFKInRequestConfigInRequestID = runtimeEntityType.AddIndex(
                new[] { inRequestID },
                name: "NCI_FK_InRequestConfig_InRequestID");

            var nCIFKInRequestConfigMaterialID = runtimeEntityType.AddIndex(
                new[] { materialID },
                name: "NCI_FK_InRequestConfig_MaterialID");

            var nCIFKInRequestConfigParentInRequestConfigID = runtimeEntityType.AddIndex(
                new[] { parentInRequestConfigID },
                name: "NCI_FK_InRequestConfig_ParentInRequestConfigID");

            var nCIFKInRequestConfigVBiACClassID = runtimeEntityType.AddIndex(
                new[] { vBiACClassID },
                name: "NCI_FK_InRequestConfig_VBiACClassID");

            var nCIFKInRequestConfigVBiACClassPropertyRelationID = runtimeEntityType.AddIndex(
                new[] { vBiACClassPropertyRelationID },
                name: "NCI_FK_InRequestConfig_VBiACClassPropertyRelationID");

            var nCIFKInRequestConfigVBiValueTypeACClassID = runtimeEntityType.AddIndex(
                new[] { vBiValueTypeACClassID },
                name: "NCI_FK_InRequestConfig_VBiValueTypeACClassID");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("InRequestID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("InRequestID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var inRequest = declaringEntityType.AddNavigation("InRequest",
                runtimeForeignKey,
                onDependent: true,
                typeof(InRequest),
                propertyInfo: typeof(InRequestConfig).GetProperty("InRequest", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_InRequest", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestConfigInRequest = principalEntityType.AddNavigation("InRequestConfig_InRequest",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequestConfig>),
                propertyInfo: typeof(InRequest).GetProperty("InRequestConfig_InRequest", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequest).GetField("_InRequestConfig_InRequest", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequestConfig_InRequestID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaterialID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaterialID") }),
                principalEntityType);

            var material = declaringEntityType.AddNavigation("Material",
                runtimeForeignKey,
                onDependent: true,
                typeof(Material),
                propertyInfo: typeof(InRequestConfig).GetProperty("Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestConfigMaterial = principalEntityType.AddNavigation("InRequestConfig_Material",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequestConfig>),
                propertyInfo: typeof(Material).GetProperty("InRequestConfig_Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Material).GetField("_InRequestConfig_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequestConfig_MaterialID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ParentInRequestConfigID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("InRequestConfigID") }),
                principalEntityType);

            var inRequestConfig1ParentInRequestConfig = declaringEntityType.AddNavigation("InRequestConfig1_ParentInRequestConfig",
                runtimeForeignKey,
                onDependent: true,
                typeof(InRequestConfig),
                propertyInfo: typeof(InRequestConfig).GetProperty("InRequestConfig1_ParentInRequestConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_InRequestConfig1_ParentInRequestConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestConfigParentInRequestConfig = principalEntityType.AddNavigation("InRequestConfig_ParentInRequestConfig",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequestConfig>),
                propertyInfo: typeof(InRequestConfig).GetProperty("InRequestConfig_ParentInRequestConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_InRequestConfig_ParentInRequestConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequestConfig_ParentInRequestConfigID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBiACClassID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassID") }),
                principalEntityType);

            var vBiACClass = declaringEntityType.AddNavigation("VBiACClass",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClass),
                propertyInfo: typeof(InRequestConfig).GetProperty("VBiACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_VBiACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestConfigVBiACClass = principalEntityType.AddNavigation("InRequestConfig_VBiACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequestConfig>),
                propertyInfo: typeof(ACClass).GetProperty("InRequestConfig_VBiACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_InRequestConfig_VBiACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequestConfig_ACClassID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey5(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBiACClassPropertyRelationID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassPropertyRelationID") }),
                principalEntityType);

            var vBiACClassPropertyRelation = declaringEntityType.AddNavigation("VBiACClassPropertyRelation",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClassPropertyRelation),
                propertyInfo: typeof(InRequestConfig).GetProperty("VBiACClassPropertyRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_VBiACClassPropertyRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestConfigVBiACClassPropertyRelation = principalEntityType.AddNavigation("InRequestConfig_VBiACClassPropertyRelation",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequestConfig>),
                propertyInfo: typeof(ACClassPropertyRelation).GetProperty("InRequestConfig_VBiACClassPropertyRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassPropertyRelation).GetField("_InRequestConfig_VBiACClassPropertyRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequestConfig_ACClassPropertyRelationID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBiValueTypeACClassID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassID") }),
                principalEntityType,
                required: true);

            var vBiValueTypeACClass = declaringEntityType.AddNavigation("VBiValueTypeACClass",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClass),
                propertyInfo: typeof(InRequestConfig).GetProperty("VBiValueTypeACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(InRequestConfig).GetField("_VBiValueTypeACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var inRequestConfigVBiValueTypeACClass = principalEntityType.AddNavigation("InRequestConfig_VBiValueTypeACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<InRequestConfig>),
                propertyInfo: typeof(ACClass).GetProperty("InRequestConfig_VBiValueTypeACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_InRequestConfig_VBiValueTypeACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_InRequestConfig_DataTypeACClassID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "InRequestConfig");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}