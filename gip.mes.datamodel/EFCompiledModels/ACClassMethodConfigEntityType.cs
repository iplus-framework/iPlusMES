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
    public partial class ACClassMethodConfigEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.ACClassMethodConfig",
                typeof(ACClassMethodConfig),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(ACClassMethodConfig)),
                propertyCount: 17,
                navigationCount: 7,
                servicePropertyCount: 1,
                foreignKeyCount: 6,
                unnamedIndexCount: 6,
                keyCount: 1);

            var aCClassMethodConfigID = runtimeEntityType.AddProperty(
                "ACClassMethodConfigID",
                typeof(Guid),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ACClassMethodConfigID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ACClassMethodConfigID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            aCClassMethodConfigID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCClassMethodID = runtimeEntityType.AddProperty(
                "ACClassMethodID",
                typeof(Guid),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ACClassMethodID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ACClassMethodID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            aCClassMethodID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var aCClassWFID = runtimeEntityType.AddProperty(
                "ACClassWFID",
                typeof(Guid?),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ACClassWFID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ACClassWFID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            aCClassWFID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var expression = runtimeEntityType.AddProperty(
                "Expression",
                typeof(string),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("Expression", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_Expression", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            expression.AddAnnotation("Relational:ColumnType", "text");
            expression.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var keyACUrl = runtimeEntityType.AddProperty(
                "KeyACUrl",
                typeof(string),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("KeyACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_KeyACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            keyACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var localConfigACUrl = runtimeEntityType.AddProperty(
                "LocalConfigACUrl",
                typeof(string),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("LocalConfigACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_LocalConfigACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            localConfigACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var parentACClassMethodConfigID = runtimeEntityType.AddProperty(
                "ParentACClassMethodConfigID",
                typeof(Guid?),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ParentACClassMethodConfigID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ParentACClassMethodConfigID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            parentACClassMethodConfigID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var preConfigACUrl = runtimeEntityType.AddProperty(
                "PreConfigACUrl",
                typeof(string),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("PreConfigACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_PreConfigACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            preConfigACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassID = runtimeEntityType.AddProperty(
                "VBiACClassID",
                typeof(Guid?),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("VBiACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_VBiACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassPropertyRelationID = runtimeEntityType.AddProperty(
                "VBiACClassPropertyRelationID",
                typeof(Guid?),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("VBiACClassPropertyRelationID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_VBiACClassPropertyRelationID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassPropertyRelationID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var valueTypeACClassID = runtimeEntityType.AddProperty(
                "ValueTypeACClassID",
                typeof(Guid),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ValueTypeACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ValueTypeACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            valueTypeACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { aCClassMethodConfigID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { aCClassMethodID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { aCClassWFID });

            var index1 = runtimeEntityType.AddIndex(
                new[] { parentACClassMethodConfigID });

            var index2 = runtimeEntityType.AddIndex(
                new[] { vBiACClassID });

            var index3 = runtimeEntityType.AddIndex(
                new[] { vBiACClassPropertyRelationID });

            var index4 = runtimeEntityType.AddIndex(
                new[] { valueTypeACClassID });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ACClassMethodID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassMethodID") }),
                principalEntityType,
                required: true);

            var aCClassMethod = declaringEntityType.AddNavigation("ACClassMethod",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClassMethod),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ACClassMethod", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ACClassMethod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCClassMethodConfig_ACClassMethod = principalEntityType.AddNavigation("ACClassMethodConfig_ACClassMethod",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACClassMethodConfig>),
                propertyInfo: typeof(ACClassMethod).GetProperty("ACClassMethodConfig_ACClassMethod", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethod).GetField("_ACClassMethodConfig_ACClassMethod", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACClassMethodConfig_ACClassMethodID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ACClassWFID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassWFID") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade);

            var aCClassWF = declaringEntityType.AddNavigation("ACClassWF",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClassWF),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ACClassWF", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ACClassWF", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCClassMethodConfig_ACClassWF = principalEntityType.AddNavigation("ACClassMethodConfig_ACClassWF",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACClassMethodConfig>),
                propertyInfo: typeof(ACClassWF).GetProperty("ACClassMethodConfig_ACClassWF", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassWF).GetField("_ACClassMethodConfig_ACClassWF", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACClassMethodConfig_ACClassWFID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ParentACClassMethodConfigID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassMethodConfigID") }),
                principalEntityType);

            var aCClassMethodConfig1_ParentACClassMethodConfig = declaringEntityType.AddNavigation("ACClassMethodConfig1_ParentACClassMethodConfig",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClassMethodConfig),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ACClassMethodConfig1_ParentACClassMethodConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ACClassMethodConfig1_ParentACClassMethodConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCClassMethodConfig_ParentACClassMethodConfig = principalEntityType.AddNavigation("ACClassMethodConfig_ParentACClassMethodConfig",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACClassMethodConfig>),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ACClassMethodConfig_ParentACClassMethodConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ACClassMethodConfig_ParentACClassMethodConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACClassMethodConfig_ParentACClassMethodConfigID");
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
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("VBiACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_VBiACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCClassMethodConfig_VBiACClass = principalEntityType.AddNavigation("ACClassMethodConfig_VBiACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACClassMethodConfig>),
                propertyInfo: typeof(ACClass).GetProperty("ACClassMethodConfig_VBiACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_ACClassMethodConfig_VBiACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACClassMethodConfig_VBiACClassID");
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
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("VBiACClassPropertyRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_VBiACClassPropertyRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCClassMethodConfig_VBiACClassPropertyRelation = principalEntityType.AddNavigation("ACClassMethodConfig_VBiACClassPropertyRelation",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACClassMethodConfig>),
                propertyInfo: typeof(ACClassPropertyRelation).GetProperty("ACClassMethodConfig_VBiACClassPropertyRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassPropertyRelation).GetField("_ACClassMethodConfig_VBiACClassPropertyRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACClassMethodConfig_VBiACClassPropertyRelationID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ValueTypeACClassID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ACClassID") }),
                principalEntityType,
                required: true);

            var valueTypeACClass = declaringEntityType.AddNavigation("ValueTypeACClass",
                runtimeForeignKey,
                onDependent: true,
                typeof(ACClass),
                propertyInfo: typeof(ACClassMethodConfig).GetProperty("ValueTypeACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassMethodConfig).GetField("_ValueTypeACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var aCClassMethodConfig_ValueTypeACClass = principalEntityType.AddNavigation("ACClassMethodConfig_ValueTypeACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ACClassMethodConfig>),
                propertyInfo: typeof(ACClass).GetProperty("ACClassMethodConfig_ValueTypeACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_ACClassMethodConfig_ValueTypeACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ACClassMethodConfig_ValueTypeACClassID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "ACClassMethodConfig");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
