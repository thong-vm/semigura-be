using Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Models;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S02002
// モジュール名称：製麴工程管理画面
// 機能概要　　　：製麴工程の情報の表示
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Vien
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S02002Controller : BaseControllerNoneAuthen
    {
        private readonly IStringLocalizer localizer;
        private readonly S02002Business business;
        public readonly LogFormater _logger;

        public S02002Controller(
            S02002Business business,
            ILogger<S02002Controller> logger,
            IStringLocalizer<S02002Controller> localizer
            )
        {
            this.localizer = localizer;
            this.business = business;
            _logger = new LogFormater(logger);
        }
        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public ActionResult Index(S02002ViewModel model)
        {
            if (model == null)
            {
                model = new S02002ViewModel();
            }

            model.IsInUse = true;
            SetModelByCookies(ref model);

            model.FactoryList = business.GetListFactory();

            // 福井酒造をデファクトとする。
            if (string.IsNullOrEmpty(model.FactoryId)
                && model.FactoryList != null
                && model.FactoryList.Any())
            {
                model.FactoryId = model.FactoryList[0].Id;
            }

            model.LotList = new List<Lot>();
            if (!string.IsNullOrEmpty(model.FactoryId))
            {
                model.LotList = business.GetLotByFactoryIdAndStatus(model.FactoryId, model.IsInUse);
                if (string.IsNullOrEmpty(model.LotId) && model.LotList.Any())
                {
                    model.LotId = model.LotList[0].Id;
                }
            }

            return View(model);
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public JsonResult LoadLotByFactoryId(string factoryId, bool isInUse)
        {
            try
            {
                var result = business.GetLotByFactoryIdAndStatus(factoryId, isInUse);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public JsonResult GetDataByLotId(string lotid)
        {
            try
            {
                var result = business.GetDataByLotId(lotid);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }


        public JsonResult GetDataByLastDay(string lotid)
        {
            try
            {
                var result = business.GetDataByLastDay(lotid);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
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
        public JsonResult GetDataBySearchDay(S02002ViewModel model)
        {
            try
            {
                var result = business.GetDataBySearchDay(model);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        private void SetModelByCookies(ref S02002ViewModel model)
        {
            if (model != null && string.IsNullOrEmpty(model.FactoryId))
            {
                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02002_FACTORYID] != null
                    && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02002_FACTORYID] != "")
                {
                    model.FactoryId = HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02002_FACTORYID];
                }

                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02002_LOTID] != null
                    && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02002_LOTID] != "")
                {
                    model.LotId = HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02002_LOTID];
                }

                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02002_ISINUSE] != null
                    && bool.TryParse(HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02002_ISINUSE], out bool IsInUse))
                {
                    model.IsInUse = IsInUse;
                }
            }
        }
    }
}