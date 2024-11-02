// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;

namespace gip2006.variobatch.processapplication
{
    /// <summary>
    /// Baseclass for multiplexing Configuration-Parameter
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'VBBSOSelDlgPluginConfigMUX'}de{'VBBSOSelDlgPluginConfigMUX'}", Global.ACKinds.TACBSOGlobal, Global.ACStorableTypes.NotStorable, false, true)]
    public class VBBSOSelDlgPluginConfigMUX : ACComponent
    {
        #region c'tors
        public VBBSOSelDlgPluginConfigMUX(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            if (ParentACComponent is VBBSOControlDialog)
            {
                CurrentACComponent = (ParentACComponent as VBBSOControlDialog).CurrentSelection;
                (ParentACComponent as VBBSOControlDialog).PropertyChanged += new PropertyChangedEventHandler(VBControldialog_PropertyChanged);
            }
            else if(ParentACComponent is VBBSOACComponentExplorer)
            {
                CurrentACComponent = (ParentACComponent as VBBSOACComponentExplorer).CurrentACComponent;
                (ParentACComponent as VBBSOACComponentExplorer).PropertyChanged += VBBSOSelDlgPluginConfigMUX_PropertyChanged;
            }
            return true;
        }



        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (ParentACComponent is VBBSOControlDialog)
            {
                (ParentACComponent as VBBSOControlDialog).PropertyChanged -= new PropertyChangedEventHandler(VBControldialog_PropertyChanged);
            }
            else if (ParentACComponent is VBBSOACComponentExplorer)
                (ParentACComponent as VBBSOACComponentExplorer).PropertyChanged -= VBBSOSelDlgPluginConfigMUX_PropertyChanged;
            
            if (_CurrentACComponent != null)
            {
                _CurrentACComponent.Detach();
                _CurrentACComponent = null;
            }

            this._CurrentACComponent = null;
            return base.ACDeInit(deleteACClassTask);
        }
        #endregion

        #region Properties
        ACRef<IACObject> _CurrentACComponent;
        [ACPropertyInfo(9999)]
        public IACObject CurrentACComponent
        {
            get
            {
                if (_CurrentACComponent == null)
                    return null;
                return _CurrentACComponent;
            }
            set
            {
                bool objectSwapped = true;
                if (_CurrentACComponent != null)
                {
                    if (_CurrentACComponent != value)
                    {
                        _CurrentACComponent.Detach();
                    }
                    else
                        objectSwapped = false;
                }
                if (value == null)
                    _CurrentACComponent = null;
                else
                    _CurrentACComponent = new ACRef<IACObject>(value, this);
                if (_CurrentACComponent != null)
                {
                    if (objectSwapped)
                    {
                        if ((_CurrentACComponent.ValueT.ACType.ACKind == Global.ACKinds.TPAProcessModule) || (_CurrentACComponent.ValueT.ACType.ACKind == Global.ACKinds.TPAModule))
                        {
                            if (_CurrentACComponent.ValueT is ACComponentProxy)
                                (_CurrentACComponent.ValueT as ACComponentProxy).InvokeACUrlCommand((_CurrentACComponent.ValueT as IACComponent).ACUrl + "\\CONV!SelectControlModule");
                            else
                                _CurrentACComponent.ValueT.ACUrlCommand("CONV!SelectControlModule");
                        }
                    }
                }
                OnPropertyChanged("CurrentACComponent");
            }
        }
        #endregion

        #region Methods
        void VBControldialog_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentSelection")
            {
                CurrentACComponent = (ParentACComponent as VBBSOControlDialog).CurrentSelection;
            }
        }

        void VBBSOSelDlgPluginConfigMUX_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentACComponent")
                CurrentACComponent = (ParentACComponent as VBBSOACComponentExplorer).CurrentACComponent;
        }
        #endregion
    }
}

