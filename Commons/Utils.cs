using Microsoft.EntityFrameworkCore;
using semigura.DBContext.Entities;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;

namespace semigura.Commons
{



    public static class Utils
    {
        public static TimeZoneInfo Tokyo_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

        public static string GenerateId(DBEntities dbContext)
        {
            //var sql = @"select replace(newid(),'-','')";
            //var id = dbContext.Database.SqlQuery<string>(sql).FirstOrDefault();
            var id = Guid.NewGuid().ToString("N");
            return id;
        }


        public static Dictionary<string, Tuple<string, int?>> GetMetadataOfTable(System.Type objectType)
        {
            var results = new Dictionary<string, Tuple<string, int?>>();

            //using (var context = new DBEntities())
            //{
            //    var oc = ((IObjectContextAdapter)context).ObjectContext;

            //    var metadata = oc.MetadataWorkspace.GetItems(DataSpace.CSpace).OfType<EntityType>();
            //    var tablename = objectType.Name;
            //    var properties = objectType.GetProperties();

            //    foreach (var prop in properties)
            //    {
            //        var obj = metadata.Where(et => et.Name == tablename)
            //             .SelectMany(et => et.Properties.Where(p => p.Name == prop.Name))
            //             .Select(p => new { p.MaxLength, p.TypeName })
            //             .FirstOrDefault();

            //        if (obj != null)
            //        {
            //            results.Add(prop.Name, Tuple.Create(obj.TypeName, obj.MaxLength));
            //        }
            //    }
            //}

            return results;
        }

        public static int? GetMaxLength(DbContext context, string tableName, string propertyName)
        {
            //var oc = ((IObjectContextAdapter)context).ObjectContext;

            //return oc.MetadataWorkspace.GetItems(DataSpace.CSpace).OfType<EntityType>()
            //         .Where(et => et.Name == tableName)
            //         .SelectMany(et => et.Properties.Where(p => p.Name == propertyName))
            //         .Select(p => p.MaxLength)
            //         .FirstOrDefault();
            return 0;
        }

        public static int? GetMaxLength<T>(DbContext context, Expression<Func<T, object>> property)
        {
            var memberExpression = (MemberExpression)property.Body;
            string propertyName = memberExpression.Member.Name;
            return GetMaxLength(context, typeof(T).Name, propertyName);
        }

        public static void Sendmail(string host, int port, string fromMail, string fromName, string fromMailPass, List<string> listToMail, List<string> listCcMail, string subject, string body)
        {
            try
            {
                var passwordDecrypt = AESCryption.Decrypt(fromMailPass, Properties.AES_IV, Properties.AES_Key);

                SmtpClient smtpClient = new SmtpClient(host);
                smtpClient.Port = port;
                smtpClient.Credentials = new System.Net.NetworkCredential(fromMail, passwordDecrypt);
                smtpClient.EnableSsl = true;

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.Body = body;
                mail.Subject = subject;

                //設定 From , To and CC
                mail.From = new MailAddress(fromMail, fromName, System.Text.Encoding.UTF8);
                foreach (var toAddress in listToMail)
                {
                    try
                    {
                        mail.To.Add(new MailAddress(toAddress));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message, ex);
                    }
                }

                foreach (var ccAddress in listCcMail)
                {
                    try
                    {
                        mail.CC.Add(new MailAddress(ccAddress));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message, ex);
                    }
                }

                if (mail.To.Any())
                {
                    smtpClient.Send(mail);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }
        }


