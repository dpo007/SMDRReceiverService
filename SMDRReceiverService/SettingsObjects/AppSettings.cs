namespace SMDRReceiverService.SettingsObjects
{
    internal class AppSettings
    {
        private static bool saveToSQL;
        private static bool saveToCSV;
        private static bool verboseLogging;

        public bool SaveToCSV
        {
            get => saveToCSV;
            set => saveToCSV = value;
        }

        public bool SaveToSQL
        {
            get => saveToSQL;
            set => saveToSQL = value;
        }

        public bool VerboseLogging
        {
            get => verboseLogging;
            set => verboseLogging = value;
        }
    }
}