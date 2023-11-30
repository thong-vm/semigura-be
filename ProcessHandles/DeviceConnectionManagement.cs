using Newtonsoft.Json.Linq;
using semigura.Commons;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using Xabe.FFmpeg;

namespace semigura.ProcessHandles
{
    public class DeviceConnectionManagement
    {
        //public static IStringLocalizer<SharedResource> sharedLocalizer;

        public const bool USING_XABE_FLG = false;
        private const int STREAM_DATA_TYPE = 1; //【1:jpeg, 2:mjpeg, 3:h264】

        private const string TERMINAL_CODE_KEY = "OpCode"; // アクションタイプのキー
        private const string TERMINAL_DATA_KEY = "Data";
        private const string TERMINAL_IMAGE_KEY = "image";

        private const int TERMINAL_CODE_PERIODIC = 10;
        private const int TERMINAL_CODE_REQUEST_FRAME = 1;
        private const int TERMINAL_SUB_ACTION_START_STREAM = 1;
        private const int TERMINAL_SUB_ACTION_STOP_STREAM = 0;
        private const int TERMINAL_SUB_ACTION_REQUEST_ONE_FRAME = 2;
        private const int TERMINAL_CODE_RESPONSE_FRAME = 11;

        private const int CLOSE_SOCKET_TIMEOUT_MS = 2500;
        private const int MAX_MESSAGE_SIZE = 1024 * 500; // 受信パケットの最大長
        private const int SET_BIT_RATE = 4096 * 1000;  // 設定するカメラのビットレートに基づく

        public static string ENVIRONMENT_FILE_FOR_WEB_EXTENSION = "jpg";

        //private static string ENVIRONMENT_FFMPEG_PATH = HostingEnvironment.MapPath(@"~/Libs/ffmpeg/_ffmpeg.exe");
        //public static string ENVIRONMENT_ROOT_PATH = HostingEnvironment.MapPath(@"~/");
        private static string ENVIRONMENT_FFMPEG_PATH = @"~/Libs/ffmpeg/_ffmpeg.exe";
        public static string ENVIRONMENT_ROOT_PATH = @"~/";
        public static string ENVIRONMENT_STREAM_PATH = $"Uploads{System.IO.Path.DirectorySeparatorChar}Stream";
        public static string ENVIRONMENT_IMAGE_PATH = $"Uploads{System.IO.Path.DirectorySeparatorChar}Images";

        public static string NAS_ROOT_DIR_PATH = $"/Fukui_Image/images";


        public static ConcurrentDictionary<string, ConnectedClient> Clients = new ConcurrentDictionary<string, ConnectedClient>();

        public static CancellationTokenSource SocketLoopTokenSource = new CancellationTokenSource();

        public static async Task CloseAllSocketsAsync()
        {
            //処理ループが終了するまで、ソケットを破棄することはできない。
            //ただし、ループを終了するとソケットが中止され、正常に閉じることができなくなる。
            var disposeQueue = new List<WebSocket>(Clients.Count);

            while (Clients.Count > 0)
            {
                var client = Clients.ElementAt(0).Value;
                System.Diagnostics.Debug.WriteLine($"Closing Socket {client.SocketId}");

                System.Diagnostics.Debug.WriteLine("... ending broadcast loop");

                if (client.Socket.State != WebSocketState.Open)
                {
                    System.Diagnostics.Debug.WriteLine($"... socket not open, state = {client.Socket.State}");
                }
                else
                {
                    var timeout = new CancellationTokenSource(CLOSE_SOCKET_TIMEOUT_MS);
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("... starting close handshake");
                        await client.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", timeout.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        // タスク/トークンのキャンセル時に正常、無視
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }

                if (Clients.TryRemove(client.SocketId, out _))
                {
                    //一度だけ廃棄しても安全なので、このループで再度処理できない場合にのみ追加する。
                    disposeQueue.Add(client.Socket);
                }

                System.Diagnostics.Debug.WriteLine("... done");
            }

            // これらがすべて閉じられたので、SocketProcessingLoopスレッドでブロックしているReceiveAsync呼び出しを終了する。
            SocketLoopTokenSource.Cancel();

            // すべてのリソースを破棄する。
            foreach (var socket in disposeQueue) socket.Dispose();
        }

        /// <summary>
        /// 端末のデータの持ち
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        //public static async Task SocketProcessingLoopAsync(ConnectedClient client)
        //{
        //    var socket = client.Socket;
        //    try
        //    {
        //        _ = Task.Run(() => CheckViewerProcessingLoopAsync(client).ConfigureAwait(false));

