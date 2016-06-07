using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMI.Tests
{
    using System.Reflection;
    using System.Runtime.InteropServices;

    public static class Helpers
    {
        private readonly static BindingFlags PrivateBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        public static Y GetPrivateProperty<X, Y>(this X self, string name)
        {
            object[] emptyArgs = new object[] { };
            var property = typeof(X).GetProperty(name, PrivateBindingFlags);
            return (Y)property.GetMethod.Invoke(self, emptyArgs);
        }

        public static Y GetPrivateVariable<X, Y>(this X self, string name)
        {
            return (Y)typeof(X).GetField(name, PrivateBindingFlags).GetValue(self);
        }

        /// <summary>
        /// convert string to byte[]
        /// </summary>
        /// <returns></returns>
        public static byte[] GetBytesFromString(string str)
        {
            System.Text.UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static string GetStringRepresentationOfSerializedData(byte[] data)
        {
#if !_LINUX
            return Encoding.Unicode.GetString(data);
#else
            return Encoding.ASCII.GetString(data);
#endif
        }
    }
}
