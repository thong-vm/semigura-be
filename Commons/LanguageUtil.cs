using System.Globalization;
using System.Web;

namespace semigura.Commons
{
    public class LanguageUtil
    {
        public static List<Languages> AvailableLanguages = new List<Languages> {
            new Languages { LanguageFullName = "日本語", LanguageCultureName = "ja", FlagIcon = "jp" },
            new Languages { LanguageFullName = "English", LanguageCultureName = "en" , FlagIcon = "gb" },
        };

        public static bool IsLanguageAvailable(string lang)
        {
            return AvailableLanguages.Where(a => a.LanguageCultureName.Equals(lang)).FirstOrDefault() != null ? true : false;
        }
        public static string GetDefaultLanguage()
        {
            return AvailableLanguages[0].LanguageCultureName;
        }
        public static void SetLanguage(string lang)
        {
            try
            {
                if (!IsLanguageAvailable(lang)) lang = GetDefaultLanguage();
                var cultureInfo = new CultureInfo(lang);
                HttpCookie langCookie = new HttpCookie(Properties.CULTURE, lang);
                langCookie.Expires = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).AddYears(1);
                System.Web.HttpContext.Current.Response.Cookies.Add(langCookie);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    public class Languages
    {
        public string LanguageFullName { get; set; }
        public string LanguageCultureName { get; set; }
        public string FlagIcon { get; set; }
    }
}