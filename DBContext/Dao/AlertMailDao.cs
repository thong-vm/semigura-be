using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class AlertMailDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;

        public AlertMailDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        public S09002ViewModel GetListAlertMail(S09002ViewModel model)
        {
            var result = new S09002ViewModel();

            var query = context.AlertMail.AsQueryable();
            if (!string.IsNullOrEmpty(model.Email))
            {
                query = query.Where(s => s.Email.Contains(model.Email));
            }

            var list = query.OrderBy(s => s.Email).Skip(model.IDisplayStart).Take(model.IDisplayLength).ToList();

            result.AlertMailList = new List<S09002ViewModel>();
            list.ForEach(s =>
            {
                //result.AlertMailList.Add(s.Map(new S09002ViewModel()));
                result.AlertMailList.Add(new S09002ViewModel());
            });

            result.TotalRecords = query.Count();
            return result;
        }

        public List<AlertMail> GetListAlertMail()
        {
            return context.AlertMail.ToList();
        }


        public AlertMail GetAlertMail(S09002ViewModel model)
        {
            return context.AlertMail.Where(s => s.Id == model.Id).FirstOrDefault();
        }

        public string Add(AlertMail entity)
        {
            var item = context.AlertMail.Where(s => s.Email == entity.Email).FirstOrDefault();
            if (item != null)
            {
                return string.Format(localizer["msg_item_exist"], localizer["email"], entity.Email);
            }

            entity.Id = Utils.GenerateId(context);
            context.AlertMail.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string Update(AlertMail entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.AlertMail.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (item != null)
            {
                var duplicateItem = context.AlertMail.Where(s => s.Id != item.Id && s.Email == entity.Email).FirstOrDefault();
                if (duplicateItem != null)
                {
                    return string.Format(localizer["msg_item_exist"], localizer["email"], entity.Email);
                }

                item.Email = entity.Email;
                item.ModifiedOn = entity.ModifiedOn;
                item.ModifiedById = entity.ModifiedById;

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public string Delete(string id)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.AlertMail.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                context.AlertMail.Remove(item);

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }
    }
}