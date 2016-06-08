using System;
using System.Runtime.InteropServices;

namespace Microsoft.Management.Infrastructure.Native
{
    internal abstract class MI_NativeObjectWithFT<FunctionTableType> : MI_NativeObject where FunctionTableType : new()
    {
        protected Lazy<FunctionTableType> mft;

        protected MI_NativeObjectWithFT(bool isDirect) : base(isDirect)
        {
            this.mft = new Lazy<FunctionTableType>(this.MarshalFT);
        }

        protected MI_NativeObjectWithFT(IntPtr existingPtr) : base(existingPtr)
        {
            this.mft = new Lazy<FunctionTableType>(this.MarshalFT);
        }

        private FunctionTableType MarshalFT()
        {
            return MI_FunctionTableCache.GetFTAsOffsetFromPtr<FunctionTableType>(this.Ptr, this.FunctionTableOffset);
        }

        protected abstract int FunctionTableOffset { get; }

        protected FunctionTableType ft
        {
            get
            {
                if (this.IsNull)
                {
                    throw new InvalidOperationException();
                }

                return this.mft.Value;
            }
        }
    }
}