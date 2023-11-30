using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class DataEntryDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;

        public DataEntryDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }
        public List<DataEntry> GetListDataEntry()
        {
            return context.DataEntry.ToList();
        }
        public List<DataEntry> GetListDataEntryByLotContainerId(string lotContainerId, DateTime? startDate, DateTime? endDate)
        {
            var query = from dataEntry in context.DataEntry
                        where dataEntry.LotContainerId == lotContainerId
                            && (startDate != null && dataEntry.MeasureDate >= startDate)
                            && (endDate == null || (endDate != null && dataEntry.MeasureDate <= endDate))

                        orderby dataEntry.MeasureDate ascending
                        select dataEntry;

            return query.ToList();
        }

        public S02001ViewModel GetListDataEntry(S02001ViewModel model)
        {
            var result = new S02001ViewModel();

            if (!string.IsNullOrEmpty(model.LotContainerId))
            {
                var query = from dataEntry in context.DataEntry
                            join lotContainer in context.LotContainer on dataEntry.LotContainerId equals lotContainer.Id
                            where dataEntry.LotContainerId == model.LotContainerId && dataEntry.MeasureDate >= lotContainer.StartDate
                            select new S02001ViewModel.DataEntryModel
                            {
                                Id = dataEntry.Id,
                                BaumeDegree = dataEntry.BaumeDegree,
                                AlcoholDegree = dataEntry.AlcoholDegree,
                                Acid = dataEntry.Acid,
                                AminoAcid = dataEntry.AminoAcid,
                                Glucose = dataEntry.Glucose,
                                MeasureDate = dataEntry.MeasureDate,
                            };

                if (model.SearchByDate != null)
                {
                    query = query.Where(s => s.MeasureDate.Year == model.SearchByDate.Value.Year && s.MeasureDate.Month == model.SearchByDate.Value.Month && s.MeasureDate.Day == model.SearchByDate.Value.Day);
                }

                result.DataEntryList = query.OrderByDescending(s => s.MeasureDate).Skip(model.IDisplayStart).Take(model.IDisplayLength).ToList();
                result.TotalRecords = query.Count();
            }


            return result;
        }

        // if success return null
        public string Delete(string dataEntryId)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.DataEntry.Where(s => s.Id == dataEntryId).FirstOrDefault();
            if (item != null)
            {
                context.DataEntry.Remove(item);
                context.SaveChanges();

                //S02001Hub.RefreshData().ConfigureAwait(false);
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        // if success return null
        public string Add(DataEntry dataEntry)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            string result = string.Empty;
            var item = context.LotContainer.Where(s => s.Id == dataEntry.LotContainerId).FirstOrDefault();
            if (item != null)
            {
                var startDate = item.StartDate;
                if (startDate <= dataEntry.MeasureDate && systemDate >= dataEntry.MeasureDate)
                {
                    dataEntry.Id = Utils.GenerateId(context);
                    context.DataEntry.Add(dataEntry);
                    context.SaveChanges();
                    result = string.Empty;

                    //S02001Hub.RefreshData().ConfigureAwait(false);
                }
                else if (systemDate < dataEntry.MeasureDate)
                {
                    result = string.Format(localizer["entryErrorMsgDay"]);
                }
                else
                {
                    result = string.Format(localizer["entryErrorMsg"], startDate);
                }
            }

            return result;
        }

        public string DeleteByLotContainerId(string lotcontainerId)
        {
            var item = context.DataEntry.Where(s => s.LotContainerId == lotcontainerId);
            if (item.Any())
            {
                //item.ForEach(s => context.DataEntry.Remove(s));
                context.DataEntry.RemoveRange(item);
                context.SaveChanges();

                //S02001Hub.RefreshData().ConfigureAwait(false);
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public string AddDataEntryEdit(DataEntry entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.LotContainer.ToArray().Where(s => s.Id == entity.LotContainerId).Select(y => y.ContainerId);
            if (!query.Any())
            {
                return string.Empty;
            }
            string containerId = query.FirstOrDefault();
            entity.Id = Utils.GenerateId(context);
            entity.ContainerId = containerId;
            entity.CreatedOn = systemDate;
            context.DataEntry.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string ModifyDataEntryEdit(DataEntry entity)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.DataEntry.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (query != null)
            {
                query.BaumeDegree = entity.BaumeDegree;
                query.AlcoholDegree = entity.AlcoholDegree;
                query.Acid = entity.Acid;
                query.AminoAcid = entity.AminoAcid;
                query.Glucose = entity.Glucose;
                context.SaveChanges();
            }
            return string.Empty;
        }
    }
}