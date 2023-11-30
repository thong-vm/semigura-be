using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Hubs;
using semigura.Models;
using semigura.Repositories;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S03005
// モジュール名称：アラート管理画面
// 機能概要　　　：アラートの詳細情報の表示
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)ベベルリ
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S03005Controller : BaseController
    {
        private readonly IStringLocalizer localizer;
        private readonly S03005Business business;
        readonly AuthCookieRepository authCookie;
        private readonly IChatHubRepository chatHub;

        public S03005Controller(
            DBEntities db,
            ILogger<S03005Controller> logger,
            S03005Business business,
            AuthCookieRepository authCookie,
            IChatHubRepository chatHub,
            IStringLocalizer<S03005Controller> localizer
            ) : base(db, logger)
        {
            this.localizer = localizer;
            this.business = business;
            this.authCookie = authCookie;
            this.chatHub = chatHub;
        }
        public ActionResult Index(S03005ViewModel model)
        {
            if (model == null)
            {
                model = new S03005ViewModel();
            }
            model.FactoryList = business.GetListFactory();
            model.ContainerList = new List<Container>();
            model.LocationList = new List<semigura.DBContext.Entities.Location>();
            if (string.IsNullOrEmpty(model.FactoryId) && model.FactoryList.Any())
            {
                model.FactoryId = model.FactoryList[0].Id;
            }
            return View(model);
        }

        public ActionResult Edit(S03005ViewModel model)
        {
            ModelState.Clear();

            if (!string.IsNullOrEmpty(model.Id))
            {
                model = business.GetNotification(model);

                if (model == null)
                {
                    ModelState.AddModelError("", localizer["C01004"]);
                }
            }

            if (model == null) model = new S03005ViewModel();

            model.FactoryList = business.GetListFactory();
            model.ContainerList = new List<Container>();
            model.ContainerList.Add(new Container()
            {
                Id = model.ContainerId,
                Code = model.Container,
            });

            model.LocationList = new List<semigura.DBContext.Entities.Location>();
            model.LocationList.Add(new semigura.DBContext.Entities.Location()
            {
                Id = model.LocationId,
                Name = model.Location,
            });

            model.LotList = new List<Lot>();
            model.LotList.Add(new Lot()
            {
                Id = model.LotId,
                Code = model.LotCode,
            });

            return View(model);
        }



        public ActionResult GetDataTable(S03005ViewModel model)
        {
            var result = business.GetListNotification(model);

            return Json(new
            {
                iTotalRecords = result.TotalRecords,
                iTotalDisplayRecords = result.TotalRecords,
                aaData = result.DataNotificationList,
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        public async Task<ActionResult> Save(S03005ViewModel model)
        {
            try
            {
                string message = business.Save(model, authCookie.UserInfo);
                if (string.IsNullOrEmpty(message))
                {
                    //NotificationHub.RefreshDataNotification().ConfigureAwait(false);
                    await chatHub.NotifyRefreshData();
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

        public ActionResult Delete(S03005ViewModel model)
        {
            try
            {
                string message = business.Delete(model);
                if (string.IsNullOrEmpty(message))
                {
                    // NotificationHub.RefreshDataNotification().ConfigureAwait(false); // TODO
                    chatHub.NotifyRefreshData();
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

        public async Task<ActionResult> CloseNotification(S03005ViewModel model)
        {
            try
            {
                string message = business.CloseNotification(model, authCookie.UserInfo);
                if (string.IsNullOrEmpty(message))
                {
                    await chatHub.NotifyOnCloseNotification();
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

        public ActionResult GetListLocation(S03005ViewModel model)
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

                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }


        public ActionResult GetListContainer(S03005ViewModel model)
        {
            try
            {
                var data = new List<Container>();
                if (!string.IsNullOrEmpty(model.FactoryId))
                {
                    data = business.GetListContainerTank(model.FactoryId);
                    data = business.GetListContainerSeiguiku(model.FactoryId);
                }

                return Json(new { status = true, data = data }/*, JsonRequestBehavior.AllowGet*/);
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