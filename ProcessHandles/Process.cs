using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using semigura.Commons;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using System.Globalization;
using System.Net;

namespace semigura.ProcessHandles
{
    public static class Process
    {
        private static readonly CultureInfo _ALERT_CULTURE_JA = CultureInfo.GetCultureInfo(semigura.Commons.Properties.LOCALE_JA);
        private static readonly string _ALERT_TITLE_RESOURCE_NAME = "process_alert_title";
        private static readonly string _ALERT_TITLE_PROCESS_MOROMI_RESOURCE_NAME = "process_moromi";
        private static readonly string _ALERT_TITLE_PROCESS_SEIGIKU_RESOURCE_NAME = "process_seigiku";
        private static readonly string _ALERT_TITLE_PROCESS_SEIGIKU_TANK_RESOURCE_NAME = "type_tank";
        private static readonly string _ALERT_TITLE_PROCESS_SEIGIKU_LOT_RESOURCE_NAME = "lot";
        private static readonly string _ALERT_CONTENT_TANK_RESOURCE_NAME = "process_alert_content_tank";
        private static readonly string _ALERT_CONTENT_SEIGIKU_RESOURCE_NAME = "process_alert_content_seigiku";

        /// <summary>
        /// センサーのデータを読み込む
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //public async static Task ReadSensorData()
        //{
        //    try
        //    {
        //        System.Diagnostics.Debug.WriteLine("ReadSensorData:" + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy/MM/dd HH:mm:ss"));
        //        Console.Error.WriteLine("ReadSensorData:" + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy/MM/dd HH:mm:ss"));

        //        //TODO
        //        var bussiness = new ProcessBusiness(new DBEntities(), null, null); // TODO

        //        var url = semigura.Commons.Properties.ONDOTORI_URL;
        //        var apiKey = semigura.Commons.Properties.ONDOTORI_API;
        //        var loginId = semigura.Commons.Properties.ONDOTORI_LOGIN_ID;
        //        var loginPass = semigura.Commons.Properties.ONDOTORI_LOGIN_PASS;

        //        if (!string.IsNullOrEmpty(url)
        //            && !string.IsNullOrEmpty(apiKey)
        //            && !string.IsNullOrEmpty(loginId)
        //            && !string.IsNullOrEmpty(loginPass))
        //        {

        //            // 製麴用端末のデータを取得
        //            var listTerminalSeigiku = bussiness.GetListTerminalByType(semigura.Commons.Properties.TERMINAL_TYPE_SEIGIKU);
        //            var hasChangedFlg1 = SeigikuTerminalProcess(listTerminalSeigiku, url, apiKey, loginId, loginPass);

        //            // ロケーション用端末のデータを取得
        //            var listTerminalLocation = bussiness.GetListTerminalByType(semigura.Commons.Properties.TERMINAL_TYPE_LOCATION);
        //            var hasChangedFlg2 = LocationTerminalProcess(listTerminalLocation, url, apiKey, loginId, loginPass);

        //            // クライアントへ最新データを更新
        //            if (hasChangedFlg1 || hasChangedFlg2)
        //            {
        //                //await S01002Hub.RefreshData().ConfigureAwait(false); // TODO
        //                //await S02001Hub.RefreshData().ConfigureAwait(false);
        //                //await S02002Hub.RefreshData().ConfigureAwait(false);
        //                //await S02003Hub.RefreshData().ConfigureAwait(false);

        //                // アラート判定
        //                var listLotContainerId = listTerminalSeigiku.Select(s => s.LotContainerId).ToList();
        //                listLotContainerId.AddRange(listTerminalSeigiku.Select(s => s.LotContainerId).ToList());
        //                _ = Task.Run(() => Process.JudgmentAlert(listLotContainerId).ConfigureAwait(false));
        //            }
        //        }
        //        else
        //        {
        //            Console.Error.WriteLine("ONDOTORIをコールする設定情報が正しくない。");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine(ex.Message, ex);
        //    }
        //}

