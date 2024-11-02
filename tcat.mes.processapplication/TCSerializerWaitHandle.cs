// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tcat.mes.processapplication
{
    public class TCSerializerWaitHandle : EventWaitHandle
    {
        [SecuritySafeCritical]
        public TCSerializerWaitHandle(bool initialState, EventResetMode mode, int NewRequestID)
            : base(initialState, mode)
        {
            _RequestID = NewRequestID;
            TimedOut = false;
        }

        public object ACMethod
        {
            get;
            set;
        }

        public object RemoteMethodInvocationResult
        {
            get;
            set;
        }

        public bool TimedOut
        {
            get;
            set;
        }

        private int _RequestID = 0;
        public int RequestID
        {
            get
            {
                return _RequestID;
            }
        }
    }
}
