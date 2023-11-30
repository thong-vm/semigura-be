using Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.ProcessHandles;
//using NReco.ImageGenerator;
using System.Drawing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace semigura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StreamCameraController : ControllerBase
    {
        private readonly IStringLocalizer localizer;
        //※参考：https://stackoverflow.com/questions/47259756/frame-by-frame-mjpeg-streaming-with-c-asp-net-mvc
        private string BOUNDARY = "frame";
        private readonly LogFormater _logger;

        public StreamCameraController(
            ILogger<StreamCameraController> logger,
            IStringLocalizer<StreamCameraController> localizer
            )
        {
            this._logger = new LogFormater(logger);
            this.localizer = localizer;
        }

        [HttpGet]
        [Route("~/api/mjpeg/live")]
        public HttpResponseMessage GetVideoContent()
        {
            //var response = Request.CreateResponse();
            //response.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)StartStream);
            //response.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("multipart/x-mixed-replace; boundary=" + BOUNDARY);
            var response = new HttpResponseMessage();
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/x-mixed-replace; boundary=" + BOUNDARY);
            return response;
        }

        /// <summary>
        /// 適切なヘッダーを作成する。
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] CreateHeader(int length)
        {
            string header =
                "--" + BOUNDARY + "\r\n" +
                "Content-Type:image/jpeg\r\n" +
                "Content-Length:" + length + "\r\n\r\n";

            return Encoding.ASCII.GetBytes(header);
        }

        public byte[] CreateFooter()
        {
            return Encoding.ASCII.GetBytes("\r\n");
        }


        private void WriteFrame(Stream stream, Bitmap frame)
        {
            // 画像データを準備
            byte[] imageData = null;

            // これは、使用後にメモリストリームが確実に破棄されるようにするため。
            using (MemoryStream ms = new MemoryStream())
            {
                frame.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imageData = ms.ToArray();
            }

            WriteFrameWithByte(stream, imageData);
        }

        /// <summary>
        /// 指定されたフレームをストリームに書き込む。
        /// </summary>
        /// <param name="stream">ストリーム</param>
        /// <param name="frame">ビットマップ形式のフレーム</param>
        private void WriteFrameWithByte(Stream stream, byte[] imageData)
        {
            // ヘッダーを準備する
            byte[] header = CreateHeader(imageData.Length);
            // フッターを準備する
            byte[] footer = CreateFooter();

            //if (HttpContext.Current.Response.IsClientConnected)
            //{
            //    // Start writing data
            //    stream.Write(header, 0, header.Length);
            //    stream.Write(imageData, 0, imageData.Length);
            //    stream.Write(footer, 0, footer.Length);
            //}
        }

        /// <summary>
        /// MJPEGStreamが実行され、クライアントが接続されている間、フレームの送信を続行する。
        /// </summary>
        /// <param name="stream">書き込むストリーム.</param>
        /// <param name="httpContent">コンテンツ情報</param>
        /// <param name="transportContext"></param>
        private void StartStream(Stream stream, HttpContent httpContent, TransportContext transportContext)
        {
            try
            {
                DrawMessage(stream, localizer["connecting"].Value);

                //string deviceId = HttpUtility.ParseQueryString(HttpContext.Current.Request.Url.Query).Get("DeviceId");
                string deviceId = HttpContext.Request.Query["DeviceId"];

                if (!string.IsNullOrEmpty(deviceId))
                {
                    string clientId = Guid.NewGuid().ToString();
                    ConnectedClient client = null;
                    DeviceConnectionManagement.Clients.TryGetValue(deviceId, out client);
                    if (client != null)
                    {
                        // ビューアクライアントの登録
                        RegistViewerClient(deviceId, clientId);
                        bool isCalledStream = false;
                        try
                        {
                            var filename = string.Empty;
                            //while (HttpContext.Current.Response.IsClientConnected)
                            //{
                            //    try
                            //    {
                            //        if (isCalledStream)
                            //        {
                            //            DeviceConnectionManagement.Clients.TryGetValue(deviceId, out client);
                            //            if (client != null)
                            //            {
                            //                var frame = GetNextFrame(client, ref filename);

                            //                if (frame != null)
                            //                {
                            //                    WriteFrameWithByte(stream, frame);
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            isCalledStream = true;

                            //            DeviceConnectionManagement.CallDeviceStartStream(client);
                            //        }
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Console.Error.WriteLine(ex.Message, ex);
                            //    }
                            //}
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message, ex);
                        }
                        finally
                        {
                            // ビューアクライアントの削除
                            RemoveViewerClient(deviceId, clientId);
                        }
                    }
                    else
                    {
                        // 端末がサーバーに接続していない。
                        DrawMessage(stream, localizer["C01007"].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private byte[] GetNextFrame(ConnectedClient client, ref string filename)
        {
            var dir = new DirectoryInfo(System.IO.Path.Combine(DeviceConnectionManagement.ENVIRONMENT_ROOT_PATH, DeviceConnectionManagement.ENVIRONMENT_STREAM_PATH, client.SocketId));
            var files = dir.GetFiles($"*.{DeviceConnectionManagement.ENVIRONMENT_FILE_FOR_WEB_EXTENSION}").ToList();

            if (files.Count() > 0)
            {
                var filenameTemp = filename;
                var index = files.FindIndex(s => s.Name.Equals(filenameTemp));

                FileInfo file = null;
                if (string.IsNullOrEmpty(filename) || index < 0)
                {
                    file = files[files.Count() - 1];
                }
                else if (index >= 0 && index < (files.Count() - 1))
                {
                    file = files[index + 1];
                }

                if (file != null)
                {
                    if (System.IO.File.Exists(file.FullName) && !Utils.IsFileLocked(file))
                    {
                        using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (Bitmap frame = (Bitmap)Bitmap.FromStream(fs))
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    frame.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                                    var imageData = ms.ToArray();
                                    filename = file.Name;
                                    return imageData;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        private void RegistViewerClient(string deviceId, string clientId)
        {
            DeviceConnectionManagement.Clients.TryGetValue(deviceId, out ConnectedClient client);
            if (client != null)
            {
                if (client.StreamViewerClientIds != null && !client.StreamViewerClientIds.Contains(clientId))
                {
                    client.StreamViewerClientIds.Add(clientId);
                }
            }
        }


        private void RemoveViewerClient(string deviceId, string clientId)
        {
            DeviceConnectionManagement.Clients.TryGetValue(deviceId, out ConnectedClient client);
            if (client != null)
            {
                if (client.StreamViewerClientIds != null)
                {
                    client.StreamViewerClientIds.Remove(clientId);
                }
            }
        }

        private void DrawMessage(Stream stream, string message)
        {
            try
            {
                var frame = ConvertSvgToByteArr(message);
                if (frame != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        WriteFrameWithByte(stream, frame);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private byte[] ConvertSvgToByteArr(string message)
        {
            try
            {
                var html = @"<html>
                                <head>
                                    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8' />
                                </head>
                                <body>
                                    <h2 style='color:blue; margin-left:35px'>{0}</h2>
                                </body>
                            </html>";
                html = string.Format(html, message);

                //HtmlToImageConverter htmlToImageConverter = new HtmlToImageConverter();
                //var imageByte = htmlToImageConverter.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Jpeg);
                //return imageByte.ToArray();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return null;
        }
    }
}