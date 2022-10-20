using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.facility;
using gip.mes.datamodel;
using gip.mes.autocomponent;

namespace gip.bso.test
{
    [ACClassInfo(Const.PackName_VarioTest, "en{'Test posting'}de{'Testbuchungen'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, Const.QueryPrefix + Facility.ClassName)]
    public class VXBSOFacilityTest : ACBSOvb
    {
        ACComponent _FacilityManager;
        //ACMethodBooking _BookingParameter;

        Material _Material1 = null;
        //Material _Material2 = null;

        Facility _FacilityLocation = null;

        Facility _Facility1 = null;
        Facility _Facility2 = null;

        MDReleaseState _MDReleaseState = null;

        HUManager _PackagingManager = null;
        #region c´tors

        public VXBSOFacilityTest(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            _FacilityManager = this.ACUrlCommand("\\Service\\FacilityManager") as ACComponent;
            _PackagingManager = new HUManager(DatabaseApp, Root);

            //CurrentHandlingUnit = "<Test></Test>";

            _Material1 = DatabaseApp.Material.Where(c => c.MaterialNo == "5000").First();
            CurrentMaterial = _Material1;
            //_Material2 = Database.Material.Include("MDWeightUnit").Include("MDQuantityUnit").Where(c => c.MaterialNo == "10010").First();

            //_FacilityLocation = Database.FacilityLocation.Where(c => c.FacilityLocationNo == "Testlager").First();

            //_Facility1 = Database.Facility.Where(c => c.FacilityNo == "TL10").First();
            //_Facility2 = Database.Facility.Where(c => c.FacilityNo == "TL11").First();

            _MDReleaseState = DatabaseApp.MDReleaseState.Where(c => c.IsDefault).First();
            return true;
        }
        #endregion

        #region BSO->ACProperty
        Material _CurrentMaterial;

        [ACPropertyCurrent(9999, Material.ClassName)]
        public Material CurrentMaterial
        {
            get
            {
                return _CurrentMaterial;
            }
            set
            {
                _CurrentMaterial = value;
                OnPropertyChanged("CurrentMaterial");
            }
        }

        //String _CurrentHandlingUnit;
        //[ACPropertyCurrent(9999, "HandlingUnit")]
        //public String CurrentHandlingUnit
        //{
        //    get
        //    {
        //        return _CurrentHandlingUnit;
        //    }
        //    set
        //    {
        //        OnPropertyChanged("CurrentHandlingUnit");
        //    }
        //}

        //String _CurrentHandlingUnit2;
        //[ACPropertyCurrent(9999, "HandlingUnit2")]
        //public String CurrentHandlingUnit2
        //{
        //    get
        //    {
        //        return _CurrentHandlingUnit2;
        //    }
        //    set
        //    {
        //        OnPropertyChanged("CurrentHandlingUnit2");
        //    }
        //}
        #endregion
        
        #region BSO->ACMethod->HU
        [ACMethodCommand("xxx", "en{'HandlingUnit A 0'}de{'HandlingUnit A 0'}", 9999)]
        public void CreateHandlingUnitA0()
        {
//            HandlingUnit handlingUnit = _PackagingManager.CreateHandlingUnit(_Material1.StorageMaterialUnit, 0, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);
//            CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'HandlingUnit A 1'}de{'HandlingUnit A 1'}", 9999)]
        public void CreateHandlingUnitA1()
        {
            //HandlingUnit handlingUnit = _PackagingManager.CreateHandlingUnit(_Material1.StorageMaterialUnit, 1, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);
            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'HandlingUnit A 2'}de{'HandlingUnit A 2'}", 9999)]
        public void CreateHandlingUnitA2()
        {
            //HandlingUnit handlingUnit = _PackagingManager.CreateHandlingUnit(_Material1.StorageMaterialUnit, 2, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);
            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'HandlingUnit A 3'}de{'HandlingUnit A 3'}", 9999)]
        public void CreateHandlingUnitA3()
        {
            //HandlingUnit handlingUnit = _PackagingManager.CreateHandlingUnit(_Material1.StorageMaterialUnit, 3, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);
            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }

        [ACMethodCommand("xxx", "en{'HandlingUnit B 0'}de{'HandlingUnit B 0'}", 9999)]
        public void CreateHandlingUnitB0()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();
            //HandlingUnit handlingUnit = _PackagingManager.CreateHandlingUnit(packagingHierarchy, 0, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);
            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'HandlingUnit B 1'}de{'HandlingUnit B 1'}", 9999)]
        public void CreateHandlingUnitB1()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();
            //HandlingUnit handlingUnit = _PackagingManager.CreateHandlingUnit(packagingHierarchy, 1, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);
            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'HandlingUnit B 2'}de{'HandlingUnit B 2'}", 9999)]
        public void CreateHandlingUnitB2()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();
            //HandlingUnit handlingUnit = _PackagingManager.CreateHandlingUnit(packagingHierarchy, 2, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);
            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'HandlingUnit B 3'}de{'HandlingUnit B 3'}", 9999)]
        public void CreateHandlingUnitB3()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();
            //HandlingUnit handlingUnit = _PackagingManager.CreateHandlingUnit(packagingHierarchy, 3, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);
            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }


