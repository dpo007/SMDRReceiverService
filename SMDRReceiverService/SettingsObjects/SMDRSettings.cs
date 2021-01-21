using System;
using System.Net;

namespace SMDRReceiverService.SettingsObjects
{
    internal class SMDRSettings
    {
        private static IPAddress ipAddress;
        private static int port;

        public IPAddress IPAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        public int Port
        {
            get { return port; }
            set
            {
                if (value < 1024 || value > 65535)
                {
                    throw new ArgumentOutOfRangeException($"\"{value}\" is not a valid port number (acceptable range: 1024-65535).  Please check settings in configuration file.");
                }
                else
                {
                    port = value;
                }
            }
        }

        public void SetIPAddressByString(string value)
        {
            if (value == "*" || value == "0.0.0.0" || value == "Any" || string.IsNullOrWhiteSpace(value))
            {
                ipAddress = IPAddress.Any;
            }
            else
            {
                if (!IPAddress.TryParse(value, out ipAddress))
                {
                    throw new ArgumentException($"\"{value}\" is not a valid IP address.  Please check settings in configuration file.");
                }
            }
        }
    }
}