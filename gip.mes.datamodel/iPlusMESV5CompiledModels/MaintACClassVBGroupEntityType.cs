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
    internal partial class MaintACClassVBGroupEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.MaintACClassVBGroup",
                typeof(MaintACClassVBGroup),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(MaintACClassVBGroup)));

            var maintACClassVBGroupID = runtimeEntityType.AddProperty(
                "MaintACClassVBGroupID",
                typeof(Guid),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("MaintACClassVBGroupID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_MaintACClassVBGroupID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            maintACClassVBGroupID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var comment = runtimeEntityType.AddProperty(
                "Comment",
                typeof(string),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("Comment", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_Comment", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                unicode: false);
            comment.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var isActive = runtimeEntityType.AddProperty(
                "IsActive",
                typeof(bool),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("IsActive", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_IsActive", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            isActive.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintACClassID = runtimeEntityType.AddProperty(
                "MaintACClassID",
                typeof(Guid?),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("MaintACClassID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_MaintACClassID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            maintACClassID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maintACClassPropertyID = runtimeEntityType.AddProperty(
                "MaintACClassPropertyID",
                typeof(Guid?),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("MaintACClassPropertyID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_MaintACClassPropertyID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            maintACClassPropertyID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBGroupID = runtimeEntityType.AddProperty(
                "VBGroupID",
                typeof(Guid),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("VBGroupID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_VBGroupID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            vBGroupID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { maintACClassVBGroupID });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PK_MaintACClassRole");

            var index = runtimeEntityType.AddIndex(
                new[] { maintACClassID });

            var index0 = runtimeEntityType.AddIndex(
                new[] { maintACClassPropertyID });

            var index1 = runtimeEntityType.AddIndex(
                new[] { vBGroupID });

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaintACClassID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaintACClassID") }),
                principalEntityType);

            var maintACClass = declaringEntityType.AddNavigation("MaintACClass",
                runtimeForeignKey,
                onDependent: true,
                typeof(MaintACClass),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("MaintACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_MaintACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var maintACClassVBGroupMaintACClass = principalEntityType.AddNavigation("MaintACClassVBGroup_MaintACClass",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MaintACClassVBGroup>),
                propertyInfo: typeof(MaintACClass).GetProperty("MaintACClassVBGroup_MaintACClass", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClass).GetField("_MaintACClassVBGroup_MaintACClass", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MaintACClassVBGroup_MaintACClassID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MaintACClassPropertyID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MaintACClassPropertyID") }),
                principalEntityType);

            var maintACClassProperty = declaringEntityType.AddNavigation("MaintACClassProperty",
                runtimeForeignKey,
                onDependent: true,
                typeof(MaintACClassProperty),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("MaintACClassProperty", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_MaintACClassProperty", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var maintACClassVBGroupMaintACClassProperty = principalEntityType.AddNavigation("MaintACClassVBGroup_MaintACClassProperty",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MaintACClassVBGroup>),
                propertyInfo: typeof(MaintACClassProperty).GetProperty("MaintACClassVBGroup_MaintACClassProperty", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassProperty).GetField("_MaintACClassVBGroup_MaintACClassProperty", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MaintACClassPropertyVBGroup_VBGroupID");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("VBGroupID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("VBGroupID") }),
                principalEntityType,
                required: true);

            var vBGroup = declaringEntityType.AddNavigation("VBGroup",
                runtimeForeignKey,
                onDependent: true,
                typeof(VBGroup),
                propertyInfo: typeof(MaintACClassVBGroup).GetProperty("VBGroup", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MaintACClassVBGroup).GetField("_VBGroup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var maintACClassVBGroupVBGroup = principalEntityType.AddNavigation("MaintACClassVBGroup_VBGroup",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<MaintACClassVBGroup>),
                propertyInfo: typeof(VBGroup).GetProperty("MaintACClassVBGroup_VBGroup", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBGroup).GetField("_MaintACClassVBGroup_VBGroup", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_MaintACClassVBGroup_VBGroupID");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "MaintACClassVBGroup");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}