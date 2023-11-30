using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.Models;

namespace semigura.DBContext.Repositories
{
    public class TerminalDao
    {
        private readonly DBEntities context;
        private readonly IStringLocalizer localizer;
        public TerminalDao(DBEntities context, IStringLocalizer localizer)
        {
            this.localizer = localizer;
            this.context = context;
        }

        public List<Terminal> GetListTerminal()
        {
            return context.Terminal.ToList();
        }

        public List<Terminal> GetListTerminalByType()
        {
            return context.Terminal.OrderBy(s => s.Code).ToList();
        }

        public List<TerminalModel> GetListTerminalByType(int type)
        {
            var query = from terminal in context.Terminal

                        join container in context.Container on terminal.ParentId equals container.Id into containerJoin
                        from containerLResult in containerJoin.DefaultIfEmpty()

                        from lotContainer in context.LotContainer.Where(s => s.EndDate == null && s.ContainerId == containerLResult.Id).DefaultIfEmpty()

                        where 1 == 1 && terminal.Type == type
                        select new TerminalModel()
                        {
                            Id = terminal.Id,
                            Code = terminal.Code,
                            Name = terminal.Name,
                            ParentId = terminal.ParentId,
                            Type = terminal.Type,
                            LoginId = terminal.LoginId,
                            Password = terminal.Password,
                            CreatedOn = terminal.CreatedOn,
                            CreatedById = terminal.CreatedById,
                            ModifiedOn = terminal.ModifiedOn,
                            ModifiedById = terminal.ModifiedById,
                            LotContainerId = (type == semigura.Commons.Properties.TERMINAL_TYPE_LOCATION) ? terminal.ParentId : lotContainer.Id,
                            LotContainerStartDate = lotContainer.StartDate,
                            LotContainerEndDate = lotContainer.EndDate,
                        };

            return query.ToList();
        }

        public TerminalModel GetTerminalByType(string terminalCode, int type = 0)
        {
            var query = from terminal in context.Terminal

                        join container in context.Container on terminal.ParentId equals container.Id into containerJoin
                        from containerLResult in containerJoin.DefaultIfEmpty()

                        from lotContainer in context.LotContainer.Where(s => s.EndDate == null && s.ContainerId == containerLResult.Id).DefaultIfEmpty()

                        where 1 == 1 && terminal.Code == terminalCode && (type <= 0 || type > 0 && terminal.Type == type)
                        select new TerminalModel()
                        {
                            Id = terminal.Id,
                            Code = terminal.Code,
                            Name = terminal.Name,
                            ParentId = terminal.ParentId,
                            Type = terminal.Type,
                            LoginId = terminal.LoginId,
                            Password = terminal.Password,
                            CreatedOn = terminal.CreatedOn,
                            CreatedById = terminal.CreatedById,
                            ModifiedOn = terminal.ModifiedOn,
                            ModifiedById = terminal.ModifiedById,
                            LotContainerId = (type == semigura.Commons.Properties.TERMINAL_TYPE_LOCATION) ? terminal.ParentId : lotContainer.Id,
                            LotContainerStartDate = lotContainer.StartDate,
                            LotContainerEndDate = lotContainer.EndDate,
                        };

            return query.FirstOrDefault();
        }

