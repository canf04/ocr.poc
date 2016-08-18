namespace MailGun
{
    public class MailGunConfiguration
    {
        public string BaseUrl => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.BaseUrl");

        public string ApiUsername => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.ApiUsername");

        public string ApiKey => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.ApiKey");

        public string Domain => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.Domain");

        public string From => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.From");

        public string ResourceUrl => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.ResourceUrl");
        public string StatsUrl => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.StatsUrl");

        public string Subject => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.Subject");

        public string Text => Util.GetValueFromAppSettings("UDocx365.Services.MailGun.MailGun.Text");
    }
}
