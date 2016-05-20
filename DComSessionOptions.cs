/*============================================================================
 * Copyright (C) Microsoft Corporation, All rights reserved. 
 *============================================================================
 */

using System;
using Microsoft.Management.Infrastructure.Options.Internal;
using NativeObject;

namespace Microsoft.Management.Infrastructure.Options
{
    /// <summary>
    /// Options of <see cref="CimSession"/> that uses DCOM as the transport protocol
    /// </summary>
    public class DComSessionOptions : CimSessionOptions
    {
        /// <summary>
        /// Creates a new <see cref="DComSessionOptions"/> instance
        /// </summary>
        public DComSessionOptions()
            : base("WMIDCOM")
        {
        }

        /// <summary>
        /// Instantiates a deep copy of <paramref name="optionsToClone"/>
        /// </summary>
        /// <param name="optionsToClone">options to clone</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="optionsToClone"/> is <c>null</c></exception>
        public DComSessionOptions(DComSessionOptions optionsToClone)
            : base(optionsToClone)
        {
        }

        /// <summary>
        /// Sets packet privacy
        /// </summary>
        /// <value></value>
        public bool PacketPrivacy
        {
            set
            {
                this.AssertNotDisposed();

                // TODO: Add SetPacketPrivacy to MI wrapper 
		//MI_Result result = Native.DestinationOptionsMethods.SetPacketPrivacy(this.DestinationOptionsHandleOnDemand, value);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
		// TODO: Add GetPacketPrivacy to MI wrapper
                //bool privacy;
                //MI_Result result = Native.DestinationOptionsMethods.GetPacketPrivacy(this.DestinationOptionsHandleOnDemand, out privacy);
                //CimException.ThrowIfMiResultFailure(result);
                //return privacy;
		return true;
            }
        }

        /// <summary>
        /// Sets packet integrity
        /// </summary>
        /// <value></value>
        public bool PacketIntegrity
        {
            set
            {
                this.AssertNotDisposed();

		// TODO: Add SetPacketIntegrity to MI wrapper
                //MI_Result result = Native.DestinationOptionsMethods.SetPacketIntegrity(
		//		this.DestinationOptionsHandleOnDemand, value);
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();
		
		// TODO: Add GetPacketIntegrity to MI wrapper
                //bool integrity;
                //MI_Result result = Native.DestinationOptionsMethods.GetPacketIntegrity(
		//                     this.DestinationOptionsHandleOnDemand, out integrity);
                //CimException.ThrowIfMiResultFailure(result);
                //return integrity;
		return true;
            }
        }

        /// <summary>
        /// Sets impersonation
        /// </summary>
        /// <value></value>
        public ImpersonationType Impersonation
        {
            set
            {
                this.AssertNotDisposed();

		// TODO: Add SetImpersonationType to MI wrapper
                //MI_Result result = Native.DestinationOptionsMethods.SetImpersonationType(
		//   this.DestinationOptionsHandleOnDemand, value.ToNativeType());
                //CimException.ThrowIfMiResultFailure(result);
            }
            get
            {
                this.AssertNotDisposed();

		// TODO: Add GetImpersonationType to MI wrapper
                //Native.DestinationOptionsMethods.MiImpersonationType type;
                //MI_Result result = Native.DestinationOptionsMethods.GetImpersonationType(
		//this.DestinationOptionsHandleOnDemand, out type);
                //CimException.ThrowIfMiResultFailure(result);
                //return (ImpersonationType)type;
		return ImpersonationType.None;
            }
        }
    }
}