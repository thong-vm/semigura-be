using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class LotContainerTerminalDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;
        public LotContainerTerminalDao(DBEntities context, IStringLocalizer localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }
        public List<LotContainerTerminal> GetListLotContainerTerminal()
        {
            return context.LotContainerTerminal.ToList();
        }

        public bool CheckListSensorInUse(string sensorID)
        {
            var query = context.LotContainerTerminal.Where(s => s.TerminalId == sensorID);
            if (query.Any())
            {
                var result = query.Where(s => s.EndDate == null);
                if (result.Any())
                {
                    return true;
                }
            }
            return false;
        }

        public string FinishDateLotContainer(string lotContainerTerminalID, ref string terminalId)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.LotContainerTerminal.Where(s => s.Id == lotContainerTerminalID).FirstOrDefault();
            if (query != null)
            {
                if (systemDate < query.StartDate)
                {
                    query.EndDate = query.StartDate;
                }
                else
                {
                    query.EndDate = systemDate;
                }
                terminalId = query.TerminalId;
                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }
            return string.Empty;
        }

        public string FinishDateLotContainerByLotContainerId(string lotContainerID)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID);
            if (query != null && query.Any())
            {
                foreach (var item in query)
                {
                    if (systemDate < item.StartDate)
                    {
                        item.EndDate = item.StartDate;
                    }
                    else
                    {
                        item.EndDate = systemDate;
                    }

                }
                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }
            return string.Empty;
        }

        public string FinishDateLotContainerGetListTerminalId(string lotContainerID, ref List<string> lstTerminalId)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID);
            if (query != null && query.Any())
            {
                foreach (var item in query)
                {
                    if (item.EndDate == null)
                    {
                        if (systemDate < item.StartDate)
                        {
                            item.EndDate = item.StartDate;
                        }
                        else
                        {
                            item.EndDate = systemDate;
                        }
                        lstTerminalId.Add(item.TerminalId);
                    }
                }
                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }
            return string.Empty;
        }


        public bool CheckUsingByTerminalId(string terminalId)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var query = context.LotContainerTerminal.Where(s => s.TerminalId == terminalId);
            if (query != null && query.Any())
            {
                foreach (var item in query)
                {
                    if (item.EndDate == null)
                    {
                        return true;
                    }
                }
                context.SaveChanges();
            }
            return false;
        }

        public string ModifyByLotContainerIDAndSensorID(string lotContainerID, string sensorID, string userID)
        {
            var item = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID && s.TerminalId == sensorID).FirstOrDefault();
            if (item != null)
            {
                var lotcontainer = context.LotContainer.Where(s => s.Id == lotContainerID).FirstOrDefault();
                item.TerminalId = sensorID;
                if (lotcontainer != null)
                {
                    item.EndDate = lotcontainer.EndDate;
                }
                context.SaveChanges();
            }
            else
            {
                LotContainerTerminal entity = new LotContainerTerminal();
                entity.Id = Utils.GenerateId(context);
                entity.LotContainerId = lotContainerID;
                entity.TerminalId = sensorID;
                entity.StartDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                entity.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                entity.CreatedById = userID;

                context.LotContainerTerminal.Add(entity);
                context.SaveChanges();
            }
            return string.Empty;
        }

        public string Add(string lotContainerID, string sensorID, string userID)
        {
            var item = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID && s.TerminalId == sensorID).FirstOrDefault();
            if (item != null)
            {
                return string.Format(localizer["msg_item_exist"], localizer["lotcode"], lotContainerID);
            }
            LotContainerTerminal entity = new LotContainerTerminal();
            entity.Id = Utils.GenerateId(context);
            entity.LotContainerId = lotContainerID;
            entity.TerminalId = sensorID;
            entity.StartDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            entity.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            entity.CreatedById = userID;

            context.LotContainerTerminal.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string AddByLot(string lotContainerID, string sensorID, string userID, Nullable<System.DateTime> startDateLot)
        {
            var item = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID && s.TerminalId == sensorID).FirstOrDefault();
            if (item != null)
            {
                return string.Format(localizer["msg_item_exist"], localizer["lotcode"], lotContainerID);
            }
            LotContainerTerminal entity = new LotContainerTerminal();
            entity.Id = Utils.GenerateId(context);
            entity.LotContainerId = lotContainerID;
            entity.TerminalId = sensorID;
            entity.StartDate = startDateLot;
            entity.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            entity.CreatedById = userID;

            context.LotContainerTerminal.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string AddCheckDate(string lotContainerID, string sensorID, string userID, Nullable<System.DateTime> startDateLot)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var item = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID && s.TerminalId == sensorID).FirstOrDefault();
            if (item != null)
            {
                return string.Format(localizer["msg_item_exist"], localizer["lotcode"], lotContainerID);
            }
            LotContainerTerminal entity = new LotContainerTerminal();
            entity.Id = Utils.GenerateId(context);
            entity.LotContainerId = lotContainerID;
            entity.TerminalId = sensorID;
            entity.StartDate = startDateLot;
            //if (startDateLot > systemDate)
            //{
            //    entity.StartDate = startDateLot;
            //}
            //else
            //{
            //    entity.StartDate = systemDate;
            //}
            entity.CreatedOn = systemDate;
            entity.CreatedById = userID;

            context.LotContainerTerminal.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string UpdateByLotContainerIDAndSensorID(string lotContainerID, S03001ViewModel.Terminal sensorID, string userID)
        {
            var item = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID && s.TerminalId == sensorID.IdOld).FirstOrDefault();
            var deleteflag = context.Terminal.Where(s => s.Id == sensorID.Id).Select(x => x.DeleteFlg).FirstOrDefault();
            var typeContainer = context.Terminal.Where(s => s.Id == sensorID.Id).Select(x => x.Type).FirstOrDefault();
            if (deleteflag == true || (typeContainer != semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU))
            {
                return string.Empty;
            }
            if (item != null)
            {
                item.TerminalId = sensorID.Id;
                item.StartDate = sensorID.StartDate;
                if (sensorID.EndDate < item.StartDate)
                {
                    item.EndDate = item.StartDate;
                }
                else
                {
                    item.EndDate = sensorID.EndDate;
                }
                item.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                item.ModifiedById = userID;
                context.SaveChanges();
            }
            else
            {
                return string.Format(localizer["msg_item_exist"], localizer["lotcode"], lotContainerID);
            }


            return string.Empty;
        }

        public string UpdateByContainerIdChange(string lotcontainerId, string containerId)
        {
            //TODO
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var ListTerminal = context.Terminal.Where(s => s.ParentId == containerId).ToList();

            foreach (var sensor in ListTerminal)
            {
                var list = context.LotContainerTerminal.Where(s => s.LotContainerId == lotcontainerId && s.TerminalId == sensor.Id).FirstOrDefault();

                if (list != null)
                {
                    if (systemDate < list.StartDate)
                    {
                        list.EndDate = list.StartDate;
                    }
                    else
                    {
                        list.EndDate = systemDate;
                    }
                }
            }
            return string.Empty;
        }

        public string UpdateByTerminal(Terminal entity)
        {
            //TODO
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var list = context.LotContainer.Where(s => s.ContainerId == entity.Id && s.EndDate == null).ToList();
            if (list != null && list.Any())
            {
                foreach (var item in list)
                {
                    item.ModifiedOn = entity.ModifiedOn;
                    item.ModifiedById = entity.ModifiedById;

                    context.SaveChanges();
                }
            }

            return string.Empty;
        }

        public string UpdateByContainerId(LotContainer entity, S03001ViewModel.Tank container)
        {
            //TODO
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var ListTerminal = context.Terminal.Where(s => s.ParentId == container.Id).ToList();
            var lotcontainer = context.LotContainer.Where(s => s.ContainerId == container.Id && s.LotId == entity.LotId).FirstOrDefault();
            foreach (var sensor in ListTerminal)
            {
                var list = context.LotContainerTerminal.Where(s => s.LotContainerId == lotcontainer.Id && s.TerminalId == sensor.Id).FirstOrDefault();

                if (list != null)
                {
                    if (container.EndDate != null)
                    {
                        if (container.EndDate < list.StartDate)
                        {
                            list.EndDate = list.StartDate;
                        }
                        else
                        {
                            list.EndDate = container.EndDate;
                        }
                    }
                    else
                    {
                        list.EndDate = null;
                    }
                }
                else
                {
                    LotContainerTerminal item = new LotContainerTerminal();
                    item.Id = Utils.GenerateId(context);
                    item.LotContainerId = lotcontainer.Id;
                    item.TerminalId = sensor.Id;
                    item.StartDate = lotcontainer.StartDate;
                    item.CreatedOn = systemDate;
                    item.CreatedById = lotcontainer.CreatedById;

                    context.LotContainerTerminal.Add(item);
                    context.SaveChanges();
                }
            }


            return string.Empty;
        }

        public string UpdatebyTerminalChange(string terminalId, string containerId, string userId)
        {
            var systemDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
            var containerId_old = context.Terminal.Where(s => s.Id == terminalId).Select(x => x.ParentId).FirstOrDefault();
            var lstLotContainer_old = context.LotContainer.Where(s => s.ContainerId == containerId_old && s.EndDate == null).ToList();

            foreach (var lotcontainer in lstLotContainer_old)
            {
                var lotContainerTerminal = context.LotContainerTerminal.Where(s => s.LotContainerId == lotcontainer.Id && s.TerminalId == terminalId).FirstOrDefault();

                if (lotContainerTerminal != null)
                {
                    lotContainerTerminal.EndDate = systemDate;
                }
            }
            var lstLotContainer_new = context.LotContainer.Where(s => s.ContainerId == containerId && s.EndDate == null).ToList();
            foreach (var lotcontainer in lstLotContainer_new)
            {
                var list = context.LotContainerTerminal.Where(s => s.LotContainerId == lotcontainer.Id && s.TerminalId == terminalId).FirstOrDefault();

                if (list != null)
                {
                    list.EndDate = null;
                }
                else
                {
                    LotContainerTerminal item = new LotContainerTerminal();
                    item.Id = Utils.GenerateId(context);
                    item.LotContainerId = lotcontainer.Id;
                    item.TerminalId = terminalId;
                    item.StartDate = systemDate;
                    item.CreatedOn = systemDate;
                    item.CreatedById = userId;

                    context.LotContainerTerminal.Add(item);
                    context.SaveChanges();
                }
            }


            return string.Empty;
        }

        public string Delete(string lotContainerID, string sensorID)
        {
            var item = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID && s.TerminalId == sensorID).FirstOrDefault();
            if (item != null)
            {
                context.LotContainerTerminal.Remove(item);

                DeleteTerminalIsDeleted(item.TerminalId);

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public string DeleteByLotContainerId(string lotContainerID)
        {
            var query = context.LotContainerTerminal.Where(s => s.LotContainerId == lotContainerID);
            if (query.Any())
            {
                foreach (var item in query)
                {
                    context.LotContainerTerminal.Remove(item);

                    DeleteTerminalIsDeleted(item.TerminalId);
                }
                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        private void DeleteTerminalIsDeleted(string terminalId)
        {
            var sql = from terminal in context.Terminal
                      join lotContainerTerminal in context.LotContainerTerminal on terminal.Id equals lotContainerTerminal.TerminalId
                      where terminal.DeleteFlg == true && terminal.Id == terminalId
                      select lotContainerTerminal;

            if (sql.Count() == 1)
            {
                var terminalDeletedItem = context.Terminal.Where(s => s.Id == terminalId).FirstOrDefault();
                if (terminalDeletedItem != null)
                {
                    context.Terminal.Remove(terminalDeletedItem);
                }
            }
        }

        public string getLCTidByLCTidTid(string lotContainerID, string terminalID)
        {
            string result = context.LotContainerTerminal
                .Where(s => s.LotContainerId == lotContainerID && s.TerminalId == terminalID)
                .Select(e => e.Id)
                .FirstOrDefault().ToString();
            return result;
        }
    }

}