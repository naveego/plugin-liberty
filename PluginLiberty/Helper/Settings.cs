using System;

namespace PluginLiberty.Helper
{
    public class Settings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public string NPI { get; set; }

        /// <summary>
        /// Validates the settings input object
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Validate()
        {
            if (String.IsNullOrEmpty(Username))
            {
                throw new Exception("the Username property must be set");
            }
            
            if (String.IsNullOrEmpty(Password))
            {
                throw new Exception("the Password property must be set");
            }
            
            if (String.IsNullOrEmpty(ApiKey))
            {
                throw new Exception("the ApiKey property must be set");
            }
                
            if (String.IsNullOrEmpty(NPI))
            {
                throw new Exception("the NPI property must be set");
            }
        }
    }
}