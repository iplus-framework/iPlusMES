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
    internal partial class PickingConfigEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.PickingConfig",
                typeof(PickingConfig),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(PickingConfig)));

            var pickingConfigID = runtimeEntityType.AddProperty(
                "PickingConfigID",
                typeof(Guid),
                propertyInfo: typeof(PickingConfig).GetProperty("PickingConfigID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_PickingConfigID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            pickingConfigID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(PickingConfig).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var expression = runtimeEntityType.AddProperty(
                "Expression",
                typeof(string),
                propertyInfo: typeof(PickingConfig).GetProperty("Expression", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_Expression", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            expression.AddAnnotation("Relational:ColumnType", "text");
            expression.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(PickingConfig).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(PickingConfig).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var keyACUrl = runtimeEntityType.AddProperty(
                "KeyACUrl",
                typeof(string),
                propertyInfo: typeof(PickingConfig).GetProperty("KeyACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_KeyACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            keyACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var localConfigACUrl = runtimeEntityType.AddProperty(
                "LocalConfigACUrl",
                typeof(string),
                propertyInfo: typeof(PickingConfig).GetProperty("LocalConfigACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_LocalConfigACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            localConfigACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialID = runtimeEntityType.AddProperty(
                "MaterialID",
                typeof(Guid?),
                propertyInfo: typeof(PickingConfig).GetProperty("MaterialID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_MaterialID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            materialID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var parentPickingConfigID = runtimeEntityType.AddProperty(
                "ParentPickingConfigID",
                typeof(Guid?),
                propertyInfo: typeof(PickingConfig).GetProperty("ParentPickingConfigID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_ParentPickingConfigID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            parentPickingConfigID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var pickingID = runtimeEntityType.AddProperty(
                "PickingID",
                typeof(Guid),
                propertyInfo: typeof(PickingConfig).GetProperty("PickingID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_PickingID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            pickingID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var preConfigACUrl = runtimeEntityType.AddProperty(
                "PreConfigACUrl",
                typeof(string),
                propertyInfo: typeof(PickingConfig).GetProperty("PreConfigACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_PreConfigACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            preConfigACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(PickingConfig).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(PickingConfig).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassID = runtimeEntityType.AddProperty(
                "VBiACClassID",
                typeof(Guid?),
                propertyInfo: typeof(PickingConfig).GetProperty("VBiACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_VBiACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassPropertyRelationID = runtimeEntityType.AddProperty(
                "VBiACClassPropertyRelationID",
                typeof(Guid?),
                propertyInfo: typeof(PickingConfig).GetProperty("VBiACClassPropertyRelationID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_VBiACClassPropertyRelationID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassPropertyRelationID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassWFID = runtimeEntityType.AddProperty(
                "VBiACClassWFID",
                typeof(Guid?),
                propertyInfo: typeof(PickingConfig).GetProperty("VBiACClassWFID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_VBiACClassWFID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassWFID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiValueTypeACClassID = runtimeEntityType.AddProperty(
                "VBiValueTypeACClassID",
                typeof(Guid),
                propertyInfo: typeof(PickingConfig).GetProperty("VBiValueTypeACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_VBiValueTypeACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            vBiValueTypeACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(PickingConfig).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { pickingConfigID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { materialID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { parentPickingConfigID });

            var index1 = runtimeEntityType.AddIndex(
                new[] { pickingID });

            var index2 = runtimeEntityType.AddIndex(
                new[] { vBiACClassID });

            var index3 = runtimeEntityType.AddIndex(
                new[] { vBiACClassPropertyRelationID });

            var index4 = runtimeEntityType.AddIndex(
                new[] { vBiACClassWFID });

            var index5 = runtimeEntityType.AddIndex(
                new[] { vBiValueTypeACClassID });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaterialID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaterialID") }),
                principalEntityType);

            var material = declaringEntityType.AddNavigation("Material",
                runtimeForeignKey,
                onDependent: true,
                typeof(Material),
                propertyInfo: typeof(PickingConfig).GetProperty("Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var pickingConfigMaterial = principalEntityType.AddNavigation("PickingConfig_Material",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<PickingConfig>),
                propertyInfo: typeof(Material).GetProperty("PickingConfig_Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Material).GetField("_PickingConfig_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_PickingConfig_MaterialID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ParentPickingConfigID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("PickingConfigID") }),
                principalEntityType);

            var pickingConfig1ParentPickingConfig = declaringEntityType.AddNavigation("PickingConfig1_ParentPickingConfig",
                runtimeForeignKey,
                onDependent: true,
                typeof(PickingConfig),
                propertyInfo: typeof(PickingConfig).GetProperty("PickingConfig1_ParentPickingConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_PickingConfig1_ParentPickingConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var pickingConfigParentPickingConfig = principalEntityType.AddNavigation("PickingConfig_ParentPickingConfig",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<PickingConfig>),
                propertyInfo: typeof(PickingConfig).GetProperty("PickingConfig_ParentPickingConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_PickingConfig_ParentPickingConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_PickingConfig_ParentPickingConfigID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("PickingID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("PickingID") }),
                principalEntityType,
                required: true);

            var picking = declaringEntityType.AddNavigation("Picking",
                runtimeForeignKey,
                onDependent: true,
                typeof(Picking),
                propertyInfo: typeof(PickingConfig).GetProperty("Picking", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_Picking", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var pickingConfigPicking = principalEntityType.AddNavigation("PickingConfig_Picking",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<PickingConfig>),
                propertyInfo: typeof(Picking).GetProperty("PickingConfig_Picking", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Picking).GetField("_PickingConfig_Picking", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_PickingConfig_PickingID");
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
                propertyInfo: typeof(PickingConfig).GetProperty("VBiACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_VBiACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var pickingConfigVBiACClass = principalEntityType.AddNavigation("PickingConfig_VBiACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<PickingConfig>),
                propertyInfo: typeof(ACClass).GetProperty("PickingConfig_VBiACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_PickingConfig_VBiACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_PickingConfig_VBiACClassID");
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
                propertyInfo: typeof(PickingConfig).GetProperty("VBiACClassPropertyRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_VBiACClassPropertyRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var pickingConfigVBiACClassPropertyRelation = principalEntityType.AddNavigation("PickingConfig_VBiACClassPropertyRelation",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<PickingConfig>),
                propertyInfo: typeof(ACClassPropertyRelation).GetProperty("PickingConfig_VBiACClassPropertyRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassPropertyRelation).GetField("_PickingConfig_VBiACClassPropertyRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_PickingConfig_VBiACClassPropertyRelationID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBiACClassWFID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassWFID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade);

            var vBiACClassWF = declaringEntityType.AddNavigation("VBiACClassWF",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClassWF),
                propertyInfo: typeof(PickingConfig).GetProperty("VBiACClassWF", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_VBiACClassWF", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var pickingConfigVBiACClassWF = principalEntityType.AddNavigation("PickingConfig_VBiACClassWF",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<PickingConfig>),
                propertyInfo: typeof(ACClassWF).GetProperty("PickingConfig_VBiACClassWF", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassWF).GetField("_PickingConfig_VBiACClassWF", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_PickingConfig_VBiACClassWFID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey7(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBiValueTypeACClassID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassID") }),
                principalEntityType,
                required: true);

            var vBiValueTypeACClass = declaringEntityType.AddNavigation("VBiValueTypeACClass",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClass),
                propertyInfo: typeof(PickingConfig).GetProperty("VBiValueTypeACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(PickingConfig).GetField("_VBiValueTypeACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var pickingConfigVBiValueTypeACClass = principalEntityType.AddNavigation("PickingConfig_VBiValueTypeACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<PickingConfig>),
                propertyInfo: typeof(ACClass).GetProperty("PickingConfig_VBiValueTypeACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_PickingConfig_VBiValueTypeACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_PickingConfig_VBiValueTypeACClassID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "PickingConfig");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}