using System;
using System.IO;

namespace Bestandenselektie.HKD.Services
{
    public class ApplicationDeployment
    {
        private static ApplicationDeployment? currentDeployment = null;
        private static bool currentDeploymentInitialized = false;

        private static bool isNetworkDeployed = false;
        private static bool isNetworkDeployedInitialized = false;

        public static bool IsNetworkDeployed
        {
            get
            {
                if (!isNetworkDeployedInitialized)
                {
                    bool.TryParse(Environment.GetEnvironmentVariable("ClickOnce_IsNetworkDeployed"), out isNetworkDeployed);
                    isNetworkDeployedInitialized = true;
                }

                return isNetworkDeployed;
            }
        }

        public static ApplicationDeployment? CurrentDeployment
        {
            get
            {
                if (!currentDeploymentInitialized)
                {
                    currentDeployment = IsNetworkDeployed ? new ApplicationDeployment() : null;
                    currentDeploymentInitialized = true;
                }

                return currentDeployment;
            }
        }

        public string? DataDirectory
        {
            get { return Environment.GetEnvironmentVariable("ClickOnce_DataDirectory"); }
        }

        private ApplicationDeployment()
        {
        }
    }
}
