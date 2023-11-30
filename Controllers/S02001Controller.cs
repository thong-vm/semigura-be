using Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Hubs;
using semigura.Models;
using semigura.Repositories;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S02001
// モジュール名称：もろみ工程管理画面
// 機能概要　　　：もろみ工程の情報の表示。
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Vien
// 　　　　　　　　2021/12/11 更新 sync-partners)Thao
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S02001Controller : BaseControllerNoneAuthen
    {
        private readonly S02001Business business;
        private readonly AuthCookieRepository authCookie;
        private readonly LogFormater _logger;
        private readonly IChatHubRepository chatHub;
        private readonly IStringLocalizer localizer;

        public S02001Controller(
            S02001Business s02001Business,
            ILogger<S02001Controller> logger,
            AuthCookieRepository authCookie,
            IChatHubRepository chatHub,
            IStringLocalizer<S02001Controller> localizer
            ) : base()
        {
            business = s02001Business;
            this.authCookie = authCookie;
            this._logger = new LogFormater(logger);
            this.chatHub = chatHub;
            this.localizer = localizer;
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public ActionResult Index(S02001ViewModel model)
        {
            if (model == null)
            {
                model = new S02001ViewModel();
            }
            model.IsInUse = true;

            SetModelByCookies(ref model);

            // ドロップボックスにデータを取得
            model.FactoryList = new List<Factory>();
            model.LotList = new List<Lot>();
            model.TankList = new List<ContainerModel>();

            model.FactoryList = business.GetListFactory();
            if (string.IsNullOrEmpty(model.FactoryId) && model.FactoryList.Any())
            {
                model.FactoryId = model.FactoryList[0].Id;
            }
            model.LotList = business.GetListLotByFactory(model.FactoryId, model.IsInUse);

            if (string.IsNullOrEmpty(model.LotId) && model.LotList.Any())
            {
                model.LotId = model.LotList[0].Id;
            }
            model.TankList = business.GetListTankByLotId(model.LotId);
            if (string.IsNullOrEmpty(model.LotContainerId) && model.TankList.Any())
            {
                model.LotContainerId = model.TankList[0].Id;
            }

            return View(model);
        }

        //[CAuthorizeAttribute]
        public ActionResult LoadListLotByFactoryId(string factoryId, bool isInUse)
        {
            try
            {
                var listLot = business.GetListLotByFactory(factoryId, isInUse);

                return Json(new { status = true, data = listLot }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public ActionResult GetListTankByLotID(S02001ViewModel model)
        {
            try
            {
                var result = business.GetListTankByLotId(model.LotId);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public ActionResult GetAllDataByLotContainer(S02001ViewModel model)
        {
            try
            {
                var result = business.GetAllDataByLotContainer(model);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        // ↓--DataEntry画面-----------------------------------------------------------------------------------------------
        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public ActionResult Edit(S02001ViewModel model)
        {
            return View(model);
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public ActionResult EditGetDataTable(S02001ViewModel model)
        {
            var result = business.GetListDataEntryByLotContainer(model);

            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                aaData = result.DataEntryList,
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public async Task<ActionResult> DeleteDataEntry(S02001ViewModel model)
        {
            try
            {
                string message = business.Delete(model);
                if (string.IsNullOrEmpty(message))
                {
                    await chatHub.NotifyOnSensorUpdated();
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

                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public async Task<ActionResult> AddDataEntry(S02001ViewModel model)
        {
            try
            {
                string message = business.Add(model, authCookie.UserInfo);
                if (string.IsNullOrEmpty(message))
                {
                    await chatHub.NotifyOnSensorUpdated();
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

                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        private void SetModelByCookies(ref S02001ViewModel model)
        {
            if (model != null && string.IsNullOrEmpty(model.FactoryId))
            {
                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_FACTORYID] != null)
                {
                    model.FactoryId = HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_FACTORYID];
                }

                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_LOTID] != null
                    && HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_LOTID] != "")
                {
                    model.LotId = HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_LOTID];
                }

                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_LOTCONTAINERID] != null
                    && HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_LOTCONTAINERID] != "")
                {
                    model.LotContainerId = HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_LOTCONTAINERID];
                }

                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_ISINUSE] != null
                    && bool.TryParse(HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S02001_ISINUSE], out bool IsInUse))
                {
                    model.IsInUse = IsInUse;
                }
            }
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public ActionResult SaveTemperateDataEntryEdit(List<S02001ViewModel.TemperateEdit> temperateChange, List<S02001ViewModel.TemperateEdit> temperateAvgChange, List<DataEntry> dataEntryChange)
        {
            try
            {
                string message = business.SaveDataEntryEdit(dataEntryChange);
                if (!string.IsNullOrEmpty(message))
                {
                    return Json(new { status = false, message = message }/*, JsonRequestBehavior.AllowGet*/);
                }
                message = business.SaveTemperateEdit(temperateChange, temperateAvgChange);
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

                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }

        }
    }
}