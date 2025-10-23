using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class iPlusMESV5Context : DbContext
{
    public iPlusMESV5Context()
    {
    }

    public iPlusMESV5Context(DbContextOptions<iPlusMESV5Context> options)
        : base(options)
    {
    }

    public virtual DbSet<ACAssembly> ACAssembly { get; set; }

    public virtual DbSet<ACChangeLog> ACChangeLog { get; set; }

    public virtual DbSet<ACClass> ACClass { get; set; }

    public virtual DbSet<ACClassConfig> ACClassConfig { get; set; }

    public virtual DbSet<ACClassDesign> ACClassDesign { get; set; }

    public virtual DbSet<ACClassMessage> ACClassMessage { get; set; }

    public virtual DbSet<ACClassMethod> ACClassMethod { get; set; }

    public virtual DbSet<ACClassMethodConfig> ACClassMethodConfig { get; set; }

    public virtual DbSet<ACClassProperty> ACClassProperty { get; set; }

    public virtual DbSet<ACClassPropertyRelation> ACClassPropertyRelation { get; set; }

    public virtual DbSet<ACClassRouteUsage> ACClassRouteUsage { get; set; }

    public virtual DbSet<ACClassRouteUsageGroup> ACClassRouteUsageGroup { get; set; }

    public virtual DbSet<ACClassRouteUsagePos> ACClassRouteUsagePos { get; set; }

    public virtual DbSet<ACClassTask> ACClassTask { get; set; }

    public virtual DbSet<ACClassTaskValue> ACClassTaskValue { get; set; }

    public virtual DbSet<ACClassTaskValuePos> ACClassTaskValuePos { get; set; }

    public virtual DbSet<ACClassText> ACClassText { get; set; }

    public virtual DbSet<ACClassWF> ACClassWF { get; set; }

    public virtual DbSet<ACClassWFEdge> ACClassWFEdge { get; set; }

    public virtual DbSet<ACPackage> ACPackage { get; set; }

    public virtual DbSet<ACProgram> ACProgram { get; set; }

    public virtual DbSet<ACProgramConfig> ACProgramConfig { get; set; }

    public virtual DbSet<ACProgramLog> ACProgramLog { get; set; }

    public virtual DbSet<ACProgramLogPropertyLog> ACProgramLogPropertyLog { get; set; }

    public virtual DbSet<ACProgramLogTask> ACProgramLogTask { get; set; }

    public virtual DbSet<ACProgramLogView> ACProgramLogView { get; set; }

    public virtual DbSet<ACProject> ACProject { get; set; }

    public virtual DbSet<ACPropertyLog> ACPropertyLog { get; set; }

    public virtual DbSet<ACPropertyLogRule> ACPropertyLogRule { get; set; }

    public virtual DbSet<Calendar> Calendar { get; set; }

    public virtual DbSet<CalendarHoliday> CalendarHoliday { get; set; }

    public virtual DbSet<CalendarShift> CalendarShift { get; set; }

    public virtual DbSet<CalendarShiftPerson> CalendarShiftPerson { get; set; }

    public virtual DbSet<Company> Company { get; set; }

    public virtual DbSet<CompanyAddress> CompanyAddress { get; set; }

    public virtual DbSet<CompanyAddressDepartment> CompanyAddressDepartment { get; set; }

    public virtual DbSet<CompanyAddressUnloadingpoint> CompanyAddressUnloadingpoint { get; set; }

    public virtual DbSet<CompanyMaterial> CompanyMaterial { get; set; }

    public virtual DbSet<CompanyMaterialHistory> CompanyMaterialHistory { get; set; }

    public virtual DbSet<CompanyMaterialPickup> CompanyMaterialPickup { get; set; }

    public virtual DbSet<CompanyMaterialStock> CompanyMaterialStock { get; set; }

    public virtual DbSet<CompanyPerson> CompanyPerson { get; set; }

    public virtual DbSet<CompanyPersonRole> CompanyPersonRole { get; set; }

    public virtual DbSet<DeliveryNote> DeliveryNote { get; set; }

    public virtual DbSet<DeliveryNotePos> DeliveryNotePos { get; set; }

    public virtual DbSet<DemandOrder> DemandOrder { get; set; }

    public virtual DbSet<DemandOrderPos> DemandOrderPos { get; set; }

    public virtual DbSet<DemandPrimary> DemandPrimary { get; set; }

    public virtual DbSet<DemandProdOrder> DemandProdOrder { get; set; }

    public virtual DbSet<Facility> Facility { get; set; }

    public virtual DbSet<FacilityBooking> FacilityBooking { get; set; }

    public virtual DbSet<FacilityBookingCharge> FacilityBookingCharge { get; set; }

    public virtual DbSet<FacilityCharge> FacilityCharge { get; set; }

    public virtual DbSet<FacilityHistory> FacilityHistory { get; set; }

    public virtual DbSet<FacilityInventory> FacilityInventory { get; set; }

    public virtual DbSet<FacilityInventoryPos> FacilityInventoryPos { get; set; }

    public virtual DbSet<FacilityLot> FacilityLot { get; set; }

    public virtual DbSet<FacilityLotStock> FacilityLotStock { get; set; }

    public virtual DbSet<FacilityMDSchedulingGroup> FacilityMDSchedulingGroup { get; set; }

    public virtual DbSet<FacilityMaterial> FacilityMaterial { get; set; }

    public virtual DbSet<FacilityMaterialOEE> FacilityMaterialOEE { get; set; }

    public virtual DbSet<FacilityPreBooking> FacilityPreBooking { get; set; }

    public virtual DbSet<FacilityReservation> FacilityReservation { get; set; }

    public virtual DbSet<FacilityStock> FacilityStock { get; set; }

    public virtual DbSet<History> History { get; set; }

    public virtual DbSet<HistoryConfig> HistoryConfig { get; set; }

    public virtual DbSet<InOrder> InOrder { get; set; }

    public virtual DbSet<InOrderConfig> InOrderConfig { get; set; }

    public virtual DbSet<InOrderPos> InOrderPos { get; set; }

    public virtual DbSet<InOrderPosSplit> InOrderPosSplit { get; set; }

    public virtual DbSet<InRequest> InRequest { get; set; }

    public virtual DbSet<InRequestConfig> InRequestConfig { get; set; }

    public virtual DbSet<InRequestPos> InRequestPos { get; set; }

    public virtual DbSet<Invoice> Invoice { get; set; }

    public virtual DbSet<InvoicePos> InvoicePos { get; set; }

    public virtual DbSet<JobTableRecalcActualQuantity> JobTableRecalcActualQuantity { get; set; }

    public virtual DbSet<LabOrder> LabOrder { get; set; }

    public virtual DbSet<LabOrderPos> LabOrderPos { get; set; }

    public virtual DbSet<Label> Label { get; set; }

    public virtual DbSet<LabelTranslation> LabelTranslation { get; set; }

    public virtual DbSet<MDBalancingMode> MDBalancingMode { get; set; }

    public virtual DbSet<MDBatchPlanGroup> MDBatchPlanGroup { get; set; }

    public virtual DbSet<MDBookingNotAvailableMode> MDBookingNotAvailableMode { get; set; }

    public virtual DbSet<MDCostCenter> MDCostCenter { get; set; }

    public virtual DbSet<MDCountry> MDCountry { get; set; }

    public virtual DbSet<MDCountryLand> MDCountryLand { get; set; }

    public virtual DbSet<MDCountrySalesTax> MDCountrySalesTax { get; set; }

    public virtual DbSet<MDCountrySalesTaxMDMaterialGroup> MDCountrySalesTaxMDMaterialGroup { get; set; }

    public virtual DbSet<MDCountrySalesTaxMaterial> MDCountrySalesTaxMaterial { get; set; }

    public virtual DbSet<MDCurrency> MDCurrency { get; set; }

    public virtual DbSet<MDCurrencyExchange> MDCurrencyExchange { get; set; }

    public virtual DbSet<MDDelivNoteState> MDDelivNoteState { get; set; }

    public virtual DbSet<MDDelivPosLoadState> MDDelivPosLoadState { get; set; }

    public virtual DbSet<MDDelivPosState> MDDelivPosState { get; set; }

    public virtual DbSet<MDDelivType> MDDelivType { get; set; }

    public virtual DbSet<MDDemandOrderState> MDDemandOrderState { get; set; }

    public virtual DbSet<MDFacilityInventoryPosState> MDFacilityInventoryPosState { get; set; }

    public virtual DbSet<MDFacilityInventoryState> MDFacilityInventoryState { get; set; }

    public virtual DbSet<MDFacilityManagementType> MDFacilityManagementType { get; set; }

    public virtual DbSet<MDFacilityType> MDFacilityType { get; set; }

    public virtual DbSet<MDFacilityVehicleType> MDFacilityVehicleType { get; set; }

    public virtual DbSet<MDGMPAdditive> MDGMPAdditive { get; set; }

    public virtual DbSet<MDGMPMaterialGroup> MDGMPMaterialGroup { get; set; }

    public virtual DbSet<MDGMPMaterialGroupPos> MDGMPMaterialGroupPos { get; set; }

    public virtual DbSet<MDInOrderPosState> MDInOrderPosState { get; set; }

    public virtual DbSet<MDInOrderState> MDInOrderState { get; set; }

    public virtual DbSet<MDInOrderType> MDInOrderType { get; set; }

    public virtual DbSet<MDInRequestState> MDInRequestState { get; set; }

    public virtual DbSet<MDInventoryManagementType> MDInventoryManagementType { get; set; }

    public virtual DbSet<MDInvoiceState> MDInvoiceState { get; set; }

    public virtual DbSet<MDInvoiceType> MDInvoiceType { get; set; }

    public virtual DbSet<MDLabOrderPosState> MDLabOrderPosState { get; set; }

    public virtual DbSet<MDLabOrderState> MDLabOrderState { get; set; }

    public virtual DbSet<MDLabTag> MDLabTag { get; set; }

    public virtual DbSet<MDMaintMode> MDMaintMode { get; set; }

    public virtual DbSet<MDMaintOrderPropertyState> MDMaintOrderPropertyState { get; set; }

    public virtual DbSet<MDMaintOrderState> MDMaintOrderState { get; set; }

    public virtual DbSet<MDMaintTaskState> MDMaintTaskState { get; set; }

    public virtual DbSet<MDMaterialGroup> MDMaterialGroup { get; set; }

    public virtual DbSet<MDMaterialType> MDMaterialType { get; set; }

    public virtual DbSet<MDMovementReason> MDMovementReason { get; set; }

    public virtual DbSet<MDOutOfferState> MDOutOfferState { get; set; }

    public virtual DbSet<MDOutOrderPlanState> MDOutOrderPlanState { get; set; }

    public virtual DbSet<MDOutOrderPosState> MDOutOrderPosState { get; set; }

    public virtual DbSet<MDOutOrderState> MDOutOrderState { get; set; }

    public virtual DbSet<MDOutOrderType> MDOutOrderType { get; set; }

    public virtual DbSet<MDPickingType> MDPickingType { get; set; }

    public virtual DbSet<MDProcessErrorAction> MDProcessErrorAction { get; set; }

    public virtual DbSet<MDProdOrderPartslistPosState> MDProdOrderPartslistPosState { get; set; }

    public virtual DbSet<MDProdOrderState> MDProdOrderState { get; set; }

    public virtual DbSet<MDRatingComplaintType> MDRatingComplaintType { get; set; }

    public virtual DbSet<MDReleaseState> MDReleaseState { get; set; }

    public virtual DbSet<MDReservationMode> MDReservationMode { get; set; }

    public virtual DbSet<MDSchedulingGroup> MDSchedulingGroup { get; set; }

    public virtual DbSet<MDSchedulingGroupWF> MDSchedulingGroupWF { get; set; }

    public virtual DbSet<MDTermOfPayment> MDTermOfPayment { get; set; }

    public virtual DbSet<MDTimeRange> MDTimeRange { get; set; }

    public virtual DbSet<MDToleranceState> MDToleranceState { get; set; }

    public virtual DbSet<MDTour> MDTour { get; set; }

    public virtual DbSet<MDTourplanPosState> MDTourplanPosState { get; set; }

    public virtual DbSet<MDTourplanState> MDTourplanState { get; set; }

    public virtual DbSet<MDTransportMode> MDTransportMode { get; set; }

    public virtual DbSet<MDUnit> MDUnit { get; set; }

    public virtual DbSet<MDUnitConversion> MDUnitConversion { get; set; }

    public virtual DbSet<MDVisitorCard> MDVisitorCard { get; set; }

    public virtual DbSet<MDVisitorCardState> MDVisitorCardState { get; set; }

    public virtual DbSet<MDVisitorVoucherState> MDVisitorVoucherState { get; set; }

    public virtual DbSet<MDZeroStockState> MDZeroStockState { get; set; }

    public virtual DbSet<MachineMaterialPosView> MachineMaterialPosView { get; set; }

    public virtual DbSet<MachineMaterialRelView> MachineMaterialRelView { get; set; }

    public virtual DbSet<MachineMaterialView> MachineMaterialView { get; set; }

    public virtual DbSet<MaintACClass> MaintACClass { get; set; }

    public virtual DbSet<MaintACClassProperty> MaintACClassProperty { get; set; }

    public virtual DbSet<MaintOrder> MaintOrder { get; set; }

    public virtual DbSet<MaintOrderAssignment> MaintOrderAssignment { get; set; }

    public virtual DbSet<MaintOrderPos> MaintOrderPos { get; set; }

    public virtual DbSet<MaintOrderProperty> MaintOrderProperty { get; set; }

    public virtual DbSet<MaintOrderTask> MaintOrderTask { get; set; }

    public virtual DbSet<Material> Material { get; set; }

    public virtual DbSet<MaterialCalculation> MaterialCalculation { get; set; }

    public virtual DbSet<MaterialConfig> MaterialConfig { get; set; }

    public virtual DbSet<MaterialGMPAdditive> MaterialGMPAdditive { get; set; }

    public virtual DbSet<MaterialHistory> MaterialHistory { get; set; }

    public virtual DbSet<MaterialStock> MaterialStock { get; set; }

    public virtual DbSet<MaterialUnit> MaterialUnit { get; set; }

    public virtual DbSet<MaterialWF> MaterialWF { get; set; }

    public virtual DbSet<MaterialWFACClassMethod> MaterialWFACClassMethod { get; set; }

    public virtual DbSet<MaterialWFACClassMethodConfig> MaterialWFACClassMethodConfig { get; set; }

    public virtual DbSet<MaterialWFConnection> MaterialWFConnection { get; set; }

    public virtual DbSet<MaterialWFRelation> MaterialWFRelation { get; set; }

    public virtual DbSet<MsgAlarmLog> MsgAlarmLog { get; set; }

    public virtual DbSet<OperationLog> OperationLog { get; set; }

    public virtual DbSet<OrderLog> OrderLog { get; set; }

    public virtual DbSet<OrderLogPosMachines> OrderLogPosMachines { get; set; }

    public virtual DbSet<OrderLogPosView> OrderLogPosView { get; set; }

    public virtual DbSet<OrderLogRelView> OrderLogRelView { get; set; }

    public virtual DbSet<OutOffer> OutOffer { get; set; }

    public virtual DbSet<OutOfferConfig> OutOfferConfig { get; set; }

    public virtual DbSet<OutOfferPos> OutOfferPos { get; set; }

    public virtual DbSet<OutOrder> OutOrder { get; set; }

    public virtual DbSet<OutOrderConfig> OutOrderConfig { get; set; }

    public virtual DbSet<OutOrderPos> OutOrderPos { get; set; }

    public virtual DbSet<OutOrderPosSplit> OutOrderPosSplit { get; set; }

    public virtual DbSet<OutOrderPosUtilization> OutOrderPosUtilization { get; set; }

    public virtual DbSet<Partslist> Partslist { get; set; }

    public virtual DbSet<PartslistACClassMethod> PartslistACClassMethod { get; set; }

    public virtual DbSet<PartslistConfig> PartslistConfig { get; set; }

    public virtual DbSet<PartslistPos> PartslistPos { get; set; }

    public virtual DbSet<PartslistPosRelation> PartslistPosRelation { get; set; }

    public virtual DbSet<PartslistPosSplit> PartslistPosSplit { get; set; }

    public virtual DbSet<PartslistStock> PartslistStock { get; set; }

    public virtual DbSet<Picking> Picking { get; set; }

    public virtual DbSet<PickingConfig> PickingConfig { get; set; }

    public virtual DbSet<PickingPos> PickingPos { get; set; }

    public virtual DbSet<PickingPosProdOrderPartslistPos> PickingPosProdOrderPartslistPos { get; set; }

    public virtual DbSet<PlanningMR> PlanningMR { get; set; }

    public virtual DbSet<PlanningMRCons> PlanningMRCons { get; set; }

    public virtual DbSet<PlanningMRPos> PlanningMRPos { get; set; }

    public virtual DbSet<PlanningMRProposal> PlanningMRProposal { get; set; }

    public virtual DbSet<PriceList> PriceList { get; set; }

    public virtual DbSet<PriceListMaterial> PriceListMaterial { get; set; }

    public virtual DbSet<ProdOrder> ProdOrder { get; set; }

    public virtual DbSet<ProdOrderBatch> ProdOrderBatch { get; set; }

    public virtual DbSet<ProdOrderBatchPlan> ProdOrderBatchPlan { get; set; }

    public virtual DbSet<ProdOrderConnectionsDetailView> ProdOrderConnectionsDetailView { get; set; }

    public virtual DbSet<ProdOrderConnectionsView> ProdOrderConnectionsView { get; set; }

    public virtual DbSet<ProdOrderInwardsView> ProdOrderInwardsView { get; set; }

    public virtual DbSet<ProdOrderOutwardsView> ProdOrderOutwardsView { get; set; }

    public virtual DbSet<ProdOrderPartslist> ProdOrderPartslist { get; set; }

    public virtual DbSet<ProdOrderPartslistConfig> ProdOrderPartslistConfig { get; set; }

    public virtual DbSet<ProdOrderPartslistPos> ProdOrderPartslistPos { get; set; }

    public virtual DbSet<ProdOrderPartslistPosFacilityLot> ProdOrderPartslistPosFacilityLot { get; set; }

    public virtual DbSet<ProdOrderPartslistPosRelation> ProdOrderPartslistPosRelation { get; set; }

    public virtual DbSet<ProdOrderPartslistPosSplit> ProdOrderPartslistPosSplit { get; set; }

    public virtual DbSet<Rating> Rating { get; set; }

    public virtual DbSet<RatingComplaint> RatingComplaint { get; set; }

    public virtual DbSet<TandTv3FilterTracking> TandTv3FilterTracking { get; set; }

    public virtual DbSet<TandTv3FilterTrackingMaterial> TandTv3FilterTrackingMaterial { get; set; }

    public virtual DbSet<TandTv3MDBookingDirection> TandTv3MDBookingDirection { get; set; }

    public virtual DbSet<TandTv3MDTrackingDirection> TandTv3MDTrackingDirection { get; set; }

    public virtual DbSet<TandTv3MDTrackingStartItemType> TandTv3MDTrackingStartItemType { get; set; }

    public virtual DbSet<TandTv3MixPoint> TandTv3MixPoint { get; set; }

    public virtual DbSet<TandTv3MixPointDeliveryNotePos> TandTv3MixPointDeliveryNotePos { get; set; }

    public virtual DbSet<TandTv3MixPointFacility> TandTv3MixPointFacility { get; set; }

    public virtual DbSet<TandTv3MixPointFacilityBookingCharge> TandTv3MixPointFacilityBookingCharge { get; set; }

    public virtual DbSet<TandTv3MixPointFacilityLot> TandTv3MixPointFacilityLot { get; set; }

    public virtual DbSet<TandTv3MixPointFacilityPreBooking> TandTv3MixPointFacilityPreBooking { get; set; }

    public virtual DbSet<TandTv3MixPointInOrderPos> TandTv3MixPointInOrderPos { get; set; }

    public virtual DbSet<TandTv3MixPointOutOrderPos> TandTv3MixPointOutOrderPos { get; set; }

    public virtual DbSet<TandTv3MixPointPickingPos> TandTv3MixPointPickingPos { get; set; }

    public virtual DbSet<TandTv3MixPointProdOrderPartslistPos> TandTv3MixPointProdOrderPartslistPos { get; set; }

    public virtual DbSet<TandTv3MixPointProdOrderPartslistPosRelation> TandTv3MixPointProdOrderPartslistPosRelation { get; set; }

    public virtual DbSet<TandTv3MixPointRelation> TandTv3MixPointRelation { get; set; }

    public virtual DbSet<TandTv3Step> TandTv3Step { get; set; }

    public virtual DbSet<Tourplan> Tourplan { get; set; }

    public virtual DbSet<TourplanConfig> TourplanConfig { get; set; }

    public virtual DbSet<TourplanPos> TourplanPos { get; set; }

    public virtual DbSet<UserSettings> UserSettings { get; set; }

    public virtual DbSet<VBConfig> VBConfig { get; set; }

    public virtual DbSet<VBGroup> VBGroup { get; set; }

    public virtual DbSet<VBGroupRight> VBGroupRight { get; set; }

    public virtual DbSet<VBLanguage> VBLanguage { get; set; }

    public virtual DbSet<VBLicense> VBLicense { get; set; }

    public virtual DbSet<VBNoConfiguration> VBNoConfiguration { get; set; }

    public virtual DbSet<VBSystem> VBSystem { get; set; }

    public virtual DbSet<VBSystemColumns> VBSystemColumns { get; set; }

    public virtual DbSet<VBTranslationView> VBTranslationView { get; set; }

    public virtual DbSet<VBUser> VBUser { get; set; }

    public virtual DbSet<VBUserACClassDesign> VBUserACClassDesign { get; set; }

    public virtual DbSet<VBUserACProject> VBUserACProject { get; set; }

    public virtual DbSet<VBUserGroup> VBUserGroup { get; set; }

    public virtual DbSet<VBUserInstance> VBUserInstance { get; set; }

    public virtual DbSet<Visitor> Visitor { get; set; }

    public virtual DbSet<VisitorVoucher> VisitorVoucher { get; set; }

    public virtual DbSet<Weighing> Weighing { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new ACMaterializationInterceptor())
            //.UseLazyLoadingProxies()
            //.UseChangeTrackingProxies()
            .UseModel(iPlusMESV5ContextModel.Instance)
            .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
            //Uncomment connection string when generating new CompiledModels
//.UseSqlServer(ConfigurationManager.ConnectionStrings["iPlusMESV5_Entities"].ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ACAssembly>(entity =>
        {
            entity.ToTable("ACAssembly");

            entity.HasIndex(e => e.AssemblyName, "UIX_Assembly").IsUnique();

            entity.Property(e => e.ACAssemblyID).ValueGeneratedNever();
            entity.Property(e => e.AssemblyDate).HasColumnType("datetime");
            entity.Property(e => e.AssemblyName)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastReflectionDate).HasColumnType("datetime");
            entity.Property(e => e.SHA1)
                .IsRequired()
                .HasMaxLength(40)
                .IsFixedLength();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ACChangeLog>(entity =>
        {
            entity.ToTable("ACChangeLog");

            entity.Property(e => e.ACChangeLogID).ValueGeneratedNever();
            entity.Property(e => e.ChangeDate).HasColumnType("datetime");
            entity.Property(e => e.XMLValue)
                .IsRequired()
                .IsUnicode(false);

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACChangeLog_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACChangeLog_ACClass");

           entity.HasOne(d => d.ACClassProperty).WithMany(p => p.ACChangeLog_ACClassProperty)
                .HasForeignKey(d => d.ACClassPropertyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACChangeLog_ACClassProperty");

           entity.HasOne(d => d.VBUser).WithMany(p => p.ACChangeLog_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACChangeLog_VBUser");
        });

        modelBuilder.Entity<ACClass>(entity =>
        {
            entity.ToTable("ACClass");

            entity.HasIndex(e => new { e.ACKindIndex, e.IsAbstract }, "NCI_ACClass_ACKindIndex_IsAbstract");

            entity.HasIndex(e => e.AssemblyQualifiedName, "NCI_ACClass_AssemblyQualifiedName");

            entity.HasIndex(e => e.ACPackageID, "NCI_FK_ACClass_ACPackageID");

            entity.HasIndex(e => e.ACProjectID, "NCI_FK_ACClass_ACProjectID");

            entity.HasIndex(e => e.BasedOnACClassID, "NCI_FK_ACClass_BasedOnACClassID");

            entity.HasIndex(e => e.PWACClassID, "NCI_FK_ACClass_PWACClassID");

            entity.HasIndex(e => e.PWMethodACClassID, "NCI_FK_ACClass_PWMethodACClassID");

            entity.HasIndex(e => e.ParentACClassID, "NCI_FK_ACClass_ParentACClassID");

            entity.HasIndex(e => new { e.ACProjectID, e.ParentACClassID, e.ACIdentifier }, "UIX_ACClass").IsUnique();

            entity.Property(e => e.ACClassID).ValueGeneratedNever();
            entity.Property(e => e.ACCaptionTranslation).IsUnicode(false);
            entity.Property(e => e.ACFilterColumns)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ACSortColumns)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ACURLCached).IsUnicode(false);
            entity.Property(e => e.ACURLComponentCached).IsUnicode(false);
            entity.Property(e => e.AssemblyQualifiedName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AssemblyQualifiedName2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLACClass).IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACPackage).WithMany(p => p.ACClass_ACPackage)
                .HasForeignKey(d => d.ACPackageID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClass_ACPackageID");

           entity.HasOne(d => d.ACProject).WithMany(p => p.ACClass_ACProject)
                .HasForeignKey(d => d.ACProjectID)
                .HasConstraintName("FK_ACClass_ACProjectID");

           entity.HasOne(d => d.ACClass1_BasedOnACClass).WithMany(p => p.ACClass_BasedOnACClass)
                .HasForeignKey(d => d.BasedOnACClassID)
                .HasConstraintName("FK_ACClass_BasedOnACClassID");

           entity.HasOne(d => d.ACClass1_PWACClass).WithMany(p => p.ACClass_PWACClass)
                .HasForeignKey(d => d.PWACClassID)
                .HasConstraintName("FK_ACClass_PWACClassID");

           entity.HasOne(d => d.ACClass1_PWMethodACClass).WithMany(p => p.ACClass_PWMethodACClass)
                .HasForeignKey(d => d.PWMethodACClassID)
                .HasConstraintName("FK_ACClass_PWMethodACClassID");

           entity.HasOne(d => d.ACClass1_ParentACClass).WithMany(p => p.ACClass_ParentACClass)
                .HasForeignKey(d => d.ParentACClassID)
                .HasConstraintName("FK_ACClass_ParentACClassID");
        });

        modelBuilder.Entity<ACClassConfig>(entity =>
        {
            entity.ToTable("ACClassConfig");

            entity.HasIndex(e => e.ACClassID, "NCI_FK_ACClassConfig_ACClassID");

            entity.HasIndex(e => e.ACClassPropertyRelationID, "NCI_FK_ACClassConfig_ACClassPropertyRelationID");

            entity.HasIndex(e => e.ParentACClassConfigID, "NCI_FK_ACClassConfig_ParentACClassConfigID");

            entity.HasIndex(e => e.ValueTypeACClassID, "NCI_FK_ACClassConfig_ValueTypeACClassID");

            entity.Property(e => e.ACClassConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACClassConfig_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACClassConfig_ACClassID");

           entity.HasOne(d => d.ACClassPropertyRelation).WithMany(p => p.ACClassConfig_ACClassPropertyRelation)
                .HasForeignKey(d => d.ACClassPropertyRelationID)
                .HasConstraintName("FK_ACClassConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.ACClassConfig1_ParentACClassConfig).WithMany(p => p.ACClassConfig_ParentACClassConfig)
                .HasForeignKey(d => d.ParentACClassConfigID)
                .HasConstraintName("FK_ACClassConfig_ParentACClassConfigID");

           entity.HasOne(d => d.ValueTypeACClass).WithMany(p => p.ACClassConfig_ValueTypeACClass)
                .HasForeignKey(d => d.ValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassConfig_ValueTypeACClassID");
        });

        modelBuilder.Entity<ACClassDesign>(entity =>
        {
            entity.ToTable("ACClassDesign");

            entity.HasIndex(e => e.ValueTypeACClassID, "NCI_FK_ACClassDesign_ValueTypeACClassID");

            entity.HasIndex(e => new { e.ACClassID, e.ACIdentifier }, "UIX_ACClassDesign").IsUnique();

            entity.Property(e => e.ACClassDesignID).ValueGeneratedNever();
            entity.Property(e => e.ACCaptionTranslation).IsUnicode(false);
            entity.Property(e => e.ACGroup)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(80)
                .IsUnicode(false);
            entity.Property(e => e.BAMLDate).HasColumnType("datetime");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.DesignNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesign)
                .IsRequired()
                .HasColumnType("text");
            entity.Property(e => e.XMLDesign2).HasColumnType("text");

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACClassDesign_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACClassDesign_ACClassID");

           entity.HasOne(d => d.ValueTypeACClass).WithMany(p => p.ACClassDesign_ValueTypeACClass)
                .HasForeignKey(d => d.ValueTypeACClassID)
                .HasConstraintName("FK_ACClassDesign_ValueTypeACClassID");
        });

        modelBuilder.Entity<ACClassMessage>(entity =>
        {
            entity.ToTable("ACClassMessage");

            entity.HasIndex(e => new { e.ACClassID, e.ACIdentifier }, "UIX_ACClassMessage").IsUnique();

            entity.Property(e => e.ACClassMessageID).ValueGeneratedNever();
            entity.Property(e => e.ACCaptionTranslation).IsUnicode(false);
            entity.Property(e => e.ACIdentifier)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACClassMessage_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACClassMessage_ACClassID");
        });

        modelBuilder.Entity<ACClassMethod>(entity =>
        {
            entity.ToTable("ACClassMethod");

            entity.HasIndex(e => e.PWACClassID, "NCI_FK_ACClassMethod_PWACClassID");

            entity.HasIndex(e => e.ParentACClassMethodID, "NCI_FK_ACClassMethod_ParentACClassMethodID");

            entity.HasIndex(e => e.ValueTypeACClassID, "NCI_FK_ACClassMethod_ValueTypeACClassID");

            entity.HasIndex(e => new { e.ACClassID, e.ACIdentifier }, "UIX_ACClassMethod").IsUnique();

            entity.Property(e => e.ACClassMethodID).ValueGeneratedNever();
            entity.Property(e => e.ACCaptionTranslation).IsUnicode(false);
            entity.Property(e => e.ACGroup)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.GenericType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InteractionVBContent)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Sourcecode).HasColumnType("text");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLACMethod).IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesign).HasColumnType("text");

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACClassMethod_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACClassMethod_ACClassID");

           entity.HasOne(d => d.AttachedFromACClass).WithMany(p => p.ACClassMethod_AttachedFromACClass)
                .HasForeignKey(d => d.AttachedFromACClassID)
                .HasConstraintName("FK_ACClassMethod_AttachedFromACClass");

           entity.HasOne(d => d.PWACClass).WithMany(p => p.ACClassMethod_PWACClass)
                .HasForeignKey(d => d.PWACClassID)
                .HasConstraintName("FK_ACClassMethod_PWACClassID");

           entity.HasOne(d => d.ACClassMethod1_ParentACClassMethod).WithMany(p => p.ACClassMethod_ParentACClassMethod)
                .HasForeignKey(d => d.ParentACClassMethodID)
                .HasConstraintName("FK_ACClassMethod_ParentACClassMethodID");

           entity.HasOne(d => d.ValueTypeACClass).WithMany(p => p.ACClassMethod_ValueTypeACClass)
                .HasForeignKey(d => d.ValueTypeACClassID)
                .HasConstraintName("FK_ACClassMethod_ValueTypeACClassID");
        });

        modelBuilder.Entity<ACClassMethodConfig>(entity =>
        {
            entity.ToTable("ACClassMethodConfig");

            entity.Property(e => e.ACClassMethodConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACClassMethod).WithMany(p => p.ACClassMethodConfig_ACClassMethod)
                .HasForeignKey(d => d.ACClassMethodID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassMethodConfig_ACClassMethodID");

           entity.HasOne(d => d.ACClassWF).WithMany(p => p.ACClassMethodConfig_ACClassWF)
                .HasForeignKey(d => d.ACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ACClassMethodConfig_ACClassWFID");

           entity.HasOne(d => d.ACClassMethodConfig1_ParentACClassMethodConfig).WithMany(p => p.ACClassMethodConfig_ParentACClassMethodConfig)
                .HasForeignKey(d => d.ParentACClassMethodConfigID)
                .HasConstraintName("FK_ACClassMethodConfig_ParentACClassMethodConfigID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.ACClassMethodConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_ACClassMethodConfig_VBiACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.ACClassMethodConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_ACClassMethodConfig_VBiACClassPropertyRelationID");

           entity.HasOne(d => d.ValueTypeACClass).WithMany(p => p.ACClassMethodConfig_ValueTypeACClass)
                .HasForeignKey(d => d.ValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassMethodConfig_ValueTypeACClassID");
        });

        modelBuilder.Entity<ACClassProperty>(entity =>
        {
            entity.ToTable("ACClassProperty");

            entity.HasIndex(e => e.BasedOnACClassPropertyID, "NCI_FK_ACClassProperty_BasedOnACClassPropertyID");

            entity.HasIndex(e => e.ConfigACClassID, "NCI_FK_ACClassProperty_ConfigACClassID");

            entity.HasIndex(e => e.ParentACClassPropertyID, "NCI_FK_ACClassProperty_ParentACClassPropertyID");

            entity.HasIndex(e => e.ValueTypeACClassID, "NCI_FK_ACClassProperty_ValueTypeACClassID");

            entity.HasIndex(e => new { e.ACClassID, e.ACIdentifier }, "UIX_ACClassProperty").IsUnique();

            entity.Property(e => e.ACClassPropertyID).ValueGeneratedNever();
            entity.Property(e => e.ACCaptionTranslation).IsUnicode(false);
            entity.Property(e => e.ACGroup)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ACSource)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CallbackMethodName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.GenericType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InputMask)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLACEventArgs).IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLValue).HasColumnType("text");

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACClassProperty_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACClassProperty_ACClassID");

           entity.HasOne(d => d.ACClassProperty1_BasedOnACClassProperty).WithMany(p => p.ACClassProperty_BasedOnACClassProperty)
                .HasForeignKey(d => d.BasedOnACClassPropertyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassProperty_BasedOnACClassPropertyID");

           entity.HasOne(d => d.ConfigACClass).WithMany(p => p.ACClassProperty_ConfigACClass)
                .HasForeignKey(d => d.ConfigACClassID)
                .HasConstraintName("FK_ACClassProperty_ConfigACClassID");

           entity.HasOne(d => d.ACClassProperty1_ParentACClassProperty).WithMany(p => p.ACClassProperty_ParentACClassProperty)
                .HasForeignKey(d => d.ParentACClassPropertyID)
                .HasConstraintName("FK_ACClassProperty_ParentACClassPropertyID");

           entity.HasOne(d => d.ValueTypeACClass).WithMany(p => p.ACClassProperty_ValueTypeACClass)
                .HasForeignKey(d => d.ValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassProperty_ValueTypeACClassID");
        });

        modelBuilder.Entity<ACClassPropertyRelation>(entity =>
        {
            entity.ToTable("ACClassPropertyRelation");

            entity.HasIndex(e => e.SourceACClassID, "NCI_FK_ACClassPropertyRelation_SourceACClassID");

            entity.HasIndex(e => e.SourceACClassPropertyID, "NCI_FK_ACClassPropertyRelation_SourceACClassPropertyID");

            entity.HasIndex(e => e.TargetACClassID, "NCI_FK_ACClassPropertyRelation_TargetACClassID");

            entity.HasIndex(e => e.TargetACClassPropertyID, "NCI_FK_ACClassPropertyRelation_TargetACClassPropertyID");

            entity.Property(e => e.ACClassPropertyRelationID).ValueGeneratedNever();
            entity.Property(e => e.ConvExpressionS)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ConvExpressionT)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.GroupName).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastManipulationDT).HasColumnType("datetime");
            entity.Property(e => e.StateName).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLValue).HasColumnType("text");

           entity.HasOne(d => d.SourceACClass).WithMany(p => p.ACClassPropertyRelation_SourceACClass)
                .HasForeignKey(d => d.SourceACClassID)
                .HasConstraintName("FK_ACClassPropertyRelation_SourceACClassID");

           entity.HasOne(d => d.SourceACClassProperty).WithMany(p => p.ACClassPropertyRelation_SourceACClassProperty)
                .HasForeignKey(d => d.SourceACClassPropertyID)
                .HasConstraintName("FK_ACClassPropertyRelation_SourceACClassPropertyID");

           entity.HasOne(d => d.TargetACClass).WithMany(p => p.ACClassPropertyRelation_TargetACClass)
                .HasForeignKey(d => d.TargetACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassPropertyRelation_TargetACClassID");

           entity.HasOne(d => d.TargetACClassProperty).WithMany(p => p.ACClassPropertyRelation_TargetACClassProperty)
                .HasForeignKey(d => d.TargetACClassPropertyID)
                .HasConstraintName("FK_ACClassPropertyRelation_TargetACClassPropertyID");
        });

        modelBuilder.Entity<ACClassRouteUsage>(entity =>
        {
            entity.ToTable("ACClassRouteUsage");

            entity.Property(e => e.ACClassRouteUsageID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ACClassRouteUsageGroup>(entity =>
        {
            entity.ToTable("ACClassRouteUsageGroup");

            entity.Property(e => e.ACClassRouteUsageGroupID).ValueGeneratedNever();

           entity.HasOne(d => d.ACClassRouteUsage).WithMany(p => p.ACClassRouteUsageGroup_ACClassRouteUsage)
                .HasForeignKey(d => d.ACClassRouteUsageID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassRouteUsageGroup_ACClassRouteUsage");
        });

        modelBuilder.Entity<ACClassRouteUsagePos>(entity =>
        {
            entity.Property(e => e.ACClassRouteUsagePosID).ValueGeneratedNever();

           entity.HasOne(d => d.ACClassRouteUsage).WithMany(p => p.ACClassRouteUsagePos_ACClassRouteUsage)
                .HasForeignKey(d => d.ACClassRouteUsageID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassRouteUsagePos_ACClassRouteUsage");
        });

        modelBuilder.Entity<ACClassTask>(entity =>
        {
            entity.ToTable("ACClassTask");

            entity.HasIndex(e => e.ACProgramID, "NCI_FK_ACClassTask_ACProgramID");

            entity.HasIndex(e => e.ContentACClassWFID, "NCI_FK_ACClassTask_ContentACClassWFID");

            entity.HasIndex(e => e.ParentACClassTaskID, "NCI_FK_ACClassTask_ParentACClassTaskID");

            entity.HasIndex(e => e.TaskTypeACClassID, "NCI_FK_ACClassTask_TaskTypeACClassID");

            entity.Property(e => e.ACClassTaskID).ValueGeneratedNever();
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLACMethod).IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ACProgram).WithMany(p => p.ACClassTask_ACProgram)
                .HasForeignKey(d => d.ACProgramID)
                .HasConstraintName("FK_ACClassTask_ACProgramID");

           entity.HasOne(d => d.ContentACClassWF).WithMany(p => p.ACClassTask_ContentACClassWF)
                .HasForeignKey(d => d.ContentACClassWFID)
                .HasConstraintName("FK_ACClassTask_ContentACClassWFID");

           entity.HasOne(d => d.ACClassTask1_ParentACClassTask).WithMany(p => p.ACClassTask_ParentACClassTask)
                .HasForeignKey(d => d.ParentACClassTaskID)
                .HasConstraintName("FK_ACClassTask_ParentACClassTaskID");

           entity.HasOne(d => d.TaskTypeACClass).WithMany(p => p.ACClassTask_TaskTypeACClass)
                .HasForeignKey(d => d.TaskTypeACClassID)
                .HasConstraintName("FK_ACClassTask_TaskTypeACClassID");
        });

        modelBuilder.Entity<ACClassTaskValue>(entity =>
        {
            entity.ToTable("ACClassTaskValue");

            entity.HasIndex(e => e.ACClassPropertyID, "NCI_FK_ACClassTaskValue_ACClassPropertyID");

            entity.HasIndex(e => e.ACClassTaskID, "NCI_FK_ACClassTaskValue_ACClassTaskID");

            entity.HasIndex(e => e.VBUserID, "NCI_FK_ACClassTaskValue_VBUserID");

            entity.Property(e => e.ACClassTaskValueID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLValue)
                .IsRequired()
                .HasColumnType("text");
            entity.Property(e => e.XMLValue2)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACClassProperty).WithMany(p => p.ACClassTaskValue_ACClassProperty)
                .HasForeignKey(d => d.ACClassPropertyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassTaskValue_ACClassPropertyID");

           entity.HasOne(d => d.ACClassTask).WithMany(p => p.ACClassTaskValue_ACClassTask)
                .HasForeignKey(d => d.ACClassTaskID)
                .HasConstraintName("FK_ACClassTaskValue_ACClassTaskID");

           entity.HasOne(d => d.VBUser).WithMany(p => p.ACClassTaskValue_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ACClassTaskValue_VBUserID");
        });

        modelBuilder.Entity<ACClassTaskValuePos>(entity =>
        {
            entity.HasIndex(e => e.ACClassTaskValueID, "NCI_FK_ACClassTaskValuePos_ACClassTaskValueID");

            entity.HasIndex(e => e.RequestID, "NCI_FK_ACClassTaskValuePos_RequestID");

            entity.Property(e => e.ACClassTaskValuePosID).ValueGeneratedNever();
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ACUrl)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.AsyncCallbackDelegateName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ClientPointName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ExecutingInstanceURL)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLACMethod)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACClassTaskValue).WithMany(p => p.ACClassTaskValuePos_ACClassTaskValue)
                .HasForeignKey(d => d.ACClassTaskValueID)
                .HasConstraintName("FK_ACClassTaskValuePos_ACClassTaskValueID");
        });

        modelBuilder.Entity<ACClassText>(entity =>
        {
            entity.ToTable("ACClassText");

            entity.HasIndex(e => new { e.ACClassID, e.ACIdentifier }, "UIX_ACClassText").IsUnique();

            entity.Property(e => e.ACClassTextID).ValueGeneratedNever();
            entity.Property(e => e.ACCaptionTranslation).IsUnicode(false);
            entity.Property(e => e.ACIdentifier)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACClassText_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACClassText_ACClassID");
        });

        modelBuilder.Entity<ACClassWF>(entity =>
        {
            entity.ToTable("ACClassWF");

            entity.HasIndex(e => e.PWACClassID, "NCI_FK_ACClassWF_PWACClassID");

            entity.HasIndex(e => e.RefPAACClassID, "NCI_FK_ACClassWF_RefPAACClassID");

            entity.HasIndex(e => e.RefPAACClassMethodID, "NCI_FK_ACClassWF_RefPAACClassMethodID");

            entity.HasIndex(e => new { e.ACClassMethodID, e.ParentACClassWFID, e.ACIdentifier }, "UIX_ACClassWF").IsUnique();

            entity.Property(e => e.ACClassWFID).ValueGeneratedNever();
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PhaseIdentifier)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XName)
                .HasMaxLength(50)
                .IsUnicode(false);

           entity.HasOne(d => d.ACClassMethod).WithMany(p => p.ACClassWF_ACClassMethod)
                .HasForeignKey(d => d.ACClassMethodID)
                .HasConstraintName("FK_ACClassWF_ACClassMethodID");

           entity.HasOne(d => d.PWACClass).WithMany(p => p.ACClassWF_PWACClass)
                .HasForeignKey(d => d.PWACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassWF_PWACClassID");

           entity.HasOne(d => d.ACClassWF1_ParentACClassWF).WithMany(p => p.ACClassWF_ParentACClassWF)
                .HasForeignKey(d => d.ParentACClassWFID)
                .HasConstraintName("FK_ACClassWF_ParentACClassWFID");

           entity.HasOne(d => d.RefPAACClass).WithMany(p => p.ACClassWF_RefPAACClass)
                .HasForeignKey(d => d.RefPAACClassID)
                .HasConstraintName("FK_ACClassWF_RefPAACClassID");

           entity.HasOne(d => d.RefPAACClassMethod).WithMany(p => p.ACClassWF_RefPAACClassMethod)
                .HasForeignKey(d => d.RefPAACClassMethodID)
                .HasConstraintName("FK_ACClassWF_RefPAACClassMethodID");
        });

        modelBuilder.Entity<ACClassWFEdge>(entity =>
        {
            entity.ToTable("ACClassWFEdge");

            entity.HasIndex(e => e.ACClassMethodID, "NCI_FK_ACClassWFEdge_ACClassMethodID");

            entity.HasIndex(e => e.SourceACClassMethodID, "NCI_FK_ACClassWFEdge_SourceACClassMethodID");

            entity.HasIndex(e => e.SourceACClassPropertyID, "NCI_FK_ACClassWFEdge_SourceACClassPropertyID");

            entity.HasIndex(e => e.SourceACClassWFID, "NCI_FK_ACClassWFEdge_SourceACClassWFID");

            entity.HasIndex(e => e.TargetACClassMethodID, "NCI_FK_ACClassWFEdge_TargetACClassMethodID");

            entity.HasIndex(e => e.TargetACClassPropertyID, "NCI_FK_ACClassWFEdge_TargetACClassPropertyID");

            entity.HasIndex(e => e.TargetACClassWFID, "NCI_FK_ACClassWFEdge_TargetACClassWFID");

            entity.Property(e => e.ACClassWFEdgeID).ValueGeneratedNever();
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.XName)
                .HasMaxLength(50)
                .IsUnicode(false);

           entity.HasOne(d => d.ACClassMethod).WithMany(p => p.ACClassWFEdge_ACClassMethod)
                .HasForeignKey(d => d.ACClassMethodID)
                .HasConstraintName("FK_ACClassWFEdge_ACClassMethodID");

           entity.HasOne(d => d.SourceACClassMethod).WithMany(p => p.ACClassWFEdge_SourceACClassMethod)
                .HasForeignKey(d => d.SourceACClassMethodID)
                .HasConstraintName("FK_ACClassWFEdge_SourceACClassMethodID");

           entity.HasOne(d => d.SourceACClassProperty).WithMany(p => p.ACClassWFEdge_SourceACClassProperty)
                .HasForeignKey(d => d.SourceACClassPropertyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassWFEdge_SourceACClassPropertyID");

           entity.HasOne(d => d.SourceACClassWF).WithMany(p => p.ACClassWFEdge_SourceACClassWF)
                .HasForeignKey(d => d.SourceACClassWFID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassWFEdge_SourceACClassWFID");

           entity.HasOne(d => d.TargetACClassMethod).WithMany(p => p.ACClassWFEdge_TargetACClassMethod)
                .HasForeignKey(d => d.TargetACClassMethodID)
                .HasConstraintName("FK_ACClassWFEdge_TargetACClassMethodID");

           entity.HasOne(d => d.TargetACClassProperty).WithMany(p => p.ACClassWFEdge_TargetACClassProperty)
                .HasForeignKey(d => d.TargetACClassPropertyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassWFEdge_TargetACClassPropertyID");

           entity.HasOne(d => d.TargetACClassWF).WithMany(p => p.ACClassWFEdge_TargetACClassWF)
                .HasForeignKey(d => d.TargetACClassWFID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACClassWFEdge_TargetACClassWFID");
        });

        modelBuilder.Entity<ACPackage>(entity =>
        {
            entity.ToTable("ACPackage");

            entity.Property(e => e.ACPackageID).ValueGeneratedNever();
            entity.Property(e => e.ACPackageName)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ACProgram>(entity =>
        {
            entity.ToTable("ACProgram");

            entity.HasIndex(e => e.ProgramACClassMethodID, "NCI_FK_ACProgram_ProgramACClassMethodID");

            entity.HasIndex(e => e.WorkflowTypeACClassID, "NCI_FK_ACProgram_WorkflowTypeACClassID");

            entity.Property(e => e.ACProgramID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PlannedStartDate).HasColumnType("datetime");
            entity.Property(e => e.ProgramName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ProgramACClassMethod).WithMany(p => p.ACProgram_ProgramACClassMethod)
                .HasForeignKey(d => d.ProgramACClassMethodID)
                .HasConstraintName("FK_ACProgram_ProgramACClassMethodID");

           entity.HasOne(d => d.WorkflowTypeACClass).WithMany(p => p.ACProgram_WorkflowTypeACClass)
                .HasForeignKey(d => d.WorkflowTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACProgram_WorkflowTypeACClassID");
        });

        modelBuilder.Entity<ACProgramConfig>(entity =>
        {
            entity.ToTable("ACProgramConfig");

            entity.HasIndex(e => e.ACClassID, "NCI_FK_ACProgramConfig_ACClassID");

            entity.HasIndex(e => e.ACClassPropertyRelationID, "NCI_FK_ACProgramConfig_ACClassPropertyRelationID");

            entity.HasIndex(e => e.ACProgramID, "NCI_FK_ACProgramConfig_ACProgramID");

            entity.HasIndex(e => e.ParentACProgramConfigID, "NCI_FK_ACProgramConfig_ParentACProgramConfigID");

            entity.HasIndex(e => e.ValueTypeACClassID, "NCI_FK_ACProgramConfig_ValueTypeACClassID");

            entity.Property(e => e.ACProgramConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACProgramConfig_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACProgramConfig_ACClassID");

           entity.HasOne(d => d.ACClassPropertyRelation).WithMany(p => p.ACProgramConfig_ACClassPropertyRelation)
                .HasForeignKey(d => d.ACClassPropertyRelationID)
                .HasConstraintName("FK_ACProgramConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.ACProgram).WithMany(p => p.ACProgramConfig_ACProgram)
                .HasForeignKey(d => d.ACProgramID)
                .HasConstraintName("FK_ACProgramConfig_ACProgramID");

           entity.HasOne(d => d.ACProgramConfig1_ParentACProgramConfig).WithMany(p => p.ACProgramConfig_ParentACProgramConfig)
                .HasForeignKey(d => d.ParentACProgramConfigID)
                .HasConstraintName("FK_ACProgramConfig_ParentACProgramConfigID");

           entity.HasOne(d => d.ValueTypeACClass).WithMany(p => p.ACProgramConfig_ValueTypeACClass)
                .HasForeignKey(d => d.ValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACProgramConfig_ValueTypeACClassID");
        });

        modelBuilder.Entity<ACProgramLog>(entity =>
        {
            entity.ToTable("ACProgramLog");

            entity.HasIndex(e => e.ACProgramID, "NCI_FK_ACProgramLog_ACProgramID");

            entity.HasIndex(e => e.ParentACProgramLogID, "NCI_FK_ACProgramLog_ParentACProgramLogID");

            entity.Property(e => e.ACProgramLogID).ValueGeneratedNever();
            entity.Property(e => e.ACUrl)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.EndDatePlan).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Message).IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.StartDatePlan).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACProgram).WithMany(p => p.ACProgramLog_ACProgram)
                .HasForeignKey(d => d.ACProgramID)
                .HasConstraintName("FK_ACProgramLog_ACProgramID");

           entity.HasOne(d => d.ACProgramLog1_ParentACProgramLog).WithMany(p => p.ACProgramLog_ParentACProgramLog)
                .HasForeignKey(d => d.ParentACProgramLogID)
                .HasConstraintName("FK_ACProgramLog_ParentACProgramLogID");
        });

        modelBuilder.Entity<ACProgramLogPropertyLog>(entity =>
        {
            entity.ToTable("ACProgramLogPropertyLog");

            entity.Property(e => e.ACProgramLogPropertyLogID).ValueGeneratedNever();

           entity.HasOne(d => d.ACPropertyLog).WithMany(p => p.ACProgramLogPropertyLog_ACPropertyLog)
                .HasForeignKey(d => d.ACPropertyLogID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACProgramLogPropertyLog_ACPropertyLogID");
        });

        modelBuilder.Entity<ACProgramLogTask>(entity =>
        {
            entity.ToTable("ACProgramLogTask");

            entity.HasIndex(e => e.ACProgramLogID, "NCI_FK_ACProgramLogTask_ACProgramLogID");

            entity.Property(e => e.ACProgramLogTaskID).ValueGeneratedNever();
            entity.Property(e => e.ACClassMethodXAML).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ACProgramLog).WithMany(p => p.ACProgramLogTask_ACProgramLog)
                .HasForeignKey(d => d.ACProgramLogID)
                .HasConstraintName("FK_ACProgramLogTask_ACProgramLogID");
        });

        modelBuilder.Entity<ACProgramLogView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ACProgramLogView");

            entity.Property(e => e.ACClassACCaptionTranslation).IsUnicode(false);
            entity.Property(e => e.ACClassACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ACProgramProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ACUrl)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.MaterialName)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MaterialNo)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PosACUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.ProdOrderBatchNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RelACUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ACProject>(entity =>
        {
            entity.ToTable("ACProject");

            entity.HasIndex(e => e.BasedOnACProjectID, "NCI_FK_ACProject_BasedOnACProjectID");

            entity.HasIndex(e => e.PAAppClassAssignmentACClassID, "NCI_FK_ACProject_PAAppClassAssignmentACClassID");

            entity.HasIndex(e => e.ACProjectNo, "UIX_ACProject_ACProjectNo").IsUnique();

            entity.Property(e => e.ACProjectID).ValueGeneratedNever();
            entity.Property(e => e.ACProjectName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ACProjectNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ACProject1_BasedOnACProject).WithMany(p => p.ACProject_BasedOnACProject)
                .HasForeignKey(d => d.BasedOnACProjectID)
                .HasConstraintName("FK_ACProject_BasedOnACProjectID");

           entity.HasOne(d => d.PAAppClassAssignmentACClass).WithMany(p => p.ACProject_PAAppClassAssignmentACClass)
                .HasForeignKey(d => d.PAAppClassAssignmentACClassID)
                .HasConstraintName("FK_ACProject_PAAppClassAssignmentACClassID");
        });

        modelBuilder.Entity<ACPropertyLog>(entity =>
        {
            entity.ToTable("ACPropertyLog");

            entity.Property(e => e.ACPropertyLogID).ValueGeneratedNever();
            entity.Property(e => e.EventTime).HasColumnType("datetime");
            entity.Property(e => e.Value)
                .IsRequired()
                .IsUnicode(false);

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACPropertyLog_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACPropertyLog_ACClass");

           entity.HasOne(d => d.ACClassProperty).WithMany(p => p.ACPropertyLog_ACClassProperty)
                .HasForeignKey(d => d.ACClassPropertyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ACPropertyLog_ACClassProperty");
        });

        modelBuilder.Entity<ACPropertyLogRule>(entity =>
        {
            entity.ToTable("ACPropertyLogRule");

            entity.Property(e => e.ACPropertyLogRuleID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.ACClass).WithMany(p => p.ACPropertyLogRule_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_ACPropertyLogRule_ACClass");
        });

        modelBuilder.Entity<Calendar>(entity =>
        {
            entity.ToTable("Calendar");

            entity.Property(e => e.CalendarID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<CalendarHoliday>(entity =>
        {
            entity.ToTable("CalendarHoliday");

            entity.HasIndex(e => e.CalendarID, "NCI_FK_CalendarHoliday_CalendarID");

            entity.HasIndex(e => e.MDCountryID, "NCI_FK_CalendarHoliday_MDCountryID");

            entity.HasIndex(e => e.MDCountryLandID, "NCI_FK_CalendarHoliday_MDCountryLandID");

            entity.Property(e => e.CalendarHolidayID).ValueGeneratedNever();
            entity.Property(e => e.HolidayName)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Calendar).WithMany(p => p.CalendarHoliday_Calendar)
                .HasForeignKey(d => d.CalendarID)
                .HasConstraintName("FK_CalendarHoliday_CalendarID");

           entity.HasOne(d => d.MDCountry).WithMany(p => p.CalendarHoliday_MDCountry)
                .HasForeignKey(d => d.MDCountryID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalendarHoliday_MDCountryID");

           entity.HasOne(d => d.MDCountryLand).WithMany(p => p.CalendarHoliday_MDCountryLand)
                .HasForeignKey(d => d.MDCountryLandID)
                .HasConstraintName("FK_CalendarHoliday_MDCountryLandID");
        });

        modelBuilder.Entity<CalendarShift>(entity =>
        {
            entity.ToTable("CalendarShift");

            entity.HasIndex(e => e.CalendarID, "NCI_FK_CalendarShift_CalendarID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_CalendarShift_MDTimeRangeID");

            entity.HasIndex(e => e.VBiACProjectID, "NCI_FK_CalendarShift_VBiACProjectID");

            entity.Property(e => e.CalendarShiftID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Calendar).WithMany(p => p.CalendarShift_Calendar)
                .HasForeignKey(d => d.CalendarID)
                .HasConstraintName("FK_CalendarShift_CalendarID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.CalendarShift_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_CalendarShift_MDTimeRangeID");

           entity.HasOne(d => d.VBiACProject).WithMany(p => p.CalendarShift_VBiACProject)
                .HasForeignKey(d => d.VBiACProjectID)
                .HasConstraintName("FK_CalendarShift_ACProjectID");
        });

        modelBuilder.Entity<CalendarShiftPerson>(entity =>
        {
            entity.ToTable("CalendarShiftPerson");

            entity.HasIndex(e => e.CalendarShiftID, "NCI_FK_CalendarShiftPerson_CalendarShiftID");

            entity.HasIndex(e => e.CompanyPersonID, "NCI_FK_CalendarShiftPerson_CompanyPersonID");

            entity.Property(e => e.CalendarShiftPersonID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CalendarShift).WithMany(p => p.CalendarShiftPerson_CalendarShift)
                .HasForeignKey(d => d.CalendarShiftID)
                .HasConstraintName("FK_CalendarShiftPerson_CalendarShiftID");

           entity.HasOne(d => d.CompanyPerson).WithMany(p => p.CalendarShiftPerson_CompanyPerson)
                .HasForeignKey(d => d.CompanyPersonID)
                .HasConstraintName("FK_CalendarShiftPerson_CompanyPersonID");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");

            entity.HasIndex(e => e.BillingMDTermOfPaymentID, "NCI_FK_Company_BillingMDTermOfPaymentID");

            entity.HasIndex(e => e.MDCurrencyID, "NCI_FK_Company_MDCurrencyID");

            entity.HasIndex(e => e.ParentCompanyID, "NCI_FK_Company_ParentCompanyID");

            entity.HasIndex(e => e.ShippingMDTermOfPaymentID, "NCI_FK_Company_ShippingMDTermOfPaymentID");

            entity.HasIndex(e => e.CompanyNo, "UIX_Company").IsUnique();

            entity.Property(e => e.CompanyID).ValueGeneratedNever();
            entity.Property(e => e.BillingAccountNo)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CompanyNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.NoteExternal)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NoteInternal)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ShippingAccountNo)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VATNumber)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.WebUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.BillingMDTermOfPayment).WithMany(p => p.Company_BillingMDTermOfPayment)
                .HasForeignKey(d => d.BillingMDTermOfPaymentID)
                .HasConstraintName("FK_Company_BillingMDTermOfPaymentID");

           entity.HasOne(d => d.MDCurrency).WithMany(p => p.Company_MDCurrency)
                .HasForeignKey(d => d.MDCurrencyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Company_MDCurrencyID");

           entity.HasOne(d => d.Company1_ParentCompany).WithMany(p => p.Company_ParentCompany)
                .HasForeignKey(d => d.ParentCompanyID)
                .HasConstraintName("FK_Company_ParentCompanyID");

           entity.HasOne(d => d.ShippingMDTermOfPayment).WithMany(p => p.Company_ShippingMDTermOfPayment)
                .HasForeignKey(d => d.ShippingMDTermOfPaymentID)
                .HasConstraintName("FK_Company_ShippingMDTermOfPaymentID");

           entity.HasOne(d => d.VBUser).WithMany(p => p.Company_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .HasConstraintName("FK_Company_VBUser");
        });

        modelBuilder.Entity<CompanyAddress>(entity =>
        {
            entity.ToTable("CompanyAddress");

            entity.HasIndex(e => e.CompanyID, "NCI_FK_CompanyAddress_CompanyID");

            entity.HasIndex(e => e.MDCountryID, "NCI_FK_CompanyAddress_MDCountryID");

            entity.HasIndex(e => e.MDCountryLandID, "NCI_FK_CompanyAddress_MDCountryLandID");

            entity.HasIndex(e => e.MDDelivTypeID, "NCI_FK_CompanyAddress_MDDelivTypeID");

            entity.Property(e => e.CompanyAddressID).ValueGeneratedNever();
            entity.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.EMail)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fax)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceIssuerNo).HasMaxLength(50);
            entity.Property(e => e.Mobile)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name1)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name2)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name3)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PostOfficeBox)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Postcode)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Street)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WebUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Company).WithMany(p => p.CompanyAddress_Company)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyAddress_CompanyID");

           entity.HasOne(d => d.MDCountry).WithMany(p => p.CompanyAddress_MDCountry)
                .HasForeignKey(d => d.MDCountryID)
                .HasConstraintName("FK_CompanyAddress_MDCountryID");

           entity.HasOne(d => d.MDCountryLand).WithMany(p => p.CompanyAddress_MDCountryLand)
                .HasForeignKey(d => d.MDCountryLandID)
                .HasConstraintName("FK_CompanyAddress_MDCountryLandID");

           entity.HasOne(d => d.MDDelivType).WithMany(p => p.CompanyAddress_MDDelivType)
                .HasForeignKey(d => d.MDDelivTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyAddress_MDDelivTypeID");
        });

        modelBuilder.Entity<CompanyAddressDepartment>(entity =>
        {
            entity.ToTable("CompanyAddressDepartment");

            entity.HasIndex(e => e.CompanyAddressID, "NCI_FK_CompanyAddressDepartment_CompanyAddressID");

            entity.Property(e => e.CompanyAddressDepartmentID).ValueGeneratedNever();
            entity.Property(e => e.DepartmentName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CompanyAddress).WithMany(p => p.CompanyAddressDepartment_CompanyAddress)
                .HasForeignKey(d => d.CompanyAddressID)
                .HasConstraintName("FK_CompanyAddressDepartment_CompanyAddressID");
        });

        modelBuilder.Entity<CompanyAddressUnloadingpoint>(entity =>
        {
            entity.ToTable("CompanyAddressUnloadingpoint");

            entity.HasIndex(e => e.CompanyAddressID, "NCI_FK_CompanyAddressUnloadingpoint_CompanyAddressID");

            entity.HasIndex(e => new { e.CompanyAddressUnloadingpointID, e.Sequence }, "UIX_CompanyAddressUnloadingpoint").IsUnique();

            entity.Property(e => e.CompanyAddressUnloadingpointID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UnloadingPointName)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CompanyAddress).WithMany(p => p.CompanyAddressUnloadingpoint_CompanyAddress)
                .HasForeignKey(d => d.CompanyAddressID)
                .HasConstraintName("FK_CompanyAddressUnloadingpoint_CompanyAddressID");
        });

        modelBuilder.Entity<CompanyMaterial>(entity =>
        {
            entity.ToTable("CompanyMaterial");

            entity.HasIndex(e => e.CompanyID, "NCI_FK_CompanyMaterial_CompanyID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_CompanyMaterial_MDUnitID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_CompanyMaterial_MaterialID");

            entity.HasIndex(e => e.CompanyMaterialNo, "UIX_CompanyMaterial_CompanyMaterialNo").IsUnique();

            entity.Property(e => e.CompanyMaterialID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CompanyMaterialName)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.CompanyMaterialNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ValidFromDate).HasColumnType("datetime");
            entity.Property(e => e.ValidToDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Company).WithMany(p => p.CompanyMaterial_Company)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyMaterial_CompanyID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.CompanyMaterial_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyMaterial_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.CompanyMaterial_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompanyMaterial_MaterialID");
        });

        modelBuilder.Entity<CompanyMaterialHistory>(entity =>
        {
            entity.ToTable("CompanyMaterialHistory");

            entity.HasIndex(e => e.CompanyMaterialID, "NCI_FK_CompanyMaterialHistory_CompanyMaterialID");

            entity.HasIndex(e => e.HistoryID, "NCI_FK_CompanyMaterialHistory_HistoryID");

            entity.Property(e => e.CompanyMaterialHistoryID).ValueGeneratedNever();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CompanyMaterial).WithMany(p => p.CompanyMaterialHistory_CompanyMaterial)
                .HasForeignKey(d => d.CompanyMaterialID)
                .HasConstraintName("FK_CompanyMaterialHistory_CompanyMaterialID");

           entity.HasOne(d => d.History).WithMany(p => p.CompanyMaterialHistory_History)
                .HasForeignKey(d => d.HistoryID)
                .HasConstraintName("FK_CompanyMaterialHistory_HistoryID");
        });

        modelBuilder.Entity<CompanyMaterialPickup>(entity =>
        {
            entity.ToTable("CompanyMaterialPickup");

            entity.HasIndex(e => e.CompanyMaterialID, "NCI_FK_CompanyMaterialPickup_CompanyMaterialID");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_CompanyMaterialPickup_InOrderPosID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_CompanyMaterialPickup_OutOrderPosID");

            entity.Property(e => e.CompanyMaterialPickupID).ValueGeneratedNever();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.CompanyMaterial).WithMany(p => p.CompanyMaterialPickup_CompanyMaterial)
                .HasForeignKey(d => d.CompanyMaterialID)
                .HasConstraintName("FK_CompanyMaterialPickup_CompanyMaterialID");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.CompanyMaterialPickup_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_CompanyMaterialPickup_InOrderPosID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.CompanyMaterialPickup_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_CompanyMaterialPickup_OutOrderPosID");
        });

        modelBuilder.Entity<CompanyMaterialStock>(entity =>
        {
            entity.ToTable("CompanyMaterialStock");

            entity.HasIndex(e => e.CompanyMaterialID, "NCI_FK_CompanyMaterialStock_CompanyMaterialID");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_CompanyMaterialStock_MDReleaseStateID");

            entity.Property(e => e.CompanyMaterialStockID).ValueGeneratedNever();
            entity.Property(e => e.DayBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MonthBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WeekBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.YearBalanceDate).HasColumnType("datetime");

           entity.HasOne(d => d.CompanyMaterial).WithMany(p => p.CompanyMaterialStock_CompanyMaterial)
                .HasForeignKey(d => d.CompanyMaterialID)
                .HasConstraintName("FK_CompanyMaterialStock_CompanyMaterialID");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.CompanyMaterialStock_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_CompanyMaterialStock_MDReleaseStateID");
        });

        modelBuilder.Entity<CompanyPerson>(entity =>
        {
            entity.ToTable("CompanyPerson");

            entity.HasIndex(e => e.CompanyID, "NCI_FK_CompanyPerson_CompanyID");

            entity.HasIndex(e => e.MDCountryID, "NCI_FK_CompanyPerson_MDCountryID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_CompanyPerson_MDTimeRangeID");

            entity.HasIndex(e => e.CompanyPersonNo, "UIX_CompanyPerson_CompanyPersonNo").IsUnique();

            entity.Property(e => e.CompanyPersonID).ValueGeneratedNever();
            entity.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CompanyPersonNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.EMail)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Fax)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name1)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name2)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name3)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PostOfficeBox)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Postcode)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Street)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Company).WithMany(p => p.CompanyPerson_Company)
                .HasForeignKey(d => d.CompanyID)
                .HasConstraintName("FK_CompanyPerson_CompanyID");

           entity.HasOne(d => d.MDCountry).WithMany(p => p.CompanyPerson_MDCountry)
                .HasForeignKey(d => d.MDCountryID)
                .HasConstraintName("FK_CompanyPerson_MDCountryID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.CompanyPerson_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_CompanyPerson_MDTimeRangeID");
        });

        modelBuilder.Entity<CompanyPersonRole>(entity =>
        {
            entity.ToTable("CompanyPersonRole");

            entity.HasIndex(e => e.CompanyAddressDepartmentID, "NCI_FK_CompanyPersonRole_CompanyAddressDepartmentID");

            entity.HasIndex(e => e.CompanyPersonID, "NCI_FK_CompanyPersonRole_CompanyPersonID");

            entity.HasIndex(e => e.VBiRoleACClassID, "NCI_FK_CompanyPersonRole_VBiRoleACClassID");

            entity.Property(e => e.CompanyPersonRoleID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CompanyAddressDepartment).WithMany(p => p.CompanyPersonRole_CompanyAddressDepartment)
                .HasForeignKey(d => d.CompanyAddressDepartmentID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CompanyPersonRole_CompanyAddressDepartmentID");

           entity.HasOne(d => d.CompanyPerson).WithMany(p => p.CompanyPersonRole_CompanyPerson)
                .HasForeignKey(d => d.CompanyPersonID)
                .HasConstraintName("FK_CompanyPersonRole_CompanyPersonID");

           entity.HasOne(d => d.VBiRoleACClass).WithMany(p => p.CompanyPersonRole_VBiRoleACClass)
                .HasForeignKey(d => d.VBiRoleACClassID)
                .HasConstraintName("FK_CompanyPersonRole_RoleACClassID");
        });

        modelBuilder.Entity<DeliveryNote>(entity =>
        {
            entity.ToTable("DeliveryNote");

            entity.HasIndex(e => e.DeliveryCompanyAddressID, "NCI_FK_DeliveryNote_DeliveryCompanyAddressID");

            entity.HasIndex(e => e.MDDelivNoteStateID, "NCI_FK_DeliveryNote_MDDelivNoteStateID");

            entity.HasIndex(e => e.ShipperCompanyAddressID, "NCI_FK_DeliveryNote_ShipperCompanyAddressID");

            entity.HasIndex(e => e.TourplanPosID, "NCI_FK_DeliveryNote_TourplanPosID");

            entity.HasIndex(e => e.VisitorVoucherID, "NCI_FK_DeliveryNote_VisitorVoucherID");

            entity.HasIndex(e => e.WeighingID, "NCI_FK_DeliveryNote_WeighingID");

            entity.HasIndex(e => e.DeliveryNoteNo, "UIX_DeliveryNote_DeliveryNoteNo").IsUnique();

            entity.Property(e => e.DeliveryNoteID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryNoteNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LossComment).IsUnicode(false);
            entity.Property(e => e.SupplierDeliveryNo)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Delivery2CompanyAddress).WithMany(p => p.DeliveryNote_Delivery2CompanyAddress)
                .HasForeignKey(d => d.Delivery2CompanyAddressID)
                .HasConstraintName("FK_DeliveryNote_Delivery2CompanyAddressID");

           entity.HasOne(d => d.DeliveryCompanyAddress).WithMany(p => p.DeliveryNote_DeliveryCompanyAddress)
                .HasForeignKey(d => d.DeliveryCompanyAddressID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryNote_DeliveryCompanyAddressID");

           entity.HasOne(d => d.MDDelivNoteState).WithMany(p => p.DeliveryNote_MDDelivNoteState)
                .HasForeignKey(d => d.MDDelivNoteStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryNote_MDDelivNoteStateID");

           entity.HasOne(d => d.ShipperCompanyAddress).WithMany(p => p.DeliveryNote_ShipperCompanyAddress)
                .HasForeignKey(d => d.ShipperCompanyAddressID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeliveryNote_ShipperCompanyAddressID");

           entity.HasOne(d => d.TourplanPos).WithMany(p => p.DeliveryNote_TourplanPos)
                .HasForeignKey(d => d.TourplanPosID)
                .HasConstraintName("FK_DeliveryNote_TourplanPosID");

           entity.HasOne(d => d.VisitorVoucher).WithMany(p => p.DeliveryNote_VisitorVoucher)
                .HasForeignKey(d => d.VisitorVoucherID)
                .HasConstraintName("FK_DeliveryNote_VisitorVoucherID");
        });

        modelBuilder.Entity<DeliveryNotePos>(entity =>
        {
            entity.HasIndex(e => e.DeliveryNoteID, "NCI_FK_DeliveryNotePos_DeliveryNoteID");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_DeliveryNotePos_InOrderPosID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_DeliveryNotePos_OutOrderPosID");

            entity.Property(e => e.DeliveryNotePosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.DeliveryNote).WithMany(p => p.DeliveryNotePos_DeliveryNote)
                .HasForeignKey(d => d.DeliveryNoteID)
                .HasConstraintName("FK_DeliveryNotePos_DeliveryNoteID");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.DeliveryNotePos_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_DeliveryNotePos_InOrderPosID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.DeliveryNotePos_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_DeliveryNotePos_OutOrderPosID");
        });

        modelBuilder.Entity<DemandOrder>(entity =>
        {
            entity.ToTable("DemandOrder");

            entity.HasIndex(e => e.MDDemandOrderStateID, "NCI_FK_DemandOrder_MDDemandOrderStateID");

            entity.HasIndex(e => e.DemandOrderNo, "UIX_DemandOrder_DemandOrderNo").IsUnique();

            entity.Property(e => e.DemandOrderID).ValueGeneratedNever();
            entity.Property(e => e.DemandOrderDate).HasColumnType("datetime");
            entity.Property(e => e.DemandOrderName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DemandOrderNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDDemandOrderState).WithMany(p => p.DemandOrder_MDDemandOrderState)
                .HasForeignKey(d => d.MDDemandOrderStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DemandOrder_MDDemandOrderStateID");
        });

        modelBuilder.Entity<DemandOrderPos>(entity =>
        {
            entity.HasIndex(e => e.ACProgramID, "NCI_FK_DemandOrderPos_ACProgramID");

            entity.HasIndex(e => e.DemandOrderID, "NCI_FK_DemandOrderPos_DemandOrderID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_DemandOrderPos_MaterialID");

            entity.HasIndex(e => e.PartslistID, "NCI_FK_DemandOrderPos_PartslistID");

            entity.HasIndex(e => e.VBiProgramACClassMethodID, "NCI_FK_DemandOrderPos_VBiProgramACClassMethodID");

            entity.Property(e => e.DemandOrderPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ACProgram).WithMany(p => p.DemandOrderPos_ACProgram)
                .HasForeignKey(d => d.ACProgramID)
                .HasConstraintName("FK_DemandOrderPos_ACProgramID");

           entity.HasOne(d => d.DemandOrder).WithMany(p => p.DemandOrderPos_DemandOrder)
                .HasForeignKey(d => d.DemandOrderID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DemandOrderPos_DemandOrderID");

           entity.HasOne(d => d.Material).WithMany(p => p.DemandOrderPos_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DemandOrderPos_MaterialID");

           entity.HasOne(d => d.Partslist).WithMany(p => p.DemandOrderPos_Partslist)
                .HasForeignKey(d => d.PartslistID)
                .HasConstraintName("FK_DemandOrderPos_PartsListID");

           entity.HasOne(d => d.VBiProgramACClassMethod).WithMany(p => p.DemandOrderPos_VBiProgramACClassMethod)
                .HasForeignKey(d => d.VBiProgramACClassMethodID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DemandOrderPos_ProgramACClassMethodID");
        });

        modelBuilder.Entity<DemandPrimary>(entity =>
        {
            entity.ToTable("DemandPrimary");

            entity.HasIndex(e => e.CalendarID, "NCI_FK_DemandPrimary_CalendarID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_DemandPrimary_MaterialID");

            entity.HasIndex(e => e.DemandPrimaryNo, "UIX_DemandPrimary_DemandPrimaryNo").IsUnique();

            entity.Property(e => e.DemandPrimaryID).ValueGeneratedNever();
            entity.Property(e => e.DemandPrimaryNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Calendar).WithMany(p => p.DemandPrimary_Calendar)
                .HasForeignKey(d => d.CalendarID)
                .HasConstraintName("FK_DemandPrimary_CalendarID");

           entity.HasOne(d => d.Material).WithMany(p => p.DemandPrimary_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_DemandPrimary_MaterialID");
        });

        modelBuilder.Entity<DemandProdOrder>(entity =>
        {
            entity.ToTable("DemandProdOrder");

            entity.HasIndex(e => e.DemandOrderID, "NCI_FK_DemandProdOrder_DemandOrderID");

            entity.Property(e => e.DemandProdOrderID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.DemandOrder).WithMany(p => p.DemandProdOrder_DemandOrder)
                .HasForeignKey(d => d.DemandOrderID)
                .HasConstraintName("FK_DemandProdOrder_DemandOrderID");
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.ToTable("Facility");

            entity.HasIndex(e => e.CompanyID, "NCI_FK_Facility_CompanyID");

            entity.HasIndex(e => e.CompanyPersonID, "NCI_FK_Facility_CompanyPersonID");

            entity.HasIndex(e => e.IncomingFacilityID, "NCI_FK_Facility_IncomingFacilityID");

            entity.HasIndex(e => e.LockedFacilityID, "NCI_FK_Facility_LockedFacilityID");

            entity.HasIndex(e => e.MDFacilityTypeID, "NCI_FK_Facility_MDFacilityTypeID");

            entity.HasIndex(e => e.MDFacilityVehicleTypeID, "NCI_FK_Facility_MDFacilityVehicleTypeID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_Facility_MDUnitID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_Facility_MaterialID");

            entity.HasIndex(e => e.OutgoingFacilityID, "NCI_FK_Facility_OutgoingFacilityID");

            entity.HasIndex(e => e.PartslistID, "NCI_FK_Facility_PartslistID");

            entity.HasIndex(e => e.VBiFacilityACClassID, "NCI_FK_Facility_VBiFacilityACClassID");

            entity.HasIndex(e => e.VBiStackCalculatorACClassID, "NCI_FK_Facility_VBiStackCalculatorACClassID");

            entity.HasIndex(e => new { e.ParentFacilityID, e.FacilityNo }, "UIX_Facility").IsUnique();

            entity.Property(e => e.FacilityID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Drivername)
                .IsRequired()
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.FacilityName)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.FacilityNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Company).WithMany(p => p.Facility_Company)
                .HasForeignKey(d => d.CompanyID)
                .HasConstraintName("FK_Facility_CompanyID");

           entity.HasOne(d => d.CompanyPerson).WithMany(p => p.Facility_CompanyPerson)
                .HasForeignKey(d => d.CompanyPersonID)
                .HasConstraintName("FK_Facility_CompanyPersonID");

           entity.HasOne(d => d.Facility1_IncomingFacility).WithMany(p => p.Facility_IncomingFacility)
                .HasForeignKey(d => d.IncomingFacilityID)
                .HasConstraintName("FK_Facility_IncomingFacilityID");

           entity.HasOne(d => d.Facility1_LockedFacility).WithMany(p => p.Facility_LockedFacility)
                .HasForeignKey(d => d.LockedFacilityID)
                .HasConstraintName("FK_Facility_LockedFacilityID");

           entity.HasOne(d => d.MDFacilityType).WithMany(p => p.Facility_MDFacilityType)
                .HasForeignKey(d => d.MDFacilityTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Facility_MDFacilityTypeID");

           entity.HasOne(d => d.MDFacilityVehicleType).WithMany(p => p.Facility_MDFacilityVehicleType)
                .HasForeignKey(d => d.MDFacilityVehicleTypeID)
                .HasConstraintName("FK_Facility_MDFacilityVehicleTypeID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.Facility_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_Facility_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.Facility_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_Facility_MaterialID");

           entity.HasOne(d => d.Facility1_OutgoingFacility).WithMany(p => p.Facility_OutgoingFacility)
                .HasForeignKey(d => d.OutgoingFacilityID)
                .HasConstraintName("FK_Facility_OutgoingFacilityID");

           entity.HasOne(d => d.Facility1_ParentFacility).WithMany(p => p.Facility_ParentFacility)
                .HasForeignKey(d => d.ParentFacilityID)
                .HasConstraintName("FK_Facility_ParentFacilityID");

           entity.HasOne(d => d.Partslist).WithMany(p => p.Facility_Partslist)
                .HasForeignKey(d => d.PartslistID)
                .HasConstraintName("FK_Facility_PartsListID");

           entity.HasOne(d => d.VBiACClassMethod).WithMany(p => p.Facility_VBiACClassMethod)
                .HasForeignKey(d => d.VBiACClassMethodID)
                .HasConstraintName("FK_Facility_ACClassMethodID");

           entity.HasOne(d => d.VBiFacilityACClass).WithMany(p => p.Facility_VBiFacilityACClass)
                .HasForeignKey(d => d.VBiFacilityACClassID)
                .HasConstraintName("FK_Facility_FacilityACClassID");

           entity.HasOne(d => d.VBiStackCalculatorACClass).WithMany(p => p.Facility_VBiStackCalculatorACClass)
                .HasForeignKey(d => d.VBiStackCalculatorACClassID)
                .HasConstraintName("FK_Facility_StackCalculatorACClassID");
        });

        modelBuilder.Entity<FacilityBooking>(entity =>
        {
            entity.ToTable("FacilityBooking");

            entity.HasIndex(e => e.CPartnerCompanyID, "NCI_FK_FacilityBooking_CPartnerCompanyID");

            entity.HasIndex(e => e.HistoryID, "NCI_FK_FacilityBooking_HistoryID");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_FacilityBooking_InOrderPosID");

            entity.HasIndex(e => e.InwardCompanyMaterialID, "NCI_FK_FacilityBooking_InwardCompanyMaterialID");

            entity.HasIndex(e => e.InwardFacilityChargeID, "NCI_FK_FacilityBooking_InwardFacilityChargeID");

            entity.HasIndex(e => e.InwardFacilityID, "NCI_FK_FacilityBooking_InwardFacilityID");

            entity.HasIndex(e => e.InwardFacilityLocationID, "NCI_FK_FacilityBooking_InwardFacilityLocationID");

            entity.HasIndex(e => e.InwardFacilityLotID, "NCI_FK_FacilityBooking_InwardFacilityLotID");

            entity.HasIndex(e => e.InwardMaterialID, "NCI_FK_FacilityBooking_InwardMaterialID");

            entity.HasIndex(e => e.InwardPartslistID, "NCI_FK_FacilityBooking_InwardPartslistID");

            entity.HasIndex(e => e.MDBalancingModeID, "NCI_FK_FacilityBooking_MDBalancingModeID");

            entity.HasIndex(e => e.MDBookingNotAvailableModeID, "NCI_FK_FacilityBooking_MDBookingNotAvailableModeID");

            entity.HasIndex(e => e.MDMovementReasonID, "NCI_FK_FacilityBooking_MDMovementReasonID");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_FacilityBooking_MDReleaseStateID");

            entity.HasIndex(e => e.MDReservationModeID, "NCI_FK_FacilityBooking_MDReservationModeID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_FacilityBooking_MDUnitID");

            entity.HasIndex(e => e.MDZeroStockStateID, "NCI_FK_FacilityBooking_MDZeroStockStateID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_FacilityBooking_OutOrderPosID");

            entity.HasIndex(e => e.OutwardCompanyMaterialID, "NCI_FK_FacilityBooking_OutwardCompanyMaterialID");

            entity.HasIndex(e => e.OutwardFacilityChargeID, "NCI_FK_FacilityBooking_OutwardFacilityChargeID");

            entity.HasIndex(e => e.OutwardFacilityID, "NCI_FK_FacilityBooking_OutwardFacilityID");

            entity.HasIndex(e => e.OutwardFacilityLocationID, "NCI_FK_FacilityBooking_OutwardFacilityLocationID");

            entity.HasIndex(e => e.OutwardFacilityLotID, "NCI_FK_FacilityBooking_OutwardFacilityLotID");

            entity.HasIndex(e => e.OutwardMaterialID, "NCI_FK_FacilityBooking_OutwardMaterialID");

            entity.HasIndex(e => e.OutwardPartslistID, "NCI_FK_FacilityBooking_OutwardPartslistID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FK_FacilityBooking_ProdOrderPartslistPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosRelationID, "NCI_FK_FacilityBooking_ProdOrderPartslistPosRelationID");

            entity.HasIndex(e => e.VBiStackCalculatorACClassID, "NCI_FK_FacilityBooking_VBiStackCalculatorACClassID");

            entity.HasIndex(e => new { e.InwardFacilityLotID, e.InOrderPosID }, "NCI_FacilityBooking_InwardFacilityLotID_InOrderPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FacilityBooking_ProdOrderPartslistPosID_InsertDate");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FacilityBooking_ProdOrderPartslistPosID_OT");

            entity.HasIndex(e => e.ProdOrderPartslistPosRelationID, "NCI_FacilityBooking_ProdOrderPartslistPosRelationID_InsertDate");

            entity.HasIndex(e => e.ProdOrderPartslistPosRelationID, "NCI_FacilityBooking_ProdOrderPartslistPosRelationID_OT");

            entity.HasIndex(e => e.FacilityBookingNo, "UIX_FacilityBooking_FacilityBookingNo").IsUnique();

            entity.Property(e => e.FacilityBookingID).ValueGeneratedNever();
            entity.Property(e => e.BookingMessage).HasColumnType("text");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.FacilityBookingNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InwardHandlingUnit).HasColumnType("text");
            entity.Property(e => e.InwardXMLIdentification).HasColumnType("text");
            entity.Property(e => e.OutwardHandlingUnit).HasColumnType("text");
            entity.Property(e => e.OutwardXMLIdentification).HasColumnType("text");
            entity.Property(e => e.ProductionDate).HasColumnType("datetime");
            entity.Property(e => e.PropertyACUrl).IsUnicode(false);
            entity.Property(e => e.RecipeOrFactoryInfo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StorageDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CPartnerCompany).WithMany(p => p.FacilityBooking_CPartnerCompany)
                .HasForeignKey(d => d.CPartnerCompanyID)
                .HasConstraintName("FK_FacilityBooking_CompanyID");

           entity.HasOne(d => d.FacilityInventoryPos).WithMany(p => p.FacilityBooking_FacilityInventoryPos)
                .HasForeignKey(d => d.FacilityInventoryPosID)
                .HasConstraintName("FK_FacilityBooking_FacilityInventoryPosID");

           entity.HasOne(d => d.History).WithMany(p => p.FacilityBooking_History)
                .HasForeignKey(d => d.HistoryID)
                .HasConstraintName("FK_FacilityBooking_HistoryID");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.FacilityBooking_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_FacilityBooking_InOrderPosID");

           entity.HasOne(d => d.InwardCompanyMaterial).WithMany(p => p.FacilityBooking_InwardCompanyMaterial)
                .HasForeignKey(d => d.InwardCompanyMaterialID)
                .HasConstraintName("FK_FacilityBooking_InwardCompanyMaterialID");

           entity.HasOne(d => d.InwardFacilityCharge).WithMany(p => p.FacilityBooking_InwardFacilityCharge)
                .HasForeignKey(d => d.InwardFacilityChargeID)
                .HasConstraintName("FK_FacilityBooking_InwardFacilityChargeID");

           entity.HasOne(d => d.InwardFacility).WithMany(p => p.FacilityBooking_InwardFacility)
                .HasForeignKey(d => d.InwardFacilityID)
                .HasConstraintName("FK_FacilityBooking_InwardFacilityID");

           entity.HasOne(d => d.InwardFacilityLocation).WithMany(p => p.FacilityBooking_InwardFacilityLocation)
                .HasForeignKey(d => d.InwardFacilityLocationID)
                .HasConstraintName("FK_FacilityBooking_InwardFacilityLocationID");

           entity.HasOne(d => d.InwardFacilityLot).WithMany(p => p.FacilityBooking_InwardFacilityLot)
                .HasForeignKey(d => d.InwardFacilityLotID)
                .HasConstraintName("FK_FacilityBooking_InwardFacilityLotID");

           entity.HasOne(d => d.InwardMaterial).WithMany(p => p.FacilityBooking_InwardMaterial)
                .HasForeignKey(d => d.InwardMaterialID)
                .HasConstraintName("FK_FacilityBooking_InwardMaterialID");

           entity.HasOne(d => d.InwardPartslist).WithMany(p => p.FacilityBooking_InwardPartslist)
                .HasForeignKey(d => d.InwardPartslistID)
                .HasConstraintName("FK_FacilityBooking_InwardPartslistID");

           entity.HasOne(d => d.MDBalancingMode).WithMany(p => p.FacilityBooking_MDBalancingMode)
                .HasForeignKey(d => d.MDBalancingModeID)
                .HasConstraintName("FK_FacilityBooking_MDBalancingModeID");

           entity.HasOne(d => d.MDBookingNotAvailableMode).WithMany(p => p.FacilityBooking_MDBookingNotAvailableMode)
                .HasForeignKey(d => d.MDBookingNotAvailableModeID)
                .HasConstraintName("FK_FacilityBooking_MDBookingNotAvailableModeID");

           entity.HasOne(d => d.MDMovementReason).WithMany(p => p.FacilityBooking_MDMovementReason)
                .HasForeignKey(d => d.MDMovementReasonID)
                .HasConstraintName("FK_FacilityBooking_MDMovementReasonID");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.FacilityBooking_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_FacilityBooking_MDReleaseStateID");

           entity.HasOne(d => d.MDReservationMode).WithMany(p => p.FacilityBooking_MDReservationMode)
                .HasForeignKey(d => d.MDReservationModeID)
                .HasConstraintName("FK_FacilityBooking_MDReservationModeID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.FacilityBooking_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_FacilityBooking_MDUnitID");

           entity.HasOne(d => d.MDZeroStockState).WithMany(p => p.FacilityBooking_MDZeroStockState)
                .HasForeignKey(d => d.MDZeroStockStateID)
                .HasConstraintName("FK_FacilityBooking_MDZeroStockStateID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.FacilityBooking_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_FacilityBooking_OutOrderPosID");

           entity.HasOne(d => d.OutwardCompanyMaterial).WithMany(p => p.FacilityBooking_OutwardCompanyMaterial)
                .HasForeignKey(d => d.OutwardCompanyMaterialID)
                .HasConstraintName("FK_FacilityBooking_OutwardCompanyMaterialID");

           entity.HasOne(d => d.OutwardFacilityCharge).WithMany(p => p.FacilityBooking_OutwardFacilityCharge)
                .HasForeignKey(d => d.OutwardFacilityChargeID)
                .HasConstraintName("FK_FacilityBooking_OutwardFacilityChargeID");

           entity.HasOne(d => d.OutwardFacility).WithMany(p => p.FacilityBooking_OutwardFacility)
                .HasForeignKey(d => d.OutwardFacilityID)
                .HasConstraintName("FK_FacilityBooking_OutwardFacilityID");

           entity.HasOne(d => d.OutwardFacilityLocation).WithMany(p => p.FacilityBooking_OutwardFacilityLocation)
                .HasForeignKey(d => d.OutwardFacilityLocationID)
                .HasConstraintName("FK_FacilityBooking_OutwardFacilityLocationID");

           entity.HasOne(d => d.OutwardFacilityLot).WithMany(p => p.FacilityBooking_OutwardFacilityLot)
                .HasForeignKey(d => d.OutwardFacilityLotID)
                .HasConstraintName("FK_FacilityBooking_OutwardFacilityLotID");

           entity.HasOne(d => d.OutwardMaterial).WithMany(p => p.FacilityBooking_OutwardMaterial)
                .HasForeignKey(d => d.OutwardMaterialID)
                .HasConstraintName("FK_FacilityBooking_OutwardMaterialID");

           entity.HasOne(d => d.OutwardPartslist).WithMany(p => p.FacilityBooking_OutwardPartslist)
                .HasForeignKey(d => d.OutwardPartslistID)
                .HasConstraintName("FK_FacilityBooking_OutwardPartsListID");

           entity.HasOne(d => d.PickingPos).WithMany(p => p.FacilityBooking_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .HasConstraintName("FK_FacilityBooking_PickingPosID");

           entity.HasOne(d => d.ProdOrderPartslistPosFacilityLot).WithMany(p => p.FacilityBooking_ProdOrderPartslistPosFacilityLot)
                .HasForeignKey(d => d.ProdOrderPartslistPosFacilityLotID)
                .HasConstraintName("FK_FacilityBooking_ProdOrderPartslistPosFacilityLot");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.FacilityBooking_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_FacilityBooking_ProdOrderPartslistPosID");

           entity.HasOne(d => d.ProdOrderPartslistPosRelation).WithMany(p => p.FacilityBooking_ProdOrderPartslistPosRelation)
                .HasForeignKey(d => d.ProdOrderPartslistPosRelationID)
                .HasConstraintName("FK_FacilityBooking_ProdOrderPartslistPosRelation");

           entity.HasOne(d => d.VBiStackCalculatorACClass).WithMany(p => p.FacilityBooking_VBiStackCalculatorACClass)
                .HasForeignKey(d => d.VBiStackCalculatorACClassID)
                .HasConstraintName("FK_FacilityBooking_StackCalculatorACClassID");
        });

        modelBuilder.Entity<FacilityBookingCharge>(entity =>
        {
            entity.ToTable("FacilityBookingCharge");

            entity.HasIndex(e => e.FacilityBookingID, "NCI_FK_FacilityBookingCharge_FacilityBookingID");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_FacilityBookingCharge_InOrderPosID");

            entity.HasIndex(e => e.InwardCPartnerCompMatID, "NCI_FK_FacilityBookingCharge_InwardCPartnerCompMatID");

            entity.HasIndex(e => e.InwardCompanyMaterialID, "NCI_FK_FacilityBookingCharge_InwardCompanyMaterialID");

            entity.HasIndex(e => e.InwardFacilityChargeID, "NCI_FK_FacilityBookingCharge_InwardFacilityChargeID");

            entity.HasIndex(e => e.InwardFacilityID, "NCI_FK_FacilityBookingCharge_InwardFacilityID");

            entity.HasIndex(e => e.InwardFacilityLocationID, "NCI_FK_FacilityBookingCharge_InwardFacilityLocationID");

            entity.HasIndex(e => e.InwardFacilityLotID, "NCI_FK_FacilityBookingCharge_InwardFacilityLotID");

            entity.HasIndex(e => e.InwardMaterialID, "NCI_FK_FacilityBookingCharge_InwardMaterialID");

            entity.HasIndex(e => e.InwardPartslistID, "NCI_FK_FacilityBookingCharge_InwardPartslistID");

            entity.HasIndex(e => e.MDBalancingModeID, "NCI_FK_FacilityBookingCharge_MDBalancingModeID");

            entity.HasIndex(e => e.MDBookingNotAvailableModeID, "NCI_FK_FacilityBookingCharge_MDBookingNotAvailableModeID");

            entity.HasIndex(e => e.MDMovementReasonID, "NCI_FK_FacilityBookingCharge_MDMovementReasonID");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_FacilityBookingCharge_MDReleaseStateID");

            entity.HasIndex(e => e.MDReservationModeID, "NCI_FK_FacilityBookingCharge_MDReservationModeID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_FacilityBookingCharge_MDUnitID");

            entity.HasIndex(e => e.MDZeroStockStateID, "NCI_FK_FacilityBookingCharge_MDZeroStockStateID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_FacilityBookingCharge_OutOrderPosID");

            entity.HasIndex(e => e.OutwardCPartnerCompMatID, "NCI_FK_FacilityBookingCharge_OutwardCPartnerCompMatID");

            entity.HasIndex(e => e.OutwardCompanyMaterialID, "NCI_FK_FacilityBookingCharge_OutwardCompanyMaterialID");

            entity.HasIndex(e => e.OutwardFacilityChargeID, "NCI_FK_FacilityBookingCharge_OutwardFacilityChargeID");

            entity.HasIndex(e => e.OutwardFacilityID, "NCI_FK_FacilityBookingCharge_OutwardFacilityID");

            entity.HasIndex(e => e.OutwardFacilityLocationID, "NCI_FK_FacilityBookingCharge_OutwardFacilityLocationID");

            entity.HasIndex(e => e.OutwardFacilityLotID, "NCI_FK_FacilityBookingCharge_OutwardFacilityLotID");

            entity.HasIndex(e => e.OutwardMaterialID, "NCI_FK_FacilityBookingCharge_OutwardMaterialID");

            entity.HasIndex(e => e.OutwardPartslistID, "NCI_FK_FacilityBookingCharge_OutwardPartslistID");

            entity.HasIndex(e => e.PickingPosID, "NCI_FK_FacilityBookingCharge_PickingPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FK_FacilityBookingCharge_ProdOrderPartslistPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosRelationID, "NCI_FK_FacilityBookingCharge_ProdOrderPartslistPosRelationID");

            entity.HasIndex(e => e.VBiStackCalculatorACClassID, "NCI_FK_FacilityBookingCharge_VBiStackCalculatorACClassID");

            entity.HasIndex(e => new { e.InwardFacilityChargeID, e.OutwardFacilityChargeID, e.FacilityBookingID }, "NCI_FacilityBookingCharge_InwardFacilityChargeID_OutwardFacilityChargeID");

            entity.HasIndex(e => new { e.InwardFacilityID, e.OutwardFacilityID }, "NCI_FacilityBookingCharge_InwardFacilityID_OutwardFacilityID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FacilityBookingCharge_ProdOrderPartslistPosID_InsertDate");

            entity.HasIndex(e => new { e.ProdOrderPartslistPosID, e.InwardFacilityChargeID }, "NCI_FacilityBookingCharge_ProdOrderPartslistPosID_InwardFacilityChargeID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FacilityBookingCharge_ProdOrderPartslistPosID_InwardFacilityID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FacilityBookingCharge_ProdOrderPartslistPosID_OT");

            entity.HasIndex(e => e.FacilityBookingChargeNo, "UIX_FacilityBookingCharge_FacilityBookingChargeNo").IsUnique();

            entity.Property(e => e.FacilityBookingChargeID).ValueGeneratedNever();
            entity.Property(e => e.BookingMessage).IsUnicode(false);
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.FacilityBookingChargeNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProductionDate).HasColumnType("datetime");
            entity.Property(e => e.RecipeOrFactoryInfo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StorageDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.FacilityBooking).WithMany(p => p.FacilityBookingCharge_FacilityBooking)
                .HasForeignKey(d => d.FacilityBookingID)
                .HasConstraintName("FK_FacilityBookingCharge_FacilityBookingID");

           entity.HasOne(d => d.FacilityInventoryPos).WithMany(p => p.FacilityBookingCharge_FacilityInventoryPos)
                .HasForeignKey(d => d.FacilityInventoryPosID)
                .HasConstraintName("FK_FacilityBookingCharge_FacilityInventoryPosID");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.FacilityBookingCharge_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_FacilityBookingCharge_InOrderPosID");

           entity.HasOne(d => d.InwardCPartnerCompMat).WithMany(p => p.FacilityBookingCharge_InwardCPartnerCompMat)
                .HasForeignKey(d => d.InwardCPartnerCompMatID)
                .HasConstraintName("FK_FacilityBookingCharge_InCPartnerCompMatID");

           entity.HasOne(d => d.InwardCompanyMaterial).WithMany(p => p.FacilityBookingCharge_InwardCompanyMaterial)
                .HasForeignKey(d => d.InwardCompanyMaterialID)
                .HasConstraintName("FK_FacilityBookingCharge_InwardCompanyMaterialID");

           entity.HasOne(d => d.InwardFacilityCharge).WithMany(p => p.FacilityBookingCharge_InwardFacilityCharge)
                .HasForeignKey(d => d.InwardFacilityChargeID)
                .HasConstraintName("FK_FacilityBookingCharge_InwardFacilityChargeID");

           entity.HasOne(d => d.InwardFacility).WithMany(p => p.FacilityBookingCharge_InwardFacility)
                .HasForeignKey(d => d.InwardFacilityID)
                .HasConstraintName("FK_FacilityBookingCharge_InwardFacilityID");

           entity.HasOne(d => d.InwardFacilityLocation).WithMany(p => p.FacilityBookingCharge_InwardFacilityLocation)
                .HasForeignKey(d => d.InwardFacilityLocationID)
                .HasConstraintName("FK_FacilityBookingCharge_InwardFacilityLocationID");

           entity.HasOne(d => d.InwardFacilityLot).WithMany(p => p.FacilityBookingCharge_InwardFacilityLot)
                .HasForeignKey(d => d.InwardFacilityLotID)
                .HasConstraintName("FK_FacilityBookingCharge_InwardFacilityLotID");

           entity.HasOne(d => d.InwardMaterial).WithMany(p => p.FacilityBookingCharge_InwardMaterial)
                .HasForeignKey(d => d.InwardMaterialID)
                .HasConstraintName("FK_FacilityBookingCharge_InwardMaterialID");

           entity.HasOne(d => d.InwardPartslist).WithMany(p => p.FacilityBookingCharge_InwardPartslist)
                .HasForeignKey(d => d.InwardPartslistID)
                .HasConstraintName("FK_FacilityBookingCharge_InwardPartsListID");

           entity.HasOne(d => d.MDBalancingMode).WithMany(p => p.FacilityBookingCharge_MDBalancingMode)
                .HasForeignKey(d => d.MDBalancingModeID)
                .HasConstraintName("FK_FacilityBookingCharge_MDBalancingModeID");

           entity.HasOne(d => d.MDBookingNotAvailableMode).WithMany(p => p.FacilityBookingCharge_MDBookingNotAvailableMode)
                .HasForeignKey(d => d.MDBookingNotAvailableModeID)
                .HasConstraintName("FK_FacilityBookingCharge_MDBookingNotAvailableModeID");

           entity.HasOne(d => d.MDMovementReason).WithMany(p => p.FacilityBookingCharge_MDMovementReason)
                .HasForeignKey(d => d.MDMovementReasonID)
                .HasConstraintName("FK_FacilityBookingCharge_MDMovementReasonID");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.FacilityBookingCharge_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_FacilityBookingCharge_MDReleaseStateID");

           entity.HasOne(d => d.MDReservationMode).WithMany(p => p.FacilityBookingCharge_MDReservationMode)
                .HasForeignKey(d => d.MDReservationModeID)
                .HasConstraintName("FK_FacilityBookingCharge_MDReservationModeID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.FacilityBookingCharge_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityBookingCharge_MDUnitID");

           entity.HasOne(d => d.MDZeroStockState).WithMany(p => p.FacilityBookingCharge_MDZeroStockState)
                .HasForeignKey(d => d.MDZeroStockStateID)
                .HasConstraintName("FK_FacilityBookingCharge_MDZeroStockStateID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.FacilityBookingCharge_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_FacilityBookingCharge_OutOrderPosID");

           entity.HasOne(d => d.OutwardCPartnerCompMat).WithMany(p => p.FacilityBookingCharge_OutwardCPartnerCompMat)
                .HasForeignKey(d => d.OutwardCPartnerCompMatID)
                .HasConstraintName("FK_FacilityBookingCharge_OutCPartnerCompMatID");

           entity.HasOne(d => d.OutwardCompanyMaterial).WithMany(p => p.FacilityBookingCharge_OutwardCompanyMaterial)
                .HasForeignKey(d => d.OutwardCompanyMaterialID)
                .HasConstraintName("FK_FacilityBookingCharge_OutwardCompanyMaterialID");

           entity.HasOne(d => d.OutwardFacilityCharge).WithMany(p => p.FacilityBookingCharge_OutwardFacilityCharge)
                .HasForeignKey(d => d.OutwardFacilityChargeID)
                .HasConstraintName("FK_FacilityBookingCharge_OutwardFacilityChargeID");

           entity.HasOne(d => d.OutwardFacility).WithMany(p => p.FacilityBookingCharge_OutwardFacility)
                .HasForeignKey(d => d.OutwardFacilityID)
                .HasConstraintName("FK_FacilityBookingCharge_OutwardFacilityID");

           entity.HasOne(d => d.OutwardFacilityLocation).WithMany(p => p.FacilityBookingCharge_OutwardFacilityLocation)
                .HasForeignKey(d => d.OutwardFacilityLocationID)
                .HasConstraintName("FK_FacilityBookingCharge_OutwardFacilityLocationID");

           entity.HasOne(d => d.OutwardFacilityLot).WithMany(p => p.FacilityBookingCharge_OutwardFacilityLot)
                .HasForeignKey(d => d.OutwardFacilityLotID)
                .HasConstraintName("FK_FacilityBookingCharge_OutwardFacilityLotID");

           entity.HasOne(d => d.OutwardMaterial).WithMany(p => p.FacilityBookingCharge_OutwardMaterial)
                .HasForeignKey(d => d.OutwardMaterialID)
                .HasConstraintName("FK_FacilityBookingCharge_OutwardMaterialID");

           entity.HasOne(d => d.OutwardPartslist).WithMany(p => p.FacilityBookingCharge_OutwardPartslist)
                .HasForeignKey(d => d.OutwardPartslistID)
                .HasConstraintName("FK_FacilityBookingCharge_OutwardPartsListID");

           entity.HasOne(d => d.PickingPos).WithMany(p => p.FacilityBookingCharge_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .HasConstraintName("FK_FacilityBookingCharge_PickingPosID");

           entity.HasOne(d => d.ProdOrderPartslistPosFacilityLot).WithMany(p => p.FacilityBookingCharge_ProdOrderPartslistPosFacilityLot)
                .HasForeignKey(d => d.ProdOrderPartslistPosFacilityLotID)
                .HasConstraintName("FK_FacilityBookingCharge_ProdOrderPartslistPosFacilityLot");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.FacilityBookingCharge_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_FacilityBookingCharge_ProdOrderPartslistPosID");

           entity.HasOne(d => d.ProdOrderPartslistPosRelation).WithMany(p => p.FacilityBookingCharge_ProdOrderPartslistPosRelation)
                .HasForeignKey(d => d.ProdOrderPartslistPosRelationID)
                .HasConstraintName("FK_FacilityBookingCharge_ProdOrderPartslistPosRelation");

           entity.HasOne(d => d.VBiStackCalculatorACClass).WithMany(p => p.FacilityBookingCharge_VBiStackCalculatorACClass)
                .HasForeignKey(d => d.VBiStackCalculatorACClassID)
                .HasConstraintName("FK_FacilityBookingCharge_StackCalculatorACClassID");
        });

        modelBuilder.Entity<FacilityCharge>(entity =>
        {
            entity.ToTable("FacilityCharge");

            entity.HasIndex(e => e.CPartnerCompanyMaterialID, "NCI_FK_FacilityCharge_CPartnerCompanyMaterialID");

            entity.HasIndex(e => e.CompanyMaterialID, "NCI_FK_FacilityCharge_CompanyMaterialID");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_FacilityCharge_MDReleaseStateID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_FacilityCharge_MDUnitID");

            entity.HasIndex(e => e.PartslistID, "NCI_FK_FacilityCharge_PartslistID");

            entity.HasIndex(e => new { e.MaterialID, e.FacilityID, e.FacilityLotID, e.SplitNo, e.FacilityChargeSortNo }, "UIX_FacilityCharge").IsUnique();

            entity.Property(e => e.FacilityChargeID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CostFix).HasColumnType("money");
            entity.Property(e => e.CostGeneral).HasColumnType("money");
            entity.Property(e => e.CostHandlingFix).HasColumnType("money");
            entity.Property(e => e.CostHandlingVar).HasColumnType("money");
            entity.Property(e => e.CostLoss).HasColumnType("money");
            entity.Property(e => e.CostMat).HasColumnType("money");
            entity.Property(e => e.CostPack).HasColumnType("money");
            entity.Property(e => e.CostReQuantity).HasColumnType("money");
            entity.Property(e => e.CostVar).HasColumnType("money");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.FillingDate).HasColumnType("datetime");
            entity.Property(e => e.HandlingUnit).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProductionDate).HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CPartnerCompanyMaterial).WithMany(p => p.FacilityCharge_CPartnerCompanyMaterial)
                .HasForeignKey(d => d.CPartnerCompanyMaterialID)
                .HasConstraintName("FK_FacilityCharge_CPartnerCompMatID");

           entity.HasOne(d => d.CompanyMaterial).WithMany(p => p.FacilityCharge_CompanyMaterial)
                .HasForeignKey(d => d.CompanyMaterialID)
                .HasConstraintName("FK_FacilityCharge_CompanyMaterialID");

           entity.HasOne(d => d.Facility).WithMany(p => p.FacilityCharge_Facility)
                .HasForeignKey(d => d.FacilityID)
                .HasConstraintName("FK_FacilityCharge_FacilityID");

           entity.HasOne(d => d.FacilityLot).WithMany(p => p.FacilityCharge_FacilityLot)
                .HasForeignKey(d => d.FacilityLotID)
                .HasConstraintName("FK_FacilityCharge_FacilityLotID");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.FacilityCharge_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_FacilityCharge_MDReleaseStateID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.FacilityCharge_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityCharge_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.FacilityCharge_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityCharge_MaterialID");

           entity.HasOne(d => d.Partslist).WithMany(p => p.FacilityCharge_Partslist)
                .HasForeignKey(d => d.PartslistID)
                .HasConstraintName("FK_FacilityCharge_PartsListID");
        });

        modelBuilder.Entity<FacilityHistory>(entity =>
        {
            entity.ToTable("FacilityHistory");

            entity.HasIndex(e => e.FacilityID, "NCI_FK_FacilityHistory_FacilityID");

            entity.HasIndex(e => e.HistoryID, "NCI_FK_FacilityHistory_HistoryID");

            entity.Property(e => e.FacilityHistoryID).ValueGeneratedNever();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Facility).WithMany(p => p.FacilityHistory_Facility)
                .HasForeignKey(d => d.FacilityID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_FacilityHistory_FacilityID");

           entity.HasOne(d => d.History).WithMany(p => p.FacilityHistory_History)
                .HasForeignKey(d => d.HistoryID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_FacilityHistory_HistoryID");
        });

        modelBuilder.Entity<FacilityInventory>(entity =>
        {
            entity.ToTable("FacilityInventory");

            entity.HasIndex(e => e.FacilityInventoryNo, "UIX_FacilityInventory").IsUnique();

            entity.Property(e => e.FacilityInventoryID).ValueGeneratedNever();
            entity.Property(e => e.FacilityInventoryName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FacilityInventoryNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Facility).WithMany(p => p.FacilityInventory_Facility)
                .HasForeignKey(d => d.FacilityID)
                .HasConstraintName("FK_FacilityInventory_Facility");

           entity.HasOne(d => d.MDFacilityInventoryState).WithMany(p => p.FacilityInventory_MDFacilityInventoryState)
                .HasForeignKey(d => d.MDFacilityInventoryStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityInventory_MDFacilityInventoryStateID");
        });

        modelBuilder.Entity<FacilityInventoryPos>(entity =>
        {
            entity.Property(e => e.FacilityInventoryPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.FacilityCharge).WithMany(p => p.FacilityInventoryPos_FacilityCharge)
                .HasForeignKey(d => d.FacilityChargeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityInventoryPos_FacilityChargeID");

           entity.HasOne(d => d.FacilityInventory).WithMany(p => p.FacilityInventoryPos_FacilityInventory)
                .HasForeignKey(d => d.FacilityInventoryID)
                .HasConstraintName("FK_FacilityInventoryPos_FacilityInventoryID");

           entity.HasOne(d => d.MDFacilityInventoryPosState).WithMany(p => p.FacilityInventoryPos_MDFacilityInventoryPosState)
                .HasForeignKey(d => d.MDFacilityInventoryPosStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityInventoryPos_MDFacilityInventoryPosStateID");
        });

        modelBuilder.Entity<FacilityLot>(entity =>
        {
            entity.ToTable("FacilityLot");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_FacilityLot_MDReleaseStateID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_FacilityLot_MaterialID");

            entity.HasIndex(e => e.ExternLotNo, "NCI_FacilityLot_ExternLotNo_FacilityLotID");

            entity.HasIndex(e => e.LotNo, "UIX_FacilityLot").IsUnique();

            entity.Property(e => e.FacilityLotID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.ExternLotNo)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ExternLotNo2)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.FillingDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LotNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProductionDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.FacilityLot_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_FacilityLot_MDReleaseStateID");

           entity.HasOne(d => d.Material).WithMany(p => p.FacilityLot_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_FacilityLot_MaterialID");
        });

        modelBuilder.Entity<FacilityLotStock>(entity =>
        {
            entity.ToTable("FacilityLotStock");

            entity.HasIndex(e => e.FacilityLotID, "NCI_FK_FacilityLotStock_FacilityLotID");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_FacilityLotStock_MDReleaseStateID");

            entity.Property(e => e.FacilityLotStockID).ValueGeneratedNever();
            entity.Property(e => e.DayBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MonthBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WeekBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.YearBalanceDate).HasColumnType("datetime");

           entity.HasOne(d => d.FacilityLot).WithMany(p => p.FacilityLotStock_FacilityLot)
                .HasForeignKey(d => d.FacilityLotID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityLotStock_FacilityLotID");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.FacilityLotStock_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_FacilityLotStock_MDReleaseStateID");
        });

        modelBuilder.Entity<FacilityMDSchedulingGroup>(entity =>
        {
            entity.ToTable("FacilityMDSchedulingGroup");

            entity.HasIndex(e => new { e.FacilityID, e.MDSchedulingGroupID }, "UIX_FacilityMDSchedulingGroup").IsUnique();

            entity.Property(e => e.FacilityMDSchedulingGroupID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.Facility).WithMany(p => p.FacilityMDSchedulingGroup_Facility)
                .HasForeignKey(d => d.FacilityID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityMDSchedulingGroup_Facility");

           entity.HasOne(d => d.MDPickingType).WithMany(p => p.FacilityMDSchedulingGroup_MDPickingType)
                .HasForeignKey(d => d.MDPickingTypeID)
                .HasConstraintName("FK_FacilityMDSchedulingGroup_MDPickingType");

           entity.HasOne(d => d.MDSchedulingGroup).WithMany(p => p.FacilityMDSchedulingGroup_MDSchedulingGroup)
                .HasForeignKey(d => d.MDSchedulingGroupID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityMDSchedulingGroup_MDSchedulingGroup");
        });

        modelBuilder.Entity<FacilityMaterial>(entity =>
        {
            entity.ToTable("FacilityMaterial");

            entity.HasIndex(e => new { e.FacilityID, e.MaterialID }, "UIX_FacilityMaterial").IsUnique();

            entity.Property(e => e.FacilityMaterialID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.Facility).WithMany(p => p.FacilityMaterial_Facility)
                .HasForeignKey(d => d.FacilityID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityMaterial_Facility");

           entity.HasOne(d => d.Material).WithMany(p => p.FacilityMaterial_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityMaterial_Material");
        });

        modelBuilder.Entity<FacilityMaterialOEE>(entity =>
        {
            entity.ToTable("FacilityMaterialOEE");

            entity.Property(e => e.FacilityMaterialOEEID).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.FacilityMaterial).WithMany(p => p.FacilityMaterialOEE_FacilityMaterial)
                .HasForeignKey(d => d.FacilityMaterialID)
                .HasConstraintName("FK_FacilityMaterialOEE_FacilityMaterial");
        });

        modelBuilder.Entity<FacilityPreBooking>(entity =>
        {
            entity.ToTable("FacilityPreBooking");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_FacilityPreBooking_InOrderPosID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_FacilityPreBooking_OutOrderPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FK_FacilityPreBooking_ProdOrderPartslistPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosRelationID, "NCI_FK_FacilityPreBooking_ProdOrderPartslistPosRelationID");

            entity.HasIndex(e => e.FacilityPreBookingNo, "UIX_FacilityPreBooking_FacilityPreBookingNo").IsUnique();

            entity.Property(e => e.FacilityPreBookingID).ValueGeneratedNever();
            entity.Property(e => e.ACMethodBookingXML).HasColumnType("text");
            entity.Property(e => e.FacilityPreBookingNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.FacilityPreBooking_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_FacilityPreBooking_InOrderPosID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.FacilityPreBooking_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_FacilityPreBooking_OutOrderPosID");

           entity.HasOne(d => d.PickingPos).WithMany(p => p.FacilityPreBooking_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .HasConstraintName("FK_FacilityPreBooking_PickingPosID");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.FacilityPreBooking_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_FacilityPreBooking_ProdOrderPartslistPosID");

           entity.HasOne(d => d.ProdOrderPartslistPosRelation).WithMany(p => p.FacilityPreBooking_ProdOrderPartslistPosRelation)
                .HasForeignKey(d => d.ProdOrderPartslistPosRelationID)
                .HasConstraintName("FK_FacilityPreBooking_ProdOrderPartslistPosRelation");
        });

        modelBuilder.Entity<FacilityReservation>(entity =>
        {
            entity.ToTable("FacilityReservation");

            entity.HasIndex(e => e.FacilityChargeID, "NCI_FK_FacilityReservation_FacilityChargeID");

            entity.HasIndex(e => e.FacilityID, "NCI_FK_FacilityReservation_FacilityID");

            entity.HasIndex(e => e.FacilityLotID, "NCI_FK_FacilityReservation_FacilityLotID");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_FacilityReservation_InOrderPosID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_FacilityReservation_MaterialID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_FacilityReservation_OutOrderPosID");

            entity.HasIndex(e => e.ParentFacilityReservationID, "NCI_FK_FacilityReservation_ParentFacilityReservationID");

            entity.HasIndex(e => e.ProdOrderBatchPlanID, "NCI_FK_FacilityReservation_ProdOrderBatchPlanID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FK_FacilityReservation_ProdOrderPartslistPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosRelationID, "NCI_FK_FacilityReservation_ProdOrderPartslistPosRelationID");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FK_FacilityReservation_VBiACClassID");

            entity.HasIndex(e => new { e.ProdOrderBatchPlanID, e.FacilityReservationID }, "NCI_FacilityReservation_ProdOrderBatchPlanID_FacilityReservationID_OT");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FacilityReservation_VBiACClassID_OT");

            entity.HasIndex(e => e.FacilityReservationNo, "UIX_FacilityReservation_FacilityReservationNo").IsUnique();

            entity.Property(e => e.FacilityReservationID).ValueGeneratedNever();
            entity.Property(e => e.CalculatedRoute).IsUnicode(false);
            entity.Property(e => e.FacilityReservationNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.FacilityCharge).WithMany(p => p.FacilityReservation_FacilityCharge)
                .HasForeignKey(d => d.FacilityChargeID)
                .HasConstraintName("FK_FacilityReservation_FacilityChargeID");

           entity.HasOne(d => d.Facility).WithMany(p => p.FacilityReservation_Facility)
                .HasForeignKey(d => d.FacilityID)
                .HasConstraintName("FK_FacilityReservation_FacilityID");

           entity.HasOne(d => d.FacilityLot).WithMany(p => p.FacilityReservation_FacilityLot)
                .HasForeignKey(d => d.FacilityLotID)
                .HasConstraintName("FK_FacilityReservation_FacilityLotID");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.FacilityReservation_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_FacilityReservation_InOrderPosID");

           entity.HasOne(d => d.Material).WithMany(p => p.FacilityReservation_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_FacilityReservation_MaterialID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.FacilityReservation_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_FacilityReservation_OutOrderPosID");

           entity.HasOne(d => d.FacilityReservation1_ParentFacilityReservation).WithMany(p => p.FacilityReservation_ParentFacilityReservation)
                .HasForeignKey(d => d.ParentFacilityReservationID)
                .HasConstraintName("FK_FacilityReservation_ParentFacilityReservationID");

           entity.HasOne(d => d.PickingPos).WithMany(p => p.FacilityReservation_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .HasConstraintName("FK_FacilityReservation_PickingPos");

           entity.HasOne(d => d.ProdOrderBatchPlan).WithMany(p => p.FacilityReservation_ProdOrderBatchPlan)
                .HasForeignKey(d => d.ProdOrderBatchPlanID)
                .HasConstraintName("FK_FacilityReservation_ProdOrderBatchPlanID");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.FacilityReservation_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_FacilityReservation_ProdOrderPartslistPosID");

           entity.HasOne(d => d.ProdOrderPartslistPosRelation).WithMany(p => p.FacilityReservation_ProdOrderPartslistPosRelation)
                .HasForeignKey(d => d.ProdOrderPartslistPosRelationID)
                .HasConstraintName("FK_FacilityReservation_ProdOrderPartslistPosRelationID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.FacilityReservation_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_FacilityReservation_ACClassID");
        });

        modelBuilder.Entity<FacilityStock>(entity =>
        {
            entity.ToTable("FacilityStock");

            entity.HasIndex(e => e.FacilityID, "NCI_FK_FacilityStock_FacilityID");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_FacilityStock_MDReleaseStateID");

            entity.Property(e => e.FacilityStockID).ValueGeneratedNever();
            entity.Property(e => e.DayBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MonthBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WeekBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.YearBalanceDate).HasColumnType("datetime");

           entity.HasOne(d => d.Facility).WithMany(p => p.FacilityStock_Facility)
                .HasForeignKey(d => d.FacilityID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FacilityStock_FacilityID");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.FacilityStock_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_FacilityStock_MDReleaseStateID");
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.ToTable("History");

            entity.Property(e => e.HistoryID).ValueGeneratedNever();
            entity.Property(e => e.BalanceDate).HasColumnType("datetime");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<HistoryConfig>(entity =>
        {
            entity.ToTable("HistoryConfig");

            entity.Property(e => e.HistoryConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.History).WithMany(p => p.HistoryConfig_History)
                .HasForeignKey(d => d.HistoryID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_HistoryConfig_HistoryID");

           entity.HasOne(d => d.HistoryConfig1_ParentHistoryConfig).WithMany(p => p.HistoryConfig_ParentHistoryConfig)
                .HasForeignKey(d => d.ParentHistoryConfigID)
                .HasConstraintName("FK_HistoryConfig_ParentHistoryConfigID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.HistoryConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_HistoryConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.HistoryConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_HistoryConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.HistoryConfig_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_HistoryConfig_ACClassWFID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.HistoryConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistoryConfig_DataTypeACClassID");
        });

        modelBuilder.Entity<InOrder>(entity =>
        {
            entity.ToTable("InOrder");

            entity.HasIndex(e => e.BasedOnInRequestID, "NCI_FK_InOrder_BasedOnInRequestID");

            entity.HasIndex(e => e.BillingCompanyAddressID, "NCI_FK_InOrder_BillingCompanyAddressID");

            entity.HasIndex(e => e.CPartnerCompanyID, "NCI_FK_InOrder_CPartnerCompanyID");

            entity.HasIndex(e => e.DeliveryCompanyAddressID, "NCI_FK_InOrder_DeliveryCompanyAddressID");

            entity.HasIndex(e => e.DistributorCompanyID, "NCI_FK_InOrder_DistributorCompanyID");

            entity.HasIndex(e => e.MDDelivTypeID, "NCI_FK_InOrder_MDDelivTypeID");

            entity.HasIndex(e => e.MDInOrderStateID, "NCI_FK_InOrder_MDInOrderStateID");

            entity.HasIndex(e => e.MDInOrderTypeID, "NCI_FK_InOrder_MDInOrderTypeID");

            entity.HasIndex(e => e.MDTermOfPaymentID, "NCI_FK_InOrder_MDTermOfPaymentID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_InOrder_MDTimeRangeID");

            entity.HasIndex(e => e.InOrderNo, "UIX_InOrder").IsUnique();

            entity.Property(e => e.InOrderID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.DistributorOrderNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InOrderDate).HasColumnType("datetime");
            entity.Property(e => e.InOrderNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.BasedOnInRequest).WithMany(p => p.InOrder_BasedOnInRequest)
                .HasForeignKey(d => d.BasedOnInRequestID)
                .HasConstraintName("FK_InOrder_BasedOnInRequestID");

           entity.HasOne(d => d.BillingCompanyAddress).WithMany(p => p.InOrder_BillingCompanyAddress)
                .HasForeignKey(d => d.BillingCompanyAddressID)
                .HasConstraintName("FK_InOrder_BillingCompanyAddressID");

           entity.HasOne(d => d.CPartnerCompany).WithMany(p => p.InOrder_CPartnerCompany)
                .HasForeignKey(d => d.CPartnerCompanyID)
                .HasConstraintName("FK_InOrder_CPartnerCompanyID");

           entity.HasOne(d => d.DeliveryCompanyAddress).WithMany(p => p.InOrder_DeliveryCompanyAddress)
                .HasForeignKey(d => d.DeliveryCompanyAddressID)
                .HasConstraintName("FK_InOrder_DeliveryCompanyAddressID");

           entity.HasOne(d => d.DistributorCompany).WithMany(p => p.InOrder_DistributorCompany)
                .HasForeignKey(d => d.DistributorCompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InOrder_CompanyID");

           entity.HasOne(d => d.IssuerCompanyPerson).WithMany(p => p.InOrder_IssuerCompanyPerson)
                .HasForeignKey(d => d.IssuerCompanyPersonID)
                .HasConstraintName("FK_InOrder_IssuerCompanyPersonID");

           entity.HasOne(d => d.MDCurrency).WithMany(p => p.InOrder_MDCurrency)
                .HasForeignKey(d => d.MDCurrencyID)
                .HasConstraintName("FK_InOrder_MDCurrencyID");

           entity.HasOne(d => d.MDDelivType).WithMany(p => p.InOrder_MDDelivType)
                .HasForeignKey(d => d.MDDelivTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InOrder_MDDelivTypeID");

           entity.HasOne(d => d.MDInOrderState).WithMany(p => p.InOrder_MDInOrderState)
                .HasForeignKey(d => d.MDInOrderStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InOrder_MDInOrderStateID");

           entity.HasOne(d => d.MDInOrderType).WithMany(p => p.InOrder_MDInOrderType)
                .HasForeignKey(d => d.MDInOrderTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InOrder_MDInOrderTypeID");

           entity.HasOne(d => d.MDTermOfPayment).WithMany(p => p.InOrder_MDTermOfPayment)
                .HasForeignKey(d => d.MDTermOfPaymentID)
                .HasConstraintName("FK_InOrder_MDTermOfPaymentID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.InOrder_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_InOrder_MDTimeRangeID");
        });

        modelBuilder.Entity<InOrderConfig>(entity =>
        {
            entity.ToTable("InOrderConfig");

            entity.HasIndex(e => e.InOrderID, "NCI_FK_InOrderConfig_InOrderID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_InOrderConfig_MaterialID");

            entity.HasIndex(e => e.ParentInOrderConfigID, "NCI_FK_InOrderConfig_ParentInOrderConfigID");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FK_InOrderConfig_VBiACClassID");

            entity.HasIndex(e => e.VBiACClassPropertyRelationID, "NCI_FK_InOrderConfig_VBiACClassPropertyRelationID");

            entity.HasIndex(e => e.VBiValueTypeACClassID, "NCI_FK_InOrderConfig_VBiValueTypeACClassID");

            entity.Property(e => e.InOrderConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.InOrder).WithMany(p => p.InOrderConfig_InOrder)
                .HasForeignKey(d => d.InOrderID)
                .HasConstraintName("FK_InOrderConfig_InOrderID");

           entity.HasOne(d => d.Material).WithMany(p => p.InOrderConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_InOrderConfig_MaterialID");

           entity.HasOne(d => d.InOrderConfig1_ParentInOrderConfig).WithMany(p => p.InOrderConfig_ParentInOrderConfig)
                .HasForeignKey(d => d.ParentInOrderConfigID)
                .HasConstraintName("FK_InOrderConfig_ParentInOrderConfigID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.InOrderConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_InOrderConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.InOrderConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_InOrderConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.InOrderConfig_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_InOrderConfig_ACClassWFID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.InOrderConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InOrderConfig_DataTypeACClassID");
        });

        modelBuilder.Entity<InOrderPos>(entity =>
        {
            entity.HasIndex(e => e.InOrderID, "NCI_FK_InOrderPos_InOrderID");

            entity.HasIndex(e => e.MDCountrySalesTaxID, "NCI_FK_InOrderPos_MDCountrySalesTaxID");

            entity.HasIndex(e => e.MDDelivPosLoadStateID, "NCI_FK_InOrderPos_MDDelivPosLoadStateID");

            entity.HasIndex(e => e.MDDelivPosStateID, "NCI_FK_InOrderPos_MDDelivPosStateID");

            entity.HasIndex(e => e.MDInOrderPosStateID, "NCI_FK_InOrderPos_MDInOrderPosStateID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_InOrderPos_MDTimeRangeID");

            entity.HasIndex(e => e.MDTransportModeID, "NCI_FK_InOrderPos_MDTransportModeID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_InOrderPos_MDUnitID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_InOrderPos_MaterialID");

            entity.HasIndex(e => e.ParentInOrderPosID, "NCI_FK_InOrderPos_ParentInOrderPosID");

            entity.HasIndex(e => e.PickupCompanyMaterialID, "NCI_FK_InOrderPos_PickupCompanyMaterialID");

            entity.HasIndex(e => new { e.InOrderPosID, e.MDDelivPosLoadStateID, e.MDDelivPosStateID }, "NCI_InOrderPos_InOrderPosID_MDDelivPosLoadStateID");

            entity.HasIndex(e => new { e.InOrderPosID, e.Sequence }, "UIX_InOrderPos").IsUnique();

            entity.Property(e => e.InOrderPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Comment2).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryDateConfirmed).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.InOrder).WithMany(p => p.InOrderPos_InOrder)
                .HasForeignKey(d => d.InOrderID)
                .HasConstraintName("FK_InOrderPos_InOrderID");

           entity.HasOne(d => d.MDCountrySalesTax).WithMany(p => p.InOrderPos_MDCountrySalesTax)
                .HasForeignKey(d => d.MDCountrySalesTaxID)
                .HasConstraintName("FK_InOrderPos_MDCountrySalesTaxID");

           entity.HasOne(d => d.MDDelivPosLoadState).WithMany(p => p.InOrderPos_MDDelivPosLoadState)
                .HasForeignKey(d => d.MDDelivPosLoadStateID)
                .HasConstraintName("FK_InOrderPos_MDDelivPosLoadStateID");

           entity.HasOne(d => d.MDDelivPosState).WithMany(p => p.InOrderPos_MDDelivPosState)
                .HasForeignKey(d => d.MDDelivPosStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InOrderPos_MDDelivPosStateID");

           entity.HasOne(d => d.MDInOrderPosState).WithMany(p => p.InOrderPos_MDInOrderPosState)
                .HasForeignKey(d => d.MDInOrderPosStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InOrderPos_MDInOrderPosStateID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.InOrderPos_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_InOrderPos_MDTimeRangeID");

           entity.HasOne(d => d.MDTransportMode).WithMany(p => p.InOrderPos_MDTransportMode)
                .HasForeignKey(d => d.MDTransportModeID)
                .HasConstraintName("FK_InOrderPos_MDTransportModeID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.InOrderPos_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_InOrderPos_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.InOrderPos_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InOrderPos_MaterialID");

           entity.HasOne(d => d.InOrderPos1_ParentInOrderPos).WithMany(p => p.InOrderPos_ParentInOrderPos)
                .HasForeignKey(d => d.ParentInOrderPosID)
                .HasConstraintName("FK_InOrderPos_ParentInOrderPosID");

           entity.HasOne(d => d.PickupCompanyMaterial).WithMany(p => p.InOrderPos_PickupCompanyMaterial)
                .HasForeignKey(d => d.PickupCompanyMaterialID)
                .HasConstraintName("FK_InOrderPos_PickupCompanyMaterialID");
        });

        modelBuilder.Entity<InOrderPosSplit>(entity =>
        {
            entity.ToTable("InOrderPosSplit");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_InOrderPosSplit_InOrderPosID");

            entity.Property(e => e.InOrderPosSplitID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.InOrderPosSplit_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_InOrderPosSplit_InOrderPosID");
        });

        modelBuilder.Entity<InRequest>(entity =>
        {
            entity.ToTable("InRequest");

            entity.HasIndex(e => e.BillingCompanyAddressID, "NCI_FK_InRequest_BillingCompanyAddressID");

            entity.HasIndex(e => e.DeliveryCompanyAddressID, "NCI_FK_InRequest_DeliveryCompanyAddressID");

            entity.HasIndex(e => e.DistributorCompanyID, "NCI_FK_InRequest_DistributorCompanyID");

            entity.HasIndex(e => e.MDDelivTypeID, "NCI_FK_InRequest_MDDelivTypeID");

            entity.HasIndex(e => e.MDInOrderTypeID, "NCI_FK_InRequest_MDInOrderTypeID");

            entity.HasIndex(e => e.MDInRequestStateID, "NCI_FK_InRequest_MDInRequestStateID");

            entity.HasIndex(e => e.MDTermOfPaymentID, "NCI_FK_InRequest_MDTermOfPaymentID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_InRequest_MDTimeRangeID");

            entity.HasIndex(e => e.InRequestNo, "UIX_InRequest_InRequestNo").IsUnique();

            entity.Property(e => e.InRequestID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.DistributorOfferNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InRequestDate).HasColumnType("datetime");
            entity.Property(e => e.InRequestNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.BillingCompanyAddress).WithMany(p => p.InRequest_BillingCompanyAddress)
                .HasForeignKey(d => d.BillingCompanyAddressID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InRequest_BillingCompanyAddressID");

           entity.HasOne(d => d.DeliveryCompanyAddress).WithMany(p => p.InRequest_DeliveryCompanyAddress)
                .HasForeignKey(d => d.DeliveryCompanyAddressID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InRequest_DeliveryCompanyAddressID");

           entity.HasOne(d => d.DistributorCompany).WithMany(p => p.InRequest_DistributorCompany)
                .HasForeignKey(d => d.DistributorCompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InRequest_CompanyID");

           entity.HasOne(d => d.MDDelivType).WithMany(p => p.InRequest_MDDelivType)
                .HasForeignKey(d => d.MDDelivTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InRequest_MDDelivTypeID");

           entity.HasOne(d => d.MDInOrderType).WithMany(p => p.InRequest_MDInOrderType)
                .HasForeignKey(d => d.MDInOrderTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InRequest_MDInOrderTypeID");

           entity.HasOne(d => d.MDInRequestState).WithMany(p => p.InRequest_MDInRequestState)
                .HasForeignKey(d => d.MDInRequestStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InRequest_MDInRequestStateID");

           entity.HasOne(d => d.MDTermOfPayment).WithMany(p => p.InRequest_MDTermOfPayment)
                .HasForeignKey(d => d.MDTermOfPaymentID)
                .HasConstraintName("FK_InRequest_MDTermOfPaymentID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.InRequest_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_InRequest_MDTimeRangeID");
        });

        modelBuilder.Entity<InRequestConfig>(entity =>
        {
            entity.ToTable("InRequestConfig");

            entity.HasIndex(e => e.InRequestID, "NCI_FK_InRequestConfig_InRequestID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_InRequestConfig_MaterialID");

            entity.HasIndex(e => e.ParentInRequestConfigID, "NCI_FK_InRequestConfig_ParentInRequestConfigID");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FK_InRequestConfig_VBiACClassID");

            entity.HasIndex(e => e.VBiACClassPropertyRelationID, "NCI_FK_InRequestConfig_VBiACClassPropertyRelationID");

            entity.HasIndex(e => e.VBiValueTypeACClassID, "NCI_FK_InRequestConfig_VBiValueTypeACClassID");

            entity.Property(e => e.InRequestConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.InRequest).WithMany(p => p.InRequestConfig_InRequest)
                .HasForeignKey(d => d.InRequestID)
                .HasConstraintName("FK_InRequestConfig_InRequestID");

           entity.HasOne(d => d.Material).WithMany(p => p.InRequestConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_InRequestConfig_MaterialID");

           entity.HasOne(d => d.InRequestConfig1_ParentInRequestConfig).WithMany(p => p.InRequestConfig_ParentInRequestConfig)
                .HasForeignKey(d => d.ParentInRequestConfigID)
                .HasConstraintName("FK_InRequestConfig_ParentInRequestConfigID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.InRequestConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_InRequestConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.InRequestConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_InRequestConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.InRequestConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InRequestConfig_DataTypeACClassID");
        });

        modelBuilder.Entity<InRequestPos>(entity =>
        {
            entity.HasIndex(e => e.InRequestID, "NCI_FK_InRequestPos_InRequestID");

            entity.HasIndex(e => e.MDCountrySalesTaxID, "NCI_FK_InRequestPos_MDCountrySalesTaxID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_InRequestPos_MDTimeRangeID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_InRequestPos_MDUnitID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_InRequestPos_MaterialID");

            entity.HasIndex(e => e.ParentInRequestPosID, "NCI_FK_InRequestPos_ParentInRequestPosID");

            entity.Property(e => e.InRequestPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Comment2).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.InRequest).WithMany(p => p.InRequestPos_InRequest)
                .HasForeignKey(d => d.InRequestID)
                .HasConstraintName("FK_InRequestPos_InRequestID");

           entity.HasOne(d => d.MDCountrySalesTax).WithMany(p => p.InRequestPos_MDCountrySalesTax)
                .HasForeignKey(d => d.MDCountrySalesTaxID)
                .HasConstraintName("FK_InRequestPos_MDCountrySalesTaxID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.InRequestPos_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_InRequestPos_MDTimeRangeID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.InRequestPos_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_InRequestPos_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.InRequestPos_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InRequestPos_MaterialID");

           entity.HasOne(d => d.InRequestPos1_ParentInRequestPos).WithMany(p => p.InRequestPos_ParentInRequestPos)
                .HasForeignKey(d => d.ParentInRequestPosID)
                .HasConstraintName("FK_InRequestPos_ParentInRequestPosID");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoice");

            entity.HasIndex(e => e.InvoiceNo, "UX_InvoiceNo").IsUnique();

            entity.Property(e => e.InvoiceID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CustRequestNo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesignEnd).HasColumnType("text");
            entity.Property(e => e.XMLDesignStart).HasColumnType("text");

           entity.HasOne(d => d.BillingCompanyAddress).WithMany(p => p.Invoice_BillingCompanyAddress)
                .HasForeignKey(d => d.BillingCompanyAddressID)
                .HasConstraintName("FK_Invoice_CompanyAddressID1");

           entity.HasOne(d => d.CustomerCompany).WithMany(p => p.Invoice_CustomerCompany)
                .HasForeignKey(d => d.CustomerCompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_CompanyID");

           entity.HasOne(d => d.DeliveryCompanyAddress).WithMany(p => p.Invoice_DeliveryCompanyAddress)
                .HasForeignKey(d => d.DeliveryCompanyAddressID)
                .HasConstraintName("FK_Invoice_CompanyAddressID");

           entity.HasOne(d => d.IssuerCompanyAddress).WithMany(p => p.Invoice_IssuerCompanyAddress)
                .HasForeignKey(d => d.IssuerCompanyAddressID)
                .HasConstraintName("FK_Invoice_CompanyAddress");

           entity.HasOne(d => d.IssuerCompanyPerson).WithMany(p => p.Invoice_IssuerCompanyPerson)
                .HasForeignKey(d => d.IssuerCompanyPersonID)
                .HasConstraintName("FK_Invoice_CompanyPerson");

           entity.HasOne(d => d.MDCurrencyExchange).WithMany(p => p.Invoice_MDCurrencyExchange)
                .HasForeignKey(d => d.MDCurrencyExchangeID)
                .HasConstraintName("FK_Invoice_MDCurrencyExchangeID");

           entity.HasOne(d => d.MDCurrency).WithMany(p => p.Invoice_MDCurrency)
                .HasForeignKey(d => d.MDCurrencyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_MDCurrencyID");

           entity.HasOne(d => d.MDInvoiceState).WithMany(p => p.Invoice_MDInvoiceState)
                .HasForeignKey(d => d.MDInvoiceStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_MDInvoiceStateID");

           entity.HasOne(d => d.MDInvoiceType).WithMany(p => p.Invoice_MDInvoiceType)
                .HasForeignKey(d => d.MDInvoiceTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_MDInvoiceTypeID");

           entity.HasOne(d => d.MDTermOfPayment).WithMany(p => p.Invoice_MDTermOfPayment)
                .HasForeignKey(d => d.MDTermOfPaymentID)
                .HasConstraintName("FK_Invoice_MDTermOfPayment");

           entity.HasOne(d => d.OutOrder).WithMany(p => p.Invoice_OutOrder)
                .HasForeignKey(d => d.OutOrderID)
                .HasConstraintName("FK_Invoice_OutOrderID");
        });

        modelBuilder.Entity<InvoicePos>(entity =>
        {
            entity.Property(e => e.InvoicePosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.SalesTax).HasColumnType("money");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesign).HasColumnType("text");

           entity.HasOne(d => d.Invoice).WithMany(p => p.InvoicePos_Invoice)
                .HasForeignKey(d => d.InvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoicePos_InvoiceID");

           entity.HasOne(d => d.MDCountrySalesTax).WithMany(p => p.InvoicePos_MDCountrySalesTax)
                .HasForeignKey(d => d.MDCountrySalesTaxID)
                .HasConstraintName("FK_InvoicePos_MDCountrySalesTaxID");

           entity.HasOne(d => d.MDCountrySalesTaxMDMaterialGroup).WithMany(p => p.InvoicePos_MDCountrySalesTaxMDMaterialGroup)
                .HasForeignKey(d => d.MDCountrySalesTaxMDMaterialGroupID)
                .HasConstraintName("FK_InvoicePos_MDCountrySalesTaxMDMaterialGroupID");

           entity.HasOne(d => d.MDCountrySalesTaxMaterial).WithMany(p => p.InvoicePos_MDCountrySalesTaxMaterial)
                .HasForeignKey(d => d.MDCountrySalesTaxMaterialID)
                .HasConstraintName("FK_InvoicePos_MDCountrySalesTaxMaterialID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.InvoicePos_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_InvoicePos_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.InvoicePos_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InvoicePos_MaterialID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.InvoicePos_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_InvoicePos_OutOrderPosID");
        });

        modelBuilder.Entity<JobTableRecalcActualQuantity>(entity =>
        {
            entity.ToTable("JobTableRecalcActualQuantity");

            entity.Property(e => e.ItemType)
                .IsRequired()
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<LabOrder>(entity =>
        {
            entity.ToTable("LabOrder");

            entity.HasIndex(e => e.BasedOnTemplateID, "NCI_FK_LabOrder_BasedOnTemplateID");

            entity.HasIndex(e => e.FacilityLotID, "NCI_FK_LabOrder_FacilityLotID");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_LabOrder_InOrderPosID");

            entity.HasIndex(e => e.MDLabOrderStateID, "NCI_FK_LabOrder_MDLabOrderStateID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_LabOrder_MaterialID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_LabOrder_OutOrderPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FK_LabOrder_ProdOrderPartslistPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_LabOrder_ProdOrderPartslistPosID_OT");

            entity.HasIndex(e => e.LabOrderNo, "UIX_LabOrder_LabOrderNo").IsUnique();

            entity.Property(e => e.LabOrderID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LabOrderNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SampleTakingDate).HasColumnType("datetime");
            entity.Property(e => e.TemplateName).HasMaxLength(250);
            entity.Property(e => e.TestDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.LabOrder1_BasedOnTemplate).WithMany(p => p.LabOrder_BasedOnTemplate)
                .HasForeignKey(d => d.BasedOnTemplateID)
                .HasConstraintName("FK_LabOrder_LabOrder");

           entity.HasOne(d => d.FacilityLot).WithMany(p => p.LabOrder_FacilityLot)
                .HasForeignKey(d => d.FacilityLotID)
                .HasConstraintName("FK_LabOrder_FacilityLotID");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.LabOrder_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_LabOrder_InOrderPosID");

           entity.HasOne(d => d.MDLabOrderState).WithMany(p => p.LabOrder_MDLabOrderState)
                .HasForeignKey(d => d.MDLabOrderStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabOrder_MDLabOrderStateID");

           entity.HasOne(d => d.Material).WithMany(p => p.LabOrder_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabOrder_MaterialID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.LabOrder_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_LabOrder_OutOrderPosID");

           entity.HasOne(d => d.PickingPos).WithMany(p => p.LabOrder_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .HasConstraintName("FK_LabOrder_PickingPosID");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.LabOrder_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_LabOrder_ProdOrderPartslistPosID");
        });

        modelBuilder.Entity<LabOrderPos>(entity =>
        {
            entity.HasIndex(e => e.LabOrderID, "NCI_FK_LabOrderPos_LabOrderID");

            entity.HasIndex(e => e.MDLabOrderPosStateID, "NCI_FK_LabOrderPos_MDLabOrderPosStateID");

            entity.HasIndex(e => e.MDLabTagID, "NCI_FK_LabOrderPos_MDLabTagID");

            entity.Property(e => e.LabOrderPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.LabOrder).WithMany(p => p.LabOrderPos_LabOrder)
                .HasForeignKey(d => d.LabOrderID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_LabOrderPos_LabOrderID");

           entity.HasOne(d => d.MDLabOrderPosState).WithMany(p => p.LabOrderPos_MDLabOrderPosState)
                .HasForeignKey(d => d.MDLabOrderPosStateID)
                .HasConstraintName("FK_LabOrderPos_MDLabOrderPosStateID");

           entity.HasOne(d => d.MDLabTag).WithMany(p => p.LabOrderPos_MDLabTag)
                .HasForeignKey(d => d.MDLabTagID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabOrderPos_MDLabTag");
        });

        modelBuilder.Entity<Label>(entity =>
        {
            entity.ToTable("Label");

            entity.Property(e => e.LabelID).ValueGeneratedNever();
            entity.Property(e => e.Desc).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(350);
        });

        modelBuilder.Entity<LabelTranslation>(entity =>
        {
            entity.ToTable("LabelTranslation");

            entity.HasIndex(e => new { e.LabelID, e.VBLanguageID }, "VBLanguage_Label_Unique").IsUnique();

            entity.Property(e => e.LabelTranslationID).ValueGeneratedNever();
            entity.Property(e => e.Desc).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(350);

           entity.HasOne(d => d.Label).WithMany(p => p.LabelTranslation_Label)
                .HasForeignKey(d => d.LabelID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabelTranslation_Label");

           entity.HasOne(d => d.VBLanguage).WithMany(p => p.LabelTranslation_VBLanguage)
                .HasForeignKey(d => d.VBLanguageID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabelTranslation_VBLanguage");
        });

        modelBuilder.Entity<MDBalancingMode>(entity =>
        {
            entity.ToTable("MDBalancingMode");

            entity.HasIndex(e => e.MDKey, "UIX_MDBalancingMode").IsUnique();

            entity.Property(e => e.MDBalancingModeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDBatchPlanGroup>(entity =>
        {
            entity.ToTable("MDBatchPlanGroup");

            entity.Property(e => e.MDBatchPlanGroupID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDBookingNotAvailableMode>(entity =>
        {
            entity.ToTable("MDBookingNotAvailableMode");

            entity.HasIndex(e => e.MDKey, "UIX_MDBookingNotAvailableMode").IsUnique();

            entity.Property(e => e.MDBookingNotAvailableModeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDCostCenter>(entity =>
        {
            entity.ToTable("MDCostCenter");

            entity.HasIndex(e => e.MDKey, "UIX_MDCostCenter").IsUnique();

            entity.Property(e => e.MDCostCenterID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDCostCenterNo)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MDCountry>(entity =>
        {
            entity.ToTable("MDCountry");

            entity.HasIndex(e => e.MDCurrencyID, "NCI_FK_MDCountry_MDCurrencyID");

            entity.HasIndex(e => e.MDKey, "UIX_MDCountry").IsUnique();

            entity.Property(e => e.MDCountryID).ValueGeneratedNever();
            entity.Property(e => e.CountryCode)
                .IsRequired()
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDCurrency).WithMany(p => p.MDCountry_MDCurrency)
                .HasForeignKey(d => d.MDCurrencyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDCountry_MDCurrencyID");
        });

        modelBuilder.Entity<MDCountryLand>(entity =>
        {
            entity.ToTable("MDCountryLand");

            entity.HasIndex(e => e.MDCountryID, "NCI_FK_MDCountryLand_MDCountryID");

            entity.HasIndex(e => e.MDKey, "UIX_MDCountryLand").IsUnique();

            entity.Property(e => e.MDCountryLandID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDCountryLandCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDCountry).WithMany(p => p.MDCountryLand_MDCountry)
                .HasForeignKey(d => d.MDCountryID)
                .HasConstraintName("FK_MDCountryLand_MDCountryID");
        });

        modelBuilder.Entity<MDCountrySalesTax>(entity =>
        {
            entity.ToTable("MDCountrySalesTax");

            entity.HasIndex(e => e.MDCountryID, "NCI_FK_MDCountrySalesTax_MDCountryID");

            entity.HasIndex(e => e.MDKey, "UIX_MDCountrySalesTax").IsUnique();

            entity.Property(e => e.MDCountrySalesTaxID).ValueGeneratedNever();
            entity.Property(e => e.DateFrom).HasColumnType("datetime");
            entity.Property(e => e.DateTo).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.SalesTax).HasColumnType("money");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDCountry).WithMany(p => p.MDCountrySalesTax_MDCountry)
                .HasForeignKey(d => d.MDCountryID)
                .HasConstraintName("FK_MDCountrySalesTax_MDCountryID");
        });

        modelBuilder.Entity<MDCountrySalesTaxMDMaterialGroup>(entity =>
        {
            entity.ToTable("MDCountrySalesTaxMDMaterialGroup");

            entity.HasIndex(e => new { e.MDCountrySalesTaxID, e.MDMaterialGroupID }, "UX_MDCountrySalesTax_MDMaterialGroup").IsUnique();

            entity.Property(e => e.MDCountrySalesTaxMDMaterialGroupID).ValueGeneratedNever();
            entity.Property(e => e.SalesTax).HasColumnType("money");

           entity.HasOne(d => d.MDCountrySalesTax).WithMany(p => p.MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax)
                .HasForeignKey(d => d.MDCountrySalesTaxID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDCountrySalesTaxMDMaterialGroup_MDCountrySalesTax");

           entity.HasOne(d => d.MDMaterialGroup).WithMany(p => p.MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup)
                .HasForeignKey(d => d.MDMaterialGroupID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup");
        });

        modelBuilder.Entity<MDCountrySalesTaxMaterial>(entity =>
        {
            entity.ToTable("MDCountrySalesTaxMaterial");

            entity.HasIndex(e => new { e.MaterialID, e.MDCountrySalesTaxID }, "UX_MDCountrySalesTax_Material").IsUnique();

            entity.Property(e => e.MDCountrySalesTaxMaterialID).ValueGeneratedNever();
            entity.Property(e => e.SalesTax).HasColumnType("money");

           entity.HasOne(d => d.MDCountrySalesTax).WithMany(p => p.MDCountrySalesTaxMaterial_MDCountrySalesTax)
                .HasForeignKey(d => d.MDCountrySalesTaxID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDCountrySalesTaxMaterial_MDCountrySalesTax");

           entity.HasOne(d => d.Material).WithMany(p => p.MDCountrySalesTaxMaterial_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDCountrySalesTaxMaterial_Material");
        });

        modelBuilder.Entity<MDCurrency>(entity =>
        {
            entity.ToTable("MDCurrency");

            entity.HasIndex(e => e.MDKey, "UIX_MDCurrency").IsUnique();

            entity.Property(e => e.MDCurrencyID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDCurrencyShortname)
                .IsRequired()
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDCurrencyExchange>(entity =>
        {
            entity.ToTable("MDCurrencyExchange");

            entity.HasIndex(e => e.MDCurrencyID, "NCI_FK_MDCurrencyExchange_MDCurrencyID");

            entity.HasIndex(e => e.ToMDCurrencyID, "NCI_FK_MDCurrencyExchange_ToMDCurrencyID");

            entity.Property(e => e.MDCurrencyExchangeID).ValueGeneratedNever();
            entity.Property(e => e.ExchangeNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDCurrency).WithMany(p => p.MDCurrencyExchange_MDCurrency)
                .HasForeignKey(d => d.MDCurrencyID)
                .HasConstraintName("FK_MDCurrencyExchange_MDCurrencyID");

           entity.HasOne(d => d.ToMDCurrency).WithMany(p => p.MDCurrencyExchange_ToMDCurrency)
                .HasForeignKey(d => d.ToMDCurrencyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDCurrencyExchange_ToMDCurrencyID");
        });

        modelBuilder.Entity<MDDelivNoteState>(entity =>
        {
            entity.ToTable("MDDelivNoteState");

            entity.HasIndex(e => e.MDKey, "UIX_MDDelivNoteState").IsUnique();

            entity.Property(e => e.MDDelivNoteStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDDelivPosLoadState>(entity =>
        {
            entity.ToTable("MDDelivPosLoadState");

            entity.HasIndex(e => e.MDKey, "UIX_MDDelivPosLoadState").IsUnique();

            entity.Property(e => e.MDDelivPosLoadStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDDelivPosState>(entity =>
        {
            entity.ToTable("MDDelivPosState");

            entity.HasIndex(e => e.MDKey, "UIX_MDDelivPosState").IsUnique();

            entity.Property(e => e.MDDelivPosStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDDelivType>(entity =>
        {
            entity.ToTable("MDDelivType");

            entity.HasIndex(e => e.MDKey, "UIX_MDDelivType").IsUnique();

            entity.Property(e => e.MDDelivTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDDemandOrderState>(entity =>
        {
            entity.ToTable("MDDemandOrderState");

            entity.HasIndex(e => e.MDKey, "UIX_MDDemandOrderState").IsUnique();

            entity.Property(e => e.MDDemandOrderStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDFacilityInventoryPosState>(entity =>
        {
            entity.ToTable("MDFacilityInventoryPosState");

            entity.HasIndex(e => e.MDKey, "UIX_MDFacilityInventoryPosState").IsUnique();

            entity.Property(e => e.MDFacilityInventoryPosStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDFacilityInventoryState>(entity =>
        {
            entity.ToTable("MDFacilityInventoryState");

            entity.HasIndex(e => e.MDKey, "UIX_MDFacilityInventoryState").IsUnique();

            entity.Property(e => e.MDFacilityInventoryStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDFacilityManagementType>(entity =>
        {
            entity.ToTable("MDFacilityManagementType");

            entity.HasIndex(e => e.MDKey, "UIX_MDFacilityManagementType").IsUnique();

            entity.Property(e => e.MDFacilityManagementTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDFacilityType>(entity =>
        {
            entity.ToTable("MDFacilityType");

            entity.HasIndex(e => e.MDKey, "UIX_MDFacilityType").IsUnique();

            entity.Property(e => e.MDFacilityTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDFacilityVehicleType>(entity =>
        {
            entity.ToTable("MDFacilityVehicleType");

            entity.HasIndex(e => e.MDKey, "UIX_MDFacilityVehicleType_MDKey").IsUnique();

            entity.Property(e => e.MDFacilityVehicleTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDGMPAdditive>(entity =>
        {
            entity.ToTable("MDGMPAdditive");

            entity.HasIndex(e => e.MDProcessErrorActionID, "NCI_FK_MDGMPAdditive_MDProcessErrorActionID");

            entity.HasIndex(e => e.MDQuantityUnitID, "NCI_FK_MDGMPAdditive_MDQuantityUnitID");

            entity.HasIndex(e => e.MDKey, "UIX_MDGMPAdditive").IsUnique();

            entity.Property(e => e.MDGMPAdditiveID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDGMPAdditiveNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDProcessErrorAction).WithMany(p => p.MDGMPAdditive_MDProcessErrorAction)
                .HasForeignKey(d => d.MDProcessErrorActionID)
                .HasConstraintName("FK_MDGMPAdditive_MDProcessErrorActionID");

           entity.HasOne(d => d.MDQuantityUnit).WithMany(p => p.MDGMPAdditive_MDQuantityUnit)
                .HasForeignKey(d => d.MDQuantityUnitID)
                .HasConstraintName("FK_MDGMPAdditive_MDQuantityUnitID");
        });

        modelBuilder.Entity<MDGMPMaterialGroup>(entity =>
        {
            entity.ToTable("MDGMPMaterialGroup");

            entity.HasIndex(e => e.MDKey, "UIX_MDGMPMaterialGroup").IsUnique();

            entity.HasIndex(e => e.MDGMPMaterialGroupNo, "UIX_MDGMPMaterialGroup_MDGMPMaterialGroupNo").IsUnique();

            entity.Property(e => e.MDGMPMaterialGroupID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDGMPMaterialGroupNo)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WearFacilityNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDGMPMaterialGroupPos>(entity =>
        {
            entity.HasIndex(e => e.MDGMPAdditiveID, "NCI_FK_MDGMPMaterialGroupPos_MDGMPAdditiveID");

            entity.HasIndex(e => new { e.MDGMPMaterialGroupPosID, e.Sequence }, "UIX_MDGMPMaterialGroupPos").IsUnique();

            entity.Property(e => e.MDGMPMaterialGroupPosID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDGMPAdditive).WithMany(p => p.MDGMPMaterialGroupPos_MDGMPAdditive)
                .HasForeignKey(d => d.MDGMPAdditiveID)
                .HasConstraintName("FK_MDGMPMaterialGroupPos_MDGMPAdditiveID");

           entity.HasOne(d => d.MDGMPMaterialGroup).WithMany(p => p.MDGMPMaterialGroupPos_MDGMPMaterialGroup)
                .HasForeignKey(d => d.MDGMPMaterialGroupID)
                .HasConstraintName("FK_MDGMPMaterialGroupPos_MDGMPMaterialGroupID");
        });

        modelBuilder.Entity<MDInOrderPosState>(entity =>
        {
            entity.ToTable("MDInOrderPosState");

            entity.HasIndex(e => e.MDKey, "UIX_MDInOrderPosState").IsUnique();

            entity.Property(e => e.MDInOrderPosStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDInOrderState>(entity =>
        {
            entity.ToTable("MDInOrderState");

            entity.HasIndex(e => e.MDKey, "UIX_MDInOrderState").IsUnique();

            entity.Property(e => e.MDInOrderStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDInOrderType>(entity =>
        {
            entity.ToTable("MDInOrderType");

            entity.HasIndex(e => e.MDKey, "UIX_MDInOrderType").IsUnique();

            entity.Property(e => e.MDInOrderTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDInRequestState>(entity =>
        {
            entity.ToTable("MDInRequestState");

            entity.HasIndex(e => e.MDKey, "UIX_MDInRequestState").IsUnique();

            entity.Property(e => e.MDInRequestStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDInventoryManagementType>(entity =>
        {
            entity.ToTable("MDInventoryManagementType");

            entity.HasIndex(e => e.MDKey, "UIX_MDInventoryManagementType").IsUnique();

            entity.Property(e => e.MDInventoryManagementTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDInvoiceState>(entity =>
        {
            entity.ToTable("MDInvoiceState");

            entity.HasIndex(e => e.MDKey, "UX_MDInvoiceState_MDKey").IsUnique();

            entity.Property(e => e.MDInvoiceStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDInvoiceType>(entity =>
        {
            entity.ToTable("MDInvoiceType");

            entity.HasIndex(e => e.MDKey, "UX_MDInvoiceType_MDKey").IsUnique();

            entity.Property(e => e.MDInvoiceTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDLabOrderPosState>(entity =>
        {
            entity.ToTable("MDLabOrderPosState");

            entity.HasIndex(e => e.MDKey, "UIX_MDLabOrderPosState").IsUnique();

            entity.Property(e => e.MDLabOrderPosStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDLabOrderState>(entity =>
        {
            entity.ToTable("MDLabOrderState");

            entity.HasIndex(e => e.MDKey, "UIX_MDLabOrderState").IsUnique();

            entity.Property(e => e.MDLabOrderStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDLabTag>(entity =>
        {
            entity.ToTable("MDLabTag");

            entity.HasIndex(e => e.MDKey, "UIX_MDLabTag").IsUnique();

            entity.Property(e => e.MDLabTagID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDMaintMode>(entity =>
        {
            entity.ToTable("MDMaintMode");

            entity.HasIndex(e => e.MDKey, "UIX_MDMaintMode").IsUnique();

            entity.Property(e => e.MDMaintModeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDMaintOrderPropertyState>(entity =>
        {
            entity.HasKey(e => e.MDMaintOrderPropertyStateID).HasName("PK_MDMaintOrderProperty");

            entity.ToTable("MDMaintOrderPropertyState");

            entity.HasIndex(e => e.MDKey, "UIX_MDMaintOrderPropertyState").IsUnique();

            entity.Property(e => e.MDMaintOrderPropertyStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDMaintOrderState>(entity =>
        {
            entity.ToTable("MDMaintOrderState");

            entity.HasIndex(e => e.MDKey, "UIX_MDMaintOrderState").IsUnique();

            entity.Property(e => e.MDMaintOrderStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDMaintTaskState>(entity =>
        {
            entity.ToTable("MDMaintTaskState");

            entity.Property(e => e.MDMaintTaskStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MDMaterialGroup>(entity =>
        {
            entity.ToTable("MDMaterialGroup");

            entity.HasIndex(e => e.MDKey, "UIX_MDMaterialGroup").IsUnique();

            entity.Property(e => e.MDMaterialGroupID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDMaterialType>(entity =>
        {
            entity.ToTable("MDMaterialType");

            entity.HasIndex(e => e.MDKey, "UIX_MDMaterialType").IsUnique();

            entity.Property(e => e.MDMaterialTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDMovementReason>(entity =>
        {
            entity.ToTable("MDMovementReason");

            entity.HasIndex(e => e.MDKey, "UIX_MDMovementReason").IsUnique();

            entity.Property(e => e.MDMovementReasonID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDOutOfferState>(entity =>
        {
            entity.ToTable("MDOutOfferState");

            entity.HasIndex(e => e.MDKey, "UIX_MDOutOfferingState").IsUnique();

            entity.Property(e => e.MDOutOfferStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDOutOrderPlanState>(entity =>
        {
            entity.ToTable("MDOutOrderPlanState");

            entity.HasIndex(e => e.MDKey, "UIX_MDOutOrderPlanState").IsUnique();

            entity.Property(e => e.MDOutOrderPlanStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDOutOrderPosState>(entity =>
        {
            entity.ToTable("MDOutOrderPosState");

            entity.HasIndex(e => e.MDKey, "UIX_MDOutOrderPosState").IsUnique();

            entity.Property(e => e.MDOutOrderPosStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDOutOrderState>(entity =>
        {
            entity.ToTable("MDOutOrderState");

            entity.HasIndex(e => e.MDKey, "UIX_MDOutOrderState").IsUnique();

            entity.Property(e => e.MDOutOrderStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDOutOrderType>(entity =>
        {
            entity.ToTable("MDOutOrderType");

            entity.HasIndex(e => e.MDKey, "UIX_MDOutOrderType").IsUnique();

            entity.Property(e => e.MDOutOrderTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDPickingType>(entity =>
        {
            entity.ToTable("MDPickingType");

            entity.Property(e => e.MDPickingTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDProcessErrorAction>(entity =>
        {
            entity.ToTable("MDProcessErrorAction");

            entity.HasIndex(e => e.MDKey, "UIX_MDProcessErrorAction").IsUnique();

            entity.Property(e => e.MDProcessErrorActionID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDProdOrderPartslistPosState>(entity =>
        {
            entity.ToTable("MDProdOrderPartslistPosState");

            entity.HasIndex(e => e.MDKey, "UIX_MDProdOrderPartslistPosState_MDKey").IsUnique();

            entity.Property(e => e.MDProdOrderPartslistPosStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDProdOrderState>(entity =>
        {
            entity.ToTable("MDProdOrderState");

            entity.HasIndex(e => e.MDKey, "UIX_MDProdOrderState").IsUnique();

            entity.Property(e => e.MDProdOrderStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDRatingComplaintType>(entity =>
        {
            entity.ToTable("MDRatingComplaintType");

            entity.HasIndex(e => e.MDKey, "IX_MDRatingComplaintType").IsUnique();

            entity.Property(e => e.MDRatingComplaintTypeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans).HasMaxLength(355);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDReleaseState>(entity =>
        {
            entity.ToTable("MDReleaseState");

            entity.HasIndex(e => e.MDKey, "UIX_MDReleaseState").IsUnique();

            entity.Property(e => e.MDReleaseStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDReservationMode>(entity =>
        {
            entity.ToTable("MDReservationMode");

            entity.HasIndex(e => e.MDKey, "UIX_MDReservationMode").IsUnique();

            entity.Property(e => e.MDReservationModeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDSchedulingGroup>(entity =>
        {
            entity.ToTable("MDSchedulingGroup");

            entity.HasIndex(e => e.MDKey, "UIX_MDSchedulingGroup").IsUnique();

            entity.Property(e => e.MDSchedulingGroupID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MDSchedulingGroupWF>(entity =>
        {
            entity.ToTable("MDSchedulingGroupWF");

            entity.HasIndex(e => new { e.MDSchedulingGroupID, e.VBiACClassWFID }, "UX_SchedulingGroup_Wf").IsUnique();

            entity.Property(e => e.MDSchedulingGroupWFID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MDSchedulingGroup).WithMany(p => p.MDSchedulingGroupWF_MDSchedulingGroup)
                .HasForeignKey(d => d.MDSchedulingGroupID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDSchedulingGroupWF_MDSchedulingGroup");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.MDSchedulingGroupWF_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDSchedulingGroupWF_VBiACClassWF");
        });

        modelBuilder.Entity<MDTermOfPayment>(entity =>
        {
            entity.ToTable("MDTermOfPayment");

            entity.HasIndex(e => e.MDKey, "UIX_MDTermOfPayment").IsUnique();

            entity.Property(e => e.MDTermOfPaymentID).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDTimeRange>(entity =>
        {
            entity.ToTable("MDTimeRange");

            entity.HasIndex(e => e.ParentMDTimeRangeID, "NCI_FK_MDTimeRange_ParentMDTimeRangeID");

            entity.HasIndex(e => e.MDKey, "UIX_MDTimeRange").IsUnique();

            entity.Property(e => e.MDTimeRangeID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDTimeRange1_ParentMDTimeRange).WithMany(p => p.MDTimeRange_ParentMDTimeRange)
                .HasForeignKey(d => d.ParentMDTimeRangeID)
                .HasConstraintName("FK_MDShift_ParentMDTimeRangeID");
        });

        modelBuilder.Entity<MDToleranceState>(entity =>
        {
            entity.ToTable("MDToleranceState");

            entity.HasIndex(e => e.MDKey, "UIX_MDToleranceState").IsUnique();

            entity.Property(e => e.MDToleranceStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDTour>(entity =>
        {
            entity.ToTable("MDTour");

            entity.HasIndex(e => e.MDKey, "UIX_MDTour").IsUnique();

            entity.Property(e => e.MDTourID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.MDTourNo)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDTourplanPosState>(entity =>
        {
            entity.ToTable("MDTourplanPosState");

            entity.HasIndex(e => e.MDKey, "UIX_MDTourplanPosState").IsUnique();

            entity.Property(e => e.MDTourplanPosStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDTourplanState>(entity =>
        {
            entity.ToTable("MDTourplanState");

            entity.HasIndex(e => e.MDKey, "UIX_MDTourplanState").IsUnique();

            entity.Property(e => e.MDTourplanStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDTransportMode>(entity =>
        {
            entity.ToTable("MDTransportMode");

            entity.HasIndex(e => e.MDKey, "UIX_MDTransportMode_MDKey").IsUnique();

            entity.Property(e => e.MDTransportModeID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDUnit>(entity =>
        {
            entity.ToTable("MDUnit");

            entity.HasIndex(e => e.ISOCode, "UIX_ISOCode").IsUnique();

            entity.Property(e => e.MDUnitID).ValueGeneratedNever();
            entity.Property(e => e.ISOCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDUnitNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.SymbolTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.TechnicalSymbol)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDUnitConversion>(entity =>
        {
            entity.ToTable("MDUnitConversion");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_MDUnitConversion_MDUnitID");

            entity.HasIndex(e => e.ToMDUnitID, "NCI_FK_MDUnitConversion_ToMDUnitID");

            entity.Property(e => e.MDUnitConversionID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MDUnit).WithMany(p => p.MDUnitConversion_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_MDUnitConversion_MDUnitID");

           entity.HasOne(d => d.ToMDUnit).WithMany(p => p.MDUnitConversion_ToMDUnit)
                .HasForeignKey(d => d.ToMDUnitID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDUnitConversion_ToMDUnitID");
        });

        modelBuilder.Entity<MDVisitorCard>(entity =>
        {
            entity.ToTable("MDVisitorCard");

            entity.HasIndex(e => e.MDVisitorCardStateID, "NCI_FK_MDVisitorCard_MDVisitorCardStateID");

            entity.HasIndex(e => e.MDVisitorCardNo, "UIX_MDVisitorCard").IsUnique();

            entity.Property(e => e.MDVisitorCardID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDVisitorCardKey)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MDVisitorCardNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDVisitorCardState).WithMany(p => p.MDVisitorCard_MDVisitorCardState)
                .HasForeignKey(d => d.MDVisitorCardStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MDVisitorCard_MDVisitorCardStateID");
        });

        modelBuilder.Entity<MDVisitorCardState>(entity =>
        {
            entity.ToTable("MDVisitorCardState");

            entity.HasIndex(e => e.MDKey, "UIX_MDVisitorCardState").IsUnique();

            entity.Property(e => e.MDVisitorCardStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDVisitorVoucherState>(entity =>
        {
            entity.ToTable("MDVisitorVoucherState");

            entity.HasIndex(e => e.MDKey, "UIX_MDVisitorVoucherState").IsUnique();

            entity.Property(e => e.MDVisitorVoucherStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MDZeroStockState>(entity =>
        {
            entity.ToTable("MDZeroStockState");

            entity.HasIndex(e => e.MDKey, "UIX_MDZeroStockState").IsUnique();

            entity.Property(e => e.MDZeroStockStateID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MDKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MDNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<MachineMaterialPosView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("MachineMaterialPosView");

            entity.Property(e => e.BasedOnMachineName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MachineName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaterialName1)
                .IsRequired()
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.MaterialNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MachineMaterialRelView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("MachineMaterialRelView");

            entity.Property(e => e.BasedOnMachineName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MachineName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaterialName1)
                .IsRequired()
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.MaterialNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MachineMaterialView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("MachineMaterialView");

            entity.Property(e => e.BasedOnMachineName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MachineName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaterialName1)
                .IsRequired()
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.MaterialNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MaintACClass>(entity =>
        {
            entity.ToTable("MaintACClass");

            entity.Property(e => e.MaintACClassID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.MaintACClass_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_MaintACClass_ACClassID");
        });

        modelBuilder.Entity<MaintACClassProperty>(entity =>
        {
            entity.ToTable("MaintACClassProperty");

            entity.Property(e => e.MaintACClassPropertyID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaxValue)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WarningValueDiff).IsUnicode(false);

           entity.HasOne(d => d.MaintACClass).WithMany(p => p.MaintACClassProperty_MaintACClass)
                .HasForeignKey(d => d.MaintACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintACClassProperty_MaintACClassID");

           entity.HasOne(d => d.VBiACClassProperty).WithMany(p => p.MaintACClassProperty_VBiACClassProperty)
                .HasForeignKey(d => d.VBiACClassPropertyID)
                .HasConstraintName("FK_MaintACClassProperty_ACClassPropertyID");
        });

        modelBuilder.Entity<MaintOrder>(entity =>
        {
            entity.ToTable("MaintOrder");

            entity.Property(e => e.MaintOrderID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastMaintTerm).HasColumnType("datetime");
            entity.Property(e => e.MaintOrderNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PlannedStartDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MaintOrder1_BasedOnMaintOrder).WithMany(p => p.MaintOrder_BasedOnMaintOrder)
                .HasForeignKey(d => d.BasedOnMaintOrderID)
                .HasConstraintName("FK_MaintOrder_BasedOnMaintOrderID");

           entity.HasOne(d => d.Facility).WithMany(p => p.MaintOrder_Facility)
                .HasForeignKey(d => d.FacilityID)
                .HasConstraintName("FK_MaintOrder_FacilityID");

           entity.HasOne(d => d.MDMaintOrderState).WithMany(p => p.MaintOrder_MDMaintOrderState)
                .HasForeignKey(d => d.MDMaintOrderStateID)
                .HasConstraintName("FK_MaintOrder_MDMaintOrderStateID");

           entity.HasOne(d => d.MaintACClass).WithMany(p => p.MaintOrder_MaintACClass)
                .HasForeignKey(d => d.MaintACClassID)
                .HasConstraintName("FK_MaintOrder_MaintACClassID");

           entity.HasOne(d => d.Picking).WithMany(p => p.MaintOrder_Picking)
                .HasForeignKey(d => d.PickingID)
                .HasConstraintName("FK_MaintOrder_PickingID");

           entity.HasOne(d => d.VBiPAACClass).WithMany(p => p.MaintOrder_VBiPAACClass)
                .HasForeignKey(d => d.VBiPAACClassID)
                .HasConstraintName("FK_MaintOrder_PAACClassID");
        });

        modelBuilder.Entity<MaintOrderAssignment>(entity =>
        {
            entity.ToTable("MaintOrderAssignment");

            entity.Property(e => e.MaintOrderAssignmentID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.Company).WithMany(p => p.MaintOrderAssignment_Company)
                .HasForeignKey(d => d.CompanyID)
                .HasConstraintName("FK_MaintOrderAssignment_Company");

           entity.HasOne(d => d.MaintOrder).WithMany(p => p.MaintOrderAssignment_MaintOrder)
                .HasForeignKey(d => d.MaintOrderID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintOrderAssignment_MaintOrder");

           entity.HasOne(d => d.VBGroup).WithMany(p => p.MaintOrderAssignment_VBGroup)
                .HasForeignKey(d => d.VBGroupID)
                .HasConstraintName("FK_MaintOrderAssignment_VBGroup");

           entity.HasOne(d => d.VBUser).WithMany(p => p.MaintOrderAssignment_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .HasConstraintName("FK_MaintOrderAssignment_VBUserID");
        });

        modelBuilder.Entity<MaintOrderPos>(entity =>
        {
            entity.Property(e => e.MaintOrderPosID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MaintOrder).WithMany(p => p.MaintOrderPos_MaintOrder)
                .HasForeignKey(d => d.MaintOrderID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintOrderPos_MaintOrderID");

           entity.HasOne(d => d.Material).WithMany(p => p.MaintOrderPos_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintOrderPos_ParentMaintOrderPosID");
        });

        modelBuilder.Entity<MaintOrderProperty>(entity =>
        {
            entity.ToTable("MaintOrderProperty");

            entity.Property(e => e.MaintOrderPropertyID).ValueGeneratedNever();
            entity.Property(e => e.ActValue)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SetValue)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MaintACClassProperty).WithMany(p => p.MaintOrderProperty_MaintACClassProperty)
                .HasForeignKey(d => d.MaintACClassPropertyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintOrderProperty_MaintACClassPropertyID");

           entity.HasOne(d => d.MaintOrder).WithMany(p => p.MaintOrderProperty_MaintOrder)
                .HasForeignKey(d => d.MaintOrderID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintOrderProperty_MaintOrderID");
        });

        modelBuilder.Entity<MaintOrderTask>(entity =>
        {
            entity.ToTable("MaintOrderTask");

            entity.Property(e => e.MaintOrderTaskID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PlannedStartDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TaskDescription).IsUnicode(false);
            entity.Property(e => e.TaskName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MDMaintTaskState).WithMany(p => p.MaintOrderTask_MDMaintTaskState)
                .HasForeignKey(d => d.MDMaintTaskStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintOrderTask_MDMaintTaskStateID");

           entity.HasOne(d => d.MaintOrder).WithMany(p => p.MaintOrderTask_MaintOrder)
                .HasForeignKey(d => d.MaintOrderID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintOrderTask_MaintOrderID");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("Material");

            entity.HasIndex(e => e.BaseMDUnitID, "NCI_FK_Material_BaseMDUnitID");

            entity.HasIndex(e => e.InFacilityID, "NCI_FK_Material_InFacilityID");

            entity.HasIndex(e => e.LabelID, "NCI_FK_Material_LabelID");

            entity.HasIndex(e => e.MDFacilityManagementTypeID, "NCI_FK_Material_MDFacilityManagementTypeID");

            entity.HasIndex(e => e.MDGMPMaterialGroupID, "NCI_FK_Material_MDGMPMaterialGroupID");

            entity.HasIndex(e => e.MDInventoryManagementTypeID, "NCI_FK_Material_MDInventoryManagementTypeID");

            entity.HasIndex(e => e.MDMaterialGroupID, "NCI_FK_Material_MDMaterialGroupID");

            entity.HasIndex(e => e.MDMaterialTypeID, "NCI_FK_Material_MDMaterialTypeID");

            entity.HasIndex(e => e.OutFacilityID, "NCI_FK_Material_OutFacilityID");

            entity.HasIndex(e => e.ProductionMaterialID, "NCI_FK_Material_ProductionMaterialID");

            entity.HasIndex(e => e.VBiProgramACClassMethodID, "NCI_FK_Material_VBiProgramACClassMethodID");

            entity.HasIndex(e => e.VBiStackCalculatorACClassID, "NCI_FK_Material_VBiStackCalculatorACClassID");

            entity.HasIndex(e => new { e.MaterialID, e.MDMaterialTypeID }, "NCI_LabOrderPos_MaterialID_MDMaterialTypeID");

            entity.HasIndex(e => new { e.MaterialID, e.MDMaterialTypeID }, "NCI_LabOrderPos_MaterialID_MDMaterialTypeID_OT");

            entity.HasIndex(e => e.MaterialNo, "UIX_Material").IsUnique();

            entity.Property(e => e.MaterialID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.MaterialName1)
                .IsRequired()
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.MaterialName2)
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.MaterialName3)
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.MaterialNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.BaseMDUnit).WithMany(p => p.Material_BaseMDUnit)
                .HasForeignKey(d => d.BaseMDUnitID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_BaseMDUnitID");

           entity.HasOne(d => d.InFacility).WithMany(p => p.Material_InFacility)
                .HasForeignKey(d => d.InFacilityID)
                .HasConstraintName("FK_Material_InFacilityID");

           entity.HasOne(d => d.Label).WithMany(p => p.Material_Label)
                .HasForeignKey(d => d.LabelID)
                .HasConstraintName("FK_Material_Label");

           entity.HasOne(d => d.MDFacilityManagementType).WithMany(p => p.Material_MDFacilityManagementType)
                .HasForeignKey(d => d.MDFacilityManagementTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_MDFacilityManagementTypeID");

           entity.HasOne(d => d.MDGMPMaterialGroup).WithMany(p => p.Material_MDGMPMaterialGroup)
                .HasForeignKey(d => d.MDGMPMaterialGroupID)
                .HasConstraintName("FK_Material_MDGMPMaterialGroupID");

           entity.HasOne(d => d.MDInventoryManagementType).WithMany(p => p.Material_MDInventoryManagementType)
                .HasForeignKey(d => d.MDInventoryManagementTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_MDInventoryManagementTypeID");

           entity.HasOne(d => d.MDMaterialGroup).WithMany(p => p.Material_MDMaterialGroup)
                .HasForeignKey(d => d.MDMaterialGroupID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Material_MDMaterialGroupID");

           entity.HasOne(d => d.MDMaterialType).WithMany(p => p.Material_MDMaterialType)
                .HasForeignKey(d => d.MDMaterialTypeID)
                .HasConstraintName("FK_Material_MDMaterialTypeID");

           entity.HasOne(d => d.OutFacility).WithMany(p => p.Material_OutFacility)
                .HasForeignKey(d => d.OutFacilityID)
                .HasConstraintName("FK_Material_OutFacilityID");

           entity.HasOne(d => d.Material1_ProductionMaterial).WithMany(p => p.Material_ProductionMaterial)
                .HasForeignKey(d => d.ProductionMaterialID)
                .HasConstraintName("FK_Material_ProductionMaterialID");

           entity.HasOne(d => d.VBiProgramACClassMethod).WithMany(p => p.Material_VBiProgramACClassMethod)
                .HasForeignKey(d => d.VBiProgramACClassMethodID)
                .HasConstraintName("FK_Material_ProgramACClassMethodID");

           entity.HasOne(d => d.VBiStackCalculatorACClass).WithMany(p => p.Material_VBiStackCalculatorACClass)
                .HasForeignKey(d => d.VBiStackCalculatorACClassID)
                .HasConstraintName("FK_Material_StackCalculatorACClassID");
        });

        modelBuilder.Entity<MaterialCalculation>(entity =>
        {
            entity.ToTable("MaterialCalculation");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_MaterialCalculation_MaterialID");

            entity.HasIndex(e => e.MaterialCalculationNo, "UIX_MaterialCalculation_MaterialCalculationNo").IsUnique();

            entity.Property(e => e.MaterialCalculationID).ValueGeneratedNever();
            entity.Property(e => e.CalculationDate).HasColumnType("datetime");
            entity.Property(e => e.CostFix).HasColumnType("money");
            entity.Property(e => e.CostGeneral).HasColumnType("money");
            entity.Property(e => e.CostHandlingFix).HasColumnType("money");
            entity.Property(e => e.CostHandlingVar).HasColumnType("money");
            entity.Property(e => e.CostLoss).HasColumnType("money");
            entity.Property(e => e.CostMat).HasColumnType("money");
            entity.Property(e => e.CostPack).HasColumnType("money");
            entity.Property(e => e.CostReQuantity).HasColumnType("money");
            entity.Property(e => e.CostVar).HasColumnType("money");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaterialCalculationNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ValidFromDate).HasColumnType("datetime");
            entity.Property(e => e.ValidToDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.MaterialCalculation_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_MaterialCalculation_MaterialID");
        });

        modelBuilder.Entity<MaterialConfig>(entity =>
        {
            entity.ToTable("MaterialConfig");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_MaterialConfig_MaterialID");

            entity.HasIndex(e => e.ParentMaterialConfigID, "NCI_FK_MaterialConfig_ParentMaterialConfigID");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FK_MaterialConfig_VBiACClassID");

            entity.HasIndex(e => e.VBiACClassPropertyRelationID, "NCI_FK_MaterialConfig_VBiACClassPropertyRelationID");

            entity.HasIndex(e => e.VBiValueTypeACClassID, "NCI_FK_MaterialConfig_VBiValueTypeACClassID");

            entity.Property(e => e.MaterialConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.MaterialConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MaterialConfig_MaterialID");

           entity.HasOne(d => d.MaterialConfig1_ParentMaterialConfig).WithMany(p => p.MaterialConfig_ParentMaterialConfig)
                .HasForeignKey(d => d.ParentMaterialConfigID)
                .HasConstraintName("FK_MaterialConfig_ParentMaterialConfigID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.MaterialConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_MaterialConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.MaterialConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_MaterialConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.MaterialConfig_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MaterialConfig_ACClassWFID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.MaterialConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialConfig_DataTypeACClassID");
        });

        modelBuilder.Entity<MaterialGMPAdditive>(entity =>
        {
            entity.ToTable("MaterialGMPAdditive");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_MaterialGMPAdditive_MaterialID");

            entity.HasIndex(e => new { e.MaterialGMPAdditiveID, e.Sequence }, "UIX_MaterialGMPAdditive").IsUnique();

            entity.Property(e => e.MaterialGMPAdditiveID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MDGMPAdditive).WithMany(p => p.MaterialGMPAdditive_MDGMPAdditive)
                .HasForeignKey(d => d.MDGMPAdditiveID)
                .HasConstraintName("FK_MaterialGMPAdditive_MDGMPAdditiveID");

           entity.HasOne(d => d.Material).WithMany(p => p.MaterialGMPAdditive_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_MaterialGMPAdditive_MaterialID");
        });

        modelBuilder.Entity<MaterialHistory>(entity =>
        {
            entity.ToTable("MaterialHistory");

            entity.HasIndex(e => e.HistoryID, "NCI_FK_MaterialHistory_HistoryID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_MaterialHistory_MaterialID");

            entity.Property(e => e.MaterialHistoryID).ValueGeneratedNever();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.History).WithMany(p => p.MaterialHistory_History)
                .HasForeignKey(d => d.HistoryID)
                .HasConstraintName("FK_MaterialHistory_HistoryID");

           entity.HasOne(d => d.Material).WithMany(p => p.MaterialHistory_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_MaterialHistory_MaterialID");
        });

        modelBuilder.Entity<MaterialStock>(entity =>
        {
            entity.ToTable("MaterialStock");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_MaterialStock_MDReleaseStateID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_MaterialStock_MaterialID");

            entity.Property(e => e.MaterialStockID).ValueGeneratedNever();
            entity.Property(e => e.DayBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MonthBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WeekBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.YearBalanceDate).HasColumnType("datetime");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.MaterialStock_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_MaterialStock_MDReleaseStateID");

           entity.HasOne(d => d.Material).WithMany(p => p.MaterialStock_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_MaterialStock_MaterialID");
        });

        modelBuilder.Entity<MaterialUnit>(entity =>
        {
            entity.ToTable("MaterialUnit");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_MaterialUnit_MaterialID");

            entity.HasIndex(e => e.ToMDUnitID, "NCI_FK_MaterialUnit_ToMDUnitID");

            entity.Property(e => e.MaterialUnitID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.MaterialUnit_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_MaterialUnit_MaterialID");

           entity.HasOne(d => d.ToMDUnit).WithMany(p => p.MaterialUnit_ToMDUnit)
                .HasForeignKey(d => d.ToMDUnitID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialUnit_ToMDUnitID");
        });

        modelBuilder.Entity<MaterialWF>(entity =>
        {
            entity.ToTable("MaterialWF");

            entity.HasIndex(e => e.MaterialWFNo, "UIX_MaterialWF_MaterialWFNo").IsUnique();

            entity.Property(e => e.MaterialWFID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaterialWFNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesign).HasColumnType("text");
        });

        modelBuilder.Entity<MaterialWFACClassMethod>(entity =>
        {
            entity.ToTable("MaterialWFACClassMethod");

            entity.HasIndex(e => e.ACClassMethodID, "NCI_FK_MaterialWFACClassMethod_ACClassMethodID");

            entity.HasIndex(e => e.MaterialWFID, "NCI_FK_MaterialWFACClassMethod_MaterialWFID");

            entity.Property(e => e.MaterialWFACClassMethodID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.ACClassMethod).WithMany(p => p.MaterialWFACClassMethod_ACClassMethod)
                .HasForeignKey(d => d.ACClassMethodID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFACClassMethod_ACClassMethod");

           entity.HasOne(d => d.MaterialWF).WithMany(p => p.MaterialWFACClassMethod_MaterialWF)
                .HasForeignKey(d => d.MaterialWFID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFACClassMethod_MaterialWF");
        });

        modelBuilder.Entity<MaterialWFACClassMethodConfig>(entity =>
        {
            entity.ToTable("MaterialWFACClassMethodConfig");

            entity.Property(e => e.MaterialWFACClassMethodConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.MaterialWFACClassMethod).WithMany(p => p.MaterialWFACClassMethodConfig_MaterialWFACClassMethod)
                .HasForeignKey(d => d.MaterialWFACClassMethodID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MaterialWFACClassMethodConfig_MaterialWFACClassMethodID");

           entity.HasOne(d => d.MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfig).WithMany(p => p.MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfig)
                .HasForeignKey(d => d.ParentMaterialWFACClassMethodConfigID)
                .HasConstraintName("FK_MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfigID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.MaterialWFACClassMethodConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_MaterialWFACClassMethodConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.MaterialWFACClassMethodConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_MaterialWFACClassMethodConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.MaterialWFACClassMethodConfig_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MaterialWFACClassMethodConfig_ACClassWFID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.MaterialWFACClassMethodConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFACClassMethodConfig_DataTypeACClassID");
        });

        modelBuilder.Entity<MaterialWFConnection>(entity =>
        {
            entity.ToTable("MaterialWFConnection");

            entity.HasIndex(e => e.ACClassWFID, "NCI_FK_MaterialWFConnection_ACClassWFID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_MaterialWFConnection_MaterialID");

            entity.HasIndex(e => e.MaterialWFACClassMethodID, "NCI_FK_MaterialWFConnection_MaterialWFACClassMethodID");

            entity.Property(e => e.MaterialWFConnectionID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.ACClassWF).WithMany(p => p.MaterialWFConnection_ACClassWF)
                .HasForeignKey(d => d.ACClassWFID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFConnection_ACClassWF");

           entity.HasOne(d => d.Material).WithMany(p => p.MaterialWFConnection_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFConnection_Material");

           entity.HasOne(d => d.MaterialWFACClassMethod).WithMany(p => p.MaterialWFConnection_MaterialWFACClassMethod)
                .HasForeignKey(d => d.MaterialWFACClassMethodID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFConnection_MaterialWFACClassMethod");
        });

        modelBuilder.Entity<MaterialWFRelation>(entity =>
        {
            entity.ToTable("MaterialWFRelation");

            entity.HasIndex(e => e.SourceMaterialID, "NCI_FK_MaterialWFRelation_SourceMaterialID");

            entity.HasIndex(e => e.TargetMaterialID, "NCI_FK_MaterialWFRelation_TargetMaterialID");

            entity.Property(e => e.MaterialWFRelationID).ValueGeneratedNever();

           entity.HasOne(d => d.MaterialWF).WithMany(p => p.MaterialWFRelation_MaterialWF)
                .HasForeignKey(d => d.MaterialWFID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFRelation_MaterialWF");

           entity.HasOne(d => d.SourceMaterial).WithMany(p => p.MaterialWFRelation_SourceMaterial)
                .HasForeignKey(d => d.SourceMaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFRelation_Material1");

           entity.HasOne(d => d.TargetMaterial).WithMany(p => p.MaterialWFRelation_TargetMaterial)
                .HasForeignKey(d => d.TargetMaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaterialWFRelation_Material");
        });

        modelBuilder.Entity<MsgAlarmLog>(entity =>
        {
            entity.ToTable("MsgAlarmLog");

            entity.HasIndex(e => e.ACProgramLogID, "NCI_FK_MsgAlarmLog_ACProgramLogID");

            entity.HasIndex(e => new { e.ACClassID, e.TimeStampOccurred }, "UIX_MsgAlarmLog");

            entity.Property(e => e.MsgAlarmLogID).ValueGeneratedNever();
            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.AcknowledgedBy)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Message).IsUnicode(false);
            entity.Property(e => e.TimeStampAcknowledged).HasColumnType("datetime");
            entity.Property(e => e.TimeStampOccurred).HasColumnType("datetime");
            entity.Property(e => e.TranslID)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACClass).WithMany(p => p.MsgAlarmLog_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MsgAlarmLog_ACClass");

           entity.HasOne(d => d.ACProgramLog).WithMany(p => p.MsgAlarmLog_ACProgramLog)
                .HasForeignKey(d => d.ACProgramLogID)
                .HasConstraintName("FK_MsgAlarmLog_ACProgramLogID");
        });

        modelBuilder.Entity<OperationLog>(entity =>
        {
            entity.ToTable("OperationLog");

            entity.Property(e => e.OperationLogID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OperationTime).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLValue).IsUnicode(false);

           entity.HasOne(d => d.ACProgramLog).WithMany(p => p.OperationLog_ACProgramLog)
                .HasForeignKey(d => d.ACProgramLogID)
                .HasConstraintName("FK_OperationLog_ACProgramLog");

           entity.HasOne(d => d.FacilityCharge).WithMany(p => p.OperationLog_FacilityCharge)
                .HasForeignKey(d => d.FacilityChargeID)
                .HasConstraintName("FK_OperationLog_FacilityCharge");

           entity.HasOne(d => d.RefACClass).WithMany(p => p.OperationLog_RefACClass)
                .HasForeignKey(d => d.RefACClassID)
                .HasConstraintName("FK_OperationLog_OperationLogRefACClassID");
        });

        modelBuilder.Entity<OrderLog>(entity =>
        {
            entity.HasKey(e => e.VBiACProgramLogID);

            entity.ToTable("OrderLog");

            entity.HasIndex(e => e.DeliveryNotePosID, "NCI_FK_OrderLog_DeliveryNotePosID");

            entity.HasIndex(e => e.PickingPosID, "NCI_FK_OrderLog_PickingPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FK_OrderLog_ProdOrderPartslistPosID");

            entity.HasIndex(e => e.ProdOrderPartslistPosRelationID, "NCI_FK_OrderLog_ProdOrderPartslistPosRelationID");

            entity.HasIndex(e => e.VBiACProgramLogID, "NCI_FK_OrderLog_VBiACProgramLogID");

            entity.Property(e => e.VBiACProgramLogID).ValueGeneratedNever();

           entity.HasOne(d => d.DeliveryNotePos).WithMany(p => p.OrderLog_DeliveryNotePos)
                .HasForeignKey(d => d.DeliveryNotePosID)
                .HasConstraintName("FK_OrderLog_DeliveryNotePosID");

           entity.HasOne(d => d.FacilityBooking).WithMany(p => p.OrderLog_FacilityBooking)
                .HasForeignKey(d => d.FacilityBookingID)
                .HasConstraintName("FK_OrderLog_FacilityBookingID");

           entity.HasOne(d => d.PickingPos).WithMany(p => p.OrderLog_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .HasConstraintName("FK_OrderLog_PickingPosID");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.OrderLog_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_OrderLog_ProdOrderPartslistPosID");

           entity.HasOne(d => d.ProdOrderPartslistPosRelation).WithMany(p => p.OrderLog_ProdOrderPartslistPosRelation)
                .HasForeignKey(d => d.ProdOrderPartslistPosRelationID)
                .HasConstraintName("FK_OrderLog_ProdOrderPartslistPosRelationID");

           entity.HasOne(d => d.VBiACProgramLog).WithOne(p => p.OrderLog)
                .HasForeignKey<OrderLog>(d => d.VBiACProgramLogID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderLog_ACProgramLogID");
        });

        modelBuilder.Entity<OrderLogPosMachines>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OrderLogPosMachines");

            entity.Property(e => e.ACUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.BasedOnMachine)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.MachineName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PosBatchNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProdOrderProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OrderLogPosView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OrderLogPosView");

            entity.Property(e => e.ACUrl)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.PosBatchNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PosMaterialName)
                .IsRequired()
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.PosMaterialNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ProdOrderProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OrderLogRelView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("OrderLogRelView");

            entity.Property(e => e.ACUrl)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.BasedOnMachine)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.MachineName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaterialName1)
                .IsRequired()
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.MaterialNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PosBatchNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PosMaterialName)
                .IsRequired()
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.PosMaterialNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ProdOrderProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OutOffer>(entity =>
        {
            entity.ToTable("OutOffer");

            entity.Property(e => e.OutOfferID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CustRequestNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OutOfferDate).HasColumnType("datetime");
            entity.Property(e => e.OutOfferNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesignEnd).HasColumnType("text");
            entity.Property(e => e.XMLDesignStart).HasColumnType("text");

           entity.HasOne(d => d.BillingCompanyAddress).WithMany(p => p.OutOffer_BillingCompanyAddress)
                .HasForeignKey(d => d.BillingCompanyAddressID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOffer_BillingCompanyAddressID");

           entity.HasOne(d => d.CustomerCompany).WithMany(p => p.OutOffer_CustomerCompany)
                .HasForeignKey(d => d.CustomerCompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOffer_CompanyID");

           entity.HasOne(d => d.DeliveryCompanyAddress).WithMany(p => p.OutOffer_DeliveryCompanyAddress)
                .HasForeignKey(d => d.DeliveryCompanyAddressID)
                .HasConstraintName("FK_OutOffer_DeliveryCompanyAddressID");

           entity.HasOne(d => d.IssuerCompanyAddress).WithMany(p => p.OutOffer_IssuerCompanyAddress)
                .HasForeignKey(d => d.IssuerCompanyAddressID)
                .HasConstraintName("FK_OutOffer_CompanyAddress");

           entity.HasOne(d => d.IssuerCompanyPerson).WithMany(p => p.OutOffer_IssuerCompanyPerson)
                .HasForeignKey(d => d.IssuerCompanyPersonID)
                .HasConstraintName("FK_OutOffer_CompanyPerson");

           entity.HasOne(d => d.MDCurrency).WithMany(p => p.OutOffer_MDCurrency)
                .HasForeignKey(d => d.MDCurrencyID)
                .HasConstraintName("FK_OutOffer_MDCurrencyID");

           entity.HasOne(d => d.MDDelivType).WithMany(p => p.OutOffer_MDDelivType)
                .HasForeignKey(d => d.MDDelivTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOffer_MDDelivTypeID");

           entity.HasOne(d => d.MDOutOfferState).WithMany(p => p.OutOffer_MDOutOfferState)
                .HasForeignKey(d => d.MDOutOfferStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOffer_MDOutOfferStateID");

           entity.HasOne(d => d.MDOutOrderType).WithMany(p => p.OutOffer_MDOutOrderType)
                .HasForeignKey(d => d.MDOutOrderTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOffer_MDOutOrderTypeID");

           entity.HasOne(d => d.MDTermOfPayment).WithMany(p => p.OutOffer_MDTermOfPayment)
                .HasForeignKey(d => d.MDTermOfPaymentID)
                .HasConstraintName("FK_OutOffer_MDTermOfPaymentID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.OutOffer_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_OutOffer_MDTimeRangeID");
        });

        modelBuilder.Entity<OutOfferConfig>(entity =>
        {
            entity.ToTable("OutOfferConfig");

            entity.Property(e => e.OutOfferConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.OutOfferConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_OutOfferConfig_MaterialID");

           entity.HasOne(d => d.OutOffer).WithMany(p => p.OutOfferConfig_OutOffer)
                .HasForeignKey(d => d.OutOfferID)
                .HasConstraintName("FK_OutOfferConfig_OutOfferID");

           entity.HasOne(d => d.OutOfferConfig1_ParentOutOfferConfig).WithMany(p => p.OutOfferConfig_ParentOutOfferConfig)
                .HasForeignKey(d => d.ParentOutOfferConfigID)
                .HasConstraintName("FK_OutOfferConfig_ParentOutOfferConfigID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.OutOfferConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_OutOfferConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.OutOfferConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_OutOfferConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.OutOfferConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOfferConfig_DataTypeACClassID");
        });

        modelBuilder.Entity<OutOfferPos>(entity =>
        {
            entity.Property(e => e.OutOfferPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Comment2).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.SalesTax).HasColumnType("money");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesign).HasColumnType("text");

           entity.HasOne(d => d.OutOfferPos1_GroupOutOfferPos).WithMany(p => p.OutOfferPos_GroupOutOfferPos)
                .HasForeignKey(d => d.GroupOutOfferPosID)
                .HasConstraintName("FK_OutOfferPos_GroupOutOfferPosID");

           entity.HasOne(d => d.MDCountrySalesTax).WithMany(p => p.OutOfferPos_MDCountrySalesTax)
                .HasForeignKey(d => d.MDCountrySalesTaxID)
                .HasConstraintName("FK_OutOfferPos_MDCountrySalesTaxID");

           entity.HasOne(d => d.MDCountrySalesTaxMDMaterialGroup).WithMany(p => p.OutOfferPos_MDCountrySalesTaxMDMaterialGroup)
                .HasForeignKey(d => d.MDCountrySalesTaxMDMaterialGroupID)
                .HasConstraintName("FK_OutOfferPos_MDCountrySalesTaxMDMaterialGroupID");

           entity.HasOne(d => d.MDCountrySalesTaxMaterial).WithMany(p => p.OutOfferPos_MDCountrySalesTaxMaterial)
                .HasForeignKey(d => d.MDCountrySalesTaxMaterialID)
                .HasConstraintName("FK_OutOfferPos_MDCountrySalesTaxMaterialID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.OutOfferPos_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_OutOfferPos_MDTimeRangeID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.OutOfferPos_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_OutOfferPos_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.OutOfferPos_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOfferPos_MaterialID");

           entity.HasOne(d => d.OutOffer).WithMany(p => p.OutOfferPos_OutOffer)
                .HasForeignKey(d => d.OutOfferID)
                .HasConstraintName("FK_OutOfferPos_OutOfferID");

           entity.HasOne(d => d.OutOfferPos1_ParentOutOfferPos).WithMany(p => p.OutOfferPos_ParentOutOfferPos)
                .HasForeignKey(d => d.ParentOutOfferPosID)
                .HasConstraintName("FK_OutOfferPos_ParentOutOfferPosID");
        });

        modelBuilder.Entity<OutOrder>(entity =>
        {
            entity.ToTable("OutOrder");

            entity.HasIndex(e => e.BasedOnOutOfferID, "NCI_FK_OutOrder_BasedOnOutOfferingID");

            entity.HasIndex(e => e.BillingCompanyAddressID, "NCI_FK_OutOrder_BillingCompanyAddressID");

            entity.HasIndex(e => e.CPartnerCompanyID, "NCI_FK_OutOrder_CPartnerCompanyID");

            entity.HasIndex(e => e.CustomerCompanyID, "NCI_FK_OutOrder_CustomerCompanyID");

            entity.HasIndex(e => e.DeliveryCompanyAddressID, "NCI_FK_OutOrder_DeliveryCompanyAddressID");

            entity.HasIndex(e => e.MDDelivTypeID, "NCI_FK_OutOrder_MDDelivTypeID");

            entity.HasIndex(e => e.MDOutOrderStateID, "NCI_FK_OutOrder_MDOutOrderStateID");

            entity.HasIndex(e => e.MDOutOrderTypeID, "NCI_FK_OutOrder_MDOutOrderTypeID");

            entity.HasIndex(e => e.MDTermOfPaymentID, "NCI_FK_OutOrder_MDTermOfPaymentID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_OutOrder_MDTimeRangeID");

            entity.HasIndex(e => e.OutOrderNo, "UIX_OutOrder").IsUnique();

            entity.Property(e => e.OutOrderID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.CustOrderNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.OutOrderDate).HasColumnType("datetime");
            entity.Property(e => e.OutOrderNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesignEnd).HasColumnType("text");
            entity.Property(e => e.XMLDesignStart).HasColumnType("text");

           entity.HasOne(d => d.BasedOnOutOffer).WithMany(p => p.OutOrder_BasedOnOutOffer)
                .HasForeignKey(d => d.BasedOnOutOfferID)
                .HasConstraintName("FK_OutOrder_BasedOnOutOfferID");

           entity.HasOne(d => d.BillingCompanyAddress).WithMany(p => p.OutOrder_BillingCompanyAddress)
                .HasForeignKey(d => d.BillingCompanyAddressID)
                .HasConstraintName("FK_OutOrder_BillingCompanyAddressID");

           entity.HasOne(d => d.CPartnerCompany).WithMany(p => p.OutOrder_CPartnerCompany)
                .HasForeignKey(d => d.CPartnerCompanyID)
                .HasConstraintName("FK_OutOrder_CPartnerCompanyID");

           entity.HasOne(d => d.CustomerCompany).WithMany(p => p.OutOrder_CustomerCompany)
                .HasForeignKey(d => d.CustomerCompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrder_CompanyID");

           entity.HasOne(d => d.DeliveryCompanyAddress).WithMany(p => p.OutOrder_DeliveryCompanyAddress)
                .HasForeignKey(d => d.DeliveryCompanyAddressID)
                .HasConstraintName("FK_OutOrder_DeliveryCompanyAddressID");

           entity.HasOne(d => d.IssuerCompanyAddress).WithMany(p => p.OutOrder_IssuerCompanyAddress)
                .HasForeignKey(d => d.IssuerCompanyAddressID)
                .HasConstraintName("FK_OutOrder_CompanyAddress");

           entity.HasOne(d => d.IssuerCompanyPerson).WithMany(p => p.OutOrder_IssuerCompanyPerson)
                .HasForeignKey(d => d.IssuerCompanyPersonID)
                .HasConstraintName("FK_OutOrder_CompanyPerson");

           entity.HasOne(d => d.MDCurrency).WithMany(p => p.OutOrder_MDCurrency)
                .HasForeignKey(d => d.MDCurrencyID)
                .HasConstraintName("FK_OutOrder_MDCurrencyID");

           entity.HasOne(d => d.MDDelivType).WithMany(p => p.OutOrder_MDDelivType)
                .HasForeignKey(d => d.MDDelivTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrder_MDDelivTypeID");

           entity.HasOne(d => d.MDOutOrderState).WithMany(p => p.OutOrder_MDOutOrderState)
                .HasForeignKey(d => d.MDOutOrderStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrder_MDOutOrderStateID");

           entity.HasOne(d => d.MDOutOrderType).WithMany(p => p.OutOrder_MDOutOrderType)
                .HasForeignKey(d => d.MDOutOrderTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrder_MDOutOrderTypeID");

           entity.HasOne(d => d.MDTermOfPayment).WithMany(p => p.OutOrder_MDTermOfPayment)
                .HasForeignKey(d => d.MDTermOfPaymentID)
                .HasConstraintName("FK_OutOrder_MDTermOfPaymentID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.OutOrder_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_OutOrder_MDTimeRangeID");
        });

        modelBuilder.Entity<OutOrderConfig>(entity =>
        {
            entity.ToTable("OutOrderConfig");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_OutOrderConfig_MaterialID");

            entity.HasIndex(e => e.OutOrderID, "NCI_FK_OutOrderConfig_OutOrderID");

            entity.HasIndex(e => e.ParentOutOrderConfigID, "NCI_FK_OutOrderConfig_ParentOutOrderConfigID");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FK_OutOrderConfig_VBiACClassID");

            entity.HasIndex(e => e.VBiACClassPropertyRelationID, "NCI_FK_OutOrderConfig_VBiACClassPropertyRelationID");

            entity.HasIndex(e => e.VBiValueTypeACClassID, "NCI_FK_OutOrderConfig_VBiValueTypeACClassID");

            entity.Property(e => e.OutOrderConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.OutOrderConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_OutOrderConfig_MaterialID");

           entity.HasOne(d => d.OutOrder).WithMany(p => p.OutOrderConfig_OutOrder)
                .HasForeignKey(d => d.OutOrderID)
                .HasConstraintName("FK_OutOrderConfig_OutOrderID");

           entity.HasOne(d => d.OutOrderConfig1_ParentOutOrderConfig).WithMany(p => p.OutOrderConfig_ParentOutOrderConfig)
                .HasForeignKey(d => d.ParentOutOrderConfigID)
                .HasConstraintName("FK_OutOrderConfig_ParentOutOrderConfigID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.OutOrderConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_OutOrderConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.OutOrderConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_OutOrderConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.OutOrderConfig_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OutOrderConfig_ACClassWFID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.OutOrderConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrderConfig_DataTypeACClassID");
        });

        modelBuilder.Entity<OutOrderPos>(entity =>
        {
            entity.HasIndex(e => e.CompanyAddressUnloadingpointID, "NCI_FK_OutOrderPos_CompanyAddressUnloadingpointID");

            entity.HasIndex(e => e.MDCountrySalesTaxID, "NCI_FK_OutOrderPos_MDCountrySalesTaxID");

            entity.HasIndex(e => e.MDDelivPosLoadStateID, "NCI_FK_OutOrderPos_MDDelivPosLoadStateID");

            entity.HasIndex(e => e.MDDelivPosStateID, "NCI_FK_OutOrderPos_MDDelivPosStateID");

            entity.HasIndex(e => e.MDOutOrderPlanStateID, "NCI_FK_OutOrderPos_MDOutOrderPlanStateID");

            entity.HasIndex(e => e.MDOutOrderPosStateID, "NCI_FK_OutOrderPos_MDOutOrderPosStateID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_OutOrderPos_MDTimeRangeID");

            entity.HasIndex(e => e.MDToleranceStateID, "NCI_FK_OutOrderPos_MDToleranceStateID");

            entity.HasIndex(e => e.MDTourplanPosStateID, "NCI_FK_OutOrderPos_MDTourplanPosStateID");

            entity.HasIndex(e => e.MDTransportModeID, "NCI_FK_OutOrderPos_MDTransportModeID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_OutOrderPos_MDUnitID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_OutOrderPos_MaterialID");

            entity.HasIndex(e => e.OutOrderID, "NCI_FK_OutOrderPos_OutOrderID");

            entity.HasIndex(e => e.ParentOutOrderPosID, "NCI_FK_OutOrderPos_ParentOutOrderPosID");

            entity.HasIndex(e => e.PickupCompanyMaterialID, "NCI_FK_OutOrderPos_PickupCompanyMaterialID");

            entity.HasIndex(e => new { e.OutOrderPosID, e.Sequence }, "UIX_OutOrderPos").IsUnique();

            entity.Property(e => e.OutOrderPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Comment2).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PriceGross).HasColumnType("money");
            entity.Property(e => e.PriceNet).HasColumnType("money");
            entity.Property(e => e.SalesTax).HasColumnType("money");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryDateConfirmed).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryMaxDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesign).HasColumnType("text");

           entity.HasOne(d => d.CompanyAddressUnloadingpoint).WithMany(p => p.OutOrderPos_CompanyAddressUnloadingpoint)
                .HasForeignKey(d => d.CompanyAddressUnloadingpointID)
                .HasConstraintName("FK_OutOrderPos_CompanyAddressUnloadingPointID");

           entity.HasOne(d => d.OutOrderPos1_GroupOutOrderPos).WithMany(p => p.OutOrderPos_GroupOutOrderPos)
                .HasForeignKey(d => d.GroupOutOrderPosID)
                .HasConstraintName("FK_OutOrderPos_GroupOutOrderPosID");

           entity.HasOne(d => d.MDCountrySalesTax).WithMany(p => p.OutOrderPos_MDCountrySalesTax)
                .HasForeignKey(d => d.MDCountrySalesTaxID)
                .HasConstraintName("FK_OutOrderPos_MDCountrySalesTaxID");

           entity.HasOne(d => d.MDCountrySalesTaxMDMaterialGroup).WithMany(p => p.OutOrderPos_MDCountrySalesTaxMDMaterialGroup)
                .HasForeignKey(d => d.MDCountrySalesTaxMDMaterialGroupID)
                .HasConstraintName("FK_OutOrderPos_MDCountrySalesTaxMDMaterialGroupID");

           entity.HasOne(d => d.MDCountrySalesTaxMaterial).WithMany(p => p.OutOrderPos_MDCountrySalesTaxMaterial)
                .HasForeignKey(d => d.MDCountrySalesTaxMaterialID)
                .HasConstraintName("FK_OutOrderPos_MDCountrySalesTaxMaterialID");

           entity.HasOne(d => d.MDDelivPosLoadState).WithMany(p => p.OutOrderPos_MDDelivPosLoadState)
                .HasForeignKey(d => d.MDDelivPosLoadStateID)
                .HasConstraintName("FK_OutOrderPos_MDDelivPosLoadStateID");

           entity.HasOne(d => d.MDDelivPosState).WithMany(p => p.OutOrderPos_MDDelivPosState)
                .HasForeignKey(d => d.MDDelivPosStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrderPos_MDDelivPosStateID");

           entity.HasOne(d => d.MDOutOrderPlanState).WithMany(p => p.OutOrderPos_MDOutOrderPlanState)
                .HasForeignKey(d => d.MDOutOrderPlanStateID)
                .HasConstraintName("FK_OutOrderPos_MDOutOrderPlanStateID");

           entity.HasOne(d => d.MDOutOrderPosState).WithMany(p => p.OutOrderPos_MDOutOrderPosState)
                .HasForeignKey(d => d.MDOutOrderPosStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrderPos_MDOutOrderPosStateID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.OutOrderPos_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_OutOrderPos_MDTimeRangeID");

           entity.HasOne(d => d.MDToleranceState).WithMany(p => p.OutOrderPos_MDToleranceState)
                .HasForeignKey(d => d.MDToleranceStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrderPos_MDToleranceStateID");

           entity.HasOne(d => d.MDTourplanPosState).WithMany(p => p.OutOrderPos_MDTourplanPosState)
                .HasForeignKey(d => d.MDTourplanPosStateID)
                .HasConstraintName("FK_OutOrderPos_MDTourplanPosStateID");

           entity.HasOne(d => d.MDTransportMode).WithMany(p => p.OutOrderPos_MDTransportMode)
                .HasForeignKey(d => d.MDTransportModeID)
                .HasConstraintName("FK_OutOrderPos_MDTransportModeID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.OutOrderPos_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_OutOrderPos_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.OutOrderPos_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrderPos_MaterialID");

           entity.HasOne(d => d.OutOrder).WithMany(p => p.OutOrderPos_OutOrder)
                .HasForeignKey(d => d.OutOrderID)
                .HasConstraintName("FK_OutOrderPos_OutOrderID");

           entity.HasOne(d => d.OutOrderPos1_ParentOutOrderPos).WithMany(p => p.OutOrderPos_ParentOutOrderPos)
                .HasForeignKey(d => d.ParentOutOrderPosID)
                .HasConstraintName("FK_OutOrderPos_ParentOutOrderPosID");

           entity.HasOne(d => d.PickupCompanyMaterial).WithMany(p => p.OutOrderPos_PickupCompanyMaterial)
                .HasForeignKey(d => d.PickupCompanyMaterialID)
                .HasConstraintName("FK_OutOrderPos_PickupCompanyMaterialID");
        });

        modelBuilder.Entity<OutOrderPosSplit>(entity =>
        {
            entity.ToTable("OutOrderPosSplit");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_OutOrderPosSplit_OutOrderPosID");

            entity.Property(e => e.OutOrderPosSplitID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.OutOrderPosSplit_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_OutOrderPosSplit_OutOrderPosID");
        });

        modelBuilder.Entity<OutOrderPosUtilization>(entity =>
        {
            entity.ToTable("OutOrderPosUtilization");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_OutOrderPosUtilization_MaterialID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_OutOrderPosUtilization_OutOrderPosID");

            entity.HasIndex(e => e.OutOrderPosUtilizationNo, "UIX_OutOrderPosUtilization_OutOrderPosUtilizationNo").IsUnique();

            entity.Property(e => e.OutOrderPosUtilizationID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OutOrderPosUtilizationNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TimeFrom).HasColumnType("datetime");
            entity.Property(e => e.TimeTo).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).IsUnicode(false);

           entity.HasOne(d => d.Material).WithMany(p => p.OutOrderPosUtilization_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_OutOrderPosUtilization_MaterialID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.OutOrderPosUtilization_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OutOrderPosUtilization_OutOrderPosID");
        });

        modelBuilder.Entity<Partslist>(entity =>
        {
            entity.ToTable("Partslist");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_Partslist_MDUnitID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_Partslist_MaterialID");

            entity.HasIndex(e => e.MaterialWFID, "NCI_FK_Partslist_MaterialWFID");

            entity.HasIndex(e => e.PreviousPartslistID, "NCI_FK_Partslist_PreviousPartslistID");

            entity.HasIndex(e => new { e.PartslistID, e.MaterialID }, "NCI_Partslist_PartslistID_MaterialID");

            entity.HasIndex(e => new { e.PartslistNo, e.PartslistVersion, e.DeleteDate }, "PartslistVersion_PartslistNo").IsUnique();

            entity.Property(e => e.PartslistID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.DeleteDate).HasColumnType("datetime");
            entity.Property(e => e.DeleteName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.EnabledFrom).HasColumnType("datetime");
            entity.Property(e => e.EnabledTo).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsInEnabledPeriod).HasComputedColumnSql("([dbo].[udf_IsTimeSpanActual]([EnabledFrom],[EnabledTo]))", false);
            entity.ToTable(tbl => tbl.HasTrigger("([dbo].[udf_IsTimeSpanActual]([EnabledFrom],[EnabledTo]))"));
                        entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.LastFormulaChange).HasColumnType("datetime");
            entity.Property(e => e.PartslistName).HasMaxLength(350);
            entity.Property(e => e.PartslistNo)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.PartslistVersion)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLComment).HasColumnType("text");
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.XMLDesign).HasColumnType("text");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.Partslist_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_Partslist_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.Partslist_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Partslist_MaterialID");

           entity.HasOne(d => d.MaterialWF).WithMany(p => p.Partslist_MaterialWF)
                .HasForeignKey(d => d.MaterialWFID)
                .HasConstraintName("FK_Partslist_MaterialWF");

           entity.HasOne(d => d.Partslist1_PreviousPartslist).WithMany(p => p.Partslist_PreviousPartslist)
                .HasForeignKey(d => d.PreviousPartslistID)
                .HasConstraintName("PreviousPartslistID_PartslistID");
        });

        modelBuilder.Entity<PartslistACClassMethod>(entity =>
        {
            entity.ToTable("PartslistACClassMethod");

            entity.Property(e => e.PartslistACClassMethodID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MaterialWFACClassMethod).WithMany(p => p.PartslistACClassMethod_MaterialWFACClassMethod)
                .HasForeignKey(d => d.MaterialWFACClassMethodID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartslistACClassMethod_MaterialWFACClassMethod");

           entity.HasOne(d => d.Partslist).WithMany(p => p.PartslistACClassMethod_Partslist)
                .HasForeignKey(d => d.PartslistID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartslistACClassMethod_Partslist");
        });

        modelBuilder.Entity<PartslistConfig>(entity =>
        {
            entity.ToTable("PartslistConfig");

            entity.HasIndex(e => e.ParentPartslistConfigID, "NCI_FK_PartslistConfig_ParentPartslistConfigID");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FK_PartslistConfig_VBiACClassID");

            entity.HasIndex(e => e.VBiACClassPropertyRelationID, "NCI_FK_PartslistConfig_VBiACClassPropertyRelationID");

            entity.HasIndex(e => e.VBiValueTypeACClassID, "NCI_FK_PartslistConfig_VBiValueTypeACClassID");

            entity.Property(e => e.PartslistConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.PartslistConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_PartslistConfig_MaterialID");

           entity.HasOne(d => d.PartslistConfig1_ParentPartslistConfig).WithMany(p => p.PartslistConfig_ParentPartslistConfig)
                .HasForeignKey(d => d.ParentPartslistConfigID)
                .HasConstraintName("FK_PartslistConfig_ParentPartslistConfigID");

           entity.HasOne(d => d.Partslist).WithMany(p => p.PartslistConfig_Partslist)
                .HasForeignKey(d => d.PartslistID)
                .HasConstraintName("FK_PartslistConfig_PartslistID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.PartslistConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_PartslistConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.PartslistConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_PartslistConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.PartslistConfig_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PartslistConfig_ACClassWFID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.PartslistConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartslistConfig_ValueTypeACClassID");
        });

        modelBuilder.Entity<PartslistPos>(entity =>
        {
            entity.HasIndex(e => e.AlternativePartslistPosID, "NCI_FK_PartslistPos_AlternativePartslistPosID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_PartslistPos_MDUnitID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_PartslistPos_MaterialID");

            entity.HasIndex(e => e.ParentPartslistID, "NCI_FK_PartslistPos_ParentPartslistID");

            entity.HasIndex(e => e.ParentPartslistPosID, "NCI_FK_PartslistPos_ParentPartslistPosID");

            entity.HasIndex(e => e.PartslistID, "NCI_FK_PartslistPos_PartslistID");

            entity.Property(e => e.PartslistPosID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.ParentPartslistID).HasComment("Selected partslist for production this position from partslist with same output material.");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.PartslistPos1_AlternativePartslistPos).WithMany(p => p.PartslistPos_AlternativePartslistPos)
                .HasForeignKey(d => d.AlternativePartslistPosID)
                .HasConstraintName("FK_PartslistPos_AlternativePartslistPosID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.PartslistPos_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_PartslistPos_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.PartslistPos_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartslistPos_MaterialID");

           entity.HasOne(d => d.ParentPartslist).WithMany(p => p.PartslistPos_ParentPartslist)
                .HasForeignKey(d => d.ParentPartslistID)
                .HasConstraintName("FK_PartslistPos_Partslist");

           entity.HasOne(d => d.PartslistPos1_ParentPartslistPos).WithMany(p => p.PartslistPos_ParentPartslistPos)
                .HasForeignKey(d => d.ParentPartslistPosID)
                .HasConstraintName("FK_PartslistPos_ParentPartslistPosID");

           entity.HasOne(d => d.Partslist).WithMany(p => p.PartslistPos_Partslist)
                .HasForeignKey(d => d.PartslistID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartslistPos_PartsListID");
        });

        modelBuilder.Entity<PartslistPosRelation>(entity =>
        {
            entity.ToTable("PartslistPosRelation");

            entity.HasIndex(e => e.MaterialWFRelationID, "NCI_FK_PartslistPosRelation_MaterialWFRelationID");

            entity.HasIndex(e => e.SourcePartslistPosID, "NCI_FK_PartslistPosRelation_SourcePartslistPosID");

            entity.HasIndex(e => e.TargetPartslistPosID, "NCI_FK_PartslistPosRelation_TargetPartslistPosID");

            entity.Property(e => e.PartslistPosRelationID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MaterialWFRelation).WithMany(p => p.PartslistPosRelation_MaterialWFRelation)
                .HasForeignKey(d => d.MaterialWFRelationID)
                .HasConstraintName("FK_PartslistPosRelation_MaterialWFRelation");

           entity.HasOne(d => d.SourcePartslistPos).WithMany(p => p.PartslistPosRelation_SourcePartslistPos)
                .HasForeignKey(d => d.SourcePartslistPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartslistPosRelation_PartslistPos1");

           entity.HasOne(d => d.TargetPartslistPos).WithMany(p => p.PartslistPosRelation_TargetPartslistPos)
                .HasForeignKey(d => d.TargetPartslistPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartslistPosRelation_PartslistPos");
        });

        modelBuilder.Entity<PartslistPosSplit>(entity =>
        {
            entity.ToTable("PartslistPosSplit");

            entity.HasIndex(e => e.PartslistPosID, "NCI_FK_PartslistPosSplit_PartslistPosID");

            entity.Property(e => e.PartslistPosSplitID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.PartslistPos).WithMany(p => p.PartslistPosSplit_PartslistPos)
                .HasForeignKey(d => d.PartslistPosID)
                .HasConstraintName("FK_PartslistPosSplit_PartslistPosID");
        });

        modelBuilder.Entity<PartslistStock>(entity =>
        {
            entity.ToTable("PartslistStock");

            entity.HasIndex(e => e.MDReleaseStateID, "NCI_FK_PartslistStock_MDReleaseStateID");

            entity.HasIndex(e => e.PartslistID, "NCI_FK_PartslistStock_PartslistID");

            entity.Property(e => e.PartslistStockID).ValueGeneratedNever();
            entity.Property(e => e.DayBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MonthBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WeekBalanceDate).HasColumnType("datetime");
            entity.Property(e => e.XMLConfig).HasColumnType("text");
            entity.Property(e => e.YearBalanceDate).HasColumnType("datetime");

           entity.HasOne(d => d.MDReleaseState).WithMany(p => p.PartslistStock_MDReleaseState)
                .HasForeignKey(d => d.MDReleaseStateID)
                .HasConstraintName("FK_PartslistStock_MDReleaseStateID");

           entity.HasOne(d => d.Partslist).WithMany(p => p.PartslistStock_Partslist)
                .HasForeignKey(d => d.PartslistID)
                .HasConstraintName("FK_PartslistStock_PartslistID");
        });

        modelBuilder.Entity<Picking>(entity =>
        {
            entity.ToTable("Picking");

            entity.HasIndex(e => e.MirroredFromPickingID, "NCI_FK_Picking_MirroredFromPickingID");

            entity.HasIndex(e => e.TourplanID, "NCI_FK_Picking_TourplanID");

            entity.HasIndex(e => e.VisitorVoucherID, "NCI_FK_Picking_VisitorVoucherID");

            entity.HasIndex(e => e.PickingNo, "UIX_Picking_PickingNo").IsUnique();

            entity.Property(e => e.PickingID).ValueGeneratedNever();
            entity.Property(e => e.CalculatedEndDate).HasColumnType("datetime");
            entity.Property(e => e.CalculatedStartDate).HasColumnType("datetime");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Comment2).IsUnicode(false);
            entity.Property(e => e.DeliveryDateFrom).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDateTo).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.PickingNo)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ScheduledEndDate).HasColumnType("datetime");
            entity.Property(e => e.ScheduledStartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ACClassMethod).WithMany(p => p.Picking_ACClassMethod)
                .HasForeignKey(d => d.ACClassMethodID)
                .HasConstraintName("FK_Picking_ACClassMethod");

           entity.HasOne(d => d.DeliveryCompanyAddress).WithMany(p => p.Picking_DeliveryCompanyAddress)
                .HasForeignKey(d => d.DeliveryCompanyAddressID)
                .HasConstraintName("FK_Picking_DeliveryCompanyAddressID");

           entity.HasOne(d => d.MDPickingType).WithMany(p => p.Picking_MDPickingType)
                .HasForeignKey(d => d.MDPickingTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Picking_MDPickingType");

           entity.HasOne(d => d.Tourplan).WithMany(p => p.Picking_Tourplan)
                .HasForeignKey(d => d.TourplanID)
                .HasConstraintName("FK_Picking_TourplanID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.Picking_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .HasConstraintName("FK_Picking_ACClassWFID");

           entity.HasOne(d => d.VisitorVoucher).WithMany(p => p.Picking_VisitorVoucher)
                .HasForeignKey(d => d.VisitorVoucherID)
                .HasConstraintName("FK_Picking_VisitorVoucherID");
        });

        modelBuilder.Entity<PickingConfig>(entity =>
        {
            entity.ToTable("PickingConfig");

            entity.Property(e => e.PickingConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.PickingConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_PickingConfig_MaterialID");

           entity.HasOne(d => d.PickingConfig1_ParentPickingConfig).WithMany(p => p.PickingConfig_ParentPickingConfig)
                .HasForeignKey(d => d.ParentPickingConfigID)
                .HasConstraintName("FK_PickingConfig_ParentPickingConfigID");

           entity.HasOne(d => d.Picking).WithMany(p => p.PickingConfig_Picking)
                .HasForeignKey(d => d.PickingID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingConfig_PickingID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.PickingConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_PickingConfig_VBiACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.PickingConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_PickingConfig_VBiACClassPropertyRelationID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.PickingConfig_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PickingConfig_VBiACClassWFID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.PickingConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingConfig_VBiValueTypeACClassID");
        });

        modelBuilder.Entity<PickingPos>(entity =>
        {
            entity.HasIndex(e => e.FromFacilityID, "NCI_FK_PickingPos_FromFacilityID");

            entity.HasIndex(e => e.InOrderPosID, "NCI_FK_PickingPos_InOrderPosID");

            entity.HasIndex(e => e.OutOrderPosID, "NCI_FK_PickingPos_OutOrderPosID");

            entity.HasIndex(e => e.PickingID, "NCI_FK_PickingPos_PickingID");

            entity.HasIndex(e => e.ToFacilityID, "NCI_FK_PickingPos_ToFacilityID");

            entity.Property(e => e.PickingPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ACClassTask).WithMany(p => p.PickingPos_ACClassTask)
                .HasForeignKey(d => d.ACClassTaskID)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PickingPos_ACClassTaskID");

           entity.HasOne(d => d.FromFacility).WithMany(p => p.PickingPos_FromFacility)
                .HasForeignKey(d => d.FromFacilityID)
                .HasConstraintName("FK_PickingPos_FromFacilityID");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.PickingPos_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_PickingPos_InOrderPosID");

           entity.HasOne(d => d.MDDelivPosLoadState).WithMany(p => p.PickingPos_MDDelivPosLoadState)
                .HasForeignKey(d => d.MDDelivPosLoadStateID)
                .HasConstraintName("FK_PickingPos_MDDelivPosLoadStateID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.PickingPos_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_PickingPos_OutOrderPosID");

           entity.HasOne(d => d.Picking).WithMany(p => p.PickingPos_Picking)
                .HasForeignKey(d => d.PickingID)
                .HasConstraintName("FK_PickingPos_PickingID");

           entity.HasOne(d => d.PickingMaterial).WithMany(p => p.PickingPos_PickingMaterial)
                .HasForeignKey(d => d.PickingMaterialID)
                .HasConstraintName("FK_PickingPos_MaterialID");

           entity.HasOne(d => d.ToFacility).WithMany(p => p.PickingPos_ToFacility)
                .HasForeignKey(d => d.ToFacilityID)
                .HasConstraintName("FK_PickingPos_ToFacilityID");
        });

        modelBuilder.Entity<PickingPosProdOrderPartslistPos>(entity =>
        {
            entity.HasIndex(e => new { e.PickingPosID, e.ProdorderPartslistPosID }, "PickingPos_ProdorderPartslistPos").IsUnique();

            entity.Property(e => e.PickingPosProdOrderPartslistPosID).ValueGeneratedNever();

           entity.HasOne(d => d.PickingPos).WithMany(p => p.PickingPosProdOrderPartslistPos_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingPosProdOrderPartslistPos_PickingPos");

           entity.HasOne(d => d.ProdorderPartslistPos).WithMany(p => p.PickingPosProdOrderPartslistPos_ProdorderPartslistPos)
                .HasForeignKey(d => d.ProdorderPartslistPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PickingPosProdOrderPartslistPos_ProdorderPartslistPos");
        });

        modelBuilder.Entity<PlanningMR>(entity =>
        {
            entity.ToTable("PlanningMR");

            entity.Property(e => e.PlanningMRID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PlanningMRNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PlanningName)
                .IsRequired()
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.RangeFrom).HasColumnType("datetime");
            entity.Property(e => e.RangeTo).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<PlanningMRCons>(entity =>
        {
            entity.Property(e => e.PlanningMRConsID).ValueGeneratedNever();
            entity.Property(e => e.ConsumptionDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.DefaultPartslist).WithMany(p => p.PlanningMRCons_DefaultPartslist)
                .HasForeignKey(d => d.DefaultPartslistID)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PlanningMRCons_PartslistID");

           entity.HasOne(d => d.Material).WithMany(p => p.PlanningMRCons_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_PlanningMRCons_Material");

           entity.HasOne(d => d.PlanningMR).WithMany(p => p.PlanningMRCons_PlanningMR)
                .HasForeignKey(d => d.PlanningMRID)
                .HasConstraintName("FK_PlanningMRCons_PlanningMR");
        });

        modelBuilder.Entity<PlanningMRPos>(entity =>
        {
            entity.Property(e => e.PlanningMRPosID).ValueGeneratedNever();
            entity.Property(e => e.ExpectedBookingDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.PlanningMRPos_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_PlanningMRPos_InOrderPos");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.PlanningMRPos_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_PlanningMRPos_OutOrderPos");

           entity.HasOne(d => d.PlanningMRCons).WithMany(p => p.PlanningMRPos_PlanningMRCons)
                .HasForeignKey(d => d.PlanningMRConsID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlanningMRPos_PlanningMRCons");

           entity.HasOne(d => d.PlanningMRProposal).WithMany(p => p.PlanningMRPos_PlanningMRProposal)
                .HasForeignKey(d => d.PlanningMRProposalID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PlanningMRPos_PlanningMRProposal");

           entity.HasOne(d => d.ProdOrderPartslist).WithMany(p => p.PlanningMRPos_ProdOrderPartslist)
                .HasForeignKey(d => d.ProdOrderPartslistID)
                .HasConstraintName("FK_PlanningMRPos_ProdOrderPartslist");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.PlanningMRPos_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_PlanningMRPos_ProdOrderPartslistPos");
        });

        modelBuilder.Entity<PlanningMRProposal>(entity =>
        {
            entity.ToTable("PlanningMRProposal");

            entity.Property(e => e.PlanningMRProposalID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.InOrder).WithMany(p => p.PlanningMRProposal_InOrder)
                .HasForeignKey(d => d.InOrderID)
                .HasConstraintName("FK_PlanningMRProposal_InOrderID");

           entity.HasOne(d => d.PlanningMR).WithMany(p => p.PlanningMRProposal_PlanningMR)
                .HasForeignKey(d => d.PlanningMRID)
                .HasConstraintName("FK_PlanningMRProposal_PlanningMR");

           entity.HasOne(d => d.ProdOrder).WithMany(p => p.PlanningMRProposal_ProdOrder)
                .HasForeignKey(d => d.ProdOrderID)
                .HasConstraintName("FK_PlanningMRProposal_ProdOrderID");

           entity.HasOne(d => d.ProdOrderPartslist).WithMany(p => p.PlanningMRProposal_ProdOrderPartslist)
                .HasForeignKey(d => d.ProdOrderPartslistID)
                .HasConstraintName("FK_PlanningMRProposal_ProdOrderPartslistID");
        });

        modelBuilder.Entity<PriceList>(entity =>
        {
            entity.ToTable("PriceList");

            entity.Property(e => e.PriceListID).ValueGeneratedNever();
            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.DateFrom).HasColumnType("datetime");
            entity.Property(e => e.DateTo).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PriceListNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.PriceListNo)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MDCurrency).WithMany(p => p.PriceList_MDCurrency)
                .HasForeignKey(d => d.MDCurrencyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PriceList_MDCurrency");
        });

        modelBuilder.Entity<PriceListMaterial>(entity =>
        {
            entity.ToTable("PriceListMaterial");

            entity.HasIndex(e => new { e.PriceListID, e.MaterialID }, "UX_PriceList_Material").IsUnique();

            entity.Property(e => e.PriceListMaterialID).ValueGeneratedNever();
            entity.Property(e => e.Price).HasColumnType("money");

           entity.HasOne(d => d.Material).WithMany(p => p.PriceListMaterial_Material)
                .HasForeignKey(d => d.MaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PriceListMaterial_Material");

           entity.HasOne(d => d.PriceList).WithMany(p => p.PriceListMaterial_PriceList)
                .HasForeignKey(d => d.PriceListID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PriceListMaterial_PriceList");
        });

        modelBuilder.Entity<ProdOrder>(entity =>
        {
            entity.ToTable("ProdOrder");

            entity.HasIndex(e => e.CPartnerCompanyID, "NCI_FK_ProdOrder_CPartnerCompanyID");

            entity.HasIndex(e => e.MDProdOrderStateID, "NCI_FK_ProdOrder_MDProdOrderStateID");

            entity.HasIndex(e => new { e.ProdOrderID, e.MDProdOrderStateID }, "NCI_PProdOrder_ProdOrderID_MDProdOrderStateID");

            entity.HasIndex(e => e.ProgramNo, "UIX_ProdOrder_ProgramNo").IsUnique();

            entity.Property(e => e.ProdOrderID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyOfExtSys)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CPartnerCompany).WithMany(p => p.ProdOrder_CPartnerCompany)
                .HasForeignKey(d => d.CPartnerCompanyID)
                .HasConstraintName("FK_ProdOrder_CPartnerCompanyID");

           entity.HasOne(d => d.MDProdOrderState).WithMany(p => p.ProdOrder_MDProdOrderState)
                .HasForeignKey(d => d.MDProdOrderStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrder_MDProdOrderStateID");
        });

        modelBuilder.Entity<ProdOrderBatch>(entity =>
        {
            entity.ToTable("ProdOrderBatch");

            entity.HasIndex(e => e.MDProdOrderStateID, "NCI_FK_ProdOrderBatch_MDProdOrderStateID");

            entity.HasIndex(e => e.ProdOrderBatchPlanID, "NCI_FK_ProdOrderBatch_ProdOrderBatchPlanID");

            entity.HasIndex(e => e.ProdOrderPartslistID, "NCI_FK_ProdOrderBatch_ProdOrderPartslistID");

            entity.HasIndex(e => e.ProdOrderBatchNo, "NCI_ProdOrderBatch_ProdOrderBatchNo");

            entity.HasIndex(e => e.ProdOrderBatchNo, "UIX_ProdOrderBatch_ProdOrderBatchNo").IsUnique();

            entity.Property(e => e.ProdOrderBatchID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProdOrderBatchNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDProdOrderState).WithMany(p => p.ProdOrderBatch_MDProdOrderState)
                .HasForeignKey(d => d.MDProdOrderStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderBatch_MDProdOrderState");

           entity.HasOne(d => d.ProdOrderBatchPlan).WithMany(p => p.ProdOrderBatch_ProdOrderBatchPlan)
                .HasForeignKey(d => d.ProdOrderBatchPlanID)
                .HasConstraintName("FK_ProdOrderBatch_ProdOrderBatchPlan");

           entity.HasOne(d => d.ProdOrderPartslist).WithMany(p => p.ProdOrderBatch_ProdOrderPartslist)
                .HasForeignKey(d => d.ProdOrderPartslistID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderBatch_ProdOrderPartslist");
        });

        modelBuilder.Entity<ProdOrderBatchPlan>(entity =>
        {
            entity.ToTable("ProdOrderBatchPlan");

            entity.HasIndex(e => e.ProdOrderPartslistID, "NCI_FK_ProdOrderBatchPlan_ProdOrderPartslistID");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FK_ProdOrderBatchPlan_ProdOrderPartslistPosID");

            entity.HasIndex(e => e.VBiACClassWFID, "NCI_FK_ProdOrderBatchPlan_VBiACClassWFID");

            entity.HasIndex(e => e.ProdOrderPartslistID, "NCI_ProdOrderBatchPlan_ProdOrderPartslistID_OT");

            entity.HasIndex(e => new { e.ProdOrderPartslistPosID, e.PlanStateIndex, e.ProdOrderBatchPlanID }, "NCI_ProdOrderBatchPlan_ProdOrderPartslistPosID_PlanStateIndex_ProdOrderBatchPlanID");

            entity.Property(e => e.ProdOrderBatchPlanID).ValueGeneratedNever();
            entity.Property(e => e.CalculatedEndDate).HasColumnType("datetime");
            entity.Property(e => e.CalculatedStartDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PlannedStartDate).HasColumnType("datetime");
            entity.Property(e => e.ScheduledEndDate).HasColumnType("datetime");
            entity.Property(e => e.ScheduledStartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDBatchPlanGroup).WithMany(p => p.ProdOrderBatchPlan_MDBatchPlanGroup)
                .HasForeignKey(d => d.MDBatchPlanGroupID)
                .HasConstraintName("FK_ProdOrderBatchPlan_MDBatchPlanGroupID");

           entity.HasOne(d => d.MaterialWFACClassMethod).WithMany(p => p.ProdOrderBatchPlan_MaterialWFACClassMethod)
                .HasForeignKey(d => d.MaterialWFACClassMethodID)
                .HasConstraintName("FK_ProdOrderBatchPlan_MaterialWFACClassMethodID");

           entity.HasOne(d => d.ProdOrderPartslist).WithMany(p => p.ProdOrderBatchPlan_ProdOrderPartslist)
                .HasForeignKey(d => d.ProdOrderPartslistID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderBatchPlan_ProdOrderPartslist");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.ProdOrderBatchPlan_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_ProdOrderBatchPlan_ProdOrderPartslistPosID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.ProdOrderBatchPlan_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .HasConstraintName("FK_ProdOrderBatchPlan_VBiACClassWFID");
        });

        modelBuilder.Entity<ProdOrderConnectionsDetailView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ProdOrderConnectionsDetailView");

            entity.Property(e => e.InwardLotNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InwardProdOrderBatchNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InwardProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OutwardProdOrderBatchNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OutwardProgramNo)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProdOrderConnectionsView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ProdOrderConnectionsView");

            entity.Property(e => e.InwardLotNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InwardProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.OutwardProgramNo)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProdOrderInwardsView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ProdOrderInwardsView");

            entity.Property(e => e.FacilityBookingChargeNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IntermediateMaterial)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.LotNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProdOrderBatchNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProdOrderOutwardsView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ProdOrderOutwardsView");

            entity.Property(e => e.FacilityBookingChargeNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IntermediateMaterial)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.LotNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProdOrderBatchNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ProgramNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProdOrderPartslist>(entity =>
        {
            entity.ToTable("ProdOrderPartslist");

            entity.HasIndex(e => e.MDProdOrderStateID, "NCI_FK_ProdOrderPartslist_MDProdOrderStateID");

            entity.HasIndex(e => e.PartslistID, "NCI_FK_ProdOrderPartslist_PartslistID");

            entity.HasIndex(e => e.ProdOrderID, "NCI_FK_ProdOrderPartslist_ProdOrderID");

            entity.HasIndex(e => new { e.ProdOrderPartslistID, e.ProdOrderID, e.MDProdOrderStateID }, "NCI_ProdOrderPartslist_ProdOrderPartslistID_ProdOrderID_MDProdOrderStateID");

            entity.Property(e => e.ProdOrderPartslistID).ValueGeneratedNever();
            entity.Property(e => e.DepartmentUserDate).HasColumnType("datetime");
            entity.Property(e => e.DepartmentUserName)
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ExternProdOrderNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastFormulaChange).HasColumnType("datetime");
            entity.Property(e => e.LossComment).IsUnicode(false);
            entity.Property(e => e.ProdUserEndDate).HasColumnType("datetime");
            entity.Property(e => e.ProdUserEndName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TargetDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDProdOrderState).WithMany(p => p.ProdOrderPartslist_MDProdOrderState)
                .HasForeignKey(d => d.MDProdOrderStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslist_MDProdOrderState");

           entity.HasOne(d => d.Partslist).WithMany(p => p.ProdOrderPartslist_Partslist)
                .HasForeignKey(d => d.PartslistID)
                .HasConstraintName("FK_ProdOrderPartslist_PartslistID");

           entity.HasOne(d => d.ProdOrder).WithMany(p => p.ProdOrderPartslist_ProdOrder)
                .HasForeignKey(d => d.ProdOrderID)
                .HasConstraintName("FK_ProdOrderPartslist_ProdOrderID");

           entity.HasOne(d => d.VBiACProgram).WithMany(p => p.ProdOrderPartslist_VBiACProgram)
                .HasForeignKey(d => d.VBiACProgramID)
                .HasConstraintName("FK_ProdOrderPartslist_ACProgramID");
        });

        modelBuilder.Entity<ProdOrderPartslistConfig>(entity =>
        {
            entity.ToTable("ProdOrderPartslistConfig");

            entity.Property(e => e.ProdOrderPartslistConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.ProdOrderPartslistConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_ProdOrderPartslistConfig_MaterialID");

           entity.HasOne(d => d.ProdOrderPartslistConfig1_ParentProdOrderPartslistConfig).WithMany(p => p.ProdOrderPartslistConfig_ParentProdOrderPartslistConfig)
                .HasForeignKey(d => d.ParentProdOrderPartslistConfigID)
                .HasConstraintName("FK_ProdOrderPartslistConfig_ParentProdOrderPartslistConfigID");

           entity.HasOne(d => d.ProdOrderPartslist).WithMany(p => p.ProdOrderPartslistConfig_ProdOrderPartslist)
                .HasForeignKey(d => d.ProdOrderPartslistID)
                .HasConstraintName("FK_ProdOrderPartslistConfig_ProdOrderPartslistID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.ProdOrderPartslistConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_ProdOrderPartslistConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.ProdOrderPartslistConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_ProdOrderPartslistConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiACClassWF).WithMany(p => p.ProdOrderPartslistConfig_VBiACClassWF)
                .HasForeignKey(d => d.VBiACClassWFID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ProdOrderPartslistConfig_ACClassWFID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.ProdOrderPartslistConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslistConfig_ValueTypeACClassID");
        });

        modelBuilder.Entity<ProdOrderPartslistPos>(entity =>
        {
            entity.HasIndex(e => e.ACClassTaskID, "NCI_FK_ProdOrderPartslistPos_ACClassTaskID");

            entity.HasIndex(e => e.AlternativeProdOrderPartslistPosID, "NCI_FK_ProdOrderPartslistPos_AlternativeProdOrderPartslistPosID");

            entity.HasIndex(e => e.BasedOnPartslistPosID, "NCI_FK_ProdOrderPartslistPos_BasedOnPartslistPosID");

            entity.HasIndex(e => e.FacilityLotID, "NCI_FK_ProdOrderPartslistPos_FacilityLotID");

            entity.HasIndex(e => e.MDProdOrderPartslistPosStateID, "NCI_FK_ProdOrderPartslistPos_MDProdOrderPartslistPosStateID");

            entity.HasIndex(e => e.MDToleranceStateID, "NCI_FK_ProdOrderPartslistPos_MDToleranceStateID");

            entity.HasIndex(e => e.MDUnitID, "NCI_FK_ProdOrderPartslistPos_MDUnitID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_ProdOrderPartslistPos_MaterialID");

            entity.HasIndex(e => e.ParentProdOrderPartslistPosID, "NCI_FK_ProdOrderPartslistPos_ParentProdOrderPartslistPosID");

            entity.HasIndex(e => e.SourceProdOrderPartslistID, "NCI_FK_ProdOrderPartslistPos_PositionUsedForProdOrderPartslistID");

            entity.HasIndex(e => e.ProdOrderBatchID, "NCI_FK_ProdOrderPartslistPos_ProdOrderBatchID");

            entity.HasIndex(e => e.ProdOrderPartslistID, "NCI_FK_ProdOrderPartslistPos_ProdOrderPartslistID");

            entity.HasIndex(e => new { e.ProdOrderBatchID, e.MaterialPosTypeIndex, e.ProdOrderPartslistID, e.MDProdOrderPartslistPosStateID }, "NCI_ProdOrderPartslistPos_OT");

            entity.HasIndex(e => new { e.ProdOrderPartslistID, e.ProdOrderPartslistPosID }, "NCI_ProdOrderPartslistPos_ProdOrderPartslistPosID_ProdOrderPartslistID");

            entity.HasIndex(e => new { e.ProdOrderPartslistPosID, e.ProdOrderPartslistID, e.MDProdOrderPartslistPosStateID }, "NCI_ProdOrderPartslistPos_ProdOrderPartslistPosID_ProdOrderPartslistID_MDProdOrderPartslistPosStateID");

            entity.Property(e => e.ProdOrderPartslistPosID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LineNumber)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ACClassTask).WithMany(p => p.ProdOrderPartslistPos_ACClassTask)
                .HasForeignKey(d => d.ACClassTaskID)
                .HasConstraintName("FK_ProdOrderPartslistPos_ACClassTask");

           entity.HasOne(d => d.ProdOrderPartslistPos1_AlternativeProdOrderPartslistPos).WithMany(p => p.ProdOrderPartslistPos_AlternativeProdOrderPartslistPos)
                .HasForeignKey(d => d.AlternativeProdOrderPartslistPosID)
                .HasConstraintName("FK_ProdOrderPartslistPos_AlternativeProdOrderPartslistPosID");

           entity.HasOne(d => d.BasedOnPartslistPos).WithMany(p => p.ProdOrderPartslistPos_BasedOnPartslistPos)
                .HasForeignKey(d => d.BasedOnPartslistPosID)
                .HasConstraintName("FK_ProdOrderPartslistPos_PartslistPos");

           entity.HasOne(d => d.FacilityLot).WithMany(p => p.ProdOrderPartslistPos_FacilityLot)
                .HasForeignKey(d => d.FacilityLotID)
                .HasConstraintName("FK_ProdOrderPartslistPos_FacilityLot");

           entity.HasOne(d => d.MDProdOrderPartslistPosState).WithMany(p => p.ProdOrderPartslistPos_MDProdOrderPartslistPosState)
                .HasForeignKey(d => d.MDProdOrderPartslistPosStateID)
                .HasConstraintName("FK_ProdOrderPartslistPos_MDProdOrderPartslistPosStateID");

           entity.HasOne(d => d.MDToleranceState).WithMany(p => p.ProdOrderPartslistPos_MDToleranceState)
                .HasForeignKey(d => d.MDToleranceStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslistPos_MDToleranceStateID");

           entity.HasOne(d => d.MDUnit).WithMany(p => p.ProdOrderPartslistPos_MDUnit)
                .HasForeignKey(d => d.MDUnitID)
                .HasConstraintName("FK_ProdOrderPartslistPos_MDUnitID");

           entity.HasOne(d => d.Material).WithMany(p => p.ProdOrderPartslistPos_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_ProdOrderPartslistPos_MaterialID");

           entity.HasOne(d => d.ProdOrderPartslistPos1_ParentProdOrderPartslistPos).WithMany(p => p.ProdOrderPartslistPos_ParentProdOrderPartslistPos)
                .HasForeignKey(d => d.ParentProdOrderPartslistPosID)
                .HasConstraintName("FK_ProdOrderPartslistPos_ParentProdOrderPartslistPosID");

           entity.HasOne(d => d.ProdOrderBatch).WithMany(p => p.ProdOrderPartslistPos_ProdOrderBatch)
                .HasForeignKey(d => d.ProdOrderBatchID)
                .HasConstraintName("FK_ProdOrderPartslistPos_ProdOrderBatch");

           entity.HasOne(d => d.ProdOrderPartslist).WithMany(p => p.ProdOrderPartslistPos_ProdOrderPartslist)
                .HasForeignKey(d => d.ProdOrderPartslistID)
                .HasConstraintName("FK_ProdOrderPartslistPos_ProdOrderPartslistID");

           entity.HasOne(d => d.SourceProdOrderPartslist).WithMany(p => p.ProdOrderPartslistPos_SourceProdOrderPartslist)
                .HasForeignKey(d => d.SourceProdOrderPartslistID)
                .HasConstraintName("FK_ProdOrderPartslistPos_ProdOrderPartslist");
        });

        modelBuilder.Entity<ProdOrderPartslistPosFacilityLot>(entity =>
        {
            entity.ToTable("ProdOrderPartslistPosFacilityLot");

            entity.HasIndex(e => new { e.ProdOrderPartslistPosID, e.FacilityLotID }, "UXPosLotUnique").IsUnique();

            entity.Property(e => e.ProdOrderPartslistPosFacilityLotID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.FacilityLot).WithMany(p => p.ProdOrderPartslistPosFacilityLot_FacilityLot)
                .HasForeignKey(d => d.FacilityLotID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslistPosFacilityLot_FacilityLot");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslistPosFacilityLot_ProdOrderPartslistPos");
        });

        modelBuilder.Entity<ProdOrderPartslistPosRelation>(entity =>
        {
            entity.HasKey(e => e.ProdOrderPartslistPosRelationID).HasName("PK_ProdOrderPartslistPosRelationID");

            entity.ToTable("ProdOrderPartslistPosRelation");

            entity.HasIndex(e => e.MDProdOrderPartslistPosStateID, "NCI_FK_ProdOrderPartslistPosRelation_MDProdOrderPartslistPosStateID");

            entity.HasIndex(e => e.MDToleranceStateID, "NCI_FK_ProdOrderPartslistPosRelation_MDToleranceStateID");

            entity.HasIndex(e => e.ParentProdOrderPartslistPosRelationID, "NCI_FK_ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelationID");

            entity.HasIndex(e => e.ProdOrderBatchID, "NCI_FK_ProdOrderPartslistPosRelation_ProdOrderBatchID");

            entity.HasIndex(e => e.SourceProdOrderPartslistPosID, "NCI_FK_ProdOrderPartslistPosRelation_SourceProdOrderPartslistPosID");

            entity.HasIndex(e => e.TargetProdOrderPartslistPosID, "NCI_FK_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID");

            entity.HasIndex(e => e.TargetProdOrderPartslistPosID, "NCI_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID_OT1");

            entity.HasIndex(e => new { e.TargetProdOrderPartslistPosID, e.Sequence, e.SourceProdOrderPartslistPosID, e.ProdOrderPartslistPosRelationID, e.TargetQuantity, e.ActualQuantity, e.TargetQuantityUOM, e.ActualQuantityUOM, e.ParentProdOrderPartslistPosRelationID, e.ProdOrderBatchID, e.MDToleranceStateID, e.MDProdOrderPartslistPosStateID }, "NCI_ProdOrderPartslistPosRelation_TargetProdOrderPartslistPosID_OT2");

            entity.HasIndex(e => new { e.ProdOrderPartslistPosRelationID, e.TargetProdOrderPartslistPosID, e.SourceProdOrderPartslistPosID }, "NCI_ProdOrderPartslistPosRelation_Target_Source");

            entity.Property(e => e.ProdOrderPartslistPosRelationID).ValueGeneratedNever();
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDProdOrderPartslistPosState).WithMany(p => p.ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState)
                .HasForeignKey(d => d.MDProdOrderPartslistPosStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState");

           entity.HasOne(d => d.MDToleranceState).WithMany(p => p.ProdOrderPartslistPosRelation_MDToleranceState)
                .HasForeignKey(d => d.MDToleranceStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslistPosRelation_MDToleranceState");

           entity.HasOne(d => d.ProdOrderPartslistPosRelation1_ParentProdOrderPartslistPosRelation).WithMany(p => p.ProdOrderPartslistPosRelation_ParentProdOrderPartslistPosRelation)
                .HasForeignKey(d => d.ParentProdOrderPartslistPosRelationID)
                .HasConstraintName("FK_ProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation");

           entity.HasOne(d => d.ProdOrderBatch).WithMany(p => p.ProdOrderPartslistPosRelation_ProdOrderBatch)
                .HasForeignKey(d => d.ProdOrderBatchID)
                .HasConstraintName("FK_ProdOrderPartslistPosRelation_ProdOrderBatch");

           entity.HasOne(d => d.SourceProdOrderPartslistPos).WithMany(p => p.ProdOrderPartslistPosRelation_SourceProdOrderPartslistPos)
                .HasForeignKey(d => d.SourceProdOrderPartslistPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslistPosRelation_ProdOrderPartslistPos1");

           entity.HasOne(d => d.TargetProdOrderPartslistPos).WithMany(p => p.ProdOrderPartslistPosRelation_TargetProdOrderPartslistPos)
                .HasForeignKey(d => d.TargetProdOrderPartslistPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProdOrderPartslistPosRelation_ProdOrderPartslistPos");
        });

        modelBuilder.Entity<ProdOrderPartslistPosSplit>(entity =>
        {
            entity.ToTable("ProdOrderPartslistPosSplit");

            entity.HasIndex(e => e.ProdOrderPartslistPosID, "NCI_FK_ProdOrderPartslistPosSplit_ProdOrderPartslistPosID");

            entity.Property(e => e.ProdOrderPartslistPosSplitID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.ProdOrderPartslistPosSplit_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .HasConstraintName("FK_ProdOrderPartslistPosSplit_ProdOrderPartslistPosID");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.ToTable("Rating");

            entity.HasIndex(e => e.CompanyID, "NCI_FK_Rating_CompanyID");

            entity.HasIndex(e => e.CompanyPersonID, "NCI_FK_Rating_CompanyPersonID");

            entity.HasIndex(e => e.DeliveryNoteID, "NCI_FK_Rating_DeliveryNoteID");

            entity.Property(e => e.RatingID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.Company).WithMany(p => p.Rating_Company)
                .HasForeignKey(d => d.CompanyID)
                .HasConstraintName("FK_Rating_Company");

           entity.HasOne(d => d.CompanyPerson).WithMany(p => p.Rating_CompanyPerson)
                .HasForeignKey(d => d.CompanyPersonID)
                .HasConstraintName("FK_Rating_CompanyPerson");

           entity.HasOne(d => d.DeliveryNote).WithMany(p => p.Rating_DeliveryNote)
                .HasForeignKey(d => d.DeliveryNoteID)
                .HasConstraintName("FK_Rating_DeliveryNote");
        });

        modelBuilder.Entity<RatingComplaint>(entity =>
        {
            entity.ToTable("RatingComplaint");

            entity.HasIndex(e => new { e.MDRatingComplaintTypeID, e.RatingID }, "IX_RatingComplaint").IsUnique();

            entity.Property(e => e.RatingComplaintID).ValueGeneratedNever();
            entity.Property(e => e.Comment).HasColumnType("ntext");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.MDRatingComplaintType).WithMany(p => p.RatingComplaint_MDRatingComplaintType)
                .HasForeignKey(d => d.MDRatingComplaintTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RatingComplaint_MDRatingComplaintType");

           entity.HasOne(d => d.Rating).WithMany(p => p.RatingComplaint_Rating)
                .HasForeignKey(d => d.RatingID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RatingComplaint_Rating");
        });

        modelBuilder.Entity<TandTv3FilterTracking>(entity =>
        {
            entity.Property(e => e.TandTv3FilterTrackingID).ValueGeneratedNever();
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.FilterDateFrom).HasColumnType("datetime");
            entity.Property(e => e.FilterDateTo).HasColumnType("datetime");
            entity.Property(e => e.FilterTrackingNo)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ItemSystemNo)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.TandTv3MDTrackingDirectionID)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.TandTv3MDTrackingStartItemTypeID)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);

           entity.HasOne(d => d.TandTv3MDTrackingDirection).WithMany(p => p.TandTv3FilterTracking_TandTv3MDTrackingDirection)
                .HasForeignKey(d => d.TandTv3MDTrackingDirectionID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3FilterTracking_TandTv3MDTrackingDirectionID");

           entity.HasOne(d => d.TandTv3MDTrackingStartItemType).WithMany(p => p.TandTv3FilterTracking_TandTv3MDTrackingStartItemType)
                .HasForeignKey(d => d.TandTv3MDTrackingStartItemTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3FilterTracking_TandTv3MDTrackingStartItemTypeID");
        });

        modelBuilder.Entity<TandTv3FilterTrackingMaterial>(entity =>
        {
            entity.Property(e => e.TandTv3FilterTrackingMaterialID).ValueGeneratedNever();

           entity.HasOne(d => d.Material).WithMany(p => p.TandTv3FilterTrackingMaterial_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_TandTv3FilterTrackingMaterial_MaterialID");

           entity.HasOne(d => d.TandTv3FilterTracking).WithMany(p => p.TandTv3FilterTrackingMaterial_TandTv3FilterTracking)
                .HasForeignKey(d => d.TandTv3FilterTrackingID)
                .HasConstraintName("FK_TandTv3FilterTrackingMaterial_TandTv3TandTv3FilterTrackingID");
        });

        modelBuilder.Entity<TandTv3MDBookingDirection>(entity =>
        {
            entity.Property(e => e.TandTv3MDBookingDirectionID).HasMaxLength(20);
        });

        modelBuilder.Entity<TandTv3MDTrackingDirection>(entity =>
        {
            entity.Property(e => e.TandTv3MDTrackingDirectionID).HasMaxLength(20);
        });

        modelBuilder.Entity<TandTv3MDTrackingStartItemType>(entity =>
        {
            entity.Property(e => e.TandTv3MDTrackingStartItemTypeID)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ACCaptionTranslation)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TandTv3MixPoint>(entity =>
        {
            entity.Property(e => e.TandTv3MixPointID).ValueGeneratedNever();

           entity.HasOne(d => d.InwardLot).WithMany(p => p.TandTv3MixPoint_InwardLot)
                .HasForeignKey(d => d.InwardLotID)
                .HasConstraintName("FK_TandTv3MixPoint_FacilityLotID");

           entity.HasOne(d => d.InwardMaterial).WithMany(p => p.TandTv3MixPoint_InwardMaterial)
                .HasForeignKey(d => d.InwardMaterialID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPoint_MaterialID");

           entity.HasOne(d => d.TandTv3Step).WithMany(p => p.TandTv3MixPoint_TandTv3Step)
                .HasForeignKey(d => d.TandTv3StepID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPoint_TandTv3TandTv3StepID");
        });

        modelBuilder.Entity<TandTv3MixPointDeliveryNotePos>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.DeliveryNotePosID }, "UIX_TandTv3MixPointDeliveryNotePos").IsUnique();

            entity.Property(e => e.TandTv3MixPointDeliveryNotePosID).ValueGeneratedNever();

           entity.HasOne(d => d.DeliveryNotePos).WithMany(p => p.TandTv3MixPointDeliveryNotePos_DeliveryNotePos)
                .HasForeignKey(d => d.DeliveryNotePosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointDeliveryNotePos_DeliveryNotePosID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointDeliveryNotePos_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointDeliveryNotePos_TandTv3TandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3MixPointFacility>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.TandTv3MDBookingDirectionID, e.FacilityID }, "UIX_TandTv3MixPointFacility").IsUnique();

            entity.Property(e => e.TandTv3MixPointFacilityID).ValueGeneratedNever();
            entity.Property(e => e.TandTv3MDBookingDirectionID)
                .IsRequired()
                .HasMaxLength(20);

           entity.HasOne(d => d.Facility).WithMany(p => p.TandTv3MixPointFacility_Facility)
                .HasForeignKey(d => d.FacilityID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacility_FacilityID");

           entity.HasOne(d => d.TandTv3MDBookingDirection).WithMany(p => p.TandTv3MixPointFacility_TandTv3MDBookingDirection)
                .HasForeignKey(d => d.TandTv3MDBookingDirectionID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacility_TandTv3TandTv3MDBookingDirectionID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointFacility_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacility_TandTv3TandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3MixPointFacilityBookingCharge>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.FacilityBookingChargeID }, "UIX_TandTv3MixPointFacilityBookingCharge").IsUnique();

            entity.Property(e => e.TandTv3MixPointFacilityBookingChargeID).ValueGeneratedNever();

           entity.HasOne(d => d.FacilityBookingCharge).WithMany(p => p.TandTv3MixPointFacilityBookingCharge_FacilityBookingCharge)
                .HasForeignKey(d => d.FacilityBookingChargeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacilityBookingCharge_FacilityBookingChargeID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacilityBookingCharge_TandTv3MixPoint");
        });

        modelBuilder.Entity<TandTv3MixPointFacilityLot>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.TandTv3MDBookingDirectionID, e.FacilityLotID }, "UIX_TandTv3MixPointFacilityLot").IsUnique();

            entity.Property(e => e.TandTv3MixPointFacilityLotID).ValueGeneratedNever();
            entity.Property(e => e.TandTv3MDBookingDirectionID)
                .IsRequired()
                .HasMaxLength(20);

           entity.HasOne(d => d.FacilityLot).WithMany(p => p.TandTv3MixPointFacilityLot_FacilityLot)
                .HasForeignKey(d => d.FacilityLotID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacilityLot_FacilityLotID");

           entity.HasOne(d => d.TandTv3MDBookingDirection).WithMany(p => p.TandTv3MixPointFacilityLot_TandTv3MDBookingDirection)
                .HasForeignKey(d => d.TandTv3MDBookingDirectionID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacilityLot_TandTv3TandTv3MDBookingDirectionID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointFacilityLot_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacilityLot_TandTv3TandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3MixPointFacilityPreBooking>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.FacilityPreBookingID }, "UIX_TandTv3MixPointFacilityPreBooking").IsUnique();

            entity.Property(e => e.TandTv3MixPointFacilityPreBookingID).ValueGeneratedNever();

           entity.HasOne(d => d.FacilityPreBooking).WithMany(p => p.TandTv3MixPointFacilityPreBooking_FacilityPreBooking)
                .HasForeignKey(d => d.FacilityPreBookingID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacilityPreBooking_FacilityPreBooking");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointFacilityPreBooking_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointFacilityPreBooking_TandTv3MixPoint");
        });

        modelBuilder.Entity<TandTv3MixPointInOrderPos>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.InOrderPosID }, "UIX_TandTv3MixPointInOrderPos").IsUnique();

            entity.Property(e => e.TandTv3MixPointInOrderPosID).ValueGeneratedNever();

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.TandTv3MixPointInOrderPos_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointInOrderPos_InOrderPosID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointInOrderPos_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointInOrderPos_TandTv3TandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3MixPointOutOrderPos>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.OutOrderPosID }, "UIX_TandTv3MixPointOutOrderPos").IsUnique();

            entity.Property(e => e.TandTv3MixPointOutOrderPosID).ValueGeneratedNever();

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.TandTv3MixPointOutOrderPos_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointOutOrderPos_OutOrderPosID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointOutOrderPos_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointOutOrderPos_TandTv3TandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3MixPointPickingPos>(entity =>
        {
            entity.Property(e => e.TandTv3MixPointPickingPosID).ValueGeneratedNever();

           entity.HasOne(d => d.PickingPos).WithMany(p => p.TandTv3MixPointPickingPos_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointPickingPos_PickingPosID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointPickingPos_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointPickingPos_TandTv3TandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3MixPointProdOrderPartslistPos>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.ProdOrderPartslistPosID }, "UIX_TandTv3MixPointProdOrderPartslistPos").IsUnique();

            entity.Property(e => e.TandTv3MixPointProdOrderPartslistPosID).ValueGeneratedNever();

           entity.HasOne(d => d.ProdOrderPartslistPos).WithMany(p => p.TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPos)
                .HasForeignKey(d => d.ProdOrderPartslistPosID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointProdOrderPartslistPos_ProdOrderPartslistPosID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointProdOrderPartslistPos_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointProdOrderPartslistPos_TandTv3TandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3MixPointProdOrderPartslistPosRelation>(entity =>
        {
            entity.HasIndex(e => new { e.TandTv3MixPointID, e.ProdOrderPartslistPosRelationID }, "UIX_TandTv3MixPointProdOrderPartslistPosRelation").IsUnique();

            entity.Property(e => e.TandTv3MixPointProdOrderPartslistPosRelationID).ValueGeneratedNever();

           entity.HasOne(d => d.ProdOrderPartslistPosRelation).WithMany(p => p.TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelation)
                .HasForeignKey(d => d.ProdOrderPartslistPosRelationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointProdOrderPartslistPosRelation_ProdOrderPartslistPosRelationID");

           entity.HasOne(d => d.TandTv3MixPoint).WithMany(p => p.TandTv3MixPointProdOrderPartslistPosRelation_TandTv3MixPoint)
                .HasForeignKey(d => d.TandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointProdOrderPartslistPosRelation_TandTv3TandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3MixPointRelation>(entity =>
        {
            entity.HasIndex(e => new { e.SourceTandTv3MixPointID, e.TargetTandTv3MixPointID }, "UIX_TandTv3MixPointRelation").IsUnique();

            entity.Property(e => e.TandTv3MixPointRelationID).ValueGeneratedNever();

           entity.HasOne(d => d.SourceTandTv3MixPoint).WithMany(p => p.TandTv3MixPointRelation_SourceTandTv3MixPoint)
                .HasForeignKey(d => d.SourceTandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointRelation_SourceTandTv3MixPointID");

           entity.HasOne(d => d.TargetTandTv3MixPoint).WithMany(p => p.TandTv3MixPointRelation_TargetTandTv3MixPoint)
                .HasForeignKey(d => d.TargetTandTv3MixPointID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TandTv3MixPointRelation_TargetTandTv3MixPointID");
        });

        modelBuilder.Entity<TandTv3Step>(entity =>
        {
            entity.Property(e => e.TandTv3StepID).ValueGeneratedNever();
            entity.Property(e => e.StepName)
                .HasMaxLength(150)
                .IsUnicode(false);

           entity.HasOne(d => d.TandTv3FilterTracking).WithMany(p => p.TandTv3Step_TandTv3FilterTracking)
                .HasForeignKey(d => d.TandTv3FilterTrackingID)
                .HasConstraintName("FK_TandTv3Step_TandTv3FilterTrackingID");
        });

        modelBuilder.Entity<Tourplan>(entity =>
        {
            entity.ToTable("Tourplan");

            entity.HasIndex(e => e.CompanyID, "NCI_FK_Tourplan_CompanyID");

            entity.HasIndex(e => e.MDTourID, "NCI_FK_Tourplan_MDTourID");

            entity.HasIndex(e => e.MDTourplanStateID, "NCI_FK_Tourplan_MDTourplanStateID");

            entity.HasIndex(e => e.TrailerFacilityID, "NCI_FK_Tourplan_TrailerFacilityID");

            entity.HasIndex(e => e.VehicleFacilityID, "NCI_FK_Tourplan_VehicleFacilityID");

            entity.HasIndex(e => e.VisitorVoucherID, "NCI_FK_Tourplan_VisitorVoucherID");

            entity.HasIndex(e => e.TourplanNo, "UIX_Tourplan").IsUnique();

            entity.Property(e => e.TourplanID).ValueGeneratedNever();
            entity.Property(e => e.ActDeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.FirstWeighingIdentityNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LastWeighingIdentityNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LoadingStation)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SecondWeighingIdentityNo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TourplanName)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.TourplanNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.Company).WithMany(p => p.Tourplan_Company)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tourplan_CompanyID");

           entity.HasOne(d => d.MDTour).WithMany(p => p.Tourplan_MDTour)
                .HasForeignKey(d => d.MDTourID)
                .HasConstraintName("FK_Tourplan_MDTourID");

           entity.HasOne(d => d.MDTourplanState).WithMany(p => p.Tourplan_MDTourplanState)
                .HasForeignKey(d => d.MDTourplanStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tourplan_MDCommissionPlanStateID");

           entity.HasOne(d => d.TrailerFacility).WithMany(p => p.Tourplan_TrailerFacility)
                .HasForeignKey(d => d.TrailerFacilityID)
                .HasConstraintName("FK_Tourplan_TrailerFacilityID");

           entity.HasOne(d => d.VehicleFacility).WithMany(p => p.Tourplan_VehicleFacility)
                .HasForeignKey(d => d.VehicleFacilityID)
                .HasConstraintName("FK_Tourplan_VehicleFacilityID");

           entity.HasOne(d => d.VisitorVoucher).WithMany(p => p.Tourplan_VisitorVoucher)
                .HasForeignKey(d => d.VisitorVoucherID)
                .HasConstraintName("FK_Tourplan_VisitorVoucherID");
        });

        modelBuilder.Entity<TourplanConfig>(entity =>
        {
            entity.ToTable("TourplanConfig");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_TourplanConfig_MaterialID");

            entity.HasIndex(e => e.ParentTourplanConfigID, "NCI_FK_TourplanConfig_ParentTourplanConfigID");

            entity.HasIndex(e => e.TourplanID, "NCI_FK_TourplanConfig_TourplanID");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FK_TourplanConfig_VBiACClassID");

            entity.HasIndex(e => e.VBiACClassPropertyRelationID, "NCI_FK_TourplanConfig_VBiACClassPropertyRelationID");

            entity.HasIndex(e => e.VBiValueTypeACClassID, "NCI_FK_TourplanConfig_VBiValueTypeACClassID");

            entity.Property(e => e.TourplanConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.Material).WithMany(p => p.TourplanConfig_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_TourplanConfig_MaterialID");

           entity.HasOne(d => d.TourplanConfig1_ParentTourplanConfig).WithMany(p => p.TourplanConfig_ParentTourplanConfig)
                .HasForeignKey(d => d.ParentTourplanConfigID)
                .HasConstraintName("FK_TourplanConfig_ParentTourplanConfigID");

           entity.HasOne(d => d.Tourplan).WithMany(p => p.TourplanConfig_Tourplan)
                .HasForeignKey(d => d.TourplanID)
                .HasConstraintName("FK_TourplanConfig_TourplanID");

           entity.HasOne(d => d.VBiACClass).WithMany(p => p.TourplanConfig_VBiACClass)
                .HasForeignKey(d => d.VBiACClassID)
                .HasConstraintName("FK_TourplanConfig_ACClassID");

           entity.HasOne(d => d.VBiACClassPropertyRelation).WithMany(p => p.TourplanConfig_VBiACClassPropertyRelation)
                .HasForeignKey(d => d.VBiACClassPropertyRelationID)
                .HasConstraintName("FK_TourplanConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBiValueTypeACClass).WithMany(p => p.TourplanConfig_VBiValueTypeACClass)
                .HasForeignKey(d => d.VBiValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourplanConfig_DataTypeACClassID");
        });

        modelBuilder.Entity<TourplanPos>(entity =>
        {
            entity.HasIndex(e => e.CompanyAddressID, "NCI_FK_TourplanPos_CompanyAddressID");

            entity.HasIndex(e => e.CompanyAddressUnloadingpointID, "NCI_FK_TourplanPos_CompanyAddressUnloadingpointID");

            entity.HasIndex(e => e.CompanyID, "NCI_FK_TourplanPos_CompanyID");

            entity.HasIndex(e => e.MDTimeRangeID, "NCI_FK_TourplanPos_MDTimeRangeID");

            entity.HasIndex(e => e.MDTourplanPosStateID, "NCI_FK_TourplanPos_MDTourplanPosStateID");

            entity.HasIndex(e => e.TourplanID, "NCI_FK_TourplanPos_TourplanID");

            entity.Property(e => e.TourplanPosID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.CompanyAddress).WithMany(p => p.TourplanPos_CompanyAddress)
                .HasForeignKey(d => d.CompanyAddressID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourplanPos_CompanyAddressID");

           entity.HasOne(d => d.CompanyAddressUnloadingpoint).WithMany(p => p.TourplanPos_CompanyAddressUnloadingpoint)
                .HasForeignKey(d => d.CompanyAddressUnloadingpointID)
                .HasConstraintName("FK_TourplanPos_CompanyAddressUnloadingPointID");

           entity.HasOne(d => d.Company).WithMany(p => p.TourplanPos_Company)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourplanPos_CompanyID");

           entity.HasOne(d => d.MDTimeRange).WithMany(p => p.TourplanPos_MDTimeRange)
                .HasForeignKey(d => d.MDTimeRangeID)
                .HasConstraintName("FK_TourplanPos_MDTimeRangeID");

           entity.HasOne(d => d.MDTourplanPosState).WithMany(p => p.TourplanPos_MDTourplanPosState)
                .HasForeignKey(d => d.MDTourplanPosStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TourplanPos_MDTourplanPosStateID");

           entity.HasOne(d => d.Tourplan).WithMany(p => p.TourplanPos_Tourplan)
                .HasForeignKey(d => d.TourplanID)
                .HasConstraintName("FK_TourplanPos_TourplanID");
        });

        modelBuilder.Entity<UserSettings>(entity =>
        {
            entity.Property(e => e.UserSettingsID).ValueGeneratedNever();

           entity.HasOne(d => d.InvoiceCompanyAddress).WithMany(p => p.UserSettings_InvoiceCompanyAddress)
                .HasForeignKey(d => d.InvoiceCompanyAddressID)
                .HasConstraintName("FK_UserSettings_CompanyAddress");

           entity.HasOne(d => d.InvoiceCompanyPerson).WithMany(p => p.UserSettings_InvoiceCompanyPerson)
                .HasForeignKey(d => d.InvoiceCompanyPersonID)
                .HasConstraintName("FK_UserSettings_CompanyPerson");

           entity.HasOne(d => d.TenantCompany).WithMany(p => p.UserSettings_TenantCompany)
                .HasForeignKey(d => d.TenantCompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSettings_Company");

           entity.HasOne(d => d.VBUser).WithMany(p => p.UserSettings_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserSettings_VBUser");
        });

        modelBuilder.Entity<VBConfig>(entity =>
        {
            entity.ToTable("VBConfig");

            entity.HasIndex(e => e.ACClassID, "NCI_FK_VBConfig_ACClassID");

            entity.HasIndex(e => e.ACClassPropertyRelationID, "NCI_FK_VBConfig_ACClassPropertyRelationID");

            entity.HasIndex(e => e.ParentVBConfigID, "NCI_FK_VBConfig_ParentVBConfigID");

            entity.HasIndex(e => e.ValueTypeACClassID, "NCI_FK_VBConfig_ValueTypeACClassID");

            entity.Property(e => e.VBConfigID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Expression).HasColumnType("text");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.KeyACUrl).IsUnicode(false);
            entity.Property(e => e.LocalConfigACUrl).IsUnicode(false);
            entity.Property(e => e.PreConfigACUrl).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACClass).WithMany(p => p.VBConfig_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_VBConfig_ACClassID");

           entity.HasOne(d => d.ACClassPropertyRelation).WithMany(p => p.VBConfig_ACClassPropertyRelation)
                .HasForeignKey(d => d.ACClassPropertyRelationID)
                .HasConstraintName("FK_VBConfig_ACClassPropertyRelationID");

           entity.HasOne(d => d.VBConfig1_ParentVBConfig).WithMany(p => p.VBConfig_ParentVBConfig)
                .HasForeignKey(d => d.ParentVBConfigID)
                .HasConstraintName("FK_VBConfig_ParentVBConfigID");

           entity.HasOne(d => d.ValueTypeACClass).WithMany(p => p.VBConfig_ValueTypeACClass)
                .HasForeignKey(d => d.ValueTypeACClassID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VBConfig_ValueTypeACClassID");
        });

        modelBuilder.Entity<VBGroup>(entity =>
        {
            entity.ToTable("VBGroup");

            entity.Property(e => e.VBGroupID).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VBGroupName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VBGroupRight>(entity =>
        {
            entity.ToTable("VBGroupRight");

            entity.HasIndex(e => e.ACClassDesignID, "NCI_FK_VBGroupRight_ACClassDesignID");

            entity.HasIndex(e => e.ACClassID, "NCI_FK_VBGroupRight_ACClassID");

            entity.HasIndex(e => e.ACClassMethodID, "NCI_FK_VBGroupRight_ACClassMethodID");

            entity.HasIndex(e => e.ACClassPropertyID, "NCI_FK_VBGroupRight_ACClassPropertyID");

            entity.HasIndex(e => e.VBGroupID, "NCI_FK_VBGroupRight_VBGroupID");

            entity.Property(e => e.VBGroupRightID).ValueGeneratedNever();

           entity.HasOne(d => d.ACClassDesign).WithMany(p => p.VBGroupRight_ACClassDesign)
                .HasForeignKey(d => d.ACClassDesignID)
                .HasConstraintName("FK_VBGroupRight_ACClassDesignID");

           entity.HasOne(d => d.ACClass).WithMany(p => p.VBGroupRight_ACClass)
                .HasForeignKey(d => d.ACClassID)
                .HasConstraintName("FK_VBGroupRight_ACClassID");

           entity.HasOne(d => d.ACClassMethod).WithMany(p => p.VBGroupRight_ACClassMethod)
                .HasForeignKey(d => d.ACClassMethodID)
                .HasConstraintName("FK_VBGroupRight_ACClassMethodID");

           entity.HasOne(d => d.ACClassProperty).WithMany(p => p.VBGroupRight_ACClassProperty)
                .HasForeignKey(d => d.ACClassPropertyID)
                .HasConstraintName("FK_VBGroupRight_ACClassPropertyID");

           entity.HasOne(d => d.VBGroup).WithMany(p => p.VBGroupRight_VBGroup)
                .HasForeignKey(d => d.VBGroupID)
                .HasConstraintName("FK_VBGroupRight_VBGroupID");
        });

        modelBuilder.Entity<VBLanguage>(entity =>
        {
            entity.HasKey(e => e.VBLanguageID).HasName("PK_MDLanguage");

            entity.ToTable("VBLanguage");

            entity.HasIndex(e => e.VBKey, "UIX_MDLanguage").IsUnique();

            entity.Property(e => e.VBLanguageID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VBKey)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.VBLanguageCode)
                .IsRequired()
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.VBNameTrans)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<VBLicense>(entity =>
        {
            entity.ToTable("VBLicense");

            entity.Property(e => e.VBLicenseID).ValueGeneratedNever();
            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LicenseNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PackageSystem).IsRequired();
            entity.Property(e => e.PackageSystem1)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.ProjectNo)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SystemCommon)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.SystemCommon1).HasMaxLength(256);
            entity.Property(e => e.SystemDB)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.SystemDS)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.SystemKey).IsUnicode(false);
            entity.Property(e => e.SystemRemote)
                .IsRequired()
                .IsUnicode(false);
        });

        modelBuilder.Entity<VBNoConfiguration>(entity =>
        {
            entity.HasKey(e => e.VBNoConfigurationID).HasName("PK_MDNoConfiguration");

            entity.ToTable("VBNoConfiguration");

            entity.Property(e => e.VBNoConfigurationID).ValueGeneratedNever();
            entity.Property(e => e.CurrentDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UsedDelimiter)
                .IsRequired()
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.UsedPrefix)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VBNoConfigurationName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");
        });

        modelBuilder.Entity<VBSystem>(entity =>
        {
            entity.ToTable("VBSystem");

            entity.Property(e => e.VBSystemID).ValueGeneratedNever();
            entity.Property(e => e.Company)
                .IsRequired()
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ProjectNo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.SystemCommon)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.SystemCommon1).HasMaxLength(256);
            entity.Property(e => e.SystemCommonPublic)
                .IsRequired()
                .IsUnicode(false);
            entity.Property(e => e.SystemInternal1).HasMaxLength(256);
            entity.Property(e => e.SystemInternal2).IsUnicode(false);
            entity.Property(e => e.SystemInternal3).HasMaxLength(256);
            entity.Property(e => e.SystemName)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SystemPrivate).IsUnicode(false);
            entity.Property(e => e.SystemRemote).IsUnicode(false);
        });

        modelBuilder.Entity<VBSystemColumns>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VBSystemColumns");

            entity.Property(e => e.columnname)
                .IsRequired()
                .HasMaxLength(128);
            entity.Property(e => e.columntype).HasMaxLength(128);
            entity.Property(e => e.tablename)
                .IsRequired()
                .HasMaxLength(128);
        });

        modelBuilder.Entity<VBTranslationView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VBTranslationView");

            entity.Property(e => e.ACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ACProjectName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MandatoryACIdentifier)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MandatoryACURLCached).IsUnicode(false);
            entity.Property(e => e.TableName)
                .IsRequired()
                .HasMaxLength(248)
                .IsUnicode(false);
            entity.Property(e => e.TranslationValue).IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VBUser>(entity =>
        {
            entity.ToTable("VBUser");

            entity.HasIndex(e => e.MenuACClassDesignID, "NCI_FK_VBUser_MenuACClassDesignID");

            entity.HasIndex(e => e.VBLanguageID, "NCI_FK_VBUser_VBLanguageID");

            entity.Property(e => e.VBUserID).ValueGeneratedNever();
            entity.Property(e => e.Initials)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VBUserName)
                .IsRequired()
                .HasMaxLength(40)
                .IsUnicode(false);
            entity.Property(e => e.VBUserNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MenuACClassDesign).WithMany(p => p.VBUser_MenuACClassDesign)
                .HasForeignKey(d => d.MenuACClassDesignID)
                .HasConstraintName("FK_VBUser_MenuACClassDesignID");

           entity.HasOne(d => d.VBLanguage).WithMany(p => p.VBUser_VBLanguage)
                .HasForeignKey(d => d.VBLanguageID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VBUser_MDLanguageID");
        });

        modelBuilder.Entity<VBUserACClassDesign>(entity =>
        {
            entity.ToTable("VBUserACClassDesign");

            entity.HasIndex(e => e.ACClassDesignID, "NCI_FK_VBUserACClassDesign_ACClassDesignID");

            entity.HasIndex(e => e.VBUserID, "NCI_FK_VBUserACClassDesign_VBUserID");

            entity.Property(e => e.VBUserACClassDesignID).ValueGeneratedNever();
            entity.Property(e => e.ACIdentifier)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLDesign)
                .IsRequired()
                .HasColumnType("text");

           entity.HasOne(d => d.ACClassDesign).WithMany(p => p.VBUserACClassDesign_ACClassDesign)
                .HasForeignKey(d => d.ACClassDesignID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_VBUserACClassDesign_ACClassDesignID");

           entity.HasOne(d => d.VBUser).WithMany(p => p.VBUserACClassDesign_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .HasConstraintName("FK_VBUserACClassDesign_VBUserID");
        });

        modelBuilder.Entity<VBUserACProject>(entity =>
        {
            entity.ToTable("VBUserACProject");

            entity.HasIndex(e => e.ACProjectID, "NCI_FK_VBUserACProject_ACProjectID");

            entity.HasIndex(e => e.VBUserID, "NCI_FK_VBUserACProject_VBUserID");

            entity.Property(e => e.VBUserACProjectID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.ACProject).WithMany(p => p.VBUserACProject_ACProject)
                .HasForeignKey(d => d.ACProjectID)
                .HasConstraintName("FK_VBUserACProject_ACProjectID");

           entity.HasOne(d => d.VBUser).WithMany(p => p.VBUserACProject_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .HasConstraintName("FK_VBUserACProject_VBUserID");
        });

        modelBuilder.Entity<VBUserGroup>(entity =>
        {
            entity.ToTable("VBUserGroup");

            entity.HasIndex(e => e.VBGroupID, "NCI_FK_VBUserGroup_VBGroupID");

            entity.HasIndex(e => e.VBUserID, "NCI_FK_VBUserGroup_VBUserID");

            entity.Property(e => e.VBUserGroupID).ValueGeneratedNever();
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.VBGroup).WithMany(p => p.VBUserGroup_VBGroup)
                .HasForeignKey(d => d.VBGroupID)
                .HasConstraintName("FK_VBUserGroup_VBGroupID");

           entity.HasOne(d => d.VBUser).WithMany(p => p.VBUserGroup_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .HasConstraintName("FK_VBUserGroup_VBUserID");
        });

        modelBuilder.Entity<VBUserInstance>(entity =>
        {
            entity.ToTable("VBUserInstance");

            entity.HasIndex(e => e.VBUserID, "NCI_FK_VBUserInstance_VBUserID");

            entity.Property(e => e.VBUserInstanceID).ValueGeneratedNever();
            entity.Property(e => e.Hostname)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.LoginDate).HasColumnType("datetime");
            entity.Property(e => e.LogoutDate).HasColumnType("datetime");
            entity.Property(e => e.ServerIPV4)
                .IsRequired()
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ServerIPV6)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SessionInfo).HasColumnType("text");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

           entity.HasOne(d => d.VBUser).WithMany(p => p.VBUserInstance_VBUser)
                .HasForeignKey(d => d.VBUserID)
                .HasConstraintName("FK_VBUserInstance_VBUserID");
        });

        modelBuilder.Entity<Visitor>(entity =>
        {
            entity.ToTable("Visitor");

            entity.HasIndex(e => e.MDVisitorCardID, "NCI_FK_Visitor_MDVisitorCardID");

            entity.HasIndex(e => e.MaterialID, "NCI_FK_Visitor_MaterialID");

            entity.HasIndex(e => e.TrailerFacilityID, "NCI_FK_Visitor_TrailerFacilityID");

            entity.HasIndex(e => e.VehicleFacilityID, "NCI_FK_Visitor_VehicleFacilityID");

            entity.HasIndex(e => e.VisitedCompanyID, "NCI_FK_Visitor_VisitedCompanyID");

            entity.HasIndex(e => e.VisitorCompanyID, "NCI_FK_Visitor_VisitorCompanyID");

            entity.HasIndex(e => e.VisitorCompanyPersonID, "NCI_FK_Visitor_VisitorCompanyPersonID");

            entity.HasIndex(e => e.VisitorNo, "UIX_Visitor_VisitorNo").IsUnique();

            entity.Property(e => e.VisitorID).ValueGeneratedNever();
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ScheduledFromDate).HasColumnType("datetime");
            entity.Property(e => e.ScheduledToDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VisitorNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDVisitorCard).WithMany(p => p.Visitor_MDVisitorCard)
                .HasForeignKey(d => d.MDVisitorCardID)
                .HasConstraintName("FK_Visitor_MDVisitorCardID");

           entity.HasOne(d => d.Material).WithMany(p => p.Visitor_Material)
                .HasForeignKey(d => d.MaterialID)
                .HasConstraintName("FK_Visitor_MaterialID");

           entity.HasOne(d => d.TrailerFacility).WithMany(p => p.Visitor_TrailerFacility)
                .HasForeignKey(d => d.TrailerFacilityID)
                .HasConstraintName("FK_Visitor_TrailerFacilityID");

           entity.HasOne(d => d.VehicleFacility).WithMany(p => p.Visitor_VehicleFacility)
                .HasForeignKey(d => d.VehicleFacilityID)
                .HasConstraintName("FK_Visitor_VehicleFacilityID");

           entity.HasOne(d => d.VisitedCompany).WithMany(p => p.Visitor_VisitedCompany)
                .HasForeignKey(d => d.VisitedCompanyID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Visitor_VisitedCompanyID");

           entity.HasOne(d => d.VisitorCompany).WithMany(p => p.Visitor_VisitorCompany)
                .HasForeignKey(d => d.VisitorCompanyID)
                .HasConstraintName("FK_Visitor_VisitorCompanyID");

           entity.HasOne(d => d.VisitorCompanyPerson).WithMany(p => p.Visitor_VisitorCompanyPerson)
                .HasForeignKey(d => d.VisitorCompanyPersonID)
                .HasConstraintName("FK_Visitor_VisitorCompanyPersonID");
        });

        modelBuilder.Entity<VisitorVoucher>(entity =>
        {
            entity.ToTable("VisitorVoucher");

            entity.HasIndex(e => e.MDVisitorCardID, "NCI_FK_VisitorVoucher_MDVisitorCardID");

            entity.HasIndex(e => e.MDVisitorVoucherStateID, "NCI_FK_VisitorVoucher_MDVisitorVoucherStateID");

            entity.HasIndex(e => e.TrailerFacilityID, "NCI_FK_VisitorVoucher_TrailerFacilityID");

            entity.HasIndex(e => e.VehicleFacilityID, "NCI_FK_VisitorVoucher_VehicleFacilityID");

            entity.HasIndex(e => e.VisitorCompanyID, "NCI_FK_VisitorVoucher_VisitorCompanyID");

            entity.HasIndex(e => e.VisitorCompanyPersonID, "NCI_FK_VisitorVoucher_VisitorCompanyPersonID");

            entity.HasIndex(e => e.VisitorID, "NCI_FK_VisitorVoucher_VisitorID");

            entity.HasIndex(e => e.WeighingID, "NCI_FK_VisitorVoucher_WeighingID");

            entity.HasIndex(e => e.VisitorVoucherNo, "UIX_VisitorVoucher_VisitorVoucherNo").IsUnique();

            entity.Property(e => e.VisitorVoucherID).ValueGeneratedNever();
            entity.Property(e => e.CheckInDate).HasColumnType("datetime");
            entity.Property(e => e.CheckOutDate).HasColumnType("datetime");
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.MDVisitorCard).WithMany(p => p.VisitorVoucher_MDVisitorCard)
                .HasForeignKey(d => d.MDVisitorCardID)
                .HasConstraintName("FK_VisitorVoucher_MDVisitorCardID");

           entity.HasOne(d => d.MDVisitorVoucherState).WithMany(p => p.VisitorVoucher_MDVisitorVoucherState)
                .HasForeignKey(d => d.MDVisitorVoucherStateID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VisitorVoucher_MDVisitorVoucherStateID");

           entity.HasOne(d => d.TrailerFacility).WithMany(p => p.VisitorVoucher_TrailerFacility)
                .HasForeignKey(d => d.TrailerFacilityID)
                .HasConstraintName("FK_VisitorVoucher_TrailerFacilityID");

           entity.HasOne(d => d.VehicleFacility).WithMany(p => p.VisitorVoucher_VehicleFacility)
                .HasForeignKey(d => d.VehicleFacilityID)
                .HasConstraintName("FK_VisitorVoucher_VehicleFacilityID");

           entity.HasOne(d => d.VisitorCompany).WithMany(p => p.VisitorVoucher_VisitorCompany)
                .HasForeignKey(d => d.VisitorCompanyID)
                .HasConstraintName("FK_VisitorVoucher_VisitorCompanyID");

           entity.HasOne(d => d.VisitorCompanyPerson).WithMany(p => p.VisitorVoucher_VisitorCompanyPerson)
                .HasForeignKey(d => d.VisitorCompanyPersonID)
                .HasConstraintName("FK_VisitorVoucher_VisitorCompanyPersonID");

           entity.HasOne(d => d.Visitor).WithMany(p => p.VisitorVoucher_Visitor)
                .HasForeignKey(d => d.VisitorID)
                .HasConstraintName("FK_VisitorVoucher_VisitorID");
        });

        modelBuilder.Entity<Weighing>(entity =>
        {
            entity.ToTable("Weighing");

            entity.HasIndex(e => e.VBiACClassID, "NCI_FK_Weighing_ACClassID");

            entity.HasIndex(e => e.WeighingNo, "UIX_Weighing_WeighingNo").IsUnique();

            entity.Property(e => e.WeighingID).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IdentNr)
                .IsRequired()
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.InsertDate).HasColumnType("datetime");
            entity.Property(e => e.InsertName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WeighingNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.WeighingTotalXML)
                .IsRequired()
                .HasColumnType("text");
            entity.Property(e => e.XMLConfig).HasColumnType("text");

           entity.HasOne(d => d.InOrderPos).WithMany(p => p.Weighing_InOrderPos)
                .HasForeignKey(d => d.InOrderPosID)
                .HasConstraintName("FK_Weighing_InOrderPosID");

           entity.HasOne(d => d.LabOrderPos).WithMany(p => p.Weighing_LabOrderPos)
                .HasForeignKey(d => d.LabOrderPosID)
                .HasConstraintName("FK_Weighing_LabOrderPosID");

           entity.HasOne(d => d.OutOrderPos).WithMany(p => p.Weighing_OutOrderPos)
                .HasForeignKey(d => d.OutOrderPosID)
                .HasConstraintName("FK_Weighing_OutOrderPosID");

           entity.HasOne(d => d.PickingPos).WithMany(p => p.Weighing_PickingPos)
                .HasForeignKey(d => d.PickingPosID)
                .HasConstraintName("FK_Weighing_PickingPosID");

           entity.HasOne(d => d.VisitorVoucher).WithMany(p => p.Weighing_VisitorVoucher)
                .HasForeignKey(d => d.VisitorVoucherID)
                .HasConstraintName("FK_Weighing_VisitorVoucherID");
        });

        modelBuilder.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications)
                    // For change tracking proxies if UseChangeTrackingProxies() is set: 
                    //.HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications)
                    .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
