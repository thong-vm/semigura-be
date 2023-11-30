namespace semigura.Commons
{
    public class Properties
    {
        public static readonly string AES_IV = @"pf69DL6GrWFyZcMK";
        public static readonly string AES_Key = @"9Fix4L4HB4PKeKWY";

        public static readonly string LOCALE_JA = "ja";
        public static readonly string LOCALE_EN = "en";
        public static readonly string JWT_TOKEN = "_token";
        public static readonly string USER_INFO = "user_info";
        public static readonly string AUTHOR_INFO = "author_info";
        public static readonly string CULTURE = "Culture";

        //public static readonly string ENVIRONMENT_ROOT_PATH = HostingEnvironment.MapPath(@"~/");
        public static string ENVIRONMENT_ROOT_PATH = @"~/";
        public static readonly string CONFIG_PROPERTIES_PATH = "config.properties";



        public static readonly int CONTAINER_TYPE_TANK = 1;// タンク
        public static readonly int CONTAINER_TYPE_SEIGIKU = 2;// 製麴

        public static readonly int TERMINAL_TYPE_TANK = 1;// タンク用
        public static readonly int TERMINAL_TYPE_SEIGIKU = 2;// 製麴用
        public static readonly int TERMINAL_TYPE_LOCATION = 3; // ロケーション用
        public static readonly int TERMINAL_TYPE_CAMERA = 4; // カメラ

        public static readonly int SENSORDATA_POSITION_TOP = 1;// 位置：上
        public static readonly int SENSORDATA_POSITION_MEDIUM = 2; // 位置：中
        public static readonly int SENSORDATA_POSITION_BOTTOM = 3; // 位置：下

        public static readonly int MEDIA_TYPE_IMAGE = 1;// イメージ
        public static readonly int MEDIA_TYPE_VIDEO = 2; // ビデオ
        public static readonly int MEDIA_TYPE_OTHER = 99; // 他

        public static readonly int MATERIAL_PREPARATIONCOMBINATION_BASE = 1;// 仕込配合：基本
        public static readonly int MATERIAL_PREPARATIONCOMBINATION_TRY = 2; // 仕込配合：試…
        public static readonly int MATERIAL_PREPARATIONCOMBINATION_SPECIAL = 3; // 仕込配合：特…

        public static readonly int MATERIAL_CLASSIFICATION_EXTRA_SPECIAL = 1;// 特上
        public static readonly int MATERIAL_CLASSIFICATION_SPECIAL_CLASS = 2;// 特等
        public static readonly int MATERIAL_CLASSIFICATION_FIRST_CLASS = 3;// 一等
        public static readonly int MATERIAL_CLASSIFICATION_SECOND_CLASS = 4;// 二等
        public static readonly int MATERIAL_CLASSIFICATION_THIRD_CLASS = 5;// 三等

        public static readonly int MATERIALSTANDVAL_TYPE_TANK = 1;// タンク用
        public static readonly int MATERIALSTANDVAL_TYPE_SEIGIKU = 2; // 製麴用
        public static readonly int MATERIALSTANDVAL_TYPE_LOCATION = 3; // ロケーション用

        public static readonly int NOTIFICATION_STATUS_OPEN = 1;// オープン
        public static readonly int NOTIFICATION_STATUS_PROCESSING = 2; // 処理中
        public static readonly int NOTIFICATION_STATUS_CLOSED = 3; // クローズ

        public static readonly int NOTIFICATION_TYPE_LOTCONTAINER = 1; // ロット容器
        public static readonly int NOTIFICATION_TYPE_LOCATION = 2;// ロケーション

        public static readonly int NOTIFICATION_LEVEL_WARNING = 1; // 警告
        public static readonly int NOTIFICATION_LEVEL_EMERGENCY = 2;// 緊急

        public static readonly string MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_1 = "吟醸酒";
        public static readonly string MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_2 = "大吟醸酒";
        public static readonly string MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_3 = "純米酒";
        public static readonly string MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_4 = "純米吟醸酒";
        public static readonly string MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_5 = "純米大吟醸酒";
        public static readonly string MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_6 = "特別純米酒";
        public static readonly string MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_7 = "本醸造酒";
        public static readonly string MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_8 = "特別本醸造酒";


        // 「appSettings」キー
        private static readonly string IS_OUTPUT_LOG_DEBUGGER_KEY = "is_output_log_debugger";
        private static readonly string IS_SERVER_DEBUGGER_KEY = "is_server_debugger";
        private static readonly string IS_CLIENT_DEBUGGER_KEY = "is_client_debugger";
        private static readonly string JS_CSS_VERSION_KEY = "js_css_version";
        private static readonly string TOKEN_TIMEOUT_KEY = "token_timeout";

        private static readonly string TANK_SENSOR_INTERVAL_KEY = "tank_sensor_interval";

        private static readonly string ONDOTORI_URL_KEY = "ondotori_url";
        private static readonly string ONDOTORI_API_KEY = "ondotori_api";
        private static readonly string ONDOTORI_LOGIN_ID_KEY = "ondotori_login_id";
        private static readonly string ONDOTORI_LOGIN_PASS_KEY = "ondotori_login_pass";
        private static readonly string ONDOTORI_INTERVAL_KEY = "ondotori_interval";

        private static readonly string SYSTEM_MAIL_ADDRESS_KEY = "system_mail_address";
        private static readonly string SYSTEM_MAIL_ENABLESSL_KEY = "system_mail_enablessl";
        private static readonly string SYSTEM_MAIL_PASSWORD_KEY = "system_mail_pwd";
        private static readonly string SYSTEM_MAIL_HOST_KEY = "system_mail_host";
        private static readonly string SYSTEM_MAIL_PORT_KEY = "system_mail_port";
        private static readonly string SYSTEM_MAIL_NAME_KEY = "system_mail_name";

        private static readonly string QNAP_DEVICE_ID_KEY = "qnap_device_id";
        private static readonly string QNAP_HOST_KEY = "qnap_host";
        private static readonly string QNAP_USER_KEY = "qnap_user";
        private static readonly string QNAP_PWD_KEY = "qnap_pwd";
        private static readonly string QNAP_REFRESH_INTERVAL_KEY = "qnap_refresh_interval";

        // Cookies
        public static readonly string COOKIES_S01002_FACTORYID = "S01002_FactoryId";
        public static readonly string COOKIES_S02001_FACTORYID = "S02001_FactoryId";
        public static readonly string COOKIES_S02001_LOTID = "S02001_LotId";
        public static readonly string COOKIES_S02001_LOTCONTAINERID = "S02001_LotContainerId";
        public static readonly string COOKIES_S02001_ISINUSE = "S02001_IsInUse";
        public static readonly string COOKIES_S02002_FACTORYID = "S02002_FactoryId";
        public static readonly string COOKIES_S02002_LOTID = "S02002_LotId";
        public static readonly string COOKIES_S02002_ISINUSE = "S02002_IsInUse";
        public static readonly string COOKIES_S02003_FACTORYID = "S02003_FactoryId";
        public static readonly string COOKIES_S02003_LOCATIONID = "S02003_LocationId";



        // AppSettings
        public static bool IS_OUTPUT_LOG_DEBUGGER
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(IS_OUTPUT_LOG_DEBUGGER_KEY);
                    if (bool.TryParse(value, out bool result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return false;
            }
        }

        public static bool IS_SERVER_DEBUGGER
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(IS_SERVER_DEBUGGER_KEY);
                    if (bool.TryParse(value, out bool result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return false;
            }
        }

        public static bool IS_CLIENT_DEBUGGER
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(IS_CLIENT_DEBUGGER_KEY);
                    if (bool.TryParse(value, out bool result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return false;
            }
        }

        public static string JS_CSS_VERSION
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(JS_CSS_VERSION_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }


        public static long TOKEN_TIMEOUT
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(TOKEN_TIMEOUT_KEY);
                    if (long.TryParse(value, out long result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return -1;
            }
        }

        public static long TANK_SENSOR_INTERVAL
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(TANK_SENSOR_INTERVAL_KEY);
                    if (long.TryParse(value, out long result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return -1;
            }
        }

        public static string ONDOTORI_URL
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(ONDOTORI_URL_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static string ONDOTORI_API
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(ONDOTORI_API_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static string ONDOTORI_LOGIN_ID
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(ONDOTORI_LOGIN_ID_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static string ONDOTORI_LOGIN_PASS
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(ONDOTORI_LOGIN_PASS_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static long ONDOTORI_INTERVAL
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(ONDOTORI_INTERVAL_KEY);
                    if (long.TryParse(value, out long result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return -1;
            }
        }

        public static string SYSTEM_MAIL_ADDRESS
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(SYSTEM_MAIL_ADDRESS_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static bool SYSTEM_MAIL_ENABLESSL
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(SYSTEM_MAIL_ENABLESSL_KEY);
                    if (bool.TryParse(value, out bool result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return false;
            }
        }

        public static string SYSTEM_MAIL_HOST
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(SYSTEM_MAIL_HOST_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static string SYSTEM_MAIL_PASSWORD
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(SYSTEM_MAIL_PASSWORD_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static int SYSTEM_MAIL_PORT
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(SYSTEM_MAIL_PORT_KEY);
                    if (int.TryParse(value, out int result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return -1;
            }
        }

        public static string SYSTEM_MAIL_NAME
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(SYSTEM_MAIL_NAME_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static string QNAP_DEVICE_ID
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(QNAP_DEVICE_ID_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static string QNAP_HOST
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(QNAP_HOST_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static string QNAP_USER
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(QNAP_USER_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static string QNAP_PWD
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(QNAP_PWD_KEY);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return string.Empty;
            }
        }

        public static long QNAP_REFRESH_INTERVAL
        {
            get
            {
                try
                {
                    var value = Utils.GetPropertyByKey(QNAP_REFRESH_INTERVAL_KEY);
                    if (long.TryParse(value, out long result))
                    {
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                return -1;
            }
        }
    }
}
