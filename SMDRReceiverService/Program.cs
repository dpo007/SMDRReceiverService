using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;

namespace SMDRReceiverService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                var parameter = string.Concat(args).ToUpper();

                switch (parameter)
                {
                    case "/I":
                    case "/INSTALL":
                    case "-I":
                    case "-INSTALL":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;

                    case "/U":
                    case "/UNINSTALL":
                    case "-U":
                    case "-UNINSTALL":
                        if (ServiceStatus("SMDRReceiverService") != "Stopped")
                            MessageBox.Show("Stop the service before attempting uninstall.");
                        else
                            ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;

                    default:
                        MessageBox.Show($"Unknown command \"{parameter}\".");
                        break;
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new SMDRRecieverService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        private static string ServiceStatus(string serviceName)
        {
            using (ServiceController sc = new ServiceController(serviceName))
            {
                return sc.Status.ToString();
            }
        }
    }
}