        //        byte[] receiveBuffer = new byte[MAX_MESSAGE_SIZE];

        //        while (socket.State == WebSocketState.Open)
        //        {
        //            WebSocketReceiveResult receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

        //            if (receiveResult.MessageType == WebSocketMessageType.Close)
        //            {
        //                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        //            }
        //            else
        //            {
        //                int count = receiveResult.Count;

        //                while (receiveResult.EndOfMessage == false)
        //                {
        //                    if (count >= MAX_MESSAGE_SIZE)
        //                    {
        //                        string closeMessage = string.Format(sharedLocalizer["C01009"], MAX_MESSAGE_SIZE);
        //                        await socket.CloseAsync(WebSocketCloseStatus.MessageTooBig, closeMessage, CancellationToken.None);
        //                        return;
        //                    }

        //                    receiveResult = await socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer, count, MAX_MESSAGE_SIZE - count), CancellationToken.None);
        //                    count += receiveResult.Count;
        //                }

        //                if (receiveResult.MessageType == WebSocketMessageType.Text)
        //                {
        //                    try
        //                    {
        //                        var receivedString = Encoding.UTF8.GetString(receiveBuffer, 0, count);
        //                        JObject json = JObject.Parse(receivedString);
        //                        var code = int.Parse(json.GetValue(TERMINAL_CODE_KEY).ToString());
        //                        if (code == TERMINAL_CODE_PERIODIC)
        //                        {
        //                            // 温度のデータ
        //                            _ = Task.Run(() => TemperatureDataProcess(client, (JObject)json["Data"]).ConfigureAwait(false));
        //                        }
        //                        else if (code == TERMINAL_CODE_RESPONSE_FRAME)
        //                        {
        //                            Console.Out.WriteLine("デバイスのデータ（ストリーム）：" + json.ToString());
        //                            // フレームのデータ（ストリーム）
        //                            var base64String = json.GetValue(TERMINAL_IMAGE_KEY).ToString();
        //                            if (!string.IsNullOrEmpty(base64String))
        //                            {
        //                                byte[] imageBytes = Convert.FromBase64String(base64String);

        //                                await FrameProcess(client, imageBytes);
        //                            }

        //                            if (client.StreamViewerClientIds.Count > 0)
        //                            {
        //                                client.IsStreaming = true;

        //                                CallDeviceSendFrame(client);
        //                            }
        //                            else
        //                            {
        //                                client.IsStreaming = false;
        //                            }
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.Error.WriteLine(ex.Message, ex);
        //                    }
        //                }
        //                else if (receiveResult.MessageType == WebSocketMessageType.Binary)
        //                {
        //                    try
        //                    {
        //                        // フレームのデータ（ストリーム）
        //                        await FrameProcess(client, receiveBuffer);

        //                        if (client.StreamViewerClientIds.Count > 0)
        //                        {
        //                            client.IsStreaming = true;

        //                            CallDeviceSendFrame(client);
        //                        }
        //                        else
        //                        {
        //                            client.IsStreaming = false;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.Error.WriteLine(ex.Message, ex);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        // タスク/トークンのキャンセル時に正常、無視
        //    }
        //    catch (WebSocketException ex)
        //    {
        //        // タスク/トークンのキャンセル時に正常、無視
        //        Console.Out.WriteLine(ex.Message, ex);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine(ex.Message, ex);
        //    }
        //    finally
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Socket {client.SocketId}: Ended processing loop in state {socket.State}");

        //        if (Clients.TryRemove(client.SocketId, out _))
        //        {
        //            try
        //            {
        //                // 接続されている可能性のある状態でソケットを離れない。
        //                if (client.Socket.State != WebSocketState.Closed)
        //                    client.Socket.Abort();

        //                // この時点で、ソケットは閉じられるか中止され、ConnectedClientオブジェクトは役に立たなくなる。
        //                if (Clients.TryRemove(client.SocketId, out _))
        //                    client.Socket.Dispose();
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.Error.WriteLine(ex.Message, ex);
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// ビューア数をチェックし、ストリームのストップを判定する。
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task CheckViewerProcessingLoopAsync(ConnectedClient client)
        {
            // すべての過去の臨時ファイルを削除
            try
            {
                await DeleteOldFile(client).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }


            while (client.Socket.State == WebSocketState.Open)
            {
                try
                {
                    if (client.StreamViewerClientIds.Count == 0 && client.IsStreaming)
                    {
                        CallDeviceStopStream(client);

                        await DeleteOldFile(client);

                        await Task.Delay(3000);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message, ex);
                }
            }
        }

