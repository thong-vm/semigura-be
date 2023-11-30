using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Hubs;
using semigura.Models;
using semigura.Repositories;


//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S03001
// モジュール名称：ロット管理画面
// 機能概要　　　：ロット情報管理
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Luan
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S03001Controller : BaseController
    {
        private readonly S03001Business business;
        private readonly AuthCookieRepository authCookie;
        private readonly IChatHubRepository chatHubRepository;
        private readonly IStringLocalizer localizer;

        public S03001Controller(DBEntities db, ILogger<S03001Controller> logger,
            S03001Business business,
            AuthCookieRepository authCookie,
            IChatHubRepository chatHubRepository,
            IStringLocalizer<S03001Controller> localizer
            ) : base(db, logger)
        {
            this.business = business;
            this.authCookie = authCookie;
            this.chatHubRepository = chatHubRepository;
            this.localizer = localizer;
        }
        public ActionResult Index()
        {
            ModelState.Clear();
            S03001ViewModel model = new S03001ViewModel();
            model.FactoryList = new List<S03001ViewModel.Factory>();
            var data = business.GetListFactory();

            foreach (var item in data)
            {
                model.FactoryList.Add(item.Map<S03001ViewModel.Factory>());
            }
            if (string.IsNullOrEmpty(model.FactoryID) && model.FactoryList.Any())
            {
                model.FactoryID = model.FactoryList[0].Id;
            }

            return View(model);
        }

        public ActionResult Edit(S03001ViewModel model)
        {
            ModelState.Clear();
            model.FactoryList = new List<S03001ViewModel.Factory>();
            model.RiceList = new List<S03001ViewModel.Rice>();
            model.TankList = new List<S03001ViewModel.Tank>();
            model.SensorList = new List<S03001ViewModel.Terminal>();
            model.KubunList = new List<S03001ViewModel.Kubun>();
            model.SeimaibuaiList = new List<S03001ViewModel.Semaibuai>();
            var dataFac = business.GetListFactory();
            var dataRice = business.GetListRice();
            var dataTank = business.GetListTank();
            var dataSensor = business.GetListSensor();

            model.KubunList = business.GetListKubun();
            model.SeimaibuaiList = business.GetListSeimaibuai();

            foreach (var (item, i) in dataFac.Select((value, i) => (value, i)))
            {
                model.FactoryList.Add(item.Map<S03001ViewModel.Factory>());
            }

            foreach (var item in dataRice)
            {
                model.RiceList.Add(item.Map<S03001ViewModel.Rice>());
            }

            foreach (var item in dataTank)
            {
                model.TankList.Add(item.Map<S03001ViewModel.Tank>());
            }

            foreach (var item in dataSensor)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    item.Name = "-";
                }
                model.SensorList.Add(item.Map<S03001ViewModel.Terminal>());
            }

            //Get InfoLot for Edit by LotId
            string factoryId = "";
            string locationId = "";
            string lotCode = "";
            Nullable<System.DateTime> startDate = new DateTime();
            Nullable<System.DateTime> endDate = new DateTime();
            string riceId = "";
            Nullable<int> kubunId = new int();
            string semaibuaiId = "";
            Nullable<decimal> riceRatio = null;
            Nullable<decimal> minTempSeigiku = null;
            Nullable<decimal> maxTempSeigiku = null;

            business.GetInfoLotByLotId(model.LotId, ref lotCode, ref factoryId, ref startDate, ref endDate, ref riceId,
                                        ref kubunId, ref semaibuaiId, ref riceRatio);
            business.GetSegikuMinMaxByLotId(model.LotId, ref minTempSeigiku, ref maxTempSeigiku);
            model.TankListId = business.GetListTankIdByLotId(model.LotId);
            model.SensorListId = business.GetListSensorIdByLotId(model.LotId);

            var selectSeimaibuai = model.SeimaibuaiList.Where(s => s.Id == semaibuaiId).FirstOrDefault();
            locationId = business.GetLocationIdforSegiku(model.LotId);

            model.FactoryID = factoryId;
            model.LocationID = locationId;
            model.StartDate = startDate;
            model.EndDate = endDate;
            model.RiceId = riceId;
            model.KubunId = kubunId;
            model.LotCode = lotCode;

            if (string.IsNullOrEmpty(model.FactoryID) && model.FactoryList.Any())
            {
                model.FactoryID = model.FactoryList[0].Id;
            }

            if (selectSeimaibuai != null)
            {
                model.SemaibuaiId = semaibuaiId;
                model.SemaibuaiValue = semaibuaiId;
            }
            else
            {
                if (semaibuaiId == null || semaibuaiId == "")
                {
                    model.SemaibuaiId = "";
                    model.SemaibuaiValue = semaibuaiId;
                }
                else
                {
                    model.SemaibuaiId = localizer["other"];
                    model.SemaibuaiValue = semaibuaiId;
                }
            }
            model.RicePolishingRatio = riceRatio;

            if (model.RiceId != null && model.RiceId != "")
            {
                model.RiceInfo = business.GetInfoRiceByRiceId(model.RiceId);
            }
            if (minTempSeigiku.HasValue && model.RiceInfo != null)
            {
                model.RiceInfo.MinTempSeigiku = minTempSeigiku;
            }

            if (maxTempSeigiku.HasValue && model.RiceInfo != null)
            {
                model.RiceInfo.MaxTempSeigiku = maxTempSeigiku;
            }

            model.ListLocation = new List<semigura.DBContext.Entities.Location>();
            if (!string.IsNullOrEmpty(model.FactoryID))
            {
                model.ListLocation = business.GetListLocationByFactoryId(model.FactoryID);
            }

            if (model.StartDate == DateTime.MinValue)
            {
                model.StartDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            }
            if (model.EndDate == DateTime.MinValue)
            {
                model.EndDate = null;
            }

            return View(model);
        }

        public ActionResult GetDataTable(S03001ViewModel model)
        {
            if (model.SEcho == "1") return Json(new { }/*, JsonRequestBehavior.AllowGet*/);

            var result = business.GetListLotContainer(model);

            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                aaData = result.LotContainerList,
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult GetListTank()
        {
            var result = business.GetListTank();

            return Json(new { result }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult GetListSensor()
        {
            var listSensor = business.GetListSensor();

            var result = listSensor.Where(s => s.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU);

            return Json(new
            {
                result
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult LoadInfoByTankID(string tankId)
        {
            var result = business.LoadInfoByTankID(tankId);

            return Json(new
            {
                result
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult GetInfoRiceByRiceId(string riceId)
        {
            var result = business.GetInfoRiceByRiceId(riceId);

            return Json(new
            {
                result
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult GetInfoRiceByRiceName(string riceName)
        {
            var result = business.GetInfoRiceByRiceName(riceName);

            return Json(new
            {
                result
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult LoadDataByFactoryId(string factoryId)
        {
            try
            {
                var result = business.GetListLocationByFactoryId(factoryId);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public async Task<ActionResult> FinishDateLot(string lotId)
        {
            try
            {
                string message = business.FinishDateLot(lotId);

                if (string.IsNullOrEmpty(message))
                {
                    // クライアントへ最新データを更新
                    await chatHubRepository.NotifyOnSensorUpdated();

                    return Json(new { status = true }/*, JsonRequestBehavior.AllowGet*/);
                }
                else
                {
                    return Json(new { status = false, message = message }/*, JsonRequestBehavior.AllowGet*/);
                }
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public async Task<ActionResult> FinishDateLotContainer(string lotContainerId, Nullable<int> type, string lotId)
        {
            try
            {
                string message = business.FinishDateLotContainer(lotContainerId, type);

                bool result = business.CheckFinishDateLotId(lotId);

                if (string.IsNullOrEmpty(message))
                {
                    // クライアントへ最新データを更新
                    //S01002Hub.RefreshData().ConfigureAwait(false);
                    //S02001Hub.RefreshData().ConfigureAwait(false);
                    //S02002Hub.RefreshData().ConfigureAwait(false);
                    //S02003Hub.RefreshData().ConfigureAwait(false);
                    await chatHubRepository.NotifyOnSensorUpdated();

                    return Json(new { status = true, flgEndLot = result }/*, JsonRequestBehavior.AllowGet*/);
                }
                else
                {
                    return Json(new { status = false, message = message }/*, JsonRequestBehavior.AllowGet*/);
                }
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public async Task<ActionResult> CloseTankOtherLot(List<msgTankSensorUsing> model)
        {
            string message = string.Empty;
            bool result = false;
            try
            {
                model.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.LotContainerId)) { return; }
                    message = business.FinishDateLotContainer(x.LotContainerId, x.Type);
                    if (!string.IsNullOrEmpty(message))
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(x.LotId)) { return; }
                    result = business.CheckFinishDateLotId(x.LotId);
                });


                if (string.IsNullOrEmpty(message))
                {
                    // クライアントへ最新データを更新
                    //S01002Hub.RefreshData().ConfigureAwait(false);
                    //S02001Hub.RefreshData().ConfigureAwait(false);
                    //S02002Hub.RefreshData().ConfigureAwait(false);
                    //S02003Hub.RefreshData().ConfigureAwait(false);
                    await chatHubRepository.NotifyRefreshData();

                    return Json(new { status = true, flgEndLot = result }/*, JsonRequestBehavior.AllowGet*/);
                }
                else
                {
                    return Json(new { status = false, message = message }/*, JsonRequestBehavior.AllowGet*/);
                }
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public ActionResult CheckPreSave(S03001ViewModel model)
        {
            List<msgTankSensorUsing> msgTankUsing = new List<msgTankSensorUsing>();
            List<msgTankSensorUsing> msgSensorUsing = new List<msgTankSensorUsing>();
            List<msgTankSensorUsing> msgTankSensorUsing = new List<msgTankSensorUsing>();
            List<string> message = new List<string>();
            try
            {
                if (model.TankListId != null)
                {
                    msgTankUsing = CheckListTankInUse(model);
                    msgTankUsing.ForEach(s =>
                    {
                        message.Add(string.Format(localizer["msg_using"].Value, s.ContainerCode_SenserCode, s.LotCode));
                    });
                    if (msgTankUsing.Count != 0)
                    {
                        msgTankSensorUsing.AddRange(msgTankUsing);
                    }
                }

                if (model.SensorListId != null)
                {
                    msgSensorUsing = CheckListSensorInUse(model);
                    msgSensorUsing.ForEach(s =>
                    {
                        message.Add(string.Format(localizer["msg_using"].Value, s.ContainerCode_SenserCode, s.LotCode));
                    });
                    if (msgSensorUsing.Count != 0)
                    {
                        msgTankSensorUsing.AddRange(msgSensorUsing);
                    }
                }
                if (msgTankSensorUsing.Count != 0)
                {
                    return Json(new { status = false, error = false, message = message, msgTankSensorUsing = msgTankSensorUsing }/*, JsonRequestBehavior.AllowGet*/);
                }


                return Json(new { status = true }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, error = true, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }


        public ActionResult Save(S03001ViewModel model)
        {
            string message = string.Empty;
            try
            {
                message = business.Save(model, authCookie.UserInfo);
                if (string.IsNullOrEmpty(message))
                {
                    // クライアントへ最新データを更新
                    //S01002Hub.RefreshData().ConfigureAwait(false);
                    //S02001Hub.RefreshData().ConfigureAwait(false);
                    //S02002Hub.RefreshData().ConfigureAwait(false);
                    //S02003Hub.RefreshData().ConfigureAwait(false);

                    return Json(new { status = true }/*, JsonRequestBehavior.AllowGet*/);
                }
                else
                {
                    return Json(new { status = false, message = message }/*, JsonRequestBehavior.AllowGet*/);
                }
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public ActionResult Delete(S03001ViewModel model)
        {
            try
            {
                string message = business.Delete(model);
                if (string.IsNullOrEmpty(message))
                {
                    return Json(new { status = true }/*, JsonRequestBehavior.AllowGet*/);
                }
                else
                {
                    return Json(new { status = false, message = message }/*, JsonRequestBehavior.AllowGet*/);
                }
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        private List<msgTankSensorUsing> CheckListTankInUse(S03001ViewModel model)
        {
            List<msgTankSensorUsing> message = new List<msgTankSensorUsing>();
            List<string> lstContainerID = new List<string>();
            List<string> lstExistContainerID = new List<string>();
            model.TankListId.ForEach(s =>
            {
                if (s.EndDate == null)
                {
                    lstContainerID.Add(s.Id);
                }
            });

            lstExistContainerID = business.CheckListTankInUse(lstContainerID);
            if (lstExistContainerID.Count > 0)
            {
                if (model.LotId != null)
                {
                    List<string> lstContainerIdThisLot = new List<string>();
                    lstContainerIdThisLot = business.CheckContainerIDthisLotID(model.LotId);
                    bool fexistequal = lstExistContainerID.All(s => lstContainerIdThisLot.Contains(s));
                    if (fexistequal)
                    {
                        return message;
                    }
                    else
                    {
                        var res = lstExistContainerID.Where(s => !lstContainerIdThisLot.Contains(s));
                        lstExistContainerID = res.ToList();
                    }

                }


                var dataTank = business.GetListTank();
                lstExistContainerID.ForEach(s =>
                {
                    msgTankSensorUsing msgTankUsing = new msgTankSensorUsing();
                    dataTank.ForEach(x =>
                    {
                        if (x.Id == s)
                        {
                            var xlotContainer = business.getLotContainerByContainerId(x.Id);
                            var lotCode = business.getLotCodeByLotContainerId(xlotContainer.Id);
                            //msgTankUsing.l = lotCode + ":" + x.Code;
                            msgTankUsing.LotId = xlotContainer.LotId;
                            msgTankUsing.LotCode = lotCode;
                            msgTankUsing.LotContainerId = xlotContainer.Id;
                            msgTankUsing.ContainerId = xlotContainer.ContainerId;
                            msgTankUsing.ContainerCode_SenserCode = x.Code;
                            msgTankUsing.Type = semigura.Commons.Properties.CONTAINER_TYPE_TANK;
                            return;
                        }
                    });
                    message.Add(msgTankUsing);
                });
                return message;
            }
            return message;
        }

        private List<msgTankSensorUsing> CheckListSensorInUse(S03001ViewModel model)
        {
            List<msgTankSensorUsing> message = new List<msgTankSensorUsing>();
            List<string> lstSensorID = new List<string>();
            List<string> lstExistSensorID = new List<string>();
            model.SensorListId.ForEach(s =>
            {
                if (s.EndDate == null)
                {
                    lstSensorID.Add(s.Id);
                }
            });

            lstExistSensorID = business.CheckListSensorInUse(lstSensorID);
            if (lstExistSensorID.Count > 0)
            {
                if (model.LotId != null)
                {
                    List<string> lstSensorIdThisLot = new List<string>();
                    lstSensorIdThisLot = business.CheckSensorIDthisLotID(model.LotId);
                    bool fexistequal = lstExistSensorID.All(s => lstSensorIdThisLot.Contains(s));
                    if (fexistequal)
                    {
                        return message;
                    }
                    else
                    {
                        var res = lstExistSensorID.Where(s => !lstSensorIdThisLot.Contains(s));
                        lstExistSensorID = res.ToList();
                    }

                }

                var dataSensor = business.GetListSensor();
                lstExistSensorID.ForEach(s =>
                {
                    msgTankSensorUsing msgSensorUsing = new msgTankSensorUsing();
                    foreach (var item in dataSensor)
                    {
                        if (item.Id == s)
                        {
                            var xlotContainer = business.getLotContainerByTerminalId(item.Id);
                            var lotCode = business.getLotCodeByLotContainerId(xlotContainer.Id);
                            var xlotContainerTerminalId = business.getLCTidByLCTidTid(xlotContainer.Id, item.Id);
                            msgSensorUsing.LotId = xlotContainer.LotId;
                            msgSensorUsing.LotCode = lotCode;
                            msgSensorUsing.LotContainerId = xlotContainerTerminalId;
                            msgSensorUsing.ContainerId = item.Id;
                            msgSensorUsing.ContainerCode_SenserCode = item.Name + "(" + item.Code + ")";
                            msgSensorUsing.Type = semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU;
                            break;
                        }
                    }
                    message.Add(msgSensorUsing);// += Code + ",";
                });
                return message;
            }
            return message;
        }

    }
    class DistinctItemComparer : IEqualityComparer<S03001ViewModel.Rice>
    {

        public bool Equals(S03001ViewModel.Rice x, S03001ViewModel.Rice y)
        {
            return (x.Name == null) || (y.Name == null) ? false : x.Name == y.Name;
        }

        public int GetHashCode(S03001ViewModel.Rice obj)
        {
            return obj.Name == null ? throw new Exception("obj.Name is null") : obj.Name.GetHashCode();
        }
    }

    public class msgTankSensorUsing
    {
        public string? LotId { get; set; }
        public string? LotCode { get; set; }
        public string? LotContainerId { get; set; }  //Type = 2 LotContainerTerminalId
        public string? ContainerId { get; set; }
        public string? ContainerCode_SenserCode { get; set; }

        public int Type { get; set; }
    }

}