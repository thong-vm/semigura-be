using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using semigura.DBContext.Business;
using semigura.DBContext.Entities;
using semigura.Models;

namespace semigura.Controllers
{
    public class S03006Controller : BaseController
    {
        private readonly IStringLocalizer localizer;
        S03006Business business;
        public S03006Controller(DBEntities db,
            ILogger<S03006Controller> logger,
            S03006Business business,
            IStringLocalizer<S03006Controller> localizer) : base(db, logger)
        {
            this.localizer = localizer;
            this.business = business;
        }
        // GET: S03006
        public ActionResult Index()
        {
            S03006ViewModel model = new S03006ViewModel();
            var data = business.GetListContainer(model);

            model.ListContainer = data.ListContainer;
            if (string.IsNullOrEmpty(model.ContainerId) && model.ListContainer.Any())
            {
                model.ContainerId = model.ListContainer[0].Id;
            }

            return View(model);
        }

        public ActionResult GetListCapacityRefer(S03006ViewModel model)
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