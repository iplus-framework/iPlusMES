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

namespace gip.bso.facility
{
    /// <summary>
    /// Businessobject or App for a warehouse overview by barcode.
    /// Currently an empty stub: the class only derives from ACBSOvb and defines no properties, methods, queries or designs of its own.
    /// It is registered in the Businessobject-Manager under the name "Warehouse overview by Barcode" (Lagerübersicht per Barcode) but offers no functionality yet.
    /// For a working warehouse overview use BSOFacilityOverview or the facility charge/lot overview BSOs instead.
    /// Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOBarcodeFacilityOverview&amp;type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
    /// </summary>
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Warehouse overview by Barcode'}de{'Lagerübersicht per Barcode'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true,
        Description = @"Businessobject or App for a warehouse overview by barcode.
        Currently an empty stub: the class only derives from ACBSOvb and defines no properties, methods, queries or designs of its own.
        It is registered in the Businessobject-Manager under the name ""Warehouse overview by Barcode"" (Lagerübersicht per Barcode) but offers no functionality yet.
        For a working warehouse overview use BSOFacilityOverview or the facility charge/lot overview BSOs instead.
        Visit the https://github.com/search?q=org%3Aiplus-framework%20BSOBarcodeFacilityOverview&type=code on github to read the source code and get a full understanding, or use the github MCP API and search for the class name.
        ")]
    public class BSOBarcodeFacilityOverview : ACBSOvb
    {

        #region c´tors

        public BSOBarcodeFacilityOverview(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        #endregion

    }
}
