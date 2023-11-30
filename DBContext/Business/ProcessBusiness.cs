using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;

namespace semigura.DBContext.Business
{
    public class ProcessBusiness : BaseBusiness
    {
        IStringLocalizer localizer;
        public ProcessBusiness(DBEntities db,
            ILogger<ProcessBusiness> logger,
            IStringLocalizer<ProcessBusiness> localizer) : base(db, logger, localizer)
        {
            this.localizer = localizer;
        }


        public List<TerminalModel> GetListTerminalByType(int type)
        {
            {
                var dao = new TerminalDao(context, localizer);

                return dao.GetListTerminalByType(type);
            }
        }

        public TerminalModel GetTerminalByType(string terminalCode, int type = 0)
        {
            {
                var dao = new TerminalDao(context, localizer);

                return dao.GetTerminalByType(terminalCode, type);
            }
        }

        public SensorData GetFirstSensorData(string teminalId)
        {
            {
                var dao = new SensorDataDao(context, localizer);

                return dao.GetFirstSensorData(teminalId);
            }
        }

        public void AddListSensorData(List<SensorData> list)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var userInfodao = new UserInfoDao(context);
                    var userInfo = userInfodao.GetFirstAdminUser();
                    var dao = new SensorDataDao(context, localizer);

                    list.ForEach(s =>
                    {
                        s.CreatedById = userInfo != null ? userInfo.Id : null;

                        dao.Add(s);
                    });

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public void AddSensorData(SensorData sensorData)
        {
            {
                try
                {
                    var dao = new SensorDataDao(context, localizer);

                    dao.Add(sensorData);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Terminal GetTerminalOfTank(string terminalCode)
        {
            {
                try
                {
                    var dao = new TerminalDao(context, localizer);

                    return dao.GetTerminalOfTank(terminalCode);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public List<LotContainerModel> GetLotContainerGoingOn(List<string> listContainerId)
        {
            {
                try
                {
                    var dao = new LotContainerDao(context, localizer);

                    return dao.GetLotContainerGoingOn(listContainerId);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public Notification GetNotificationNotClosed(string lotContainerId)
        {
            {
                try
                {
                    var dao = new NotificationDao(context, localizer);

                    return dao.GetNotificationNotClosed(lotContainerId);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void UpdateOrInsertNotification(List<Notification> notifications)
        {
            //List<Notification> sendMailNotificationList = new List<Notification>();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var userInfodao = new UserInfoDao(context);
                    var userInfo = userInfodao.GetFirstAdminUser();
                    var notificationDao = new NotificationDao(context, localizer);
                    foreach (var notification in notifications)
                    {
                        if (!string.IsNullOrEmpty(notification.Id))
                        {
                            notification.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                            notification.ModifiedById = userInfo != null ? userInfo.Id : null;

                            notificationDao.UpdateContent(notification);
                        }
                        else
                        {
                            notification.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                            notification.CreatedById = userInfo != null ? userInfo.Id : null;
                            notification.PersonInCharge = userInfo != null ? userInfo.Id : null;

                            notificationDao.Add(notification);

                            //sendMailNotificationList.Add(notification);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            if (notifications != null && notifications.Any())
            {
                // クライアントへ最新データを更新
                //NotificationHub.RefreshDataNotification().ConfigureAwait(false); // TODO

                notifications.ForEach(s =>
                {
                    //NotificationHub.SendOneMsgNotification(s.Title).ConfigureAwait(false); // TODO
                });

                // メールを送る
                if (notifications != null && notifications.Any())
                {
                    SendMailAlert(notifications);
                }
            }
        }

        private List<AlertMail> GetListAlertMail()
        {
            {
                try
                {
                    var dao = new AlertMailDao(context, localizer);

                    return dao.GetListAlertMail();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void SendMailAlert(List<Notification> notifications)
        {
            // メール設定情報を取得する。
            var emailAddress = semigura.Commons.Properties.SYSTEM_MAIL_ADDRESS;
            var emailPass = semigura.Commons.Properties.SYSTEM_MAIL_PASSWORD;
            var emailHost = semigura.Commons.Properties.SYSTEM_MAIL_HOST;
            var emailPort = semigura.Commons.Properties.SYSTEM_MAIL_PORT;
            var emailName = semigura.Commons.Properties.SYSTEM_MAIL_NAME;

            if (!string.IsNullOrEmpty(emailAddress)
                && !string.IsNullOrEmpty(emailPass)
                && !string.IsNullOrEmpty(emailHost)
                && !string.IsNullOrEmpty(emailName)
                && emailPort != 0)
            {
                // メールを送る。
                var listAlertMail = GetListAlertMail();
                if (listAlertMail.Any())
                {
                    foreach (var notification in notifications)
                    {
                        var subject = notification.Title;
                        var body = !string.IsNullOrEmpty(notification.Content) ? notification.Content.Replace(Environment.NewLine, "<br/>") : string.Empty;

                        var listToMail = listAlertMail.Select(s => s.Email).ToList();
                        var listCcMail = new List<string>();

                        Utils.Sendmail(emailHost, emailPort, emailAddress, emailName, emailPass, listToMail, listCcMail, subject, body);
                    }
                }
            }
        }

        public string AddMedia(Media media)
        {
            try
            {
                {
                    var dao = new MediaDao(context, localizer);
                    return dao.Add(media);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// ゴミデータを削除
        /// </summary>
        public void DeleteJunkData()
        {
            // なし
        }

        public void DeleteMediaNotFound(string mediaId)
        {
            {
                try
                {
                    var dao = new MediaDao(context, localizer);

                    dao.DeleteById(mediaId);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}