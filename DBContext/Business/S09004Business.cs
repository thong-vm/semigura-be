using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S09004Business : BaseBusiness
    {
        private readonly IStringLocalizer<S09001Business> localizer;

        public S09004Business(DBEntities db,
            ILogger<S09004Business> logger,
            IStringLocalizer<S09001Business> localizer) : base(db, logger, localizer)
        {
            this.localizer = localizer;
        }

        public S09004ViewModel GetListTerminal(S09004ViewModel model)
        {
            try
            {
                var dao = new TerminalDao(context, localizer);

                var res = dao.GetListTerminal(model);
                res.TerminalList.ForEach(ter => ter.localizer = localizer);
                return res;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        //public S09004ViewModel? GetTerminal(S09004ViewModel model)
        //{
        //    try
        //    {
        //        var dao = new TerminalDao(dbContext, localizer);

        //        var terminal = dao.GetTerminal(model);

        //        if (terminal != null)
        //        {
        //            return terminal.Map(new S09004ViewModel());
        //        }

        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex.Message);
        //        throw;
        //    }
        //}
        public S09004ViewModel? GetTerminalWithFactory(S09004ViewModel model)
        {
            try
            {
                var dao = new TerminalDao(context, localizer);

                var terminalModel = dao.GetTerminalWithFactory(model);

                if (terminalModel != null)
                {
                    return terminalModel.Map(new S09004ViewModel());
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public string Save(S09004ViewModel model, UserInfoModel UserInfoModel)
        {
            string message = string.Empty;
            try
            {
                var dao = new TerminalDao(context, localizer);
                var lotContainerTerminalDao = new LotContainerTerminalDao(context, localizer);
                lotContainerTerminalDao.UpdatebyTerminalChange(model.Id, model.ContainerId, UserInfoModel.Id);
                if (!string.IsNullOrEmpty(model.Id))
                {
                    string userID = UserInfoModel.Id;
                    Terminal entity = model.Map<Terminal>();
                    entity.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                    entity.ModifiedById = UserInfoModel.Id;

                    message = dao.Update(entity);

                }
                else
                {
                    Terminal entity = model.Map<Terminal>();
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

        public string Delete(S09004ViewModel model)
        {
            string message = string.Empty;
            try
            {
                var dao = new TerminalDao(context, localizer);
                message = dao.Delete(model.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return message;
        }

        public List<Factory> GetListFactory()
        {
            try
            {
                var dao = new FactoryDao(context);
                return dao.GetListFactory();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<Container> GetListContainerTank(string factoryId)
        {
            try
            {
                var dao = new ContainerDao(context, localizer);
                return dao.GetListContainer(factoryId, Commons.Properties.CONTAINER_TYPE_TANK);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<semigura.DBContext.Entities.Location> GetListLocation(string factoryId)
        {
            try
            {
                var dao = new LocationDao(context);
                return dao.GetListLocation(factoryId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}