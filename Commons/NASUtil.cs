using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace semigura.Commons
{
    public class NASUtil
    {
        private static readonly string STATUS_SUCCESS = "1";
        private static readonly string SID_KEY = "sid";
        private static readonly string STATUS_KEY = "status";
        private static readonly string RESULT_KEY = "result";
        private static readonly string QCLOUD_TUNNEL_SID_KEY = "qcloud_tunnel_sid";
        private static readonly string TUNNEL_AGENT_DOMAIN_NAME_KEY = "tunnel_agent_domain_name";

        private static DateTime? _refreshed_time = null;
        private static string _sid = null;
        private static string _qcloud_tunnel_sid = null;
        private static string _api_host = null;

        private static bool _isRefreshing = false;


        private static bool GetMyqnapCloudHost()
        {
            try
            {
                return false;
                var uri = "{0}/ajax/get_tunnel?device_id={1}";
                uri = string.Format(uri, Properties.QNAP_HOST, Properties.QNAP_DEVICE_ID);

                var client = new RestClient(uri);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddCookie("qcloud_tunnel_agent_id", Properties.QNAP_DEVICE_ID);
                request.AddCookie("qcloud_tunnel_sid", _qcloud_tunnel_sid);
                IRestResponse response = client.Execute(request);
                var json = JObject.Parse(response.Content);
                if (json != null
                    && json[RESULT_KEY] != null
                    && json[RESULT_KEY][TUNNEL_AGENT_DOMAIN_NAME_KEY] != null)
                {
                    var domainName = (string)json[RESULT_KEY][TUNNEL_AGENT_DOMAIN_NAME_KEY];
                    _api_host = "https://" + domainName.Split('?')[0];

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"GetMyqnapCloudHost qnap_host:{Properties.QNAP_HOST}, device_id: {Properties.QNAP_DEVICE_ID}");
                Console.Error.WriteLine(ex.Message, ex);
            }

            return false;
        }

        private static bool RefreshQcloudTunnelSid()
        {
            try
            {
                var uri = "{0}?tunnel_agent_id={1}&lang=en";
                uri = string.Format(uri, _api_host, Properties.QNAP_DEVICE_ID);

                CookieContainer cookies = new CookieContainer();
                System.Net.Http.HttpClientHandler handler = new System.Net.Http.HttpClientHandler();
                handler.CookieContainer = cookies;

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(handler);
                System.Net.Http.HttpResponseMessage response = client.GetAsync(uri).Result;

                CookieCollection responseCookies = cookies.GetCookies(new Uri(uri));
                foreach (Cookie cookie in responseCookies)
                {
                    if (QCLOUD_TUNNEL_SID_KEY.Equals(cookie.Name))
                    {
                        _qcloud_tunnel_sid = cookie.Value;

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"RefreshQcloudTunnelSid _api_host:{_api_host}, device_id: {Properties.QNAP_DEVICE_ID}");
                Console.Error.WriteLine(ex.Message, ex);
            }

            return false;
        }

        /// <summary>
        /// ログイン
        /// </summary>
        private static bool RefreshSID(string user, string pwd)
        {
            try
            {
                var uri = "{0}/cgi-bin/filemanager/wfm2Login.cgi?user={1}&pwd={2}";
                uri = string.Format(uri, _api_host, user, pwd);

                var client = new RestClient(uri);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddCookie("qcloud_tunnel_agent_id", Properties.QNAP_DEVICE_ID);
                request.AddCookie("qcloud_tunnel_sid", _qcloud_tunnel_sid);
                var response = client.Execute(request);
                var json = JObject.Parse(response.Content);
                _sid = json.GetValue(SID_KEY).ToString();
                _refreshed_time = DateTime.UtcNow;

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"RefreshSID _api_host:{_api_host}, user:{user}, pwd:{pwd}");
                Console.Error.WriteLine(ex.Message, ex);

                throw ex;
            }
        }

        private static string GetSID()
        {
            if (string.IsNullOrEmpty(_api_host)
                || string.IsNullOrEmpty(_qcloud_tunnel_sid)
                || string.IsNullOrEmpty(_sid)
                || _refreshed_time == null
                || (string.IsNullOrEmpty(_sid) && _refreshed_time != null && DateTime.UtcNow.Subtract((DateTime)_refreshed_time).TotalSeconds >= Properties.QNAP_REFRESH_INTERVAL))
            {
                _isRefreshing = true;

                try
                {
                    //APIのホットを取得
                    if (GetMyqnapCloudHost())
                    {
                        //[qcloud_tunnel_sid]を取得
                        if (RefreshQcloudTunnelSid())
                        {
                            RefreshSID(Properties.QNAP_USER, Properties.QNAP_PWD);
                        }
                    }
                }
                finally
                {
                    _isRefreshing = false;
                }
            }

            return _sid;
        }

        private static bool IsValidImage(byte[] bytes)
        {
            try
            {
                //using (MemoryStream ms = new MemoryStream(bytes))
                //    Image.FromStream(ms);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ビュー
        /// </summary>
        public static byte[] GetViewer(string dirPath, string fileName)
        {
            var imageData = GetViewerHandle(dirPath, fileName);
            if (imageData == null || (imageData != null && imageData.Length != 0 && !IsValidImage(imageData)))
            {
                // 再試行
                if (AcceptRetry())
                {
                    _refreshed_time = null;
                    imageData = GetViewerHandle(dirPath, fileName);
                }
            }


            return imageData;
        }

        private static byte[]? GetViewerHandle(string dirPath, string fileName)
        {
            var sid = string.Empty;
            try
            {
                if (Waiting())
                {
                    sid = GetSID();
                    var uri = "{0}/cgi-bin/filemanager/utilRequest.cgi?sid={1}&func=get_viewer&source_path={2}&source_file={3}";
                    uri = string.Format(uri, _api_host, sid, dirPath, fileName);

                    var client = new RestClient(uri);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddCookie("qcloud_tunnel_agent_id", Properties.QNAP_DEVICE_ID);
                    request.AddCookie("qcloud_tunnel_sid", _qcloud_tunnel_sid);
                    return client.DownloadData(request);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"GetViewerHandle _api_host:{_api_host}, sid:{sid}, dirPath:{dirPath}, fileName:{fileName}");
                Console.Error.WriteLine(ex.Message, ex);
            }

            return null;
        }

        public static bool Upload(string dirPath, FileInfo file)
        {
            if (UploadHandle(dirPath, file))
            {
                return true;
            }
            else
            {
                // 再試行
                if (AcceptRetry())
                {
                    _refreshed_time = null;
                    return UploadHandle(dirPath, file);
                }
            }

            return false;
        }

        private static bool UploadHandle(string dirPath, FileInfo file)
        {
            var sid = string.Empty;
            try
            {
                if (Waiting())
                {
                    sid = GetSID();
                    var uri = "{0}/cgi-bin/filemanager/utilRequest.cgi?func=upload&type=standard&overwrite=1&sid={1}&dest_path={2}&progress={3}";
                    uri = string.Format(uri, _api_host, sid, dirPath, file.Name);

                    var client = new RestClient(uri);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddCookie("qcloud_tunnel_agent_id", Properties.QNAP_DEVICE_ID);
                    request.AddCookie("qcloud_tunnel_sid", _qcloud_tunnel_sid);
                    request.AddFile("progress", file.FullName);
                    IRestResponse response = client.Execute(request);
                    var json = JObject.Parse(response.Content);
                    if (json.GetValue(STATUS_KEY).ToString() == STATUS_SUCCESS)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"UploadHandle _api_host:{_api_host}, sid:{sid}, dirPath:{dirPath}, fileName:{file.Name}");
                Console.Error.WriteLine(ex.Message, ex);
            }

            return false;
        }

        public static bool DeleteFolder(string dirPath, string folderName)
        {
            if (DeleteFolderHandle(dirPath, folderName))
            {
                return true;
            }
            else
            {
                // 再試行
                if (AcceptRetry())
                {
                    _refreshed_time = null;
                    return DeleteFolderHandle(dirPath, folderName);
                }
            }

            return false;
        }

        private static bool DeleteFolderHandle(string dirPath, string folderName)
        {
            var sid = string.Empty;
            try
            {
                if (Waiting())
                {
                    sid = GetSID();
                    var uri = "{0}/cgi-bin/filemanager/utilRequest.cgi?func=delete&sid={1}&path={2}&file_total=1&file_name={3}&v=1&force=1";
                    uri = string.Format(uri, _api_host, sid, dirPath, folderName);

                    var client = new RestClient(uri);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddCookie("qcloud_tunnel_agent_id", Properties.QNAP_DEVICE_ID);
                    request.AddCookie("qcloud_tunnel_sid", _qcloud_tunnel_sid);
                    IRestResponse response = client.Execute(request);
                    var json = JObject.Parse(response.Content);
                    if (json.GetValue(STATUS_KEY).ToString() == STATUS_SUCCESS)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"UploadHandle _api_host:{_api_host}, sid:{sid}, dirPath:{dirPath}, folderName:{folderName}");
                Console.Error.WriteLine(ex.Message, ex);
            }

            return false;
        }

        public static bool DeleteFiles(string dirPath, List<string> listFileName)
        {
            if (DeleteFilesHandle(dirPath, listFileName))
            {
                return true;
            }
            else
            {
                // 再試行
                if (AcceptRetry())
                {
                    _refreshed_time = null;
                    return DeleteFilesHandle(dirPath, listFileName);
                }
            }

            return false;
        }

        private static bool DeleteFilesHandle(string dirPath, List<string> listFileName)
        {
            var sid = string.Empty;
            try
            {
                if (Waiting())
                {
                    sid = GetSID();
                    var uri = "{0}/cgi-bin/filemanager/utilRequest.cgi?func=delete&sid={1}&path={2}&file_total={3}&v=1&force=1&{4}";
                    uri = string.Format(uri, _api_host, sid, dirPath, listFileName.Count, string.Join("&", listFileName));

                    var client = new RestClient(uri);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddCookie("qcloud_tunnel_agent_id", Properties.QNAP_DEVICE_ID);
                    request.AddCookie("qcloud_tunnel_sid", _qcloud_tunnel_sid);
                    IRestResponse response = client.Execute(request);
                    var json = JObject.Parse(response.Content);
                    if (json.GetValue(STATUS_KEY).ToString() == STATUS_SUCCESS)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"DeleteFilesHandle _api_host:{_api_host}, sid:{sid}, dirPath:{dirPath}, listFileName:{listFileName}");
                Console.Error.WriteLine(ex.Message, ex);
            }

            return false;
        }

        public static bool CreateDir(string dirPath, string folderName)
        {
            if (CreateDirHandle(dirPath, folderName))
            {
                return true;
            }
            else
            {
                // 再試行
                if (AcceptRetry())
                {
                    _refreshed_time = null;
                    return CreateDirHandle(dirPath, folderName);
                }
            }

            return false;
        }

        private static bool CreateDirHandle(string dirPath, string folderName)
        {
            var sid = string.Empty;
            try
            {
                if (Waiting())
                {
                    sid = GetSID();
                    var uri = "{0}/cgi-bin/filemanager/utilRequest.cgi?func=createdir&sid={1}&dest_path={2}&dest_folder={3}";
                    uri = string.Format(uri, _api_host, sid, dirPath, folderName);

                    var client = new RestClient(uri);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddCookie("qcloud_tunnel_agent_id", Properties.QNAP_DEVICE_ID);
                    request.AddCookie("qcloud_tunnel_sid", _qcloud_tunnel_sid);
                    IRestResponse response = client.Execute(request);
                    var json = JObject.Parse(response.Content);
                    if (json.GetValue(STATUS_KEY).ToString() == STATUS_SUCCESS)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"DeleteFilesHandle _api_host:{_api_host}, sid:{sid}, dirPath:{dirPath}, folderName:{folderName}");
                Console.Error.WriteLine(ex.Message, ex);
            }

            return false;
        }

        public static bool IsFolderExist(string dirPath, string folderName)
        {
            if (IsFolderExistHandle(dirPath, folderName))
            {
                return true;
            }
            else
            {
                // 再試行
                if (AcceptRetry())
                {
                    _refreshed_time = null;
                    return IsFolderExistHandle(dirPath, folderName);
                }
            }
            return false;
        }

        private static bool IsFolderExistHandle(string dirPath, string folderName)
        {
            var sid = string.Empty;
            try
            {
                if (Waiting())
                {
                    sid = GetSID();
                    var uri = "{0}/cgi-bin/filemanager/utilRequest.cgi?func=get_tree&sid={1}&is_iso=0&node={2}";
                    uri = string.Format(uri, _api_host, sid, dirPath, folderName);

                    var client = new RestClient(uri);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddCookie("qcloud_tunnel_agent_id", Properties.QNAP_DEVICE_ID);
                    request.AddCookie("qcloud_tunnel_sid", _qcloud_tunnel_sid);
                    IRestResponse response = client.Execute(request);
                    var json = JArray.Parse(response.Content);
                    var selectPatern = string.Format("$.[?(@.text == '{0}')]", folderName);
                    JToken item = json.SelectToken(selectPatern);

                    if (item != null)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"DeleteFilesHandle _api_host:{_api_host}, sid:{sid}, dirPath:{dirPath}, folderName:{folderName}");
                Console.Error.WriteLine(ex.Message, ex);
            }

            return false;
        }

        private static bool AcceptRetry()
        {
            if (_refreshed_time != null && DateTime.UtcNow.Subtract((DateTime)_refreshed_time).TotalSeconds >= 60)
            {
                return true;
            }
            return false;
        }

        private static bool Waiting()
        {
            var result = false;
            var pointTime = DateTime.UtcNow;
            while (_isRefreshing)
            {
                Thread.Sleep(1000);

                if (DateTime.UtcNow.Subtract((DateTime)pointTime).TotalSeconds >= 10)
                {
                    break;
                }
            }

            if (!_isRefreshing)
            {
                result = true;
            }

            return result;
        }
    }
}