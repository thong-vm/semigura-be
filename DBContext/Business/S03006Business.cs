using Microsoft.Extensions.Localization;
using semigura.DBContext.Entities;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S03006Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S03006Business(DBEntities db,
            ILogger<S03006Business> logger,
            IStringLocalizer<S03006Business> localizer) : base(db, logger, localizer)
        {
            this.localizer = localizer;
        }


        public S03006ViewModel GetListContainer(S03006ViewModel model)
        {
            try
            {
                var dao = new ContainerDao(context, localizer);

                return dao.GetListContainer(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<CapacityRefer> GetListCapacityRefer(string containerId)
        {
            try
            {
                var dao = new CapacityReferDao(context);

                return dao.GetListCapacityRefer(containerId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

    }
}