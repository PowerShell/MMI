/*
**==============================================================================
**
** Copyright (c) Microsoft Corporation. All rights reserved. See file LICENSE
** for license information.
**
**==============================================================================
*/


namespace Microsoft.Management.Infrastructure
{
    /// <summary>
    /// Represents a single item of a streamed out parameter array.
    /// </summary>
    public class CimMethodStreamedResult : CimMethodResultBase
    {
        internal CimMethodStreamedResult(string parameterName, object parameterValue, CimType parameterType)
        {
            this.ParameterName = parameterName;
            this.ItemValue = parameterValue;
            this.ItemType = parameterType;
        }

        public string ParameterName { get; private set; }

        public object ItemValue { get; private set; }
        public CimType ItemType { get; private set; }
    }
}
