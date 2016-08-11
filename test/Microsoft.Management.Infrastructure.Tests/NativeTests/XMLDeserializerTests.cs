/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests;
using Xunit;

namespace MMI.Tests.Native
{
    public class XMLDeserializerTests : NativeDeserializerTestsBase
    {
        public XMLDeserializerTests() : base(MI_SerializationFormat.XML)
        {
        }
        
        [WindowsFact]
        public void CanDeserializeInstance()
        {
            this.VerifyRoundTripInstance();
        }

        [WindowsFact]
        public void CanDeserializeClass()
        {
            this.VerifyRoundTripClass();
        }

        [WindowsFact]
        public void CanDeserializeInstanceWithCallback()
        {
            this.VerifyRoundtripWithCallback();
        }
    }
}
