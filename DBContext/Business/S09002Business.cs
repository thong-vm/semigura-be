using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S09002Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S09002Business(DBEntities db,
            ILogger<S09002Business> logger,
            IStringLocalizer<S09002Business> localizer) : base(db, logger, localizer)
        {
            this.localizer = localizer;
        }

        public S09002ViewModel GetListAlertMail(S09002ViewModel model)
        {
            {
                var dao = new AlertMailDao(context, localizer);

                return dao.GetListAlertMail(model);
            }
        }

        public S09002ViewModel? GetAlertMail(S09002ViewModel model)
        {
            try
            {
                var dao = new AlertMailDao(context, localizer);

                var alertMail = dao.GetAlertMail(model);

                if (alertMail != null)
                {
                    return alertMail.Map<S09002ViewModel>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public string Save(S09002ViewModel model, UserInfoModel UserInfoModel)
        {
            string message = string.Empty;
            try
            {
                var dao = new AlertMailDao(context, localizer);

                if (!string.IsNullOrEmpty(model.Id))
                {
                    AlertMail entity = model.Map<AlertMail>();
                    entity.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                    entity.ModifiedById = UserInfoModel.Id;

                    message = dao.Update(entity);
                }
                else
                {
                    AlertMail entity = model.Map<AlertMail>();
                    entity.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                    entity.CreatedById = UserInfoModel.Id;

                    message = dao.Add(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return message;
        }

        public string Delete(S09002ViewModel model)
        {
            string message = string.Empty;
            try
            {
                var dao = new AlertMailDao(context, localizer);
                message = dao.Delete(model.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return message;
        }
    }
}