using Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using System.Net;
using System.Net.Mail;
using System.Net.WebSockets;

namespace semigura.Controllers
{
    public class SendMailInfo
    {
        public string Subject;
        public string Body;
        public List<string> MailList;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class WSDeviceController : ControllerBase
    {
        private readonly IStringLocalizer localizer;
        private readonly DBEntities _db;
        private readonly ProcessBusiness business;
        private readonly LogFormater logger;

        public WSDeviceController(DBEntities db,
            ProcessBusiness business,
            ILogger<WSDeviceController> logger,
            IStringLocalizer<WSDeviceController> localizer)
        {
            this.localizer = localizer;
            _db = db;
            this.business = business;
            this.logger = new LogFormater(logger);
        }

        [Route("/api/CheckTemperature")]
        [HttpPost]
        public ActionResult PostCheckTemperature(List<SensorData> listdata)
        {
            try
            {
                // check abnormal
                List<bool> checkResult = CheckAbnormalTemperature(listdata, _db);
                bool isError = checkResult.Any(c => c == false);
                // get mail list
                if (isError == true)
                {
                    List<string> listmail = EmailList;

                    // send notification
                    var res = SendNotificationMail(listmail);
                    if (!res)
                    {
                        return new ObjectResult("Send notification mail error!") { StatusCode = (int?)HttpStatusCode.InternalServerError };
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = (int?)HttpStatusCode.InternalServerError };
            }
        }

        [Route("~/api/WSDevice")]
        public HttpResponseMessage Get()
        {
            if (Commons.Properties.IS_SERVER_DEBUGGER) System.Diagnostics.Debugger.Launch();

            //if (HttpContext.Current.IsWebSocketRequest)
            //{
            //    HttpContext.Current.AcceptWebSocketRequest(DeviceHandle);
            //}

            //return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {

            }
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        //private async Task DeviceHandle(WebSocketContext wsContext)
        //{
        //    if (Commons.Properties.IS_SERVER_DEBUGGER) System.Diagnostics.Debugger.Launch();

        //    WebSocket socket = wsContext.WebSocket;
        //    //string deviceId = System.Web.HttpUtility.ParseQueryString(HttpContext.Request.Query).Get("DeviceId");
        //    string deviceId = HttpContext.Request.Query["DeviceId"];

        //    if (!string.IsNullOrEmpty(deviceId) && business.GetTerminalOfTank(deviceId) != null)
        //    {
        //        //if (!DeviceConnectionManagement.Clients.ContainsKey(deviceId))
        //        //{

        //        var socketId = deviceId;
        //        var completion = new TaskCompletionSource<object>();
        //        var client = new ConnectedClient(socketId, socket, completion);
        //        if (DeviceConnectionManagement.Clients.TryRemove(socketId, out ConnectedClient clientTemp))
        //        {
        //            try
        //            {
        //                clientTemp.Socket.Abort();
        //            }
        //            catch (Exception ex)
        //            {
        //                logger.Error(ex.Message, ex);
        //            }
        //        }
        //        DeviceConnectionManagement.Clients.TryAdd(socketId, client);

        //        _ = Task.Run(() => DeviceConnectionManagement.SocketProcessingLoopAsync(client).ConfigureAwait(false));
        //        await completion.Task;

        //        //}
        //        //else
        //        //{
        //        //    LogUtil.Info(Resources.Common.C01008);

        //        //    await AbnormallyClosed(socket, Resources.Common.C01008);
        //        //}
        //    }
        //    else
        //    {
        //        var message = string.Format(localizer["C01006"].Value, deviceId);
        //        logger.Info(message);

        //        await AbnormallyClosed(socket, message);
        //    }
        //}

        private async Task AbnormallyClosed(WebSocket socket, string message)
        {
            try
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, message, CancellationToken.None);
            }
            catch
            {
                // なし
            }
        }
        //create methor Check Temperature >> return OK/NG
        //if NG >> get mail list >> 
        private List<bool> CheckAbnormalTemperature(List<SensorData> listdata, DBEntities db)
        {

            List<bool> result = new List<bool>();
            foreach (SensorData sensor in listdata)
            {
                string id = sensor.LotContainerId;
                LotContainer lotContainer = db.LotContainer.Find(id);
                if (sensor.Temperature1 > lotContainer.TempMax || lotContainer.TempMin > sensor.Temperature1)
                {
                    result.Add(false);
                }
                else
                { result.Add(true); }
            }

            return result;
        }

        [Route("/api/getmaillist")]
        [HttpGet]
        // get mail list
        public ActionResult GetMailList()
        {
            try
            {
                List<string> listmail = EmailList;
                return Ok(listmail);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return new ObjectResult(ex.Message) {StatusCode = 500 };
            }

        }
        private List<string> EmailList =>  _db.AlertMail.Select(e=>e.Email).ToList();

        //send notification mail
        public bool SendNotificationMail(List<string> maillist)
        {
            try
            {
                string fromMailPass = Commons.Properties.SYSTEM_MAIL_PASSWORD;
                string passwordDecrypt = AESCryption.Decrypt(fromMailPass, Commons.Properties.AES_IV, Commons.Properties.AES_Key);

                SmtpClient smtpClient = new SmtpClient(Commons.Properties.SYSTEM_MAIL_HOST);
                smtpClient.Port = Commons.Properties.SYSTEM_MAIL_PORT;
                smtpClient.Credentials = new System.Net.NetworkCredential(Commons.Properties.SYSTEM_MAIL_ADDRESS, passwordDecrypt);
                smtpClient.EnableSsl = Commons.Properties.SYSTEM_MAIL_ENABLESSL;

                MailMessage mail = new MailMessage();
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.Body = "This is BODY";
                mail.Subject = "This is Subject";
                foreach (string toAddress in maillist)
                {
                    try
                    {
                        mail.To.Add(new MailAddress(toAddress));
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message, ex);
                    }
                }
                //設定 From , To and CC
                mail.From = new MailAddress(Commons.Properties.SYSTEM_MAIL_ADDRESS, Commons.Properties.SYSTEM_MAIL_HOST, System.Text.Encoding.UTF8);
                {
                    smtpClient.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                return false;
            }
        }

    }
}