        public S09004ViewModel GetListTerminal(S09004ViewModel model)
        {
            var result = new S09004ViewModel();

            var query = from terminal in context.Terminal

                        join location in context.Location on terminal.ParentId equals location.Id into locationJoin
                        from locationLResult in locationJoin.DefaultIfEmpty()

                        join factory1 in context.Factory on locationLResult.FactotyId equals factory1.Id into factory1Join
                        from factory1LResult in factory1Join.DefaultIfEmpty()

                        join container in context.Container on terminal.ParentId equals container.Id into containerJoin
                        from containerLResult in containerJoin.DefaultIfEmpty()

                        join locationOfContainer in context.Location on containerLResult.LocationId equals locationOfContainer.Id into locationOfContainerJoin
                        from locationOfContainerJoinLResult in locationOfContainerJoin.DefaultIfEmpty()

                        join factory2 in context.Factory on locationOfContainerJoinLResult.FactotyId equals factory2.Id into factory2Join
                        from factory2LResult in factory2Join.DefaultIfEmpty()

                            // 製麹の場合に仕込号数を取得
                        from lotContainer in context.LotContainer.Where(s => s.ContainerId == containerLResult.Id && s.EndDate == null).DefaultIfEmpty()
                        from lot in context.Lot.Where(s => s.Id == lotContainer.LotId).DefaultIfEmpty()

                        where terminal.DeleteFlg != true
                        select new TerminalModel()
                        {
                            Id = terminal.Id,
                            Code = terminal.Code,
                            Name = terminal.Name,
                            ParentId = terminal.Type == semigura.Commons.Properties.TERMINAL_TYPE_SEIGIKU ?
                                        (!string.IsNullOrEmpty(lot.Id) ? lot.Id : null) :
                                        (!string.IsNullOrEmpty(locationLResult.Id) ? locationLResult.Id : (!string.IsNullOrEmpty(containerLResult.Id) ? containerLResult.Id : null)),
                            ParentName = terminal.Type == semigura.Commons.Properties.TERMINAL_TYPE_SEIGIKU ?
                                        (!string.IsNullOrEmpty(lot.Code) ? lot.Code : null) :
                                        (!string.IsNullOrEmpty(locationLResult.Id) ? locationLResult.Name : (!string.IsNullOrEmpty(containerLResult.Id) ? containerLResult.Code : null)),
                            FactoryId = !string.IsNullOrEmpty(factory1LResult.Id) ? factory1LResult.Id : (!string.IsNullOrEmpty(factory2LResult.Id) ? factory2LResult.Id : null),
                            FactoryName = !string.IsNullOrEmpty(factory1LResult.Id) ? factory1LResult.Name : (!string.IsNullOrEmpty(factory2LResult.Id) ? factory2LResult.Name : null),
                            Type = terminal.Type,
                            LoginId = terminal.LoginId,
                            Password = terminal.Password,
                            CreatedOn = terminal.CreatedOn,
                            CreatedById = terminal.CreatedById,
                            ModifiedOn = terminal.ModifiedOn,
                            ModifiedById = terminal.ModifiedById,
                        };


            if (!string.IsNullOrEmpty(model.Code))
            {
                query = query.Where(s => s.Code.Contains(model.Code));
            }

            if (!string.IsNullOrEmpty(model.Name))
            {
                query = query.Where(s => s.Name.Contains(model.Name));
            }

            if (model.Type != null)
            {
                query = query.Where(s => s.Type == model.Type);
            }

            if (!string.IsNullOrEmpty(model.FactoryId))
            {
                query = query.Where(s => s.FactoryId.Contains(model.FactoryId));
            }

            if (!string.IsNullOrEmpty(model.ParentId))
            {
                query = query.Where(s => s.ParentId.Contains(model.ParentId));
            }

            if (model.IsNotUsed)
            {
                query = query.Where(s => string.IsNullOrEmpty(s.ParentId));
            }

            result.TerminalList = query.OrderBy(s => s.Code).Skip(model.IDisplayStart).Take(model.IDisplayLength).ToList();

            result.TotalRecords = query.Count();
            return result;
        }


        public Terminal GetTerminal(S09004ViewModel model)
        {
            return context.Terminal.Where(s => s.Id == model.Id).FirstOrDefault();
        }

        public string GetTerminalCodeById(string terminalId)
        {
            var query = context.Terminal.Where(s => s.Id == terminalId).Select(x => x.Code).FirstOrDefault();
            return query;
        }

        public TerminalModel GetTerminalWithFactory(S09004ViewModel model)
        {
            var query = from terminal in context.Terminal

                        join location in context.Location on terminal.ParentId equals location.Id into locationJoin
                        from locationLResult in locationJoin.DefaultIfEmpty()

                        join factory1 in context.Factory on locationLResult.FactotyId equals factory1.Id into factory1Join
                        from factory1LResult in factory1Join.DefaultIfEmpty()

                        join container in context.Container on terminal.ParentId equals container.Id into containerJoin
                        from containerLResult in containerJoin.DefaultIfEmpty()

                        join locationOfContainer in context.Location on containerLResult.LocationId equals locationOfContainer.Id into locationOfContainerJoin
                        from locationOfContainerJoinLResult in locationOfContainerJoin.DefaultIfEmpty()

                        join factory2 in context.Factory on locationOfContainerJoinLResult.FactotyId equals factory2.Id into factory2Join
                        from factory2LResult in factory2Join.DefaultIfEmpty()

                        where 1 == 1 && terminal.Id == model.Id
                        select new TerminalModel()
                        {
                            Id = terminal.Id,
                            Code = terminal.Code,
                            Name = terminal.Name,
                            ParentId = !string.IsNullOrEmpty(locationLResult.Id) ? locationLResult.Id : (!string.IsNullOrEmpty(containerLResult.Id) ? containerLResult.Id : null),
                            ParentName = !string.IsNullOrEmpty(locationLResult.Id) ? locationLResult.Name : (!string.IsNullOrEmpty(containerLResult.Id) ? containerLResult.Code : null),
                            FactoryId = !string.IsNullOrEmpty(factory1LResult.Id) ? factory1LResult.Id : (!string.IsNullOrEmpty(factory2LResult.Id) ? factory2LResult.Id : null),
                            FactoryName = !string.IsNullOrEmpty(factory1LResult.Id) ? factory1LResult.Name : (!string.IsNullOrEmpty(factory2LResult.Id) ? factory2LResult.Name : null),
                            Type = terminal.Type,
                            LoginId = terminal.LoginId,
                            Password = terminal.Password,
                            CreatedOn = terminal.CreatedOn,
                            CreatedById = terminal.CreatedById,
                            ModifiedOn = terminal.ModifiedOn,
                            ModifiedById = terminal.ModifiedById,
                        };

            return query.FirstOrDefault();
        }

