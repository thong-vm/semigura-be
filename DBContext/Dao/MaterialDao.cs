using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class MaterialDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;

        public MaterialDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        public List<Material> GetListMaterial()
        {
            return context.Material.ToList();
        }

        public S09001ViewModel GetMaterial(S09001ViewModel model)
        {
            S09001ViewModel result = null;
            var query = from material in context.Material

                        join material_standval in context.MaterialStandVal on material.Id equals material_standval.MaterialId into materialStandValJoin
                        from materialStandValLResult in materialStandValJoin.DefaultIfEmpty()

                        where material.Id.Equals(model.Id)
                        select new
                        {
                            material,
                            materialStandValLResult.Id,
                            materialStandValLResult.TempMin,
                            materialStandValLResult.TempMax,
                            materialStandValLResult.HumidityMin,
                            materialStandValLResult.HumidityMax,
                            materialStandValLResult.Type,
                        };

            var lst = query.ToList();
            if (lst.Count > 0)
            {
                result = lst[0].material.Map<S09001ViewModel>();
                result.MaterialStandValList = new List<S09001ViewModel>();

                foreach (var item in lst)
                {
                    result.MaterialStandValList.Add(item.Map<S09001ViewModel>());
                }
            }

            return result;
        }


        public S09001ViewModel GetLisMaterial(S09001ViewModel model)
        {
            var result = new S09001ViewModel();

            var query = from material in context.Material

                        join material_standval in context.MaterialStandVal on material.Id equals material_standval.MaterialId into materialStandValJoin
                        from materialStandValLResult in materialStandValJoin.DefaultIfEmpty()

                        select new MaterialModel()
                        {
                            Id = material.Id,
                            Name = material.Name,
                            Note = material.Note,
                            TempMin = materialStandValLResult.TempMin,
                            TempMax = materialStandValLResult.TempMax,
                            HumidityMin = materialStandValLResult.HumidityMin,
                            HumidityMax = materialStandValLResult.HumidityMax,
                            Type = materialStandValLResult.Type,
                        };

            if (!string.IsNullOrEmpty(model.Name))
            {
                query = query.Where(s => s.Name.Contains(model.Name));
            }

            var materialId = context.Material.OrderBy(s => s.Name).Select(s => s.Id).ToList();

            result.MaterialList = query.OrderBy(s => s.Name).Skip(model.IDisplayStart).Take(model.IDisplayLength).ToList();
            result.MaterialList.ForEach(s =>
            {
                if (materialId.Contains(s.Id))
                {
                    s.No = materialId.IndexOf(s.Id) + 1;
                }
            });

            result.TotalRecords = query.Count();
            return result;
        }

        public S03001ViewModel.Rice GetInfoRiceByRiceId(string riceId)
        {
            var material = context.Material.AsQueryable();
            var materialstandval = context.MaterialStandVal.AsQueryable();

            material = material.Where(s => s.Id.Equals(riceId));
            materialstandval = materialstandval.Where(s => s.MaterialId.Equals(riceId));

            var queryRice = from sMat in material
                            join sMatVal in materialstandval
                            on sMat.Id equals sMatVal.MaterialId
                            select new S03001ViewModel.Rice
                            {
                                Id = sMat.Id,
                                Name = sMat.Name,
                                MinTempMoromi = null,
                                MaxTempMoromi = null,
                                MinTempSeigiku = null,
                                MaxTempSeigiku = null

                            };
            var queryMoromi = from sMat in material
                              join sMatVal in materialstandval
                              on sMat.Id equals sMatVal.MaterialId
                              where sMatVal.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK
                              select new
                              {
                                  MinTempMoromi = sMatVal.TempMin,
                                  MaxTempMoromi = sMatVal.TempMax
                              };
            var querySeigiku = from sMat in material
                               join sMatVal in materialstandval
                               on sMat.Id equals sMatVal.MaterialId
                               where sMatVal.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU
                               select new
                               {
                                   MinTempSeigiku = sMatVal.TempMin,
                                   MaxTempSeigiku = sMatVal.TempMax
                               };
            S03001ViewModel.Rice result = new S03001ViewModel.Rice();
            result = queryRice.FirstOrDefault();
            if (queryMoromi.Any())
            {
                result.MinTempMoromi = queryMoromi.FirstOrDefault().MinTempMoromi;
                result.MaxTempMoromi = queryMoromi.FirstOrDefault().MaxTempMoromi;
            }
            if (querySeigiku.Any())
            {
                result.MinTempSeigiku = querySeigiku.FirstOrDefault().MinTempSeigiku;
                result.MaxTempSeigiku = querySeigiku.FirstOrDefault().MaxTempSeigiku;
            }
            return result;
        }
        public List<S03001ViewModel.Semaibuai> GetInfoRiceByRiceName(string riceName)
        {
            List<S03001ViewModel.Semaibuai> result;
            result = context.Material.Where(x => x.Name == riceName).Select(y => new S03001ViewModel.Semaibuai
            {
                Id = y.Id,
                Name = "",//y.RicePolishingRatioName
            }).ToList();

            return result;
        }

        public string Add(Material entity)
        {
            var item = context.Material.Where(s => s.Name == entity.Name).FirstOrDefault();
            if (item != null)
            {
                return string.Format(localizer["msg_item_exist"], localizer["material"], entity.Name);
            }
            entity.Id = Utils.GenerateId(context);
            context.Material.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string Update(Material entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.Material.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (item != null)
            {
                var duplicateItem = context.Material.Where(s => s.Id != item.Id && s.Name == entity.Name).FirstOrDefault();
                if (duplicateItem != null)
                {
                    return string.Format(localizer["msg_item_exist"], localizer["material"], entity.Name);
                }

                item.Name = entity.Name;
                item.Note = entity.Note;
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
            var item = context.Material.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                context.Material.Remove(item);

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