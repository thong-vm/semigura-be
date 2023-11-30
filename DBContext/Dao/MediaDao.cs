using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;

namespace semigura.DBContext.Repositories
{
    public class MediaDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;
        public MediaDao(DBEntities context, IStringLocalizer localizer)
        {
            this.localizer = localizer;
            this.context = context;
        }

        public Media GetMedia(string id)
        {
            return context.Media.Where(s => s.Id == id).FirstOrDefault();
        }

        public List<MediaModel> GetListMedia(string lotContainerId, int type, int takeNum)
        {
            return context.Media.Where(s => s.LotContainerId == lotContainerId && s.Type == type)
                .OrderByDescending(s => s.CreatedOn).Take(takeNum)
                .Select(s => new MediaModel
                {
                    Id = s.Id,
                    Path = s.Path,
                    Size = s.Size,
                    Type = s.Type,
                    TerminalId = s.TerminalId,
                    CreatedOn = s.CreatedOn,
                    CreatedById = s.CreatedById,
                    ModifiedOn = s.ModifiedOn,
                    ModifiedById = s.ModifiedById,
                    LotContainerId = s.LotContainerId,
                })
                .ToList();
        }

        public string DeleteByLotContainerId(string lotcontainerId)
        {
            var item = context.Media.Where(s => s.LotContainerId == lotcontainerId);
            if (item.Any())
            {
                //item.ForEach(s => context.Media.Remove(s));
                context.Media.RemoveRange(item);
                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public string Add(Media entity)
        {
            entity.Id = Utils.GenerateId(context);
            context.Media.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string DeleteById(string id)
        {
            var item = context.Media.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                context.Media.Remove(item);
                context.SaveChanges();
            }

            return string.Empty;
        }
    }
}