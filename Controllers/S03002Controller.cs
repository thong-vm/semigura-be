using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Models;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S03002
// モジュール名称：タンク管理画面
// 機能概要　　　：タンクの詳細情報の表示
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Luan
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S03002Controller : BaseController
    {
        private readonly IStringLocalizer localizer;
        private readonly S03002Business business;

        public S03002Controller(DBEntities db,
         ILogger<S03002Controller> logger,
         S03002Business business,
         IStringLocalizer<S03002Controller> localizer) : base(db, logger)
        {
            this.localizer = localizer;
            this.business = business;
        }
        public ActionResult Index()
        {
            ModelState.Clear();
            S03002ViewModel model = new S03002ViewModel();
            model.ListFactory = business.GetListFactory();
            model.ListLocation = new List<semigura.DBContext.Entities.Location>();
            model.ListLot = new List<Lot>();
            if (string.IsNullOrEmpty(model.FactoryId) && model.ListFactory.Any())
            {
                model.FactoryId = model.ListFactory[0].Id;
            }

            return View(model);
        }

        public ActionResult GetDataTable(S03002ViewModel model)
        {
            var result = business.GetListLotContainer(model);

            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                aaData = result.LotContainerList,
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult LoadDataByFactoryId(S03002ViewModel model)
        {
            try
            {
                var result = business.GetDataByFactoryId(model.FactoryId);

                return Json(new { status = true, data = result }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);
                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public ActionResult GetListCapacityRefer(S03002ViewModel model)
        {
            try
            {
                var result = business.GetListCapacityRefer(model.ContainerId);

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