namespace SMDRReceiverService.SettingsObjects
{
    internal class CSVSettings
    {
        private static string cSVLogPath;
        private static bool includeHeader;

        public string CSVLogPath
        {
            get => cSVLogPath;
            set => cSVLogPath = value;
        }

        public bool IncludeHeader
        {
            get => includeHeader;
            set => includeHeader = value;
        }
    }
}