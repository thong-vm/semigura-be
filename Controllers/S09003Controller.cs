using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Models;
using semigura.Repositories;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S09003
// モジュール名称：タンクマスター管理画面
// 機能概要　　　：タンクマスター管理
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Luan
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S09003Controller : BaseController
    {
        private readonly IStringLocalizer localizer;
        private readonly S09003Business business;
        private readonly AuthCookieRepository authCookie;

        public S09003Controller(DBEntities db,
            ILogger<S09003Controller> logger,
            S09003Business business,
            AuthCookieRepository authCookie,
            IStringLocalizer<S09003Controller> localizer) : base(db, logger)
        {
            this.localizer = localizer;
            this.business = business;
            this.authCookie = authCookie;
        }

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Edit(S09003ViewModel model)
        {
            ModelState.Clear();

            if (!string.IsNullOrEmpty(model.Id))
            {
                var newmodel = business.GetContainer(model.Id);

                if (newmodel == null)
                {
                    ModelState.AddModelError("", localizer["C01004"]);
                    model = new S09003ViewModel();
                }
                else
                {
                    model = newmodel;
                }
            }

            model.ListFactory = business.GetListFactory();
            model.ListLocation = new List<semigura.DBContext.Entities.Location>();
            if (!string.IsNullOrEmpty(model.FactoryId))
            {
                model.ListLocation = business.GetListLocationByFactoryId(model.FactoryId);
            }

            return View(model);
        }


        public ActionResult GetDataTable(S09003ViewModel model)
        {
            var result = business.GetListContainer(model);

            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                aaData = result.ListContainer,
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult Save(S09003ViewModel model)
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

        public ActionResult Delete(S09003ViewModel model)
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
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }
    }
}