        private static async Task DeleteOldFile(ConnectedClient client)
        {
            // すべてのファイルを削除
            var dir = new DirectoryInfo(System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, client.SocketId));
            var files = dir.GetFiles($"*.{ENVIRONMENT_FILE_FOR_WEB_EXTENSION}");
            foreach (var file in files)
            {
                file.Delete();
            }

            await Task.Delay(0);
        }

        /// <summary>
        /// 品温のデータを処理
        /// </summary>
        /// <param name="client"></param>
        /// <param name="receiveBuffer"></param>
        /// <returns></returns>
        //public static async Task TemperatureDataProcess(ConnectedClient client, JObject data)
        //{
        //    Console.Out.WriteLine("TemperatureDataProcess Start!");
        //    if (semigura.Commons.Properties.IS_SERVER_DEBUGGER) System.Diagnostics.Debugger.Launch();

        //    try
        //    {
        //        Console.Out.WriteLine("デバイスのデータ：" + data.ToString());
        //        var bussiness = new ProcessBusiness(new DBEntities(), null, null); // TODO
        //        var terminal = bussiness.GetTerminalByType(client.SocketId);
        //        if (terminal != null && !string.IsNullOrEmpty(terminal.LotContainerId))
        //        {
        //            // 品温保存
        //            var ch1 = (decimal)data["Temp"]["temperature1"];
        //            var ch2 = (decimal)data["Temp"]["temperature2"];
        //            var ch3 = (decimal)data["Temp"]["temperature3"];

        //            var sensorData = new SensorData
        //            {
        //                TerminalId = terminal.Id,
        //                Temperature1 = ch1,
        //                Temperature2 = ch2,
        //                Temperature3 = ch3,
        //                MeasureDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time),
        //                CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time),
        //                LotContainerId = terminal.LotContainerId,
        //            };

        //            bussiness.AddSensorData(sensorData);


        //            // イメージ保存
        //            var base64String = data.GetValue(TERMINAL_IMAGE_KEY).ToString();
        //            if (!string.IsNullOrEmpty(base64String))
        //            {
        //                byte[] imageBytes = Convert.FromBase64String(base64String);

        //                if (!Directory.Exists(Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_IMAGE_PATH, terminal.LotContainerId)))
        //                {
        //                    Directory.CreateDirectory(Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_IMAGE_PATH, terminal.LotContainerId));
        //                }

        //                var fileName = $"{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time).ToString("yyyyMMddHHmmssfff")}.{ENVIRONMENT_FILE_FOR_WEB_EXTENSION}";
        //                var filePath = Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_IMAGE_PATH, terminal.LotContainerId, fileName);

        //                File.WriteAllBytes(filePath, imageBytes.ToArray());

        //                var media = new Media
        //                {
        //                    Path = fileName,
        //                    Type = semigura.Commons.Properties.MEDIA_TYPE_IMAGE,
        //                    TerminalId = terminal.Id,
        //                    CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time),
        //                    LotContainerId = terminal.LotContainerId,
        //                };

        //                var message = bussiness.AddMedia(media);

        //                //20220308 thaonv update
        //                if (string.IsNullOrEmpty(message))
        //                {
        //                    try
        //                    {
        //                        // フォルダが存在するかチェック、存在しない場合、作成
        //                        var folderName = terminal.LotContainerId;
        //                        if (!NASUtil.IsFolderExist(NAS_ROOT_DIR_PATH, folderName)) NASUtil.CreateDir(NAS_ROOT_DIR_PATH, folderName);

        //                        // ファイルをアップロード
        //                        var dirPath = NAS_ROOT_DIR_PATH + "/" + folderName;
        //                        var file = new FileInfo(filePath);
        //                        NASUtil.Upload(dirPath, file);

        //                        // ファイルを削除
        //                        // File.Delete(filePath);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.Error.WriteLine(ex.Message, ex);
        //                    }
        //                }
        //            }

        //            // クライアントへ最新データを更新
        //            //await S01002Hub.RefreshData().ConfigureAwait(false); // TODO
        //            //await S02001Hub.RefreshData().ConfigureAwait(false);
        //            //await S02002Hub.RefreshData().ConfigureAwait(false);
        //            //await S02003Hub.RefreshData().ConfigureAwait(false);

        //            // アラート判定
        //            var listLotContainerId = new List<string>() { terminal.LotContainerId };
        //            _ = Task.Run(() => Process.JudgmentAlert(listLotContainerId).ConfigureAwait(false));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine(ex.Message, ex);
        //    }

