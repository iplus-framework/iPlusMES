// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
// ***********************************************************************
// Assembly         : gip.bso.facility
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 10-16-2012
// ***********************************************************************
// <copyright file="BSOFacilityOverviewExplorer.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gip.bso.manufacturing
{
    /// <summary>
    /// Business object or App for tasks by barcode in manufacturing.
    /// Currently an empty placeholder: the class derives from ACBSOvb and adds no properties or methods of its own.
    /// It serves as the anchor for the barcode task designs (ACClassDesign) of the manufacturing module; the actual
    /// barcode task logic is implemented in the process modules/functions that are bound to these designs.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOBarcodeTasksManu&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Tasks by Barcode in manufacturing'}de{'Aufgaben per Barcode in der Produktion'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Business object or App for tasks by barcode in manufacturing.
        Currently an empty placeholder: the class derives from ACBSOvb and adds no properties or methods of its own.
        It serves as the anchor for the barcode task designs (ACClassDesign) of the manufacturing module; the actual barcode task logic is implemented in the process modules/functions that are bound to these designs.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOBarcodeTasksManu&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOBarcodeTasksManu : ACBSOvb
    {

        #region c´tors

        public BSOBarcodeTasksManu(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion

    }
}