        [ACMethodCommand("xxx", "en{'Einpacken Charge1 200'}de{'Einpacken Charge1 200'}", 9999)]
        public void Einpacken0()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);
            //HUData huData = _PackagingManager.InPacking(handlingUnit, _Material1.MaterialNo, 200, 200, "Charge1");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }

        [ACMethodCommand("xxx", "en{'Einpacken Charge1 400'}de{'Einpacken Charge1 400'}", 9999)]
        public void Einpacken1()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);
            //HUData huData = _PackagingManager.InPacking(handlingUnit, _Material1.MaterialNo, 400, 400, "Charge1");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'Einpacken Charge1 450'}de{'Einpacken Charge1 450'}", 9999)]
        public void Einpacken2()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);
            //HUData huData = _PackagingManager.InPacking(handlingUnit, _Material1.MaterialNo, 450, 450, "Charge1");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'Einpacken Charge1 1000'}de{'Einpacken Charge1 1000'}", 9999)]
        public void Einpacken3()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);
            //HUData huData = _PackagingManager.InPacking(handlingUnit, _Material1.MaterialNo, 1000, 1000, "Charge1");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }

        [ACMethodCommand("xxx", "en{'Einpacken Charge2 200'}de{'Einpacken Charge2 200'}", 9999)]
        public void EinpackenC20()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);
            //HUData huData = _PackagingManager.InPacking(handlingUnit, _Material1.MaterialNo, 200, 200, "Charge2");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }

        [ACMethodCommand("xxx", "en{'Einpacken Charge2 400'}de{'Einpacken Charge2 400'}", 9999)]
        public void EinpackenC21()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);
            //HUData huData = _PackagingManager.InPacking(handlingUnit, _Material1.MaterialNo, 400, 400, "Charge2");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'Einpacken Charge2 450'}de{'Einpacken Charge2 450'}", 9999)]
        public void EinpackenC22()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);
            //HUData huData = _PackagingManager.InPacking(handlingUnit, _Material1.MaterialNo, 450, 450, "Charge2");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }
        [ACMethodCommand("xxx", "en{'Einpacken Charge2 1000'}de{'Einpacken Charge2 1000'}", 9999)]
        public void EinpackenC23()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);
            //HUData huData = _PackagingManager.InPacking(handlingUnit, _Material1.MaterialNo, 1000, 1000, "Charge2");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
        }

        [ACMethodCommand("xxx", "en{'Auspacken 0'}de{'Auspacken 0'}", 9999)]
        public void Auspacken0()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.OutPacking(handlingUnit, 0);

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Auspacken 1'}de{'Auspacken 1'}", 9999)]
        public void Auspacken1()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.OutPacking(handlingUnit, 1);

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Auspacken 2'}de{'Auspacken 2'}", 9999)]
        public void Auspacken2()
        {
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.OutPacking(handlingUnit, 2);

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Auspacken 3'}de{'Auspacken 3'}", 9999)]
        public void Auspacken3()
        {
        }

        [ACMethodCommand("xxx", "en{'Umpacken 0'}de{'Umpacken 0'}", 9999)]
        public void Umpacken0()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();

            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.RePacking(handlingUnit, packagingHierarchy, 0);

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Umpacken 1'}de{'Umpacken 1'}", 9999)]
        public void Umpacken1()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c=>c.MaterialUnitName=="Variante2").First();

            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.RePacking(handlingUnit, packagingHierarchy, 1);

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Umpacken 2'}de{'Umpacken 2'}", 9999)]
        public void Umpacken2()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c=>c.MaterialUnitName=="Variante2").First();

            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.RePacking(handlingUnit, packagingHierarchy, 2);

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Umpacken 3'}de{'Umpacken 3'}", 9999)]
        public void Umpacken3()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c=>c.MaterialUnitName=="Variante2").First();

            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.RePacking(handlingUnit, packagingHierarchy, 3);

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }

        [ACMethodCommand("xxx", "en{'Abbuchen 200'}de{'Abbuchen 200'}", 9999)]
        public void Abbuchen0()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c=>c.MaterialUnitName=="Variante2").First();

            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.OutPackingMaterial(handlingUnit, _Material1.MaterialNo, 200, 0, "");

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Abbuchen 400'}de{'Abbuchen 400'}", 9999)]
        public void Abbuchen1()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();

            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.OutPackingMaterial(handlingUnit, _Material1.MaterialNo, 400, 0, "");

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Abbuchen 450'}de{'Abbuchen 450'}", 9999)]
        public void Abbuchen2()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();

            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.OutPackingMaterial(handlingUnit, _Material1.MaterialNo, 450, 0, "");

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }
        [ACMethodCommand("xxx", "en{'Abbuchen 1000'}de{'Abbuchen 1000'}", 9999)]
        public void Abbuchen3()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();

            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HUData huData = _PackagingManager.OutPackingMaterial(handlingUnit, _Material1.MaterialNo, 1000, 0, "");

            //if (handlingUnit != null)
            //{
            //    CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //}
            //else
            //{
            //    CurrentHandlingUnit = "";
            //}
        }

        [ACMethodCommand("xxx", "en{'Teilen HULevel 0'}de{'Teilen HULevel 0'}", 9999)]
        public void Teilen0()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HandlingUnit handlingUnit2 = _PackagingManager.CreateHandlingUnit(packagingHierarchy, 0, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);

            //HUData huData = _PackagingManager.SplitPackaging(handlingUnit, handlingUnit2, _Material1.MaterialNo, 400, 0, "");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //CurrentHandlingUnit2 = _PackagingManager.SerializeHU(handlingUnit2);
        }
        [ACMethodCommand("xxx", "en{'Teilen HULevel 1'}de{'Teilen HULevel 1'}", 9999)]
        public void Teilen1()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HandlingUnit handlingUnit2 = _PackagingManager.CreateHandlingUnit(packagingHierarchy, 1, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);

            //HUData huData = _PackagingManager.SplitPackaging(handlingUnit, handlingUnit2, _Material1.MaterialNo, 400, 0, "");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //CurrentHandlingUnit2 = _PackagingManager.SerializeHU(handlingUnit2);
        }
        [ACMethodCommand("xxx", "en{'Teilen HULevel 2'}de{'Teilen HULevel 2'}", 9999)]
        public void Teilen2()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HandlingUnit handlingUnit2 = _PackagingManager.CreateHandlingUnit(packagingHierarchy, 2, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);

            //HUData huData = _PackagingManager.SplitPackaging(handlingUnit, handlingUnit2, _Material1.MaterialNo, 400, 0, "");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //CurrentHandlingUnit2 = _PackagingManager.SerializeHU(handlingUnit2);
        }
        [ACMethodCommand("xxx", "en{'Teilen HULevel 3'}de{'Teilen HULevel 3'}", 9999)]
        public void Teilen3()
        {
            //IPackagingHierarchy packagingHierarchy = _Material1.MaterialUnit_Material.Where(c => c.MaterialUnitName == "Variante2").First();
            //HandlingUnit handlingUnit = _PackagingManager.DeserializeHU(CurrentHandlingUnit);

            //HandlingUnit handlingUnit2 = _PackagingManager.CreateHandlingUnit(packagingHierarchy, 3, _Material1.MaterialNo, _Material1.StorageMDQuantityUnit, _Material1.StorageMDWeightUnit, _Material1.StorageMaterialUnit.NetWeight);

            //HUData huData = _PackagingManager.SplitPackaging(handlingUnit, handlingUnit2, _Material1.MaterialNo, 400, 0, "");

            //CurrentHandlingUnit = _PackagingManager.SerializeHU(handlingUnit);
            //CurrentHandlingUnit2 = _PackagingManager.SerializeHU(handlingUnit2);
        }

        #endregion

        #region BSO->ACMethod

        #region Buchung Zugang (InwardMovementUnscheduled)
        [ACMethodCommand("xxx", "en{'Lagerzugang'}de{'Lagerzugang'}", 9999)]
        public void InwardFacilityMovement()
        {
            ACMethodBooking parameter = _FacilityManager.ACUrlACTypeSignature("!InwardMovement_FacilityCharge", Database.ContextIPlus) as ACMethodBooking;

            parameter.OutwardFacilityLocation = _FacilityLocation;
            parameter.OutwardFacility = _Facility1;
            parameter.OutwardMaterial = _Material1;
            parameter.OutwardQuantity = 100;

            // TODO: Damir nach Mengeneinheitsumstellung
            //parameter.QuantityUnit = _Material1.MDQuantityUnit;
            //parameter.MDWeightUnit = _Material1.BaseMDWeightUnit;
            parameter.MDReleaseState = _MDReleaseState;
            parameter.IgnoreManagement = false;
            parameter.StorageDate = DateTime.Now;

            _FacilityManager.ACUrlCommand("!BookFacility", parameter, this.DatabaseApp);
        }
        #endregion

        #region Buchung Abgang(OutwardMovementUnscheduled)
        [ACMethodCommand("xxx", "en{'Lagerabgang'}de{'Lagerabgang'}", 9999)]
        public void OutwardFacilityMovement()
        {
            ACMethodBooking parameter = _FacilityManager.ACUrlACTypeSignature("!OutwardMovement_FacilityCharge", Database.ContextIPlus) as ACMethodBooking;

            parameter.InwardFacilityLocation = _FacilityLocation;
            parameter.InwardFacility = _Facility1;
            parameter.InwardMaterial = _Material1;
            parameter.InwardQuantity = 100;

            // TODO: Damir nach Mengeneinheitsumstellung
            //parameter.QuantityUnit = _Material1.MDQuantityUnit;
            //parameter.MDWeightUnit = _Material1.BaseMDWeightUnit;
            //parameter.ReleaseState = _MDReleaseState;
            //parameter.IgnoreManagement = false;
            //parameter.StorageDate = DateTime.Now;
            _FacilityManager.ACUrlCommand("!BookFacility", parameter, this.DatabaseApp);
        }
        #endregion

        #region Umlagerung (Relocation)
        [ACMethodCommand("xxx", "en{'Umlagerung'}de{'Umlagerung'}", 9999)]
        public void FacilityRelocation()
        {
            ACMethodBooking parameter = _FacilityManager.ACUrlACTypeSignature("!Relocation_FacilityCharge", Database.ContextIPlus) as ACMethodBooking;

            parameter.InwardFacilityLocation = _FacilityLocation;
            parameter.InwardFacility = _Facility1;
            parameter.InwardMaterial = _Material1;
            parameter.InwardQuantity = 100;

            parameter.OutwardFacilityLocation = _FacilityLocation;
            parameter.OutwardFacility = _Facility2;
            parameter.OutwardMaterial = _Material1;
            parameter.OutwardQuantity = 100;

            // TODO: Damir nach Mengeneinheitsumstellung
            //parameter.QuantityUnit = _Material1.MDQuantityUnit;
            //parameter.MDWeightUnit = _Material1.BaseMDWeightUnit;
            //parameter.ReleaseState = _MDReleaseState;
            //parameter.IgnoreManagement = false;
            //parameter.StorageDate = DateTime.Now;


            _FacilityManager.ACUrlCommand("!BookFacility", parameter, this.DatabaseApp);
        }
        #endregion

        #endregion


        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case"CreateHandlingUnitA0":
                    CreateHandlingUnitA0();
                    return true;
                case"CreateHandlingUnitA1":
                    CreateHandlingUnitA1();
                    return true;
                case"CreateHandlingUnitA2":
                    CreateHandlingUnitA2();
                    return true;
                case"CreateHandlingUnitA3":
                    CreateHandlingUnitA3();
                    return true;
                case"CreateHandlingUnitB0":
                    CreateHandlingUnitB0();
                    return true;
                case"CreateHandlingUnitB1":
                    CreateHandlingUnitB1();
                    return true;
                case"CreateHandlingUnitB2":
                    CreateHandlingUnitB2();
                    return true;
                case"CreateHandlingUnitB3":
                    CreateHandlingUnitB3();
                    return true;
                case"Einpacken0":
                    Einpacken0();
                    return true;
                case"Einpacken1":
                    Einpacken1();
                    return true;
                case"Einpacken2":
                    Einpacken2();
                    return true;
                case"Einpacken3":
                    Einpacken3();
                    return true;
                case"EinpackenC20":
                    EinpackenC20();
                    return true;
                case"EinpackenC21":
                    EinpackenC21();
                    return true;
                case"EinpackenC22":
                    EinpackenC22();
                    return true;
                case"EinpackenC23":
                    EinpackenC23();
                    return true;
                case"Auspacken0":
                    Auspacken0();
                    return true;
                case"Auspacken1":
                    Auspacken1();
                    return true;
                case"Auspacken2":
                    Auspacken2();
                    return true;
                case"Auspacken3":
                    Auspacken3();
                    return true;
                case"Umpacken0":
                    Umpacken0();
                    return true;
                case"Umpacken1":
                    Umpacken1();
                    return true;
                case"Umpacken2":
                    Umpacken2();
                    return true;
                case"Umpacken3":
                    Umpacken3();
                    return true;
                case"Abbuchen0":
                    Abbuchen0();
                    return true;
                case"Abbuchen1":
                    Abbuchen1();
                    return true;
                case"Abbuchen2":
                    Abbuchen2();
                    return true;
                case"Abbuchen3":
                    Abbuchen3();
                    return true;
                case"Teilen0":
                    Teilen0();
                    return true;
                case"Teilen1":
                    Teilen1();
                    return true;
                case"Teilen2":
                    Teilen2();
                    return true;
                case"Teilen3":
                    Teilen3();
                    return true;
                case"InwardFacilityMovement":
                    InwardFacilityMovement();
                    return true;
                case"OutwardFacilityMovement":
                    OutwardFacilityMovement();
                    return true;
                case"FacilityRelocation":
                    FacilityRelocation();
                    return true;
            }
                return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion


    }


}