        //    Console.Out.WriteLine("TemperatureDataProcess Stop!");
        //}

        /// <summary>
        /// カメラのデータを処理
        /// </summary>
        /// <param name="client"></param>
        /// <param name="receiveBuffer"></param>
        /// <returns></returns>
        public static async Task FrameProcess(ConnectedClient client, byte[] receiveBuffer)
        {
            try
            {
                switch (STREAM_DATA_TYPE)
                {
                    case 1:
                        await JPEGProcess(client, receiveBuffer);
                        break;
                    case 2:
                        if (USING_XABE_FLG) { await MJPEGWithXabeProcess(client, receiveBuffer); } else { await MJPEGProcess(client, receiveBuffer); }
                        break;
                    case 3:
                        if (USING_XABE_FLG) { await H264WithXabeProcess(client, receiveBuffer); } else { await H264Process(client, receiveBuffer); }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }
        }

        /// <summary>
        /// Jpeg処理
        /// </summary>
        /// <returns></returns>
        private static async Task JPEGProcess(ConnectedClient client, byte[] receiveBuffer)
        {
            SaveToPhysicalFile(receiveBuffer, client.SocketId, ENVIRONMENT_FILE_FOR_WEB_EXTENSION);

            await DeleteOldestJpgFile(client).ConfigureAwait(false);
        }

        /// <summary>
        /// Mjpeg処理
        /// </summary>
        /// <returns></returns>
        private static async Task MJPEGProcess(ConnectedClient client, byte[] receiveBuffer)
        {
            var mjpegFilePath = SaveToPhysicalFile(receiveBuffer, client.SocketId, "mjpeg");

            var filePath = System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, client.SocketId, $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{ENVIRONMENT_FILE_FOR_WEB_EXTENSION}");

            var paremeters = " -i {0} {1}";
            paremeters = string.Format(paremeters, mjpegFilePath, filePath);
            Execute(ENVIRONMENT_FFMPEG_PATH, paremeters);

            await DeleteOldestJpgFile(client).ConfigureAwait(false);
        }

        /// <summary>
        /// Mjpeg処理(Xabe)
        /// </summary>
        /// <returns></returns>
        private static async Task MJPEGWithXabeProcess(ConnectedClient client, byte[] receiveBuffer)
        {
            var mjpegFilePath = SaveToPhysicalFile(receiveBuffer, client.SocketId, "mjpeg");

            var filePath = System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, client.SocketId, $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{ENVIRONMENT_FILE_FOR_WEB_EXTENSION}");

            //FFmpeg.SetExecutablesPath(HostingEnvironment.MapPath("~/Libs/"), "fconvert", "fprobe");
            FFmpeg.SetExecutablesPath("~/Libs/", "fconvert", "fprobe");
            Func<string, string> outputFileNameBuilder = (number) =>
            {
                return filePath;
            };

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(mjpegFilePath);

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.Reverse()
                ?.SetSize(VideoSize.Hd1080)
                ?.SetBitrate(SET_BIT_RATE)
                ?.SetCodec(VideoCodec.jpeg2000);

            await FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .ExtractEveryNthFrame(1, outputFileNameBuilder)
                .Start();

            await DeleteOldestJpgFile(client).ConfigureAwait(false);
        }

        /// <summary>
        /// H264処理
        /// </summary>
        /// <returns></returns>
        private static async Task H264Process(ConnectedClient client, byte[] receiveBuffer)
        {
            var h264FilePath = SaveToPhysicalFile(receiveBuffer, client.SocketId, "h264");

            var filePath = System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, client.SocketId, $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{ENVIRONMENT_FILE_FOR_WEB_EXTENSION}");

            var paremeters = " -i {0} {1}";
            paremeters = string.Format(paremeters, h264FilePath, filePath);
            Execute(ENVIRONMENT_FFMPEG_PATH, paremeters);

            await DeleteOldestJpgFile(client).ConfigureAwait(false);
        }

        /// <summary>
        /// H264処理(Xabe)
        /// </summary>
        /// <returns></returns>
        private static async Task H264WithXabeProcess(ConnectedClient client, byte[] receiveBuffer)
        {
            var h264FilePath = SaveToPhysicalFile(receiveBuffer, client.SocketId, "h264");

            var filePath = System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, client.SocketId, $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{ENVIRONMENT_FILE_FOR_WEB_EXTENSION}");

            //FFmpeg.SetExecutablesPath(HostingEnvironment.MapPath("~/Libs/"), "fconvert", "fprobe");
            FFmpeg.SetExecutablesPath("~/Libs/", "fconvert", "fprobe");
            Func<string, string> outputFileNameBuilder = (number) =>
            {
                return filePath;
            };

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(h264FilePath);

            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()
                ?.SetCodec(VideoCodec.h264)
                ?.Reverse()
                ?.SetSize(VideoSize.Hd1080)
                ?.SetBitrate(SET_BIT_RATE)
                ?.SetCodec(VideoCodec.jpeg2000);

            await FFmpeg.Conversions.New()
                .AddStream(videoStream)
                .ExtractEveryNthFrame(1, outputFileNameBuilder)
                .Start();

            await DeleteOldestJpgFile(client).ConfigureAwait(false);
        }

        /// <summary>
        /// ffmpeg実施
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string Execute(string exePath, string parameters)
        {
            string result = string.Empty;

            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = parameters;
                p.Start();
                p.WaitForExit();

                result = p.StandardOutput.ReadToEnd();
            }

            return result;
        }

