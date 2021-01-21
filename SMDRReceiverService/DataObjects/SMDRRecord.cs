using System;
using System.Net;

namespace SMDRReceiverService.DataObjects
{
    public class SMDRRecord
    {
        private IPAddress record_ClientIP;
        private IPAddress server_IP_Address_of_the_Called_Extension;
        private IPAddress server_IP_Address_of_the_Caller_Extension;
        public string Account { get; set; }
        public string Amount_at_Last_User_Change { get; set; }
        public string AuthCode { get; set; }
        public bool AuthValid { get; set; }
        public string Call_Charge { get; set; }
        public int Call_ID { get; set; }
        public DateTime Call_Start { get; set; }
        public int Call_Units { get; set; }
        public string Called_Number { get; set; }
        public string Caller { get; set; }
        public TimeSpan Connected_Time { get; set; }
        public bool Continuation { get; set; }
        public int Cost_per_Unit { get; set; }
        public string Currency { get; set; }
        public string Dialled_Number { get; set; }
        public char Direction { get; set; }
        public string External_Targeted_Number { get; set; }
        public string External_Targeter_Id { get; set; }
        public string External_Targeting_Cause { get; set; }
        public int Hold_Time { get; set; }
        public bool Is_Internal { get; set; }
        public int Mark_Up { get; set; }
        public int Park_Time { get; set; }
        public string Party1Device { get; set; }
        public string Party1Name { get; set; }
        public string Party2Device { get; set; }
        public string Party2Name { get; set; }

        public IPAddress Record_ClientIP
        {
            get => record_ClientIP;
            set => record_ClientIP = value;
        }

        public DateTime Record_CreationDate { get; }
        public Guid Record_GUID { get; }
        public int Record_ID { get; }
        public int Ring_Time { get; set; }

        public IPAddress Server_IP_Address_of_the_Called_Extension
        {
            get => server_IP_Address_of_the_Called_Extension;
            set => server_IP_Address_of_the_Called_Extension = value;
        }

        public IPAddress Server_IP_Address_of_the_Caller_Extension
        {
            get => server_IP_Address_of_the_Caller_Extension;
            set => server_IP_Address_of_the_Caller_Extension = value;
        }

        public int Unique_Call_ID_for_the_Called_Extension { get; set; }
        public int Unique_Call_ID_for_the_Caller_Extension { get; set; }
        public int Units_at_Last_User_Change { get; set; }
        public string User_Charged { get; set; }
        public DateTime UTC_Time { get; set; }

        public static bool ConvertZeroOneStringToBool(string stringToConvert)
        {
            if (string.IsNullOrWhiteSpace(stringToConvert)) return false;
            if (stringToConvert.Trim() == "0") return false;
            if (stringToConvert.Trim() == "1") return true;

            throw new ArgumentOutOfRangeException(stringToConvert, $"Unexpected value \"{stringToConvert}\".");
        }

        public void Set_Record_ClientIP_ByString(string value)
        {
            if (!IPAddress.TryParse(value, out record_ClientIP))
            {
                throw new ArgumentException($"\"{value}\" is not a valid IP address.");
            }
        }

        public void Set_Server_IP_Address_of_the_Called_Extension_ByString(string value)
        {
            if (!IPAddress.TryParse(value, out server_IP_Address_of_the_Called_Extension))
            {
                throw new ArgumentException($"\"{value}\" is not a valid IP address.");
            }
        }

        public void Set_Server_IP_Address_of_the_Caller_Extension_ByString(string value)
        {
            if (!IPAddress.TryParse(value, out server_IP_Address_of_the_Caller_Extension))
            {
                throw new ArgumentException($"\"{value}\" is not a valid IP address.");
            }
        }

        internal static bool ValidateFields(string data, out string validationFailureReason)
        {
            validationFailureReason = null;

            if (data.IndexOf("\n") > -1)
                validationFailureReason = $"Unexpected Newline character found.";

            string[] fields = data.Split(',');
            int expectedFieldCount = 35;

            if (fields.Length != expectedFieldCount)
                validationFailureReason = $"Invalid field count.  Expecting {expectedFieldCount}, found {fields.Length}.";

            // Validations complete.
            if (string.IsNullOrWhiteSpace(validationFailureReason))
                // All fields successfully validated
                return true;
            else
                // Something failed validation.
                return false;
        }
    }
}