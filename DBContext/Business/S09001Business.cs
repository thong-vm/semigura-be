using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;
using semigura.Models;

namespace semigura.DBContext.Business
{
    public class S09001Business : BaseBusiness
    {
        IStringLocalizer localizer;
        public S09001Business(DBEntities db, ILogger<S09001Business> logger,
            IStringLocalizer<S09001Business> localizer
            ) : base(db, logger, localizer)
        {
            this.localizer = localizer;
        }


        public S09001ViewModel GetMaterial(S09001ViewModel model)
        {
            try
            {
                var dao = new MaterialDao(context, localizer);
                return dao.GetMaterial(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }


        public S09001ViewModel GetListMaterial(S09001ViewModel model)
        {
            try
            {
                var dao = new MaterialDao(context, localizer);

                var res = dao.GetLisMaterial(model);
                res.MaterialList.ForEach(m => m.localizer = localizer);
                return res;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public string Save(S09001ViewModel model, UserInfoModel UserInfoModel)
        {
            string message = string.Empty;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var materialDao = new MaterialDao(context, localizer);
                    var materialStandValDao = new MaterialStandValDao(context, localizer);
                    var meterialId = model.Id;
                    if (!string.IsNullOrEmpty(meterialId))
                    {
                        Material entity = model.Map<Material>();
                        entity.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                        entity.ModifiedById = UserInfoModel.Id;

                        message = materialDao.Update(entity);

                        if (string.IsNullOrEmpty(model.Id))
                        {
                            throw new Exception("modle.Id empty");
                        }
                        materialStandValDao.Delete(model.Id);
                    }
                    else
                    {
                        Material entity = model.Map<Material>();
                        entity.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                        entity.CreatedById = UserInfoModel.Id;

                        message = materialDao.Add(entity);
                        meterialId = entity.Id;
                    }

                    if (string.IsNullOrEmpty(message))
                    {
                        if (model.MaterialStandValList != null && model.MaterialStandValList.Any())
                        {
                            foreach (var item in model.MaterialStandValList)
                            {
                                MaterialStandVal entity = item.Map<MaterialStandVal>();
                                entity.MaterialId = meterialId;
                                entity.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                                entity.CreatedById = UserInfoModel.Id;

                                message = materialStandValDao.Add(entity);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(message))
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    _logger.Error(ex.Message);
                    throw;
                }
            }

            return message;
        }

        public string Delete(S09001ViewModel model)
        {
            string message;
            try
            {
                var materialDao = new MaterialDao(context, localizer);
                var materialStandValDao = new MaterialStandValDao(context, localizer);

                if (string.IsNullOrEmpty(model.Id))
                {
                    throw new Exception("modle.Id empty");
                }

                message = materialDao.Delete(model.Id);
                message = materialStandValDao.Delete(model.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }

            return message;
        }
    }
}