        private static string SaveToPhysicalFile(byte[] buffer, string socketId, string extension)
        {
            if (!Directory.Exists(System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, socketId)))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, socketId));
            }
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.{extension}";
            var filePath = System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, socketId, fileName);
            File.WriteAllBytes(filePath, buffer.ToArray());

            return filePath;
        }

        private static async Task DeleteOldestJpgFile(ConnectedClient client)
        {
            // 現有フレーム数が最大フレーム数を超えている場合、最古フレームを削除
            var dirPath = System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_STREAM_PATH, client.SocketId);
            DirectoryInfo d = new DirectoryInfo(dirPath);
            FileInfo[] files = d.GetFiles($"*.{ENVIRONMENT_FILE_FOR_WEB_EXTENSION}");
            if (files.Count() > ConnectedClient.FrameDicItemMax)
            {
                var file = files.OrderBy(s => s.Name).ToList()[0];
                if (!Utils.IsFileLocked(file))
                {
                    file.Delete();
                }
            }
            await Task.Delay(0);
        }


        /// <summary>
        /// ストリームをスタート
        /// </summary>
        /// <param name="client"></param>
        public static void CallDeviceStartStream(ConnectedClient client)
        {
            Console.Out.WriteLine("CallDeviceStartStream Start!");
            try
            {
                var message = new JObject();
                message.Add(new JProperty(TERMINAL_CODE_KEY, TERMINAL_CODE_REQUEST_FRAME));
                message.Add(new JProperty(TERMINAL_DATA_KEY, TERMINAL_SUB_ACTION_START_STREAM));

                if (client.Socket.State == WebSocketState.Open)
                {
                    ArraySegment<byte> outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message.ToString()));
                    client.Socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.Out.WriteLine("CallDeviceStartStream:" + message.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }
            Console.Out.WriteLine("CallDeviceStartStream Start!");
        }

        /// <summary>
        /// ストリームをストップ
        /// </summary>
        /// <param name="client"></param>
        public static void CallDeviceStopStream(ConnectedClient client)
        {
            Console.Out.WriteLine("CallDeviceStopStream Start!");
            try
            {
                var message = new JObject();
                message.Add(new JProperty(TERMINAL_CODE_KEY, TERMINAL_CODE_REQUEST_FRAME));
                message.Add(new JProperty(TERMINAL_DATA_KEY, TERMINAL_SUB_ACTION_STOP_STREAM));

                if (client.Socket.State == WebSocketState.Open)
                {
                    ArraySegment<byte> outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message.ToString()));
                    client.Socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);

                    Console.Out.WriteLine("CallDeviceStopStream:" + message.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }
            Console.Out.WriteLine("CallDeviceStopStream Stop!");
        }

        /// <summary>
        /// フレームをリクエスト
        /// </summary>
        /// <param name="client"></param>
        public static void CallDeviceSendFrame(ConnectedClient client)
        {
            Console.Out.WriteLine("CallDeviceSendFrame Start!");
            try
            {
                var message = new JObject();
                message.Add(new JProperty(TERMINAL_CODE_KEY, TERMINAL_CODE_REQUEST_FRAME));
                message.Add(new JProperty(TERMINAL_DATA_KEY, TERMINAL_SUB_ACTION_REQUEST_ONE_FRAME));

                if (client.Socket.State == WebSocketState.Open)
                {
                    ArraySegment<byte> outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message.ToString()));
                    client.Socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message, ex);
            }
            Console.Out.WriteLine("CallDeviceSendFrame Stop!");
        }
    }
}