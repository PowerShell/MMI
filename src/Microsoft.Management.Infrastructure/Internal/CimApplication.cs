/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


using Microsoft.Management.Infrastructure.Native;
using System;

namespace Microsoft.Management.Infrastructure.Internal
{
    internal static class CimApplication
    {
        #region Initializing CimApplication singleton

        static internal string ApplicationID = "CoreCLRSingletonAppDomain";

        static private MI_Application GetApplicationHandle()
        {
            MI_Application applicationHandle;
            MI_Instance errorDetailsHandle;
            MI_Result result = MI_Application.Initialize(ApplicationID,
                             out errorDetailsHandle,
                             out applicationHandle);
            CimException.ThrowIfMiResultFailure(result, errorDetailsHandle);

            return applicationHandle;
        }

        static public MI_Application Handle
        {
            get
            {
                return CimApplication.LazyHandle.Value;
            }
        }

        private static readonly Lazy<MI_Application> LazyHandle = new Lazy<MI_Application>(CimApplication.GetApplicationHandle);

        #endregion Initializing CimApplication singleton
    }
}
