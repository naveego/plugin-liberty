using System;
using System.Text.RegularExpressions;

namespace PluginLiberty.Helper
{
    public class Settings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public string NPI { get; set; }
        public string QueryStartDate { get; set; }

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
            if (String.IsNullOrEmpty(QueryStartDate))
            {
                throw new Exception("the QueryStartDate property must be set");
            }

            Regex dateValidationRgx = new Regex(@"^(19|20)\d\d-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[01])$");

            if (!dateValidationRgx.IsMatch(QueryStartDate))
            {
                throw new Exception("the QueryStartDate property must match yyyy-MM-dd format");
            }
            if (DateTime.Compare(DateTime.Parse(QueryStartDate), DateTime.Today)>0)
            {
                throw new Exception("the QueryStartDate must be equal to or before today");
            }
        }
    }
}