using System;
using System.Configuration;

namespace MailGun
{
    public static class Util
    {
        public static string GetValueFromAppSettings(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value))
            {
                string message = $"{key} is not defined in AppSettings section of the application configuration file.";
                throw new InvalidOperationException(message);
            }
            return value;
        }
    }
}