        public List<string> GetListSensorIdbyLotId(string lotId)
        {
            var query = from lotcontainer in context.LotContainer
                        join lotcontainerterminal in context.LotContainerTerminal
                        on lotcontainer.Id equals lotcontainerterminal.LotContainerId
                        join container in context.Container
                        on lotcontainer.ContainerId equals container.Id
                        join terminal in context.Terminal on lotcontainerterminal.TerminalId equals terminal.Id
                        where (lotcontainer.LotId == lotId && container.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU)
                        select (terminal.Id);

            return query.Select(c => c.ToString()).ToList();
        }

        public List<string> GetListSensorIdUsingByLotId(string lotId)
        {
            var query = from lotcontainer in context.LotContainer
                        join lotcontainerterminal in context.LotContainerTerminal
                        on lotcontainer.Id equals lotcontainerterminal.LotContainerId
                        join terminal in context.Terminal on lotcontainerterminal.TerminalId equals terminal.Id
                        where (lotcontainer.LotId == lotId && terminal.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU && lotcontainerterminal.EndDate == null)
                        select (terminal.Id);

            return query.Select(c => c.ToString()).ToList();
        }

        public List<S03001ViewModel.Terminal> GetListSensorByLotId(string lotId)
        {
            var query = from lotcontainer in context.LotContainer
                        join container in context.Container on lotcontainer.ContainerId equals container.Id
                        join lotcontainerterminal in context.LotContainerTerminal on lotcontainer.Id equals lotcontainerterminal.LotContainerId
                        join terminal in context.Terminal on lotcontainerterminal.TerminalId equals terminal.Id
                        where lotcontainer.LotId == lotId && container.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU
                        select new S03001ViewModel.Terminal
                        {
                            Id = terminal.Id,
                            Code = terminal.Code,
                            Name = terminal.Name,
                            StartDate = lotcontainerterminal.StartDate,
                            EndDate = lotcontainerterminal.EndDate,
                            IdOld = terminal.Id,
                            DeleteFlg = terminal.DeleteFlg,
                            Type = terminal.Type,
                        };


            var result = query.ToList();
            return result;
        }

        public string Add(Terminal entity)
        {
            var item = context.Terminal.Where(s => s.Code == entity.Code && s.Type == entity.Type && s.DeleteFlg != true).FirstOrDefault();
            if (item != null)
            {
                var model = new S09004ViewModel();
                model.localizer = localizer;
                model.Type = entity.Type;
                return string.Format(localizer["msg_item_exist"], localizer["terminal_code"], entity.Code, localizer["type"], model.TypeLabel);
            }

            entity.Id = Utils.GenerateId(context);
            context.Terminal.Add(entity);
            context.SaveChanges();

            return string.Empty;
        }

