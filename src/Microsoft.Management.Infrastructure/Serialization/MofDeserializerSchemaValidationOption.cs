/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Internal;
using Microsoft.Management.Infrastructure.Native;
using Microsoft.Management.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Management.Infrastructure.Serialization
{
    /// <summary>
    /// Schema validation option
    /// </summary>
    /// <seealso cref="CimOperationOptions"/>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "This is a direct representation of the native flags (which doesn't have None)")]
    internal enum MofDeserializerSchemaValidationOption : int
    {
        /* MOF schema validation option's value */
        Default = 0,
        Strict = 1,
        Loose = 2,
        IgnorePropertyType = 3,
        Ignore = 4
    };
}