        /// <summary>
        /// Gets the list of countries based on ISO 3166-1
        /// ※参考：https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes　
        /// </summary>
        /// <returns>Returns the list of countries based on ISO 3166-1 </returns>
        public static List<RegionInfo> GetCountriesByIso3166()
        {
            List<RegionInfo> countries = new List<RegionInfo>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo country = new RegionInfo(culture.LCID);
                if (countries.Where(p => p.Name == country.Name).Count() == 0)
                    countries.Add(country);
            }
            return countries.OrderBy(p => p.EnglishName).ToList();
        }

        /// <summary>
        /// Gets the list of countries by selected country codes.
        /// </summary>
        /// <param name="code">List of culture codes.</param>
        /// <returns>Returns the list of countries by selected country codes.</returns>
        public static List<RegionInfo> GetCountriesByCode(List<string> codes)
        {
            List<RegionInfo> countries = new List<RegionInfo>();
            if (codes != null && codes.Count > 0)
            {
                foreach (string code in codes)
                {
                    try
                    {
                        countries.Add(new RegionInfo(code));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message, ex);
                    }
                }
            }
            return countries.OrderBy(p => p.EnglishName).ToList();
        }

        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            //DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            //dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();

            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).LocalDateTime;
            dateTime = TimeZoneInfo.ConvertTime(dateTime, Utils.Tokyo_Standard_Time);
            return dateTime;
        }

        public static long? DateTimeToUnixTimeStamp(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
            }

            return null;
        }

        public static string DateTimeToLongTimeString(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                return ((DateTime)dateTime).ToString("yyyy/MM/dd HH:mm:ss");
            }

            return null;
        }


        public static DateTime ConvertToSystemDatetime(DateTime datetime, string clientTimezoneOffset)
        {
            var datetimeResult = datetime;
            try
            {
                if (datetime != null && !string.IsNullOrEmpty(clientTimezoneOffset))
                {
                    var timeZones = TimeZoneInfo.GetSystemTimeZones();
                    var numberOfMinutes = Int32.Parse(clientTimezoneOffset) * (-1);
                    var timeSpan = TimeSpan.FromMinutes(numberOfMinutes);
                    var userTimeZone = timeZones.Where(s => s.BaseUtcOffset == timeSpan).FirstOrDefault();

                    var dateTimeUnspec = DateTime.SpecifyKind(datetimeResult, DateTimeKind.Unspecified);
                    datetimeResult = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, userTimeZone);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }

            return datetimeResult;
        }

        public static DateTime ConvertToSystemDatetimeByTimezone(DateTime datetime, TimeZoneInfo timezoneInfo)
        {
            var datetimeResult = datetime;
            try
            {
                if (datetime != null && timezoneInfo != null)
                {
                    var dateTimeUnspec = DateTime.SpecifyKind(datetimeResult, DateTimeKind.Unspecified);
                    datetimeResult = TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, timezoneInfo);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }

            return datetimeResult;
        }

        public static TimeZoneInfo GetTimezoneByClientTimezoneOffset(DateTime datetime, string clientTimezoneOffset)
        {
            var userTimeZone = TimeZoneInfo.Local;
            try
            {
                if (datetime != null && !string.IsNullOrEmpty(clientTimezoneOffset))
                {
                    var timeZones = TimeZoneInfo.GetSystemTimeZones();
                    var numberOfMinutes = Int32.Parse(clientTimezoneOffset) * (-1);
                    var timeSpan = TimeSpan.FromMinutes(numberOfMinutes);
                    var timeZone = timeZones.Where(s => s.BaseUtcOffset == timeSpan).FirstOrDefault();

                    if (timeZone != null)
                    {
                        userTimeZone = timeZone;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }

            return userTimeZone;
        }

        public static Stopwatch MeasureStart()
        {
            var sw = new System.Diagnostics.Stopwatch();
            // 計測開始
            sw.Start();

            return sw;
        }

        public static void MeasureStop(MethodBase methodBase, Stopwatch sw)
        {
            // 計測停止
            if (sw != null && methodBase != null)
            {
                sw.Stop();

                var message = $"計測_{methodBase.ReflectedType.Name}.{methodBase.Name}：{sw.ElapsedMilliseconds} milliseconds";
                System.Diagnostics.Debug.WriteLine(message);
                Console.Out.WriteLine(message);
            }
        }

        public static string GetMinifiedFileName(string fileName)
        {
            try
            {
                //if (Properties.IS_CLIENT_DEBUGGER)
                if (true)
                {
                    var newFileName =
                            string.Format(
                                "{0}?v={1}",
                                fileName,
                                Properties.JS_CSS_VERSION);
                }
                else
                {
                    if (!string.IsNullOrEmpty(fileName) && fileName.LastIndexOf('.') > 0)
                    {
                        var index = fileName.LastIndexOf('.');

                        var newFileName =
                            string.Format(
                                "{0}.min.{1}?v={2}",
                                fileName.Substring(0, index),
                                fileName.Substring(index + 1, fileName.Length - index - 1),
                                Properties.JS_CSS_VERSION);

                        return newFileName;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }

            return fileName;
        }

        public static string GetIPAddress(System.Web.HttpRequestBase request)
        {
            string szRemoteAddr = request.UserHostAddress;
            string szXForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];
            string szIP = "";

            if (szXForwardedFor == null)
            {
                szIP = szRemoteAddr;
            }
            else
            {
                szIP = szXForwardedFor;
                if (szIP.IndexOf(",") > 0)
                {
                    string[] arIPs = szIP.Split(',');

                    foreach (string item in arIPs)
                    {
                        if (!IsPrivate(item))
                        {
                            return item;
                        }
                    }
                }
            }
            return szIP;
        }

        private static bool IsPrivate(string ipAddress)
        {
            int[] ipParts = ipAddress.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => int.Parse(s)).ToArray();
            // in private ip range
            if (ipParts[0] == 10 ||
                (ipParts[0] == 192 && ipParts[1] == 168) ||
                (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31)))
            {
                return true;
            }

            // IP Address is probably public.
            // This doesn't catch some VPN ranges like OpenVPN and Hamachi.
            return false;
        }

        public static string GetPropertyByKey(string key)
        {
            string result = "";

            var dataDic = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(Properties.CONFIG_PROPERTIES_PATH))
            {
                if (!string.IsNullOrEmpty(row) && !row.StartsWith("#"))
                {
                    var splitIndex = row.IndexOf('=');
                    if (splitIndex > 0 && row.Length > splitIndex)
                    {
                        dataDic.Add(row.Substring(0, splitIndex), row.Substring(splitIndex + 1));
                    }
                }
            }

            if (dataDic.ContainsKey(key))
            {
                result = dataDic[key];
            }

            return result;
        }
    }
}