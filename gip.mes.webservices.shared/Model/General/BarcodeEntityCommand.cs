﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace gip.mes.webservices
{
    [DataContract(Name = "cBEC")]
    public class BarcodeEntityCommand
    {
        [DataMember(Name = "xACMN")]
        public string ACMethodName
        {
            get;
            set;
        }

        [DataMember(Name = "xACC")]
        public string ACCaption
        {
            get;
            set;
        }

        [DataMember(Name = "xACMI")]
        public bool ACMethodInvoked
        {
            get;
            set;
        }
    }
}