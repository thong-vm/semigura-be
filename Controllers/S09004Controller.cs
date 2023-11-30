using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Models;
using semigura.Repositories;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S09004
// モジュール名称：端末管理画面
// 機能概要　　　：端末の情報管理
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Thao
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S09004Controller : BaseController
    {
        private readonly IStringLocalizer localizer;
        private readonly AuthCookieRepository authCookie;
        private readonly S09004Business business;

        public S09004Controller(DBEntities db,
            ILogger<S09004Controller> logger,
            S09004Business business,
            AuthCookieRepository authCookie,
            IStringLocalizer<S09004Controller> localizer) : base(db, logger)
        {
            this.localizer = localizer;
            this.authCookie = authCookie;
            this.business = business;
        }

        public ActionResult Index()
        {
            var model = new S09004ViewModel();
            model.localizer = localizer;
            model.FactoryList = business.GetListFactory();
            model.ContainerList = new List<Container>();
            model.LocationList = new List<semigura.DBContext.Entities.Location>();

            return View(model);
        }

        public ActionResult Edit(S09004ViewModel model)
        {
            ModelState.Clear();

            if (!string.IsNullOrEmpty(model.Id))
            {
                var newmodel = business.GetTerminalWithFactory(model);

                if (newmodel == null)
                {
                    ModelState.AddModelError("", localizer["C01004"].Value);
                    model = new S09004ViewModel();
                }
                else
                {
                    model = newmodel;
                }
            }
            model.localizer = localizer;
            model.FactoryList = business.GetListFactory();
            model.ContainerList = new List<Container>();
            model.LocationList = new List<semigura.DBContext.Entities.Location>();

            if (!string.IsNullOrEmpty(model.FactoryId))
            {
                if (model.Type == Commons.Properties.TERMINAL_TYPE_TANK
                || model.Type == Commons.Properties.TERMINAL_TYPE_SEIGIKU
                || model.Type == Commons.Properties.TERMINAL_TYPE_CAMERA)
                {
                    model.ContainerId = model.ParentId;
                    model.ContainerList = business.GetListContainerTank(model.FactoryId);
                }
                else if (model.Type == Commons.Properties.TERMINAL_TYPE_LOCATION)
                {
                    model.LocationId = model.ParentId;
                    model.LocationList = business.GetListLocation(model.FactoryId);
                }
            }

            return View(model);
        }


        public ActionResult GetDataTable(S09004ViewModel model)
        {
            var result = business.GetListTerminal(model);

            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                aaData = result.TerminalList,
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult Save(S09004ViewModel model)
        {
            try
            {
                if (model.Type == Commons.Properties.TERMINAL_TYPE_SEIGIKU)
                {
                    model.ParentId = null;
                    model.FactoryId = null;
                }
                else if (!string.IsNullOrEmpty(model.ContainerId))
                {
                    model.ParentId = model.ContainerId;
                }
                else if (!string.IsNullOrEmpty(model.LocationId))
                {
                    model.ParentId = model.LocationId;
                }

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

                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public ActionResult Delete(S09004ViewModel model)
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

        public ActionResult GetListLocation(S09004ViewModel model)
        {
            try
            {
                var data = new List<semigura.DBContext.Entities.Location>();
                if (!string.IsNullOrEmpty(model.FactoryId))
                {
                    data = business.GetListLocation(model.FactoryId);
                }

                return Json(new { status = true, data = data }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public ActionResult GetListContainer(S09004ViewModel model)
        {
            try
            {
                var data = new List<Container>();
                if (!string.IsNullOrEmpty(model.FactoryId))
                {
                    data = business.GetListContainerTank(model.FactoryId);
                }

                return Json(new { status = true, data = data }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = localizer["C01003"].Value }/*, JsonRequestBehavior.AllowGet*/);
            }
        }
    }
}