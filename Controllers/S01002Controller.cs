using Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.Models;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S01002
// モジュール名称：ホーム画面
// 機能概要　　　：ロットに属するロケーション、タンクの情報の表示
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Thao
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S01002Controller : BaseControllerNoneAuthen
    {
        private readonly IStringLocalizer localizer;
        private readonly S01002Business business;
        private readonly LogFormater _logger;

        public S01002Controller(S01002Business business,
            ILogger<S01002Controller> logger,
            IStringLocalizer<S01002Controller> localizer
            )
        {
            this.business = business;
            this._logger = new LogFormater(logger);
            this.localizer = localizer;
        }
        //[CAuthenticationAttribute]
        //[CAuthorizeAttribute]
        public ActionResult Index(S01002ViewModel model)
        {
            if (model == null)
            {
                model = new S01002ViewModel();
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

            return View(model);
        }

        public JsonResult LoadData(string factoryId)
        {
            try
            {
                var result = business.GetFactoryInfoById(factoryId);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"].ToString() }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        private void SetModelByCookies(ref S01002ViewModel model)
        {
            if (string.IsNullOrEmpty(model.FactoryId))
            {
                if (HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S01002_FACTORYID] != null)
                {
                    model.FactoryId = HttpContext.Request.Cookies[semigura.Commons.Properties.COOKIES_S01002_FACTORYID];
                }
            }
        }
    }
}