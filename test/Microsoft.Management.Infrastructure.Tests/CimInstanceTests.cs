/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/
using System;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Native;
using MMI.Tests.Native;
using Xunit;

namespace MMI.Tests.Internal
{
    /// <summary>
    /// Example test that the string is correct.
    /// </summary>
    public class CimInstanceTests : IDisposable
    {
        MI_Instance instance;

        public CimInstanceTests()
        {
            var res = StaticFixtures.Application.NewInstance("TestClass", null, out this.instance);
            MIAssert.Succeeded(res);
        }

        public void Dispose()
        {
            if(this.instance != null)
            {
                this.instance.Delete();
            }
        }

        [WindowsFact]
        public void CanCreateCimInstance()
        {
            new CimInstance(this.instance);
        }

        [WindowsFact]
        public void CimInstanceThrowsOnBadHandle()
        {
            Assert.Throws<ArgumentNullException>(() => new CimInstance(MI_Instance.Null));
        }

        [WindowsFact]
        public void CanGetPropertiesOfEmptyInstance()
        {
            var cimInstance = new CimInstance(this.instance);
            var properties = cimInstance.CimInstanceProperties;
            Assert.Equal(0, properties.Count, "Expect 0 properties for empty instance");
        }

        [WindowsFact]
        public void CanGetNullPropertyOfInstance()
        {
            var res = this.instance.AddElement("Foo", MI_Value.Null, MI_Type.MI_INSTANCE, MI_Flags.MI_FLAG_NULL);
            MIAssert.Succeeded(res);

            var cimInstance = new CimInstance(this.instance);
            var properties = cimInstance.CimInstanceProperties;
            Assert.Equal(1, properties.Count, "Expect 1 property");
            var property = properties["Foo"];
            Assert.NotNull(property, "Expect property object to be non-null");
            Assert.Equal(CimType.Instance, property.CimType, "Expect type to roundtrip correctly");
            Assert.Equal(CimFlags.NullValue, property.Flags & CimFlags.NullValue, "Expect to get null value flags");
            Assert.Null(property.Value, "Expect property value to be null");
        }

        [WindowsFact]
        public void CanGetPrimitivePropertyOfInstance()
        {
            MI_Value primitiveValue = MI_Value.NewDirectPtr();
            primitiveValue.Uint8 = 42;
            var res = this.instance.AddElement("Foo", primitiveValue, MI_Type.MI_UINT8, MI_Flags.None);
            MIAssert.Succeeded(res);

            var cimInstance = new CimInstance(this.instance);
            var properties = cimInstance.CimInstanceProperties;
            Assert.Equal(1, properties.Count, "Expect 1 property");
            var property = properties["Foo"];
            Assert.NotNull(property, "Expect property object to be non-null");
            Assert.Equal(CimType.UInt8, property.CimType, "Expect type to roundtrip correctly");
            Assert.NotEqual(CimFlags.NullValue, property.Flags & CimFlags.NullValue, "Expect to not get null flags");
            Assert.Equal<byte>(42, (byte)property.Value, "Expect property value to match original value");
        }
    }
}
