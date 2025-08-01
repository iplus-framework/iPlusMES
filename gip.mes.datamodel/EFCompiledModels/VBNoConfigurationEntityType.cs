﻿// <auto-generated />
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using gip.core.datamodel;

#pragma warning disable 219, 612, 618
#nullable disable

namespace gip.mes.datamodel
{
    [EntityFrameworkInternal]
    public partial class VBNoConfigurationEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.VBNoConfiguration",
                typeof(VBNoConfiguration),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(VBNoConfiguration)),
                propertyCount: 15,
                servicePropertyCount: 1,
                keyCount: 1);

            var vBNoConfigurationID = runtimeEntityType.AddProperty(
                "VBNoConfigurationID",
                typeof(Guid),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("VBNoConfigurationID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_VBNoConfigurationID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            vBNoConfigurationID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var currentCounter = runtimeEntityType.AddProperty(
                "CurrentCounter",
                typeof(int),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("CurrentCounter", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_CurrentCounter", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            currentCounter.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var currentDate = runtimeEntityType.AddProperty(
                "CurrentDate",
                typeof(DateTime),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("CurrentDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_CurrentDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            currentDate.AddAnnotation("Relational:ColumnType", "datetime");
            currentDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertDate = runtimeEntityType.AddProperty(
                "InsertDate",
                typeof(DateTime),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("InsertDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_InsertDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            insertDate.AddAnnotation("Relational:ColumnType", "datetime");
            insertDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var insertName = runtimeEntityType.AddProperty(
                "InsertName",
                typeof(string),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("InsertName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_InsertName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            insertName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var maxCounter = runtimeEntityType.AddProperty(
                "MaxCounter",
                typeof(int),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("MaxCounter", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_MaxCounter", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            maxCounter.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var minCounter = runtimeEntityType.AddProperty(
                "MinCounter",
                typeof(int),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("MinCounter", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_MinCounter", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            minCounter.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var rowVersion = runtimeEntityType.AddProperty(
                "RowVersion",
                typeof(byte[]),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("RowVersion", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_RowVersion", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                concurrencyToken: true,
                valueGenerated: ValueGenerated.OnAddOrUpdate,
                beforeSaveBehavior: PropertySaveBehavior.Ignore,
                afterSaveBehavior: PropertySaveBehavior.Ignore);
            rowVersion.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateDate = runtimeEntityType.AddProperty(
                "UpdateDate",
                typeof(DateTime),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("UpdateDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_UpdateDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            updateDate.AddAnnotation("Relational:ColumnType", "datetime");
            updateDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var updateName = runtimeEntityType.AddProperty(
                "UpdateName",
                typeof(string),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("UpdateName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_UpdateName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 20,
                unicode: false);
            updateName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var useDate = runtimeEntityType.AddProperty(
                "UseDate",
                typeof(bool),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("UseDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_UseDate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: false);
            useDate.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var usedDelimiter = runtimeEntityType.AddProperty(
                "UsedDelimiter",
                typeof(string),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("UsedDelimiter", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_UsedDelimiter", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 1,
                unicode: false);
            usedDelimiter.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var usedPrefix = runtimeEntityType.AddProperty(
                "UsedPrefix",
                typeof(string),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("UsedPrefix", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_UsedPrefix", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                maxLength: 20,
                unicode: false);
            usedPrefix.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var vBNoConfigurationName = runtimeEntityType.AddProperty(
                "VBNoConfigurationName",
                typeof(string),
                propertyInfo: typeof(VBNoConfiguration).GetProperty("VBNoConfigurationName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_VBNoConfigurationName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                maxLength: 50,
                unicode: false);
            vBNoConfigurationName.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(VBNoConfiguration).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(VBNoConfiguration).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { vBNoConfigurationID });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PK_MDNoConfiguration");

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "VBNoConfiguration");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