        public string Update(Terminal entity)
        {
            var item = context.Terminal.Where(s => s.Id == entity.Id).FirstOrDefault();
            if (item != null)
            {
                // 重複のデータをチェック
                var duplicateItem = context.Terminal.Where(s => s.Id != item.Id && s.Code == entity.Code && s.Type == entity.Type && s.DeleteFlg != true).FirstOrDefault();
                if (duplicateItem != null)
                {
                    var model = new S09004ViewModel();
                    model.localizer = localizer;
                    model.Type = entity.Type;
                    return string.Format(localizer["msg_item_exist"], localizer["terminal_code"], entity.Code, localizer["type"], model.TypeLabel);
                }

                // 仕込号数で使われているかチェック
                if (item.Type != entity.Type)
                {
                    var sql = from lotContainerTerminal in context.LotContainerTerminal
                              join lotContainer in context.LotContainer on lotContainerTerminal.LotContainerId equals lotContainer.Id
                              join lot in context.Lot on lotContainer.LotId equals lot.Id
                              where lotContainerTerminal.TerminalId == item.Id && lotContainerTerminal.EndDate == null
                              select new
                              {
                                  Id = lotContainerTerminal.Id,
                                  TerminalId = lotContainerTerminal.TerminalId,
                                  LotCode = lot.Code,
                              };

                    var usingItem = sql.FirstOrDefault();

                    if (usingItem != null)
                    {
                        return string.Format(localizer["msg_terminal_using_cannot_change_type"], usingItem.LotCode);
                    }
                }

                if (item.Type != entity.Type || (item.Type == entity.Type && item.Type != semigura.Commons.Properties.TERMINAL_TYPE_SEIGIKU))
                {
                    item.ParentId = entity.ParentId;
                }

                item.Code = entity.Code;
                item.Name = entity.Name;
                item.Type = entity.Type;
                item.LoginId = entity.LoginId;
                item.Password = entity.Password;
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

        public string UpdateParenID(string id, string parentId, DateTime? endDate, string lotcontainerId)
        {
            var typeContainer = context.Terminal.Where(s => s.Id == id).Select(x => x.Type).FirstOrDefault();
            if (typeContainer != semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU)
            {
                return string.Empty;
            }
            var item = context.Terminal.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                if (endDate != null)
                {
                    if (lotcontainerId != string.Empty)
                    {
                        var FinishDate = context.LotContainerTerminal.Where(s => s.LotContainerId == lotcontainerId && s.TerminalId == id).Select(y => y.EndDate).FirstOrDefault();
                        if (FinishDate == null)
                        {
                            item.ParentId = null;
                        }
                    }
                    else
                    {
                        item.ParentId = null;
                    }
                }
                else
                {
                    item.ParentId = parentId;
                }


                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }

        public string RemoveParenID(string id)
        {
            var item = context.Terminal.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                item.ParentId = null;
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
            var item = context.Terminal.Where(s => s.Id == id).FirstOrDefault();
            if (item != null)
            {
                var sql = from lotContainerTerminal in context.LotContainerTerminal
                          join lotContainer in context.LotContainer on lotContainerTerminal.LotContainerId equals lotContainer.Id
                          join lot in context.Lot on lotContainer.LotId equals lot.Id
                          where lotContainerTerminal.TerminalId == item.Id
                          select new
                          {
                              LotContainerTerminalEndDate = lotContainerTerminal.EndDate,
                              LotCode = lot.Code,
                          };

                var usingItem = sql.Where(s => s.LotContainerTerminalEndDate == null).FirstOrDefault();

                if (usingItem != null)
                {
                    return string.Format(localizer["msg_terminal_using"], usingItem.LotCode);
                }

                var linkingFlg = sql.Count() > 0;
                if (linkingFlg)
                {
                    item.DeleteFlg = true;
                }
                else
                {
                    context.Terminal.Remove(item);
                }

                context.SaveChanges();
            }
            else
            {
                return localizer["C01004"];
            }

            return string.Empty;
        }
        public void DeleteByContainerId(string containerId)
        {
            var query = context.Terminal.Where(s => s.ParentId == containerId && s.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU);
            if (query.Any())
            {
                foreach (var item in query)
                {
                    item.ParentId = null;
                }
                context.SaveChanges();
            }
        }

        public string DeleteParentIdBySensorId(string lotcontainerId, string sensorId)
        {
            var item = context.Terminal.Where(s => s.Id == sensorId).FirstOrDefault();
            var endDate = context.LotContainerTerminal.Where(s => s.LotContainerId == lotcontainerId && s.TerminalId == sensorId).Select(y => y.EndDate).FirstOrDefault();
            if (item != null && endDate == null)
            {
                item.ParentId = null;
                context.SaveChanges();
            }

            return string.Empty;
        }

        public Terminal GetTerminalOfTank(string terminalCode)
        {
            return context.Terminal.Where(s => s.Code == terminalCode && (s.Type == semigura.Commons.Properties.TERMINAL_TYPE_TANK || s.Type == semigura.Commons.Properties.TERMINAL_TYPE_CAMERA)).FirstOrDefault();
        }

        public List<string> getListTerminalID(string containerID)
        {
            var query = context.Terminal.Where(s => s.ParentId == containerID).Select(y => y.Id).ToList();
            return query;
        }
    }
}