/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MMI.Tests
{
    internal class WindowsFact :
#if _LINUX
        Attribute
#else
        FactAttribute
#endif
    {
    }
}
