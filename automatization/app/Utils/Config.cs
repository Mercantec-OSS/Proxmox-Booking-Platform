namespace VMwareBookingSystem.Utils
{
    public class Config
    {
        public string VERSION => ParseVariable("VERSION");
        public bool TEST_MODE => ParseVariable("TEST_MODE").ToLower() == "true";
        public string SCRIPTS_PATH => ParseVariable("SCRIPTS_PATH");
        public string VM_VCENTER_IP => ParseVariable("VM_VCENTER_IP");
        public string VM_VCENTER_USER => ParseVariable("VM_VCENTER_USER");
        public string VM_VCENTER_PASSWORD => ParseVariable("VM_VCENTER_PASSWORD");
        public string VM_CLUSTER_NAME => ParseVariable("VM_CLUSTER_NAME");
        public string VM_DATASTORE_NAME => ParseVariable("VM_DATASTORE_NAME");

        private string ParseVariable(string variableName)
        {
            string variable = Environment.GetEnvironmentVariable(variableName) ?? "";
            if (string.IsNullOrEmpty(variable))
            {
                string errorMsg = $"{variableName} is not set";
                throw new Exception(errorMsg);
            }
            return variable;
        }
    }
}
