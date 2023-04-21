// ***********************************************************************
// Assembly         : gip.core.datamodel
// Author           : DLisak
// Created          : 10-16-2012
//
// Last Modified By : DLisak
// Last Modified On : 01-19-2013
// ***********************************************************************
// <copyright file="WeighingValue.cs" company="gip mbh, Oftersheim, Germany">
//     Copyright (c) gip mbh, Oftersheim, Germany. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    /// <summary>
    /// Class WeighingValue
    /// </summary>
    [DataContract]
    public class WeighingValue
    {
        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <value>The scale.</value>
        [DataMember]
        public ACRef<gip.core.datamodel.ACClass> Scale { get; private set; }

        /// <summary>
        /// Gets or sets the ident nr.
        /// </summary>
        /// <value>The ident nr.</value>
        [DataMember]
        public String IdentNr { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [DataMember]
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the encrypted value.
        /// </summary>
        /// <value>The encrypted value.</value>
        [DataMember]
        public String EncryptedValue { get; set; }
    }
}
