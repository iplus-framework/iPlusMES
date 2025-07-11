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
    public partial class ProdOrderPartslistPosRelationEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "gip.mes.datamodel.ProdOrderPartslistPosRelation",
                typeof(ProdOrderPartslistPosRelation),
                baseEntityType,
                indexerPropertyInfo: RuntimeEntityType.FindIndexerProperty(typeof(ProdOrderPartslistPosRelation)),
                propertyCount: 15,
                navigationCount: 13,
                servicePropertyCount: 1,
                foreignKeyCount: 6,
                namedIndexCount: 9,
                keyCount: 1);

            var prodOrderPartslistPosRelationID = runtimeEntityType.AddProperty(
                "ProdOrderPartslistPosRelationID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("ProdOrderPartslistPosRelationID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_ProdOrderPartslistPosRelationID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw,
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            prodOrderPartslistPosRelationID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var actualQuantity = runtimeEntityType.AddProperty(
                "ActualQuantity",
                typeof(double),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("ActualQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_ActualQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            actualQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var actualQuantityUOM = runtimeEntityType.AddProperty(
                "ActualQuantityUOM",
                typeof(double),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("ActualQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_ActualQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            actualQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var anterograde = runtimeEntityType.AddProperty(
                "Anterograde",
                typeof(bool?),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("Anterograde", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_Anterograde", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            anterograde.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDProdOrderPartslistPosStateID = runtimeEntityType.AddProperty(
                "MDProdOrderPartslistPosStateID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("MDProdOrderPartslistPosStateID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_MDProdOrderPartslistPosStateID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            mDProdOrderPartslistPosStateID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var mDToleranceStateID = runtimeEntityType.AddProperty(
                "MDToleranceStateID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("MDToleranceStateID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_MDToleranceStateID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            mDToleranceStateID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var parentProdOrderPartslistPosRelationID = runtimeEntityType.AddProperty(
                "ParentProdOrderPartslistPosRelationID",
                typeof(Guid?),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("ParentProdOrderPartslistPosRelationID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_ParentProdOrderPartslistPosRelationID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            parentProdOrderPartslistPosRelationID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var prodOrderBatchID = runtimeEntityType.AddProperty(
                "ProdOrderBatchID",
                typeof(Guid?),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("ProdOrderBatchID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_ProdOrderBatchID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            prodOrderBatchID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var retrogradeFIFO = runtimeEntityType.AddProperty(
                "RetrogradeFIFO",
                typeof(bool?),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("RetrogradeFIFO", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_RetrogradeFIFO", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            retrogradeFIFO.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sequence = runtimeEntityType.AddProperty(
                "Sequence",
                typeof(int),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("Sequence", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_Sequence", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0);
            sequence.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var sourceProdOrderPartslistPosID = runtimeEntityType.AddProperty(
                "SourceProdOrderPartslistPosID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("SourceProdOrderPartslistPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_SourceProdOrderPartslistPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            sourceProdOrderPartslistPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetProdOrderPartslistPosID = runtimeEntityType.AddProperty(
                "TargetProdOrderPartslistPosID",
                typeof(Guid),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("TargetProdOrderPartslistPosID", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_TargetProdOrderPartslistPosID", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: new Guid("00000000-0000-0000-0000-000000000000"));
            targetProdOrderPartslistPosID.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetQuantity = runtimeEntityType.AddProperty(
                "TargetQuantity",
                typeof(double),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("TargetQuantity", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_TargetQuantity", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetQuantity.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var targetQuantityUOM = runtimeEntityType.AddProperty(
                "TargetQuantityUOM",
                typeof(double),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("TargetQuantityUOM", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_TargetQuantityUOM", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                sentinel: 0.0);
            targetQuantityUOM.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var xMLConfig = runtimeEntityType.AddProperty(
                "XMLConfig",
                typeof(string),
                propertyInfo: typeof(VBEntityObject).GetProperty("XMLConfig", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_XMLConfig", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            xMLConfig.AddAnnotation("Relational:ColumnType", "text");
            xMLConfig.AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

            var lazyLoader = runtimeEntityType.AddServiceProperty(
                "LazyLoader",
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("LazyLoader", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                serviceType: typeof(ILazyLoader));

            var key = runtimeEntityType.AddKey(
                new[] { prodOrderPartslistPosRelationID });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "PK_ProdOrderPartslistPosRelationID");

            var nCI_FK_ProdOrderPartslistPosRelation_MDProdOrderPartslistPosStateID = runtimeEntityType.AddIndex(
                new[] { mDProdOrderPartslistPosStateID },
                name: "NCI_FK_ProdOrderPartslistPosRelation_MDProdOrderPartslistPosStateID");

            var nCI_FK_ProdOrderPartslistPosRelation_MDToleranceStateID = runtimeEntityType.AddIndex(
                new[] { mDToleranceStateID },
                name: "NCI_FK_ProdOrderPartslistPosRelation_MDToleranceStateID");

            var nCI_FK_ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelationID = runtimeEntityType.AddIndex(
                new[] { parentProdOrderPartslistPosRelationID },
                name: "NCI_FK_ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelationID");

            var nCI_FK_ProdOrderPartslistPosRelation_ProdOrderBatchID = runtimeEntityType.AddIndex(
                new[] { prodOrderBatchID },
                name: "NCI_FK_ProdOrderPartslistPosRelation_ProdOrderBatchID");

            var nCI_FK_ProdOrderPartslistPosRelation_SourceProdOrderPartslistPosID = runtimeEntityType.AddIndex(
                new[] { sourceProdOrderPartslistPosID },
                name: "NCI_FK_ProdOrderPartslistPosRelation_SourceProdOrderPartslistPosID");

            var nCI_FK_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID = runtimeEntityType.AddIndex(
                new[] { targetProdOrderPartslistPosID },
                name: "NCI_FK_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID");

            var nCI_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID_OT1 = runtimeEntityType.AddIndex(
                new[] { targetProdOrderPartslistPosID },
                name: "NCI_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID_OT1");

            var nCI_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID_OT2 = runtimeEntityType.AddIndex(
                new[] { targetProdOrderPartslistPosID, sequence, sourceProdOrderPartslistPosID, prodOrderPartslistPosRelationID, targetQuantity, actualQuantity, targetQuantityUOM, actualQuantityUOM, parentProdOrderPartslistPosRelationID, prodOrderBatchID, mDToleranceStateID, mDProdOrderPartslistPosStateID },
                name: "NCI_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID_OT2");

            var nCI_ProdOrderPartslistPosRelation_Target_Source = runtimeEntityType.AddIndex(
                new[] { prodOrderPartslistPosRelationID, targetProdOrderPartslistPosID, sourceProdOrderPartslistPosID },
                name: "NCI_ProdOrderPartslistPosRelation_Target_Source");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDProdOrderPartslistPosStateID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDProdOrderPartslistPosStateID") }),
                principalEntityType,
                required: true);

            var mDProdOrderPartslistPosState = declaringEntityType.AddNavigation("MDProdOrderPartslistPosState",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDProdOrderPartslistPosState),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("MDProdOrderPartslistPosState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_MDProdOrderPartslistPosState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslistPosRelation_MDProdOrderPartslistPosState = principalEntityType.AddNavigation("ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslistPosRelation>),
                propertyInfo: typeof(MDProdOrderPartslistPosState).GetProperty("ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDProdOrderPartslistPosState).GetField("_ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("MDToleranceStateID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("MDToleranceStateID") }),
                principalEntityType,
                required: true);

            var mDToleranceState = declaringEntityType.AddNavigation("MDToleranceState",
                runtimeForeignKey,
                onDependent: true,
                typeof(MDToleranceState),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("MDToleranceState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_MDToleranceState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslistPosRelation_MDToleranceState = principalEntityType.AddNavigation("ProdOrderPartslistPosRelation_MDToleranceState",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslistPosRelation>),
                propertyInfo: typeof(MDToleranceState).GetProperty("ProdOrderPartslistPosRelation_MDToleranceState", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(MDToleranceState).GetField("_ProdOrderPartslistPosRelation_MDToleranceState", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslistPosRelation_MDToleranceState");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey3(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ParentProdOrderPartslistPosRelationID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ProdOrderPartslistPosRelationID") }),
                principalEntityType);

            var prodOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation = declaringEntityType.AddNavigation("ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation",
                runtimeForeignKey,
                onDependent: true,
                typeof(ProdOrderPartslistPosRelation),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation = principalEntityType.AddNavigation("ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslistPosRelation>),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey4(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ProdOrderBatchID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ProdOrderBatchID") }),
                principalEntityType);

            var prodOrderBatch = declaringEntityType.AddNavigation("ProdOrderBatch",
                runtimeForeignKey,
                onDependent: true,
                typeof(ProdOrderBatch),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("ProdOrderBatch", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_ProdOrderBatch", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslistPosRelation_ProdOrderBatch = principalEntityType.AddNavigation("ProdOrderPartslistPosRelation_ProdOrderBatch",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslistPosRelation>),
                propertyInfo: typeof(ProdOrderBatch).GetProperty("ProdOrderPartslistPosRelation_ProdOrderBatch", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderBatch).GetField("_ProdOrderPartslistPosRelation_ProdOrderBatch", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslistPosRelation_ProdOrderBatch");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey5(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("SourceProdOrderPartslistPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ProdOrderPartslistPosID") }),
                principalEntityType,
                required: true);

            var sourceProdOrderPartslistPos = declaringEntityType.AddNavigation("SourceProdOrderPartslistPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(ProdOrderPartslistPos),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("SourceProdOrderPartslistPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_SourceProdOrderPartslistPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslistPosRelation_SourceProdOrderPartslistPos = principalEntityType.AddNavigation("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslistPosRelation>),
                propertyInfo: typeof(ProdOrderPartslistPos).GetProperty("ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPos).GetField("_ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslistPosRelation_ProdOrderPartslistPos1");
            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey6(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("TargetProdOrderPartslistPosID") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("ProdOrderPartslistPosID") }),
                principalEntityType,
                required: true);

            var targetProdOrderPartslistPos = declaringEntityType.AddNavigation("TargetProdOrderPartslistPos",
                runtimeForeignKey,
                onDependent: true,
                typeof(ProdOrderPartslistPos),
                propertyInfo: typeof(ProdOrderPartslistPosRelation).GetProperty("TargetProdOrderPartslistPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPosRelation).GetField("_TargetProdOrderPartslistPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            var prodOrderPartslistPosRelation_TargetProdOrderPartslistPos = principalEntityType.AddNavigation("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<ProdOrderPartslistPosRelation>),
                propertyInfo: typeof(ProdOrderPartslistPos).GetProperty("ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(ProdOrderPartslistPos).GetField("_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                propertyAccessMode: PropertyAccessMode.Field);

            runtimeForeignKey.AddAnnotation("Relational:Name", "FK_ProdOrderPartslistPosRelation_ProdOrderPartslistPos");
            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "ProdOrderPartslistPosRelation");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
