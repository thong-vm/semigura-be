using Microsoft.Extensions.Localization;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace semigura.DBContext.Business
{
    public class S01001Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S01001Business([NotNull] DBEntities db,
            ILogger<S01001Business> logger,
            IStringLocalizer<S01001Business> localizer)
            : base(db, logger, localizer)
        {
            this.localizer = localizer;
        }

        public UserInfoModel GetUser(string username, string password)
        {
            try
            {
                var dao = new UserInfoDao(context);
                return dao.GetUser(username, password);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<AuthorInfoModel> GetAuthorInfo(UserInfoModel model)
        {
            try
            {
                var dao = new UserInfoDao(context);

                return dao.GetAuthorInfo(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public IQueryable<UserInfo> UserInfos()
        {
            return context.UserInfo;
        }
    }
}