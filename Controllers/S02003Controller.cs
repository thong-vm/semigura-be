using Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Models;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S02003
// モジュール名称：ロケーション管理画面
// 機能概要　　　：ロケーションの温度、湿度の表示
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Vien
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S02003Controller : BaseControllerNoneAuthen
    {
        private readonly string optionAllValue = "all";
        private readonly S02003Business business;
        private readonly LogFormater _logger;
        private readonly IStringLocalizer localizer;

        public S02003Controller(S02003Business business,
            ILogger<S02003Controller> logger,
            IStringLocalizer<S02003Controller> localizer)
        {
            this.business = business;
            this._logger = new LogFormater(logger);
            this.localizer = localizer;
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public ActionResult Index(S02003ViewModel model)
        {
            if (model == null)
            {
                model = new S02003ViewModel();
            }
            SetModelByCookies(ref model);

            model.FactoryList = business.GetListFactory();

            // 福井酒造をデファクトとする。
            if (string.IsNullOrEmpty(model.FactoryId)
                && model.FactoryList != null
                && model.FactoryList.Any())
            {
                model.FactoryId = model.FactoryList[0].Id;
            }

            model.LocationList = new List<semigura.DBContext.Entities.Location>();
            if (!string.IsNullOrEmpty(model.FactoryId))
            {
                model.LocationList = business.GetLocationByFactoryId(model.FactoryId);
                // 全てオプションを追加
                if (model.LocationList != null && model.LocationList.Any())
                {
                    model.LocationList.Insert(0, new semigura.DBContext.Entities.Location
                    {
                        Id = optionAllValue,
                        Name = localizer["all"].Value,
                    });
                }
                if (string.IsNullOrEmpty(model.LocationId) && model.LocationList.Any())
                {
                    model.LocationId = model.LocationList[0].Id;
                }
            }

            return View(model);
        }

        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public JsonResult LoadLocationByFactoryId(string factoryId)
        {
            try
            {
                var result = business.GetLocationByFactoryId(factoryId);

                // 全てオプションを追加
                if (result != null && result.Any())
                {
                    result.Insert(0, new semigura.DBContext.Entities.Location
                    {
                        Id = optionAllValue,
                        Name = localizer["all"].Value,
                    });
                }

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
        public JsonResult GetDataByLocationId(S02003ViewModel model)
        {
            try
            {
                var result = business.GetDataByLocationId(model);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }
        public JsonResult GetDataByRoomName(S02003ViewModel model)
        {
            try
            {
                var result = business.GetDataByRoomName(model);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }


        private void SetModelByCookies(ref S02003ViewModel model)
        {
            if (model != null && string.IsNullOrEmpty(model.FactoryId))
            {
                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02003_FACTORYID] != null
                    && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02003_FACTORYID] != "")
                {
                    model.FactoryId = HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02003_FACTORYID];
                }

                if (HttpContext != null && HttpContext.Request != null && HttpContext.Request.Cookies != null && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02003_LOCATIONID] != null
                    && HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02003_LOCATIONID] != "")
                {
                    model.LocationId = HttpContext.Request.Cookies[Commons.Properties.COOKIES_S02003_LOCATIONID];
                }
            }
        }
    }
}