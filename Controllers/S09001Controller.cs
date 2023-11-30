using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Models;
using semigura.Repositories;
using System.Text.Json;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S09001
// モジュール名称：材料管理画面
// 機能概要　　　：材料の情報管理
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Thao
//
//******************************************************************************

namespace semigura.Controllers
{

    public class S09001Controller : BaseController
    {
        private readonly IStringLocalizer localizer;
        private readonly S09001Business business;
        private readonly AuthCookieRepository authCookie;

        public S09001Controller(
            DBEntities dBEntities,
            ILogger<S09001Controller> logger,
            S09001Business business,
            AuthCookieRepository authCookie,
            IStringLocalizer<S09001Controller> localizer
            ) : base(dBEntities, logger)
        {
            this.business = business;
            this.authCookie = authCookie;
            this.localizer = localizer;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(S09001ViewModel model)
        {
            ModelState.Clear();
            if (!string.IsNullOrEmpty(model.Id))
            {
                model = business.GetMaterial(model);

                if (model == null)
                {
                    ModelState.AddModelError("", localizer["C01004"]);
                }
                else if (model.MaterialStandValList == null || !model.MaterialStandValList.Any())
                {
                    model.MaterialStandValList = new List<S09001ViewModel>();
                    model.MaterialStandValList.Add(new S09001ViewModel() { Type = 1 });
                    model.MaterialStandValList.Add(new S09001ViewModel() { Type = 2 });
                }
            }

            return View(model);
        }

        public ActionResult GetDataTable(S09001ViewModel model)
        {
            try
            {
                if (model.SEcho == "1") return Json(new { }/*, JsonRequestBehavior.AllowGet*/);

                var result = business.GetListMaterial(model);
                _logger.LogInfo(JsonSerializer.Serialize(result));

                return Json(new
                {
                    iTotalRecords = result.TotalRecords,
                    iTotalDisplayRecords = result.TotalRecords,
                    aaData = result.MaterialList,
                }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = localizer["C01003"] }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        private string ValidateSave(S09001ViewModel model)
        {
            var message = string.Empty;

            if (model.MaterialStandValList != null && model.MaterialStandValList.Any())
            {
                for (int i = 0; i < model.MaterialStandValList.Count; i++)
                {
                    var data = model.MaterialStandValList[i];
                    if (data.TempMin >= data.TempMax)
                    {
                        return string.Format(localizer["msg_validation_min_max"].Value, localizer["temp_min"].Value, localizer["temp_max"].Value);
                    }

                    if (model.MaterialStandValList.Where(s => s.Type == data.Type).Count() >= 2)
                    {
                        return localizer["msg_validation_duplicate"].Value;
                    }
                }
            }

            return message;
        }

        public ActionResult Save(S09001ViewModel model)
        {
            try
            {
                // 検証
                var message = ValidateSave(model);
                if (!string.IsNullOrEmpty(message))
                {
                    return Json(new { status = false, message = message }/*, JsonRequestBehavior.AllowGet*/);
                }

                // 保存
                //var userData = HttpContext.User.FindFirstValue(ClaimTypes.UserData);
                message = business.Save(model, authCookie.UserInfo);
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

        public ActionResult Delete(S09001ViewModel model)
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