        /// <summary>
        /// 製麹のセンサーのデータを読み込む。
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="url"></param>
        /// <param name="apiKey"></param>
        /// <param name="loginId"></param>
        /// <param name="loginPass"></param>
        private static bool SeigikuTerminalProcess(List<TerminalModel> terminals, string url, string apiKey, string loginId, string loginPass)
        {
            var hasChangedFlg = false;

            foreach (var terminal in terminals)
            {
                try
                {
                    if (!string.IsNullOrEmpty(terminal.LotContainerId))
                    {
                        // 端末が製麴に使用中の場合
                        // TODO
                        var bussiness = new ProcessBusiness(new DBEntities(), null, null);
                        var sensorData = bussiness.GetFirstSensorData(terminal.Id);

                        var bodyObj = new JObject();
                        bodyObj.Add(new JProperty("api-key", apiKey));
                        bodyObj.Add(new JProperty("login-id", loginId));
                        bodyObj.Add(new JProperty("login-pass", loginPass));
                        bodyObj.Add(new JProperty("remote-serial", terminal.Code));

                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Headers.Add("X-HTTP-Method-Override", "GET");
                        httpWebRequest.Method = "POST";
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                        Console.Out.WriteLine(bodyObj.ToString());
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            var json = JsonConvert.SerializeObject(bodyObj);
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            JObject json = JObject.Parse(result);

                            var sensorDataList = new List<SensorData>();

                            // 参考：https://www.newtonsoft.com/json/help/html/queryinglinqtojson.htm
                            JArray dataArr = (JArray)json["data"];

                            for (int i = 0; i < dataArr.Count; i++)
                            {
                                try
                                {
                                    JObject obj = (JObject)dataArr[i];
                                    var unixtime = (long)obj["unixtime"];
                                    var ch1 = (decimal)obj["ch1"];
                                    var ch2 = (decimal)obj["ch2"];

                                    if ((terminal.LotContainerStartDate != null && terminal.LotContainerStartDate.Value < Utils.UnixTimeStampToDateTime(unixtime))
                                        && (terminal.LotContainerEndDate == null || (terminal.LotContainerEndDate != null && terminal.LotContainerEndDate.Value < Utils.UnixTimeStampToDateTime(unixtime)))
                                        && (sensorData == null || (sensorData != null && sensorData.MeasureDate < Utils.UnixTimeStampToDateTime(unixtime))))
                                    {
                                        sensorDataList.Add(new SensorData
                                        {
                                            TerminalId = terminal.Id,
                                            Temperature1 = ch1,
                                            Temperature2 = ch2,
                                            MeasureDate = Utils.UnixTimeStampToDateTime(unixtime),
                                            CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time),
                                            LotContainerId = terminal.LotContainerId,
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.Error.WriteLine(ex.Message, ex);
                                }
                            }

                            if (sensorDataList != null && sensorDataList.Any())
                            {
                                bussiness.AddListSensorData(sensorDataList);
                                hasChangedFlg = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                Thread.Sleep(5000);
            }

            return hasChangedFlg;
        }

        /// <summary>
        /// ロケーションのセンサーのデータを読み込む。
        /// </summary>
        /// <param name="terminals"></param>
        /// <param name="url"></param>
        /// <param name="apiKey"></param>
        /// <param name="loginId"></param>
        /// <param name="loginPass"></param>
        private static bool LocationTerminalProcess(List<TerminalModel> terminals, string url, string apiKey, string loginId, string loginPass)
        {
            var hasChangedFlg = false;

            foreach (var terminal in terminals)
            {
                try
                {
                    if (!string.IsNullOrEmpty(terminal.LotContainerId))
                    {
                        // 端末がロケーションに使用中の場合
                        // TODO
                        var bussiness = new ProcessBusiness(new DBEntities(), null, null); // TODO
                        var sensorData = bussiness.GetFirstSensorData(terminal.Id);

                        var bodyObj = new JObject();
                        bodyObj.Add(new JProperty("api-key", apiKey));
                        bodyObj.Add(new JProperty("login-id", loginId));
                        bodyObj.Add(new JProperty("login-pass", loginPass));
                        bodyObj.Add(new JProperty("remote-serial", terminal.Code));

                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Headers.Add("X-HTTP-Method-Override", "GET");
                        httpWebRequest.Method = "POST";
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            var json = JsonConvert.SerializeObject(bodyObj);
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var result = streamReader.ReadToEnd();
                            JObject json = JObject.Parse(result);

                            var sensorDataList = new List<SensorData>();

                            // 参考：https://www.newtonsoft.com/json/help/html/queryinglinqtojson.htm
                            JArray dataArr = (JArray)json["data"];

                            for (int i = 0; i < dataArr.Count; i++)
                            {
                                try
                                {
                                    JObject obj = (JObject)dataArr[i];
                                    var unixtime = (long)obj["unixtime"];
                                    var ch1 = (decimal)obj["ch1"];
                                    var ch2 = (decimal)obj["ch2"];

                                    if (sensorData == null || (sensorData != null && sensorData.MeasureDate < Utils.UnixTimeStampToDateTime(unixtime)))
                                    {
                                        sensorDataList.Add(new SensorData
                                        {
                                            TerminalId = terminal.Id,
                                            Temperature1 = ch1,
                                            Humidity = ch2,
                                            MeasureDate = Utils.UnixTimeStampToDateTime(unixtime),
                                            CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time),
                                            LotContainerId = terminal.LotContainerId,
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.Error.WriteLine(ex.Message, ex);
                                }
                            }

                            if (sensorDataList != null && sensorDataList.Any())
                            {
                                bussiness.AddListSensorData(sensorDataList);
                                hasChangedFlg = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }

                Thread.Sleep(5000);
            }

            return hasChangedFlg;
        }

        /// <summary>
        /// アラート判定
        /// </summary>
        /// <returns></returns>
        //public async static Task JudgmentAlert(List<string> listContainerId)
        //{
        //    // TODO
        //    var bussiness = new ProcessBusiness(new DBEntities(), null, null); // TODO
        //    var results = bussiness.GetLotContainerGoingOn(listContainerId);

        //    var groups = results.GroupBy(s => new { s.Id, s.LotId, s.LotCode, s.ContainerId, s.TankCode, s.ContainerType, s.LocationId, s.TempMin, s.TempMax }).ToList();

        //    var updateOrAddList = new List<Notification>();

        //    foreach (var grp in groups)
        //    {
        //        var lastestSensorData = grp.Where(s => s.MeasureDate.HasValue == true).ToList().OrderByDescending(s => s.MeasureDate).FirstOrDefault();
        //        if (lastestSensorData != null)
        //        {
        //            var notification = bussiness.GetNotificationNotClosed(grp.Key.Id);

        //            // タイトル
        //            var title = string.Empty;
        //            if (grp.Key.ContainerType == semigura.Commons.Properties.CONTAINER_TYPE_TANK)
        //            {
        //                var resouce = new System.Resources.ResourceManager(typeof(S03005)).GetString(_ALERT_TITLE_RESOURCE_NAME, _ALERT_CULTURE_JA);
        //                if (resouce != null)
        //                {
        //                    title = resouce;

        //                    var moromiResouce = new System.Resources.ResourceManager(typeof(S03005)).GetString(_ALERT_TITLE_PROCESS_MOROMI_RESOURCE_NAME, _ALERT_CULTURE_JA);
        //                    var tankResouce = new System.Resources.ResourceManager(typeof(S03005)).GetString(_ALERT_TITLE_PROCESS_SEIGIKU_TANK_RESOURCE_NAME, _ALERT_CULTURE_JA);
        //                    title = string.Format(title, moromiResouce, tankResouce, grp.Key.TankCode);
        //                }

        //            }
        //            else
        //            {
        //                var resouce = new System.Resources.ResourceManager(typeof(S03005)).GetString(_ALERT_TITLE_RESOURCE_NAME, _ALERT_CULTURE_JA);
        //                if (resouce != null)
        //                {
        //                    title = resouce;

        //                    var moromiResouce = new System.Resources.ResourceManager(typeof(S03005)).GetString(_ALERT_TITLE_PROCESS_SEIGIKU_RESOURCE_NAME, _ALERT_CULTURE_JA);
        //                    var lotResouce = new System.Resources.ResourceManager(typeof(S03005)).GetString(_ALERT_TITLE_PROCESS_SEIGIKU_LOT_RESOURCE_NAME, _ALERT_CULTURE_JA);
        //                    title = string.Format(title, moromiResouce, lotResouce, grp.Key.LotCode);
        //                }
        //            }

        //            // コンテンツ
        //            var temmplateContent = string.Empty;
        //            var tankOrSensorName = string.Empty;
        //            if (lastestSensorData.ContainerType == semigura.Commons.Properties.CONTAINER_TYPE_TANK)
        //            {
        //                var resouce = new System.Resources.ResourceManager(typeof(S03005)).GetString(_ALERT_CONTENT_TANK_RESOURCE_NAME, _ALERT_CULTURE_JA);
        //                if (resouce != null)
        //                {
        //                    temmplateContent = resouce;
        //                }

        //                tankOrSensorName = lastestSensorData.TankCode;
        //            }
        //            else
        //            {
        //                var resouce = new System.Resources.ResourceManager(typeof(S03005)).GetString(_ALERT_CONTENT_SEIGIKU_RESOURCE_NAME, _ALERT_CULTURE_JA);
        //                if (resouce != null)
        //                {
        //                    temmplateContent = resouce;
        //                }
        //                tankOrSensorName = string.Format("{0}({1})", lastestSensorData.TerminalCode, lastestSensorData.TerminalName);
        //            }
        //            var content = notification != null ? notification.Content : string.Empty;
        //            DateTime? checkedTime = notification != null ? (notification.ModifiedOn != null ? notification.ModifiedOn : notification.CreatedOn) : null;

        //            if (checkedTime.HasValue)
        //            {
        //                if (lastestSensorData.MeasureDate > checkedTime)
        //                {
        //                    var newContent = string.Format("{0}：", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy/MM/dd HH:mm"));
        //                    newContent += Environment.NewLine;
        //                    newContent += string.Format(
        //                        temmplateContent,
        //                        grp.Key.TempMin.HasValue ? ((decimal)grp.Key.TempMin).ToString("0.#") : "-",
        //                        grp.Key.TempMax.HasValue ? ((decimal)grp.Key.TempMax).ToString("0.#") : "-",
        //                        lastestSensorData.TemperatureAvg.HasValue ? ((decimal)lastestSensorData.TemperatureAvg).ToString("0.#") : "-",
        //                        lastestSensorData.TerminalCode);
        //                    newContent += Environment.NewLine;
        //                    newContent += Environment.NewLine;

        //                    content = newContent + content;


        //                    // 更新アイテムを追加
        //                    notification.Content = content;
        //                    notification.Title = title;
        //                    updateOrAddList.Add(notification);
        //                }
        //            }
        //            else
        //            {
        //                content += string.Format("{0}：", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyy/MM/dd HH:mm"));
        //                content += Environment.NewLine;
        //                content += string.Format(
        //                        temmplateContent,
        //                        grp.Key.TempMin.HasValue ? ((decimal)grp.Key.TempMin).ToString("0.#") : "-",
        //                        grp.Key.TempMax.HasValue ? ((decimal)grp.Key.TempMax).ToString("0.#") : "-",
        //                        lastestSensorData.TemperatureAvg.HasValue ? ((decimal)lastestSensorData.TemperatureAvg).ToString("0.#") : "-",
        //                        lastestSensorData.TerminalCode);
        //                content += Environment.NewLine;

        //                // 新規登録のアイテムを追加
        //                updateOrAddList.Add(new Notification
        //                {
        //                    ParentId = grp.Key.Id,
        //                    Type = semigura.Commons.Properties.NOTIFICATION_TYPE_LOTCONTAINER,
        //                    Title = title,
        //                    Content = content,
        //                    Level = semigura.Commons.Properties.NOTIFICATION_LEVEL_WARNING,
        //                    Status = semigura.Commons.Properties.NOTIFICATION_STATUS_OPEN,
        //                });
        //            }
        //        }
        //    }


        //    bussiness.UpdateOrInsertNotification(updateOrAddList);

        //    await Task.Delay(0);
        //}

        /// <summary>
        /// ゴミデータを削除
        /// </summary>
        public async static Task DeleteJunkData()
        {
            try
            {
                // デバイスの画像を削除
                int maxDays = 15;
                string[] files = Directory.GetFiles(
                    System.IO.Path.Combine(
                        DeviceConnectionManagement.ENVIRONMENT_ROOT_PATH,
                        DeviceConnectionManagement.ENVIRONMENT_IMAGE_PATH),
                    "*.jpg",
                    SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    DateTime creation = File.GetCreationTime(file);
                    if (DateTime.UtcNow.Subtract(creation).TotalDays > maxDays)
                    {
                        File.Delete(file);
                    }
                }

                // 他
                // TODO
                var bussiness = new ProcessBusiness(new DBEntities(), null, null); // TODO
                bussiness.DeleteJunkData();

                await Task.Delay(0);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }
        }
    }
}