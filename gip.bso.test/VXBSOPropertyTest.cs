// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using gip.mes.autocomponent;

namespace gip.bso.test
{
    #region Damir Typtest
    //[ACClassPropertyBindingSource("Int32TestVariante5", typeof(Int32))]
    //[ACClassPropertyBindingTarget("MaterialTestVariante4", typeof(Material))]

    // Mit Class-Attributen laut Norbert, geht das nicht!!!
    //[ACClassPropertyBindingSource("Int32TestVariante6", typeof(Int32))]
    #endregion

    [ACClassInfo(Const.PackName_VarioTest, "en{'Propertytest'}de{'Eigenschaftstest'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class VXBSOPropertyTest : ACBSOvb
    {
        #region c´tors

        public VXBSOPropertyTest(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            //DatabaseMode = DatabaseModes.OwnDB;

            //_EventSubscr = new ACPointEventSubscr(this, "EventSubscr", 0);
            //_ACMixing = new ACRef<IACComponent>("\\Produktion1\\M1\\Mixing", ACRef<IACComponent>.Mode.AutoStartStop, this);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;
            // Damir Test:
            //Material x = new Material();
            //x.MaterialName1 = "Hallo";

            //if (_ACMixing.IsObjLoaded)
            //{
            //    IACMember x = _ACMixing.ValueT.GetMember("Sollgewicht");
            //    if (x != null)
            //        ACMemberList.Add(x);
            //    x = _ACMixing.ValueT.GetMember("Artikel");
            //    if (x != null)
            //        ACMemberList.Add(x);
            //    x = _ACMixing.ValueT.GetMember("Kommando");
            //    if (x != null)
            //        ACMemberList.Add(x);
            //    x = _ACMixing.ValueT.GetMember("Istgewicht");
            //    if (x != null)
            //        ACMemberList.Add(x);
            //    x = _ACMixing.ValueT.GetMember("Quellzelle");
            //    if (x != null)
            //        ACMemberList.Add(x);
            //    x = _ACMixing.ValueT.GetMember("MixingInfo");
            //    if (x != null)
            //        ACMemberList.Add(x);
            //    x = _ACMixing.ValueT.GetMember("MixingInfoList");
            //    if (x != null)
            //        ACMemberList.Add(x);
            //}

            //_CurrentMaterial = Database.Material.Where(c => c.MaterialNo == "4711").First();

            //_TestThread = new Thread(OnTimer);
            //_TestThread.Start();

            return true;
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            //if (_TestThread != null)
            //    _TestThread.Abort();

            /*if (_ACMixing.IsAttached)
            {
                _ACMixing.Detach();
                //_ACMixing.ACUrlCommand("\\Brötchenlinie2\\Mischer1\\~Mischen");
                //_ACMixing.Detach();
                //ACUrlCommand("\\Brötchenlinie2\\Mischer1\\~Mischen");
                //_ACMixing = null;
            }
            if (_Beltscale.IsAttached)
            {
                EventSubscr.UnSubscribeAllEvents(_Beltscale.Obj);
                _Beltscale.Detach();
            }
            if (_Mixer.IsAttached)
            {
                EventSubscr.UnSubscribeAllEvents(_Beltscale.Obj);
                _Mixer.Detach();
            }*/

            return await base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Typtest

        /*string _CurrentName = "Daniel";
        [ACPropertyCurrent(9999, "Name", "Name")]
        public string CurrentName
        {
            get
            {
                return _CurrentName;
            }
            set
            {
                _CurrentName = value;
                OnPropertyChanged("CurrentName");
            }
        }

        Material _CurrentMaterial = null;
        [ACPropertyCurrent(9999, Material.ClassName, Material.ClassName)]
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
        }*/

        #region Damir Typtest
        // Methode wird aufgrufen für Objekte die als Assembly instanziert werden, oder für Proxy-Objekte auf Client-Seite
        // Falls 
        /*new public static IACPropertyBase CreateACPropertyOfUnknownType(ACClassProperty acClassProperty, IACObjectWithBinding forACComponent)
        {
            switch (acClassProperty.ACIdentifier)
            {
                /*case "MaterialTestVariante3":
                    return ACPropertyFactory<Material>.New(acClassProperty, forACComponent);
                case "MaterialTestVariante4":
                    return ACPropertyFactory<Material>.New(acClassProperty, forACComponent);*/
        /*case "MaterialSource":
            return ACPropertyFactory<Material>.New(acClassProperty, forACComponent);*/
        /*default:
            return ACObjectNav<ACProject>.CreateACPropertyOfUnknownType(acClassProperty, forACComponent);
    }
}*/
        #endregion


        #region Damir Typtest
        //private ACRef<IACComponent> _ACMixing;

        /*[ACPropertyBindingTarget]
        public IACPropertyNet<Int32> Int32Target { get; set; }
        public void OnSetInt32Target(IACPropertyNetValueEvent valueEvent)
        {
            Int32 stop = (valueEvent as ACPropertyValueEvent<Int32>).Value;
        }

        [ACPropertyBindingSource]
        public IACPropertyNet<Int32> Int32Source { get; set; }
        public void OnSetInt32Source(IACPropertyNetValueEvent valueEvent)
        {
            Int32 stop = (valueEvent as ACPropertyValueEvent<Int32>).Value;
            //Type
        }

        //[ACPropertyBindingSource]
        [ACPropertyCurrent(9999, "MaterialSource")]
        public Material MaterialSource { get; set; }
        public void OnSetMaterialSource(IACPropertyNetValueEvent valueEvent)
        {
            //Int32 stop = (valueEvent as ACPropertyValueEvent<Int32>).Value;
        }


        private IACPropertyNet<double> _Sollgewicht = null;
        [ACProperty()]
        public IACPropertyNet<double> Sollgewicht 
        {
            get
            {
                return _Sollgewicht;
            }
        }

        private IACPropertyNet<double> _Istgewicht = null;
        [ACProperty()]
        public IACPropertyNet<double> Istgewicht 
        {
            get
            {   
                return _Istgewicht;
            }
        }

        private IACPropertyNet<string> _Artikel = null;
        [ACProperty()]
        public IACPropertyNet<string> Artikel
        {
            get
            {
                return _Artikel;
            }
        }

        private IACPropertyNet<BitAccessForInt16> _Kommando = null;
        [ACProperty()]
        public IACPropertyNet<BitAccessForInt16> Kommando
        {
            get
            {
                return _Kommando;
            }
        }
        
        private IACPropertyNet<int> _Quellzelle = null;
        [ACProperty()]
        public IACPropertyNet<int> Quellzelle 
        {
            get
            {
                return _Quellzelle;
            }
        }


        private IACPropertyNet<ACMixingInfo> _MixingInfo1 = null;
        [ACProperty()]
        public IACPropertyNet<ACMixingInfo> MixingInfo1
        {
            get
            {
                return _MixingInfo1;
            }
        }


        private IACPropertyNet<BindingList<ACMixingInfo>> _MixingInfo1List = null;
        [ACProperty()]
        public IACPropertyNet<BindingList<ACMixingInfo>> MixingInfo1List
        {
            get
            {
                return _MixingInfo1List;
            }
        }

        BindingList<ACMixingInfo> _MixingInfoList = new BindingList<ACMixingInfo>();
        [ACPropertyList(9999, "MixingInfo")]
        public IEnumerable<ACMixingInfo> MixingInfoList
        {
            get
            {
                if (MixingInfo1List.ValueT == null)
                    return null;
                _MixingInfoList = new BindingList<ACMixingInfo>();
                foreach (ACMixingInfo info in MixingInfo1List.ValueT)
                {
                    _MixingInfoList.Add(info);
                }
                return _MixingInfoList;
            }
        }


        ACMixingInfo _CurrentMixingInfo;
        [ACPropertyCurrent(9999, "MixingInfo")]
        public ACMixingInfo CurrentACMixingInfo
        {
            get
            {
                return _CurrentMixingInfo;
            }
            set
            {
                _CurrentMixingInfo = value;
                OnPropertyChanged("CurrentACMixingInfo");
            }
        }

        ACMixingInfo _SelectedMixingInfo;
        [ACPropertySelected(9999, "MixingInfo")]
        public ACMixingInfo SelectedMixingInfo
        {
            get
            {
                return _SelectedMixingInfo;
            }
            set
            {
                _SelectedMixingInfo = value;
                OnPropertyChanged("SelectedMixingInfo");
            }
        }



        double _CurrentMultiplyResult = 0;
        [ACPropertyCurrent(9999, "CurrentMultiplyResult")]
        public double CurrentMultiplyResult
        {
            get
            {
                return _CurrentMultiplyResult;
            }
            set
            {
                _CurrentMultiplyResult = value;
                OnPropertyChanged("CurrentMultiplyResult");
            }
        }*/


        //[ACMethodCommand("Multipliziere")]
        //public void MultiplyValues()
        //{
        //    /*if (_ACMixing.IsObjLoaded)
        //    {

        //        object result = ACUrlCommand(_ACMixing.ACUrl + "!MultiplyValues", Istgewicht.ValueT, Quellzelle.ValueT );
        //        if (result != null)
        //            CurrentMultiplyResult = (double) result;
        //    }*/
        //}

        // Variante 1: Ohne Delegat-Methode UND PropertyInfo
        /*[ACPropertyBindingTarget]
        public IACProperty<Int16> Int16TestVariante1 { get; set; }

        // Variante 2: Als Get/Set-Funktion auf normalen Typ ohne Delegat-Methode UND PropertyInfo
        private IACProperty<Int32> _Int32TestVariante2 = null;
        [ACPropertyBindingSource]
        public Int32 Int32TestVariante2
        {
            get
            {
                if (_Int32TestVariante2 == null)
                    return 0;
                return _Int32TestVariante2.Value;
            }
            set
            {
                if (_Int32TestVariante2 != null)
                {
                    _Int32TestVariante2.Value = value;
                }
            }
        }

        // Variante 3: Mit Delegat-Methode UND PropertyInfo: FAVORIT!!
        [ACPropertyBindingSource]
        public IACProperty<Material> MaterialTestVariante3 { get; set; }
        public void OnSetACPropertyMaterial(IACPropertyNetValueEvent valueEvent)
        {
        }

        // Variante 4: Generic mit Delegat-Methode OHNE PropertyInfo jedoch als Klassenregistrierung
        // Macht nicht soviel Sinn aufgrund erhöhter Fehlergefahr bei zwei Deklarationen
        // Variante 3 ist besser!
        public IACProperty<Material> MaterialTestVariante4 = null;
        public void OnSetACPropertyMaterial2(IACPropertyNetValueEvent valueEvent)
        {
        }

        // Variante 5: Direketer Zugriff auf ACPropertyServer-Variable nicht möglich (nur mit SetPropertyValue())
        // aber mit Delegat-Methode OHNE PropertyInfo jedoch als Klassenregistrierung
        public void OnSetACPropertyInt32(IACPropertyNetValueEvent valueEvent)
        {
            valueEvent.Handled = true;
        }

        [ACPropertyBindingSource]
        public IACProperty<Int32> Int32TestVariante6 { get; set; }
        public void OnSetACPropertyInt32TestVariante6(IACPropertyNetValueEvent valueEvent)
        {
            Int32 stop = (valueEvent as ACPropertyValueEvent<Int32>).Value;
        }*/


        /*ACPointEventSubscr _EventSubscr;
        [ACPropertyEventPoint(0, false)]
        public ACPointEventSubscr EventSubscr
        {
            get
            {
                return _EventSubscr;
            }
        }


        private ACRef<IACObjectWithBinding> _Beltscale = new ACRef<IACObjectWithBinding>("\\Verladung1\\WB1", ACRef<IACObjectWithBinding>.Mode.AutoStartStop);
        private ACRef<IACObjectWithBinding> _Mixer = new ACRef<IACObjectWithBinding>("\\Produktion1\\M1", ACRef<IACObjectWithBinding>.Mode.AutoStartStop);
        Thread _TestThread;
        bool subscribed = false;
        private void OnTimer()
        {
            while (true)
            {
                Thread.Sleep(20000);
                if (_Beltscale.IsObjLoaded && _Mixer.IsObjLoaded)
                {
                    //_Beltscale.Obj.MaxTempExEvent += EventCallback;
                    if (!subscribed)
                    {
                        _EventCounter = 0;
                        //subscribed = EventSubscr.SubscribeAllEvents(_Beltscale.Obj, EventCallback);
                        //MappingServicePoint
                        IACPointNetBase MappingPoint = ((ACObject)_Beltscale.Obj).GetPointNet("MappingServicePoint");
                        if (MappingPoint != null)
                            MappingPoint.Subscribe();
                        MappingPoint = ((ACObject)_Mixer.Obj).GetPointNet("MappingClientPoint");
                        if (MappingPoint != null)
                            MappingPoint.Subscribe();
                        subscribed = true;
                    }
                    if (subscribed && _EventCounter > 50)
                    {
                        //subscribed = !EventSubscr.UnSubscribeAllEvents(_Beltscale.Obj);
                    }
                }
            }
        }


        [ACMethodInfo("EventCallback")]
        public void EventCallback(IACPointNetBase sender, ACEventArgs e, IACObject wrapObject)
        {
            _EventCounter++;
        }

        private int _EventCounter = 0;*/

        #endregion


        /*CFGBSOACPropertyTest _Configuration = null;
        public CFGBSOACPropertyTest Configuration
        {
            get
            {
                if (_Configuration == null)
                    _Configuration = ACUrlCommand("\\Configurations\\CFGBSOACPropertyTest") as CFGBSOACPropertyTest;

                return _Configuration;
            }
        }*/
        #endregion

        //[ACPropertyCurrent(9999, "Name", "Name")]
        //public string CurrentNameX
        //{
        //    get;
        //    set;
        //}

        //[ACMethodInfo("","",9999)]
        //public void TestIt()
        //{
        //}
    }
}
