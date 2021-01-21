using SMDRReceiverService.SettingsObjects;
using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace SMDRReceiverService
{
    public partial class SMDRRecieverService : ServiceBase
    {
        internal AppSettings appSettings;
        internal CSVSettings csvSettings;
        internal SMDRSettings smdrSettings;
        internal SQLSettings sqlSettings;

        public SMDRRecieverService()
        {
            InitializeComponent();
            eventLog1 = new EventLog
            {
                Source = "SMDRReceiverService",
                Log = "Application"
            };
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Starting up...", EventLogEntryType.Information, 2000);

            LoadSettings();

            if (appSettings.SaveToCSV)
            {
                try
                {
                    CreateCSVLogFolder(csvSettings.CSVLogPath);
                }
                catch (Exception ex)
                {
                    eventLog1.WriteEntry($"Error creating CSV log folder.  Error: {ex.Message}", EventLogEntryType.Error, 1310);
                    // Stop Windows service.
                    Stop();
                    return;
                }
            }

            // Create the Server Object and Start it.
            SMDRCaptureServer server = new SMDRCaptureServer(eventLog1, smdrSettings, sqlSettings, csvSettings, appSettings);
            server.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Stopping...", EventLogEntryType.Information, 2001);

            if (SMDRCaptureServer.GetIsRunning())
                SMDRCaptureServer.Stop();
        }

        private void CreateCSVLogFolder(string pathForCSVs)
        {
            if (string.IsNullOrWhiteSpace(pathForCSVs))
                throw new Exception("Service not started.  Invalid path for CSVs, please check configuration file.");

            if (!Directory.Exists(pathForCSVs))
            {
                // Try to create the directory path.
                try
                {
                    DirectoryInfo di = Directory.CreateDirectory(pathForCSVs);
                    eventLog1.WriteEntry($"CSV log folder created: \"{di.FullName}\".", EventLogEntryType.Information, 1003);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to create folder \"{pathForCSVs}\".  Error: \"{ex.Message}\"  Please check path setting in configuration file.");
                }
            }
        }

        private void LoadSettings()
        {
            eventLog1.WriteEntry("Loading settings...", EventLogEntryType.Information, 1000);

            // Load application settings.
            appSettings = new AppSettings
            {
                SaveToCSV = Properties.Settings.Default.SaveToCSV,
                SaveToSQL = Properties.Settings.Default.SaveToSQL,
                VerboseLogging = Properties.Settings.Default.VerboseEventLogging
            };

            // Load CSV settings.
            csvSettings = new CSVSettings
            {
                CSVLogPath = Properties.Settings.Default.PathForCSVs
            };

            // Load SQL settings.
            sqlSettings = new SQLSettings
            {
                Server = Properties.Settings.Default.SQL_Server,
                Database = Properties.Settings.Default.SQL_Database,
                Username = Properties.Settings.Default.SQL_Username,
                Password = Properties.Settings.Default.SQL_Password
            };

            // Load SMDR Server settings.
            smdrSettings = new SMDRSettings
            {
                Port = Properties.Settings.Default.SMDR_ListeningPort
            };
            smdrSettings.SetIPAddressByString(Properties.Settings.Default.SMDR_ListeningIP);
        }
    }
}