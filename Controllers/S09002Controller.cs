using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Models;
using semigura.Repositories;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S09002
// モジュール名称：アラートメール画面
// 機能概要　　　：アラートがあるとき、アラートを送るメールの設定
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Thao
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S09002Controller : BaseController
    {
        private readonly IStringLocalizer localizer;
        private readonly S09002Business business;
        private readonly AuthCookieRepository authCookie;

        public S09002Controller(DBEntities dBEntities,
            ILogger<S09002Controller> logger,
            S09002Business business,
            AuthCookieRepository authCookie,
            IStringLocalizer<S09002Controller> localizer) : base(dBEntities, logger)
        {
            this.localizer = localizer;
            this.business = business;
            this.authCookie = authCookie;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(S09002ViewModel model)
        {
            ModelState.Clear();
            if (!string.IsNullOrEmpty(model.Id))
            {
                var newmodel = business.GetAlertMail(model);

                if (newmodel == null)
                {
                    ModelState.AddModelError("", localizer["C01004"]);
                }
                else
                {
                    model = newmodel;
                }
            }
            return View(model);
        }


        public ActionResult GetDataTable(S09002ViewModel model)
        {
            var result = business.GetListAlertMail(model);

            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                aaData = result.AlertMailList,
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult Save(S09002ViewModel model)
        {
            try
            {
                string message = business.Save(model, authCookie.UserInfo);
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

        public ActionResult Delete(S09002ViewModel model)
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

                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }
    }
}