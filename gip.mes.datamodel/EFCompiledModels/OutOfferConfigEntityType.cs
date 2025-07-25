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
    public partial class OutOfferConfigEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.OutOfferConfig",
                typeof(OutOfferConfig),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(OutOfferConfig)),
                propertyCount: 17,
                navigationCount: 7,
                servicePropertyCount: 1,
                foreignKeyCount: 6,
                unnamedIndexCount: 6,
                keyCount: 1);

            var outOfferConfigID = runtimeEntityType.AddProperty(
                "OutOfferConfigID",
                typeof(Guid),
                propertyInfo: typeof(OutOfferConfig).GetProperty("OutOfferConfigID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_OutOfferConfigID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            outOfferConfigID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(OutOfferConfig).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var expression = runtimeEntityType.AddProperty(
                "Expression",
                typeof(string),
                propertyInfo: typeof(OutOfferConfig).GetProperty("Expression", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_Expression", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            expression.AddAnnotation("Relational:ColumnType", "text");
            expression.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(OutOfferConfig).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(OutOfferConfig).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var keyACUrl = runtimeEntityType.AddProperty(
                "KeyACUrl",
                typeof(string),
                propertyInfo: typeof(OutOfferConfig).GetProperty("KeyACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_KeyACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            keyACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var localConfigACUrl = runtimeEntityType.AddProperty(
                "LocalConfigACUrl",
                typeof(string),
                propertyInfo: typeof(OutOfferConfig).GetProperty("LocalConfigACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_LocalConfigACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            localConfigACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var materialID = runtimeEntityType.AddProperty(
                "MaterialID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferConfig).GetProperty("MaterialID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_MaterialID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            materialID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var outOfferID = runtimeEntityType.AddProperty(
                "OutOfferID",
                typeof(Guid),
                propertyInfo: typeof(OutOfferConfig).GetProperty("OutOfferID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_OutOfferID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            outOfferID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var parentOutOfferConfigID = runtimeEntityType.AddProperty(
                "ParentOutOfferConfigID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferConfig).GetProperty("ParentOutOfferConfigID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_ParentOutOfferConfigID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            parentOutOfferConfigID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var preConfigACUrl = runtimeEntityType.AddProperty(
                "PreConfigACUrl",
                typeof(string),
                propertyInfo: typeof(OutOfferConfig).GetProperty("PreConfigACUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_PreConfigACUrl", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            preConfigACUrl.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(OutOfferConfig).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(OutOfferConfig).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassID = runtimeEntityType.AddProperty(
                "VBiACClassID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferConfig).GetProperty("VBiACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_VBiACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiACClassPropertyRelationID = runtimeEntityType.AddProperty(
                "VBiACClassPropertyRelationID",
                typeof(Guid?),
                propertyInfo: typeof(OutOfferConfig).GetProperty("VBiACClassPropertyRelationID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_VBiACClassPropertyRelationID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            vBiACClassPropertyRelationID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBiValueTypeACClassID = runtimeEntityType.AddProperty(
                "VBiValueTypeACClassID",
                typeof(Guid),
                propertyInfo: typeof(OutOfferConfig).GetProperty("VBiValueTypeACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_VBiValueTypeACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            vBiValueTypeACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(OutOfferConfig).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { outOfferConfigID });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { materialID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { outOfferID });

            var index1 = runtimeEntityType.AddIndex(
                new[] { parentOutOfferConfigID });

            var index2 = runtimeEntityType.AddIndex(
                new[] { vBiACClassID });

            var index3 = runtimeEntityType.AddIndex(
                new[] { vBiACClassPropertyRelationID });

            var index4 = runtimeEntityType.AddIndex(
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
                propertyInfo: typeof(OutOfferConfig).GetProperty("Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferConfig_Material = principalEntityType.AddNavigation("OutOfferConfig_Material",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferConfig>),
                propertyInfo: typeof(Material).GetProperty("OutOfferConfig_Material", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Material).GetField("_OutOfferConfig_Material", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferConfig_MaterialID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
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
                propertyInfo: typeof(OutOfferConfig).GetProperty("OutOffer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_OutOffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferConfig_OutOffer = principalEntityType.AddNavigation("OutOfferConfig_OutOffer",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferConfig>),
                propertyInfo: typeof(OutOffer).GetProperty("OutOfferConfig_OutOffer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOffer).GetField("_OutOfferConfig_OutOffer", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferConfig_OutOfferID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ParentOutOfferConfigID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("OutOfferConfigID") }),
                principalEntityType);

            var outOfferConfig1_ParentOutOfferConfig = declaringEntityType.AddNavigation("OutOfferConfig1_ParentOutOfferConfig",
                runtimeForeignKey,
                onDependent: true,
                typeof(OutOfferConfig),
                propertyInfo: typeof(OutOfferConfig).GetProperty("OutOfferConfig1_ParentOutOfferConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_OutOfferConfig1_ParentOutOfferConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferConfig_ParentOutOfferConfig = principalEntityType.AddNavigation("OutOfferConfig_ParentOutOfferConfig",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferConfig>),
                propertyInfo: typeof(OutOfferConfig).GetProperty("OutOfferConfig_ParentOutOfferConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_OutOfferConfig_ParentOutOfferConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferConfig_ParentOutOfferConfigID");
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
                propertyInfo: typeof(OutOfferConfig).GetProperty("VBiACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_VBiACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferConfig_VBiACClass = principalEntityType.AddNavigation("OutOfferConfig_VBiACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferConfig>),
                propertyInfo: typeof(ACClass).GetProperty("OutOfferConfig_VBiACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_OutOfferConfig_VBiACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferConfig_ACClassID");
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
                propertyInfo: typeof(OutOfferConfig).GetProperty("VBiACClassPropertyRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_VBiACClassPropertyRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferConfig_VBiACClassPropertyRelation = principalEntityType.AddNavigation("OutOfferConfig_VBiACClassPropertyRelation",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferConfig>),
                propertyInfo: typeof(ACClassPropertyRelation).GetProperty("OutOfferConfig_VBiACClassPropertyRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClassPropertyRelation).GetField("_OutOfferConfig_VBiACClassPropertyRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferConfig_ACClassPropertyRelationID");
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
                propertyInfo: typeof(OutOfferConfig).GetProperty("VBiValueTypeACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(OutOfferConfig).GetField("_VBiValueTypeACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var outOfferConfig_VBiValueTypeACClass = principalEntityType.AddNavigation("OutOfferConfig_VBiValueTypeACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<OutOfferConfig>),
                propertyInfo: typeof(ACClass).GetProperty("OutOfferConfig_VBiValueTypeACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ACClass).GetField("_OutOfferConfig_VBiValueTypeACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_OutOfferConfig_DataTypeACClassID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "OutOfferConfig");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
