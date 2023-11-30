using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;

namespace semigura.DBContext.Repositories
{
    public class MaterialStandValDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;

        public MaterialStandValDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        public string Add(MaterialStandVal entity)
        {
            entity.Id = Utils.GenerateId(context);
            context.MaterialStandVal.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string Delete(string materialId)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var items = context.MaterialStandVal.Where(s => s.MaterialId == materialId).ToList();
            if (items != null)
            {
                foreach (var item in items)
                {
                    context.MaterialStandVal.Remove(item);
                }

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