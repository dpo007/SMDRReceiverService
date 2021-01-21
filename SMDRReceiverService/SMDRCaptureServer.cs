using ObjectDumper;
using SMDRReceiverService.DataObjects;
using SMDRReceiverService.SettingsObjects;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SMDRReceiverService
{
    internal class SMDRCaptureServer
    {
        // Current number of active clients (threads).
        private static int clientCount = 0;

        // Flag to indicate that the server is running.
        private static volatile bool isRunning;

        private static TcpListener listener;

        // Object to use for thread-locked sections.
        private static readonly object lockTarget = new object();

        // Constructor
        public SMDRCaptureServer(EventLog eventLog, SMDRSettings smdrSettings, SQLSettings sqlSettings, CSVSettings csvSettings, AppSettings appSettings)
        {
            EventLog = eventLog;
            SmdrSettings = smdrSettings;
            CsvSettings = csvSettings;
            SqlSettings = sqlSettings;
            AppSettings = appSettings;

            lock (lockTarget)
            {
                string logEntryText = $"Configured to listen for SMDR at {smdrSettings.IPAddress}:{smdrSettings.Port}.";
                logEntryText += $"\r\n\r\nSQL logging enabled: {appSettings.SaveToSQL}";

                if (appSettings.SaveToSQL)
                {
                    logEntryText += $"\r\n- Server: {sqlSettings.Server}";
                    logEntryText += $"\r\n- Database: {sqlSettings.Database}";
                    logEntryText += $"\r\n- User: {sqlSettings.Username}";
                }

                logEntryText += $"\r\n\r\nCSV logging enabled: {appSettings.SaveToCSV}";

                if (appSettings.SaveToCSV)
                {
                    logEntryText += $"\r\n- Logging folder: {csvSettings.CSVLogPath}";
                }

                EventLog.WriteEntry(logEntryText, EventLogEntryType.Information, 1005);
            }
        }

        // Holders for various settings.
        internal static AppSettings AppSettings { get; set; }

        internal static CSVSettings CsvSettings { get; set; }

        internal static EventLog EventLog { get; set; }

        internal static SMDRSettings SmdrSettings { get; set; }

        internal static SQLSettings SqlSettings { get; set; }

        /// <summary>
        /// Public access to Listening Server status.
        /// </summary>
        /// <returns>True if running, False if not.</returns>
        public static bool GetIsRunning()
        {
            return isRunning;
        }

        /// <summary>
        /// Reverse DNS lookup
        /// </summary>
        /// <param name="ipAddress">IP Address to look up.</param>
        /// <returns>Resolved name, or null if it can't/doesn't resolve.</returns>
        public static string ReverseDNSLookup(string ipAddress)
        {
            if ((!IPAddress.TryParse(ipAddress, out _)) || string.IsNullOrWhiteSpace(ipAddress))
                return null;

            try
            {
                IPHostEntry host = Dns.GetHostEntry(ipAddress);
                if (!string.IsNullOrWhiteSpace(host.HostName))
                    return host.HostName;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        internal static void Stop()
        {
            // Disable TCP listener loop.
            isRunning = false;

            // Stop the listener.
            listener.Stop();
            lock (lockTarget)
            {
                EventLog.WriteEntry($"Listener stopped.", EventLogEntryType.Information, 1003);
            }

            if (clientCount > 0)
            {
                lock (lockTarget)
                {
                    EventLog.WriteEntry($"Waiting for {clientCount} connections to finish...", EventLogEntryType.Information, 1004);
                }

                int lastClientCount = clientCount;

                // Stick around until all clients are done.
                while (clientCount > 0)
                {
                    if (lastClientCount != clientCount)
                    {
                        lock (lockTarget)
                        {
                            EventLog.WriteEntry($"Waiting for {clientCount} connections to finish...", EventLogEntryType.Information, 1004);
                        }

                        lastClientCount = clientCount;
                    }
                }

                lock (lockTarget)
                {
                    EventLog.WriteEntry("All client connections are now closed.", EventLogEntryType.Information, 1006);
                }
            }
        }

        internal async void ListeningServer(object obj)
        {
            // Start listening for clients.
            listener = new TcpListener(SmdrSettings.IPAddress, SmdrSettings.Port);
            listener.Start();

            // Flag that the server is running.
            isRunning = true;

            // Start loop to check listener for clients.
            while (isRunning)
            {
                try
                {
                    // Waiting for client connection.
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    // Client connection found.

                    // Increment the count of outstanding clients
                    Interlocked.Increment(ref clientCount);

                    // When we are done, use a continuation to decrement the count
                    await HandleClient(tcpClient).ContinueWith((_t) => Interlocked.Decrement(ref clientCount));
                }
                catch (Exception ex)
                {
                    lock (lockTarget)
                    {
                        EventLog.WriteEntry($"Exception while accepting client: {ex.Message}", EventLogEntryType.Error, 1300);
                    }
                }
            }
        }

        internal void Start()
        {
            // Create a thread to run the TCP listener.
            new Thread(new ParameterizedThreadStart(ListeningServer)).Start();
        }

        private string BuildSQLInsertCommand(SMDRRecord smdrRecord, IPEndPoint clientIPEndPoint)
        {
            string sqlCommand = $@"
                INSERT INTO [dbo].[CallLog]
	                ([Record_ClientIP]
	                ,[Call Start]
	                ,[Connected Time]
	                ,[Ring Time]
	                ,[Caller]
	                ,[Direction]
	                ,[Called Number]
	                ,[Dialled Number]
	                ,[Account]
	                ,[Is Internal]
	                ,[Call ID]
	                ,[Continuation]
	                ,[Party1Device]
	                ,[Party1Name]
	                ,[Party2Device]
	                ,[Party2Name]
	                ,[Hold Time]
	                ,[Park Time]
	                ,[AuthValid]
	                ,[AuthCode]
	                ,[User Charged]
	                ,[Call Charge]
	                ,[Currency]
	                ,[Amount at Last User Change]
	                ,[Call Units]
	                ,[Units at Last User Change]
	                ,[Cost per Unit]
	                ,[Mark Up]
	                ,[External Targeting Cause]
	                ,[External Targeter Id]
	                ,[External Targeted Number]
	                ,[Server IP Address of the Caller Extension]
	                ,[Unique Call ID for the Caller Extension]
	                ,[Server IP Address of the Called Extension]
	                ,[Unique Call ID for the Called Extension]
	                ,[UTC Time])
                VALUES
	                ('{clientIPEndPoint.Address}'
	                ,'{smdrRecord.Call_Start}'
	                ,'{smdrRecord.Connected_Time}'
	                ,'{smdrRecord.Ring_Time}'
	                ,'{smdrRecord.Caller}'
	                ,'{smdrRecord.Direction}'
	                ,'{smdrRecord.Called_Number}'
	                ,'{smdrRecord.Dialled_Number}'
	                ,'{smdrRecord.Account}'
	                ,'{smdrRecord.Is_Internal}'
	                ,'{smdrRecord.Call_ID}'
	                ,'{smdrRecord.Continuation}'
	                ,'{smdrRecord.Party1Device}'
	                ,'{smdrRecord.Party1Name}'
	                ,'{smdrRecord.Party2Device}'
	                ,'{smdrRecord.Party2Name}'
	                ,'{smdrRecord.Hold_Time}'
	                ,'{smdrRecord.Park_Time}'
	                ,'{smdrRecord.AuthValid}'
	                ,'{smdrRecord.AuthCode}'
	                ,'{smdrRecord.User_Charged}'
	                ,'{smdrRecord.Call_Charge}'
	                ,'{smdrRecord.Currency}'
	                ,'{smdrRecord.Amount_at_Last_User_Change}'
	                ,'{smdrRecord.Call_Units}'
	                ,'{smdrRecord.Units_at_Last_User_Change}'
	                ,'{smdrRecord.Cost_per_Unit}'
	                ,'{smdrRecord.Mark_Up}'
	                ,'{smdrRecord.External_Targeting_Cause}'
	                ,'{smdrRecord.External_Targeter_Id}'
	                ,'{smdrRecord.External_Targeted_Number}'
	                ,'{smdrRecord.Server_IP_Address_of_the_Caller_Extension}'
	                ,'{smdrRecord.Unique_Call_ID_for_the_Caller_Extension}'
	                ,'{smdrRecord.Server_IP_Address_of_the_Called_Extension}'
	                ,'{smdrRecord.Unique_Call_ID_for_the_Called_Extension}'
	                ,'{smdrRecord.UTC_Time}');";

            return sqlCommand;
        }

        /// <summary>
        /// Task to process TCP client.
        /// </summary>
        /// <param name="tcpClient">TCP client to process.</param>
        /// <returns>Task</returns>
        private async Task HandleClient(TcpClient tcpClient)
        {
            using (tcpClient)
            {
                IPEndPoint remoteIpEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                string hostName = ReverseDNSLookup(remoteIpEndPoint.Address.ToString());

                if (AppSettings.VerboseLogging)
                {
                    lock (lockTarget)
                    {
                        EventLog.WriteEntry($"Connected.  Client: {remoteIpEndPoint.Address}:{remoteIpEndPoint.Port}\r\nRDNS resolves as: \"{hostName}\"\r\nCurrent Client Count: {clientCount}", EventLogEntryType.Information, 1001);
                    }
                }

                try
                {
                    // Create a Stream object for reading and writing (from/to the client).
                    using (NetworkStream networkStream = tcpClient.GetStream())
                    {
                        // Create a Reader to read from the Stream.
                        using (StreamReader reader = new StreamReader(networkStream))
                        {
                            // Process data sent by the client.
                            string incomingData = await reader.ReadLineAsync();
                            if (!string.IsNullOrWhiteSpace(incomingData))
                            {
                                // ID to associate event log entries that may not come in order.
                                Guid dataLoggingID = Guid.NewGuid();

                                if (AppSettings.VerboseLogging)
                                {
                                    lock (lockTarget)
                                    {
                                        EventLog.WriteEntry($"{dataLoggingID}\r\nIncoming data:\r\n\r\n{incomingData}", EventLogEntryType.Information, 1500);
                                    }
                                }

                                // Validate incoming data.
                                if (!SMDRRecord.ValidateFields(incomingData, out string validationFailureReason))
                                {
                                    // Failed validation
                                    string validationErrorMessage = $"Incoming data (ID: {dataLoggingID}) failed validation!  Reason: {validationFailureReason}";

                                    lock (lockTarget)
                                    {
                                        EventLog.WriteEntry($"{validationErrorMessage}\r\n\r\nData Received:\r\n\r\n{incomingData}\r\n\r\nClient IP: {remoteIpEndPoint.Address}\r\nRDNS resolves as: \"{hostName}\"", EventLogEntryType.Error, 1303);
                                    }
                                    throw new Exception(validationErrorMessage);
                                }

                                SMDRRecord currentSMDRRecord = new SMDRRecord();
                                PopulateSMDRRecordFromCSVData(currentSMDRRecord, incomingData);

                                if (AppSettings.VerboseLogging)
                                {
                                    string recDump = ObjectDumperExtensions.DumpToString(currentSMDRRecord, "SMDRRecord");
                                    lock (lockTarget)
                                    {
                                        EventLog.WriteEntry($"{dataLoggingID}\r\nRecord dump:\r\n\r\n{recDump}", EventLogEntryType.Information, 1501);
                                    }
                                }

                                if (AppSettings.SaveToCSV)
                                {
                                    // Use a different log file for each month
                                    string csvLogfileName = $"SMDRLog_{DateTime.Now:MMM-yyyy}.csv";
                                    string csvFilePath = Path.Combine(CsvSettings.CSVLogPath, csvLogfileName);

                                    try
                                    {
                                        lock (lockTarget)
                                        {
                                            // Append data record to CSV log.
                                            File.AppendAllText(csvFilePath, $"{incomingData}{Environment.NewLine}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lock (lockTarget)
                                        {
                                            EventLog.WriteEntry($"{dataLoggingID}\r\nError appending CSV log \"{csvFilePath}\".\r\nError:\r\n\r\n{ex.Message}", EventLogEntryType.Error, 1304);
                                        }
                                    }
                                }

                                if (AppSettings.SaveToSQL)
                                {
                                    using (SqlConnection sqlConn = new SqlConnection())
                                    {
                                        try
                                        {
                                            sqlConn.ConnectionString = SqlSettings.ConnectionString();
                                            sqlConn.Open();

                                            string sqlInsertCommand = BuildSQLInsertCommand(currentSMDRRecord, remoteIpEndPoint);
                                            SqlCommand sqlCmd = new SqlCommand(sqlInsertCommand, sqlConn);

                                            try
                                            {
                                                sqlCmd.ExecuteNonQuery();
                                            }
                                            catch (SqlException ex)
                                            {
                                                throw new Exception($"Error executing SQL query.\r\nError:\r\n\r\n{ex.Message}\r\n\r\nQuery:\r\n\r\n{sqlInsertCommand}");
                                            }

                                            sqlConn.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            lock (lockTarget)
                                            {
                                                EventLog.WriteEntry($"{dataLoggingID}\r\nError communicating with SQL server.\r\nInner error:\r\n\r\n{ex.Message}", EventLogEntryType.Error, 1304);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (lockTarget)
                    {
                        EventLog.WriteEntry($"Exception while handling incoming data: {ex.Message}", EventLogEntryType.Error, 1301);
                    }
                }
                finally
                {
                    // End this connection.
                    tcpClient.Close();
                    if (AppSettings.VerboseLogging)
                    {
                        lock (lockTarget)
                        {
                            EventLog.WriteEntry($"Disconnected Client: {remoteIpEndPoint.Address}:{remoteIpEndPoint.Port} (RDNS resolves as: {hostName})", EventLogEntryType.Information, 1002);
                        }
                    }
                }
            }
        }

        private void PopulateSMDRRecordFromCSVData(SMDRRecord smdrRecord, string data)
        {
            // Split the (validated) CSV data into fields.
            string[] textRecordFields = data.Split(',');

            smdrRecord.Call_Start = DateTime.Parse(textRecordFields[0]);
            smdrRecord.Connected_Time = TimeSpan.Parse(textRecordFields[1]);
            if (!string.IsNullOrWhiteSpace(textRecordFields[2]))
                smdrRecord.Ring_Time = int.Parse(textRecordFields[2]);
            smdrRecord.Caller = textRecordFields[3];
            smdrRecord.Direction = textRecordFields[4].ToUpper().ToCharArray()[0];
            smdrRecord.Called_Number = textRecordFields[5];
            smdrRecord.Dialled_Number = textRecordFields[6];
            smdrRecord.Account = textRecordFields[7];
            smdrRecord.Is_Internal = SMDRRecord.ConvertZeroOneStringToBool(textRecordFields[8]);
            smdrRecord.Call_ID = int.Parse(textRecordFields[9]);
            smdrRecord.Continuation = SMDRRecord.ConvertZeroOneStringToBool(textRecordFields[10]);
            smdrRecord.Party1Device = textRecordFields[11];
            smdrRecord.Party1Name = textRecordFields[12];
            smdrRecord.Party2Device = textRecordFields[13];
            smdrRecord.Party2Name = textRecordFields[14];
            if (!string.IsNullOrWhiteSpace(textRecordFields[15]))
                smdrRecord.Hold_Time = int.Parse(textRecordFields[15]);
            if (!string.IsNullOrWhiteSpace(textRecordFields[16]))
                smdrRecord.Park_Time = int.Parse(textRecordFields[16]);
            smdrRecord.AuthValid = SMDRRecord.ConvertZeroOneStringToBool(textRecordFields[17]);
            smdrRecord.AuthCode = textRecordFields[18];
            smdrRecord.User_Charged = textRecordFields[19];
            smdrRecord.Call_Charge = textRecordFields[20];
            smdrRecord.Currency = textRecordFields[21];
            smdrRecord.Amount_at_Last_User_Change = textRecordFields[22];
            if (!string.IsNullOrWhiteSpace(textRecordFields[23]))
                smdrRecord.Call_Units = int.Parse(textRecordFields[23]);
            if (!string.IsNullOrWhiteSpace(textRecordFields[24]))
                smdrRecord.Units_at_Last_User_Change = int.Parse(textRecordFields[24]);
            if (!string.IsNullOrWhiteSpace(textRecordFields[25]))
                smdrRecord.Cost_per_Unit = int.Parse(textRecordFields[25]);
            if (!string.IsNullOrWhiteSpace(textRecordFields[26]))
                smdrRecord.Mark_Up = int.Parse(textRecordFields[26]);
            smdrRecord.External_Targeting_Cause = textRecordFields[27];
            smdrRecord.External_Targeter_Id = textRecordFields[28];
            smdrRecord.External_Targeted_Number = textRecordFields[29];
            smdrRecord.Set_Server_IP_Address_of_the_Caller_Extension_ByString(textRecordFields[30]);
            if (!string.IsNullOrWhiteSpace(textRecordFields[31]))
                smdrRecord.Unique_Call_ID_for_the_Caller_Extension = int.Parse(textRecordFields[31]);
            smdrRecord.Set_Server_IP_Address_of_the_Called_Extension_ByString(textRecordFields[32]);
            if (!string.IsNullOrWhiteSpace(textRecordFields[33]))
                smdrRecord.Unique_Call_ID_for_the_Called_Extension = int.Parse(textRecordFields[33]);
            if (!string.IsNullOrWhiteSpace(textRecordFields[34]))
                smdrRecord.UTC_Time = DateTime.Parse(textRecordFields[34]);
        }
    }
}