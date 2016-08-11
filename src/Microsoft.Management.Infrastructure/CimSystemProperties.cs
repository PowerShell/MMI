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
    public class CimSystemProperties
    {
        private string _namespace;
        private string _serverName;
        private string _className;
        private string _path;

        internal CimSystemProperties()
        {
        }

        internal void UpdateCimSystemProperties(string systemNamespace, string serverName, string className)
        {
            _namespace = systemNamespace;
            _serverName = serverName;
            _className = className;
        }

        internal void UpdateSystemPath(string Path)
        {
            _path = Path;
        }

        /// <summary>
        /// Namespace of CIM object
        /// </summary>
        public string Namespace
        {
            get
            {
                return _namespace;
            }
        }

        /// <summary>
        /// Server Name of CIM object
        /// </summary>
        public string ServerName
        {
            get
            {
                return _serverName;
            }
        }

        /// <summary>
        /// Class Name of CIM object
        /// </summary>
        public string ClassName
        {
            get
            {
                return _className;
            }
        }

        /// <summary>
        /// Object path of CIM object
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
        }
    }
}
