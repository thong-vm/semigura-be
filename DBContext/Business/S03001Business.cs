using Microsoft.Extensions.Localization;
using semigura.Commons;
using semigura.DBContext.Entities;
using semigura.DBContext.Models;
using semigura.DBContext.Repositories;
using semigura.Models;
using semigura.ProcessHandles;
using semigura.Repositories;

namespace semigura.DBContext.Business
{
    public class S03001Business : BaseBusiness
    {
        private readonly AuthCookieRepository authCookie;
        IStringLocalizer localizer;

        public S03001Business(DBEntities db,
            ILogger<S03001Business> logger,
            AuthCookieRepository authCookie,
            IStringLocalizer<S03001Business> localizer
            ) : base(db, logger, localizer)
        {
            this.authCookie = authCookie;
            this.localizer = localizer;
        }

        public List<Factory> GetListFactory() => context.Factory.ToList();

        public List<Material> GetListRice() => context.Material.ToList();

        public List<Container> GetListTank() => context.Container.Where(s => s.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK).OrderBy(s => s.Code).ToList();

        public List<Terminal> GetListSensor() => context.Terminal.OrderBy(s => s.Code).ToList();
        public bool GetInfoLotByLotId(string lotId,
                                        ref string lotCode,
                                        ref string factoryId,
                                        ref Nullable<System.DateTime> startDate,
                                        ref Nullable<System.DateTime> endDate,
                                        ref string riceId,
                                        ref Nullable<int> kubunId,
                                        ref string semaibuaiId,
                                        ref Nullable<decimal> riceRatio)
        {
            try
            {
                var lotDao = new LotDao(context, localizer);
                return lotDao.GetInfoLotByLotId(lotId, ref lotCode, ref factoryId, ref startDate, ref endDate, ref riceId,
                                                                ref kubunId, ref semaibuaiId, ref riceRatio);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public bool GetSegikuMinMaxByLotId(string lotId,
                                    ref Nullable<decimal> minTempSegiku,
                                    ref Nullable<decimal> maxTempSegiku)
        {
            try
            {
                var lotcontainerDao = new LotContainerDao(context, localizer);
                return lotcontainerDao.GetSegikuMinMaxByLotId(lotId, ref minTempSegiku, ref maxTempSegiku);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
        public List<S03001ViewModel.Tank> GetListTankIdByLotId(string lotId)
        {
            try
            {
                var lotDao = new LotDao(context, localizer);
                return lotDao.GetListTankIdByLotId(lotId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<S03001ViewModel.Terminal> GetListSensorIdByLotId(string lotId)
        {
            try
            {
                var terminalDao = new TerminalDao(context, localizer);
                return terminalDao.GetListSensorByLotId(lotId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public S03001ViewModel.Rice GetInfoRiceByRiceId(string riceId)
        {
            try
            {
                var materialdao = new MaterialDao(context, localizer);
                return materialdao.GetInfoRiceByRiceId(riceId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<S03001ViewModel.Semaibuai> GetInfoRiceByRiceName(string riceName)
        {
            try
            {
                var materialdao = new MaterialDao(context, localizer);
                return materialdao.GetInfoRiceByRiceName(riceName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public S03001ViewModel GetListLotContainer(S03001ViewModel model)
        {
            try
            {
                S03001ViewModel result = new S03001ViewModel();
                var dao = new LotContainerDao(context, localizer);

                result = dao.GetLisLotContainerForLot(model);
                result.LotContainerList?.ForEach(s =>
                {
                    s.KubunName = GetNameKubunById(s.Kubun);
                });
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public string GetNameKubunById(Nullable<int> kubunId)
        {

            if (kubunId == semigura.Commons.Properties.MATERIAL_CLASSIFICATION_EXTRA_SPECIAL)
            {
                return localizer["extra_special"];
            }
            else if (kubunId == semigura.Commons.Properties.MATERIAL_CLASSIFICATION_SPECIAL_CLASS)
            {
                return localizer["special_class"];
            }
            else if (kubunId == semigura.Commons.Properties.MATERIAL_CLASSIFICATION_FIRST_CLASS)
            {
                return localizer["first_class"];
            }
            else if (kubunId == semigura.Commons.Properties.MATERIAL_CLASSIFICATION_SECOND_CLASS)
            {
                return localizer["second_class"];
            }
            else if (kubunId == semigura.Commons.Properties.MATERIAL_CLASSIFICATION_THIRD_CLASS)
            {
                return localizer["third_class"];
            }
            else
            {
                return string.Empty;
            }
        }
        public S03001ViewModel.Tank LoadInfoByTankID(string tankId)
        {
            try
            {
                var dao = new LotContainerDao(context, localizer);

                return dao.LoadInfoByTankID(tankId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public string FinishDateLot(string lotId)
        {
            string message = string.Empty;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var lotDao = new LotDao(context, localizer);
                    message = lotDao.FinishDateLot(lotId);

                    List<LotContainerModel> lstLotContainerID = new List<LotContainerModel>();
                    var lotcontainerDao = new LotContainerDao(context, localizer);
                    lstLotContainerID = lotcontainerDao.GetListLotContainerIdbyLotId(lotId);
                    var lotcontainerterminalDao = new LotContainerTerminalDao(context, localizer);
                    var terminalDao = new TerminalDao(context, localizer);
                    List<string> lstTerminalId = new List<string>();
                    if (lstLotContainerID != null)
                    {
                        lstLotContainerID.ForEach(s =>
                        {
                            lotcontainerDao.FinishDateLotContainer(s.Id);
                            if (s.Type == semigura.Commons.Properties.CONTAINER_TYPE_TANK)
                            {
                                lotcontainerterminalDao.FinishDateLotContainerByLotContainerId(s.Id);
                            }
                            else if (s.Type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU)
                            {

                                lotcontainerterminalDao.FinishDateLotContainerGetListTerminalId(s.Id, ref lstTerminalId);
                                lstTerminalId.ForEach(terminalId =>
                                {
                                    terminalDao.RemoveParenID(terminalId);
                                });

                            }
                        });
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);

                    transaction.Rollback();
                    throw;
                }
            }

            return message;
        }

        public string FinishDateLotContainer(string lotContainerId, Nullable<int> type)
        {
            string message = string.Empty;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var lotcontainerDao = new LotContainerDao(context, localizer);
                    if (type == semigura.Commons.Properties.CONTAINER_TYPE_TANK)
                    {
                        message = lotcontainerDao.FinishDateLotContainer(lotContainerId);

                        var lotcontainerterminalDao = new LotContainerTerminalDao(context, localizer);
                        lotcontainerterminalDao.FinishDateLotContainerByLotContainerId(lotContainerId);
                    }
                    else if (type == semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU)
                    {
                        var teminalId = string.Empty;
                        var lotcontainerterminalDao = new LotContainerTerminalDao(context, localizer);
                        message = lotcontainerterminalDao.FinishDateLotContainer(lotContainerId, ref teminalId);
                        if (message != string.Empty)
                        {
                            return message;
                        }
                        message = lotcontainerDao.FinishDateLotContainerSeigiku(lotContainerId);
                        if (message != string.Empty)
                        {
                            return message;
                        }
                        var terminalDao = new TerminalDao(context, localizer);
                        message = terminalDao.RemoveParenID(teminalId);
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error(ex.Message);
                    throw;
                }

                return message;
            }

        }

        public List<string> CheckContainerIDthisLotID(string lotId)
        {
            try
            {
                var daoLotContainer = new LotContainerDao(context, localizer);
                List<string> lstContainerIdThis = new List<string>();
                lstContainerIdThis = daoLotContainer.GetListContainerIdUsingByLotId(lotId);
                return lstContainerIdThis;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<string> CheckSensorIDthisLotID(string lotId)
        {
            try
            {
                var terminaldao = new TerminalDao(context, localizer);
                List<string> lstSensorIdThis = new List<string>();
                lstSensorIdThis = terminaldao.GetListSensorIdUsingByLotId(lotId);
                return lstSensorIdThis;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<semigura.DBContext.Entities.Location> GetListLocationByFactoryId(string factoryId)
        {
            try
            {
                var locationDao = new LocationDao(context);
                var result = locationDao.GetListLocation(factoryId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public List<S03001ViewModel.Semaibuai> GetListSeimaibuai()
        {
            List<S03001ViewModel.Semaibuai> semaibuai = new List<S03001ViewModel.Semaibuai>()
            {
                new S03001ViewModel.Semaibuai
                {
                    Id = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_1,
                    Name = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_1
                },
                new S03001ViewModel.Semaibuai
                {
                    Id = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_2,
                    Name = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_2
                },
                new S03001ViewModel.Semaibuai
                {
                    Id = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_3,
                    Name = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_3
                },
                new S03001ViewModel.Semaibuai
                {
                    Id = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_4,
                    Name = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_4
                },
                new S03001ViewModel.Semaibuai
                {
                    Id = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_5,
                    Name = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_5
                },
                new S03001ViewModel.Semaibuai
                {
                    Id = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_6,
                    Name = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_6
                },
                new S03001ViewModel.Semaibuai
                {
                    Id = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_7,
                    Name = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_7
                },
                new S03001ViewModel.Semaibuai
                {
                    Id = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_8,
                    Name = semigura.Commons.Properties.MATERIALSTANDVAL_RICE_POLISHING_RATIO_NAME_8
                },
                new S03001ViewModel.Semaibuai
                {
                    Id = localizer["other"],
                    Name = localizer["other"]
                }
            };

            return semaibuai;
        }

        public List<S03001ViewModel.Kubun> GetListKubun()
        {
            List<S03001ViewModel.Kubun> lstKubun = new List<S03001ViewModel.Kubun>()
            {
                new S03001ViewModel.Kubun
                {
                    Id = semigura.Commons.Properties.MATERIAL_CLASSIFICATION_EXTRA_SPECIAL,
                    Name = localizer["extra_special"]
                },
                new S03001ViewModel.Kubun
                {
                    Id = semigura.Commons.Properties.MATERIAL_CLASSIFICATION_SPECIAL_CLASS ,
                    Name = localizer["special_class"]
                },
                new S03001ViewModel.Kubun
                {
                    Id = semigura.Commons.Properties.MATERIAL_CLASSIFICATION_FIRST_CLASS ,
                    Name = localizer["first_class"]
                },
                new S03001ViewModel.Kubun
                {
                    Id = semigura.Commons.Properties.MATERIAL_CLASSIFICATION_SECOND_CLASS ,
                    Name = localizer["second_class"]
                },
                new S03001ViewModel.Kubun
                {
                    Id = semigura.Commons.Properties.MATERIAL_CLASSIFICATION_THIRD_CLASS  ,
                    Name = localizer["third_class"]
                }
            };

            return lstKubun;
        }

        public string Save(S03001ViewModel model, UserInfoModel UserInfoModel)
        {
            string message = string.Empty;
            try
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.EndDate < model.StartDate)
                        {
                            return localizer["msg_chk_startenddate"];
                        }
                        var daoLot = new LotDao(context, localizer);
                        var daoLotContainer = new LotContainerDao(context, localizer);
                        string userID = UserInfoModel.Id;

                        //Update Lot
                        if (!string.IsNullOrEmpty(model.LotId))
                        {
                            //Update EndDate of LotContainer
                            bool flgEndDateEmpty = false;
                            if (model.TankListId != null)
                            {
                                model.TankListId.ForEach(s =>
                                {
                                    if (s.EndDate == null)
                                    {
                                        flgEndDateEmpty = true;
                                        return;
                                    }
                                });
                            }

                            if (model.SensorListId != null)
                            {
                                model.SensorListId.ForEach(s =>
                                {
                                    if (s.EndDate == null)
                                    {
                                        flgEndDateEmpty = true;
                                        return;
                                    }
                                });
                            }
                            if (flgEndDateEmpty)
                            {
                                model.EndDate = null;
                            }

                            //1. Update Lot DB
                            Lot entityLot = model.Map<Lot>();
                            entityLot.Id = model.LotId;
                            entityLot.Code = model.LotCode;
                            entityLot.FactotyId = model.FactoryID;
                            entityLot.MaterialId = model.RiceId;
                            entityLot.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                            entityLot.ModifiedById = UserInfoModel.Id;
                            entityLot.MaterialClassification = model.KubunId;
                            entityLot.RicePolishingRatioName = model.SemaibuaiId;
                            entityLot.RicePolishingRatio = model.RicePolishingRatio;

                            message = daoLot.Update(entityLot);
                            if (message != "")
                            {
                                return message;
                            }

                            LotContainer entityLotContainer = model.Map<LotContainer>();
                            entityLotContainer.LocationId = model.LocationID;
                            entityLotContainer.ModifiedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                            entityLotContainer.ModifiedById = UserInfoModel.Id;

                            //2. Update Tank
                            //  2.1 Add Tank
                            //  2.2 Delete Tank
                            //  2.3 Update Tank
                            if (model.TankListId != null)
                            {
                                List<string> lstContainerIdOld = new List<string>();
                                List<string> lstContainerIdNew = new List<string>();
                                List<string> lstContainerIdAdd = new List<string>();
                                List<string> lstContainerIdDelete = new List<string>();
                                List<string> lstContainerIdModify = new List<string>();
                                List<S03001ViewModel.Tank> lstContainerIdUpdate = new List<S03001ViewModel.Tank>();

                                lstContainerIdOld = daoLotContainer.GetListContainerIdbyLotId(entityLotContainer.LotId);
                                model.TankListId.ForEach(s =>
                                {
                                    if (s.IdOld != null)
                                    {
                                        lstContainerIdUpdate.Add(s);
                                        lstContainerIdModify.Add(s.IdOld);
                                    }
                                    else
                                    {
                                        lstContainerIdNew.Add(s.Id);
                                    }
                                });

                                List<string> lstLotContainerID = new List<string>();
                                lstContainerIdAdd = lstContainerIdNew.Except(lstContainerIdOld).ToList();
                                lstContainerIdDelete = lstContainerIdOld.Except(lstContainerIdNew).ToList();
                                lstContainerIdDelete = lstContainerIdDelete.Except(lstContainerIdModify).ToList();

                                //  2.1 Add Tank
                                //      2.1.1 Add Container in LotContainer DB
                                //      2.1.2 Add Container in LotContainerTerminal DB
                                if (lstContainerIdAdd.Count > 0)
                                {
                                    List<S03001ViewModel.Tank> lstTankIdAdd = new List<S03001ViewModel.Tank>();
                                    lstContainerIdAdd.ForEach(id =>
                                    {
                                        var result = model.TankListId.Where(s => s.Id == id).FirstOrDefault();
                                        lstTankIdAdd.Add(result);
                                    });
                                    //      2.1.1 Add Container in LotContainer DB
                                    message = daoLotContainer.AddContainerByLotID(entityLotContainer, lstTankIdAdd, ref lstLotContainerID);
                                    //      2.1.2 Add Container in LotContainerTerminal DB
                                    //lstLotContainerID.ForEach(s =>
                                    //{
                                    //    AddLotContainerTerminalForTank(lstContainerIdAdd, s);
                                    //});
                                    for (var i = 0; i < lstLotContainerID.Count; i++)
                                    {
                                        AddLotContainerTerminalForTank(lstContainerIdAdd[i], lstLotContainerID[i], authCookie.UserInfo);
                                    }
                                }
                                //  2.2 Delete Tank
                                //      2.2.1 Delete LotContainer DB
                                //      2.2.2 Delete LotContaineTerminal DB
                                //      2.2.3 Delete Notification DB
                                if (lstContainerIdDelete.Count > 0)
                                {
                                    //      2.2.1 Delete LotContainer DB
                                    lstLotContainerID.Clear();
                                    lstContainerIdDelete.ForEach(s =>
                                    {
                                        string lotcontainerID = string.Empty;
                                        message = daoLotContainer.DeleteByContainerId(model.LotId, s, ref lotcontainerID);
                                        lstLotContainerID.Add(lotcontainerID);
                                    });
                                    //      2.2.2 Delete LotContaineTerminal DB
                                    //      2.2.3 Delete Notification DB
                                    lstLotContainerID.ForEach(s =>
                                    {
                                        ModifyListLotContainerTerminal(lstContainerIdDelete, s, authCookie.UserInfo);
                                        DeleteByLotContainerId(s);
                                    });


                                }
                                //  2.3 Update Tank
                                //      Update LotContainer DB
                                if (lstContainerIdUpdate.Count > 0)
                                {
                                    var lotcontainerterminalDao = new LotContainerTerminalDao(context, localizer);
                                    List<string> lstLotContainerId = new List<string>();
                                    List<string> lstContainerIdChange = new List<string>();
                                    lstContainerIdUpdate.ForEach(s =>
                                    {
                                        daoLotContainer.UpdatebyLotIdAndContainerId(entityLotContainer, s, ref lstLotContainerId, ref lstContainerIdChange);
                                        lotcontainerterminalDao.UpdateByContainerId(entityLotContainer, s);
                                    });

                                    for (int i = 0; i < lstLotContainerId.Count; i++)
                                    {
                                        lotcontainerterminalDao.UpdateByContainerIdChange(lstLotContainerId[i], lstContainerIdChange[i]);
                                    }
                                }
                            }
                            else
                            {
                                //  2.2 Delete Tank
                                List<string> lstContainerIdOld = new List<string>();
                                lstContainerIdOld = daoLotContainer.GetListContainerIdbyLotId(entityLotContainer.LotId);
                                if (lstContainerIdOld.Count > 0)
                                {
                                    List<string> lstLotContainerID = new List<string>();
                                    lstContainerIdOld.ForEach(s =>
                                    {
                                        string lotcontainerID = string.Empty;
                                        message = daoLotContainer.DeleteByContainerId(model.LotId, s, ref lotcontainerID);
                                        lstLotContainerID.Add(lotcontainerID);
                                    });

                                    lstLotContainerID.ForEach(s =>
                                    {
                                        ModifyListLotContainerTerminal(lstContainerIdOld, s, authCookie.UserInfo);
                                        DeleteByLotContainerId(s);
                                    });
                                }

                            }

                            //3. Update Sensor
                            //  3.1 Check Container Exist
                            //  3.2 Delete Sensor
                            //  3.3 Delete ParenId
                            //  3.4 Update Sensor
                            //  3.5 Add Sensor
                            if (model.SensorListId != null)
                            {
                                Boolean flgEndLotContainer = true;
                                var terminalDao = new TerminalDao(context, localizer);
                                List<string> lstSensorIdOld = new List<string>();
                                List<string> lstSensorIdAdd = new List<string>();
                                List<string> lstSensorIdDelete = new List<string>();
                                List<string> lstDeleteParentId = new List<string>();
                                List<string> lstSensorIdModify = new List<string>();
                                List<string> lstSensorIdUsed = new List<string>();
                                List<S03001ViewModel.Terminal> lstSensorUpdate = new List<S03001ViewModel.Terminal>();

                                //  3.1 Check Container Exist
                                string LotContainerID = string.Empty;
                                string ContainerID = string.Empty;
                                daoLotContainer.GetlstConIdAndlstLotConIdbyLotId(model.LotId, ref ContainerID, ref LotContainerID);
                                if (ContainerID == string.Empty)
                                {
                                    //create Container
                                    var containerdao = new ContainerDao(context, localizer);
                                    Container entityContainer = new Container();
                                    entityContainer.Code = model.LotId;
                                    entityContainer.Type = semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU;
                                    entityContainer.LocationId = model.LocationID;

                                    containerdao.AddContainerTermiald(entityContainer, ref ContainerID);

                                    //add ContainerID and LotID into LotContainer
                                    entityLotContainer.TempMin = model.MinTempSeigiku;
                                    entityLotContainer.TempMax = model.MaxTempSeigiku;
                                    message = daoLotContainer.AddforTerminal(entityLotContainer, ContainerID, ref LotContainerID);
                                }

                                lstSensorIdOld = terminalDao.GetListSensorIdbyLotId(entityLotContainer.LotId);
                                model.SensorListId.ForEach(s =>
                                {
                                    if (s.IdOld != null)
                                    {
                                        if (s.IdOld != s.Id)
                                        {
                                            lstDeleteParentId.Add(s.IdOld);
                                        }
                                        lstSensorUpdate.Add(s);
                                        lstSensorIdUsed.Add(s.IdOld);
                                    }
                                    else
                                    {
                                        lstSensorIdAdd.Add(s.Id);
                                    }
                                    if (s.EndDate == null)
                                    {
                                        flgEndLotContainer = false;
                                    }

                                });

                                lstSensorIdDelete = lstSensorIdOld.Except(lstSensorIdUsed).ToList();

                                //  3.2 Delete Sensor
                                if (lstSensorIdDelete.Count > 0)
                                {
                                    var lotcontainerterminaldao = new LotContainerTerminalDao(context, localizer);
                                    var sensordataDao = new SensorDataDao(context, localizer);
                                    lstSensorIdDelete.ForEach(s =>
                                    {
                                        terminalDao.DeleteParentIdBySensorId(LotContainerID, s);
                                        lotcontainerterminaldao.Delete(LotContainerID, s);
                                        sensordataDao.DeleteByLotContainerIdAndTerminalId(LotContainerID, s);
                                    });
                                }

                                // 3.3 Delete ParentId
                                lstDeleteParentId.ForEach(s =>
                                {
                                    terminalDao.DeleteParentIdBySensorId(LotContainerID, s);
                                });

                                //  3.4 Update Sensor
                                if (lstSensorUpdate.Count > 0)
                                {
                                    entityLotContainer.TempMin = model.MinTempSeigiku;
                                    entityLotContainer.TempMax = model.MaxTempSeigiku;
                                    message = daoLotContainer.UpdateById(entityLotContainer, LotContainerID);

                                    List<string> lstSensorIdUpdate = new List<string>();
                                    var terminaldao = new TerminalDao(context, localizer);
                                    var lotcontainerterminalDao = new LotContainerTerminalDao(context, localizer);
                                    lstSensorUpdate.ForEach(s =>
                                    {
                                        lstSensorIdUpdate.Add(s.Id);
                                        terminaldao.UpdateParenID(s.Id, ContainerID, s.EndDate, LotContainerID);
                                        lotcontainerterminalDao.UpdateByLotContainerIDAndSensorID(LotContainerID, s, userID);
                                    });


                                }

                                //  3.5 Add Sensor
                                if (lstSensorIdAdd.Count > 0)
                                {
                                    var terminaldao = new TerminalDao(context, localizer);
                                    var lotcontainerterminaldao = new LotContainerTerminalDao(context, localizer);
                                    lstSensorIdAdd.ForEach(s =>
                                    {
                                        var startDate = model.SensorListId.Where(x => x.Id == s).Select(e => e.StartDate).FirstOrDefault();
                                        terminaldao.UpdateParenID(s, ContainerID, null, string.Empty);
                                        lotcontainerterminaldao.AddCheckDate(LotContainerID, s, userID, startDate);
                                    });
                                }
                                if (flgEndLotContainer == true)
                                {
                                    daoLotContainer.FinishDateLotContainer(LotContainerID);
                                }
                                else
                                {
                                    daoLotContainer.RollBackDateLotContainer(LotContainerID);
                                }
                            }
                            else
                            {
                                //  3.2 Delete Sensor
                                var terminalDao = new TerminalDao(context, localizer);
                                List<string> lstSensorIdOld = new List<string>();
                                lstSensorIdOld = terminalDao.GetListSensorIdbyLotId(entityLotContainer.LotId);
                                if (lstSensorIdOld.Count > 0)
                                {
                                    string LotContainerID = string.Empty;
                                    string ContainerID = string.Empty;
                                    daoLotContainer.GetlstConIdAndlstLotConIdbyLotId(model.LotId, ref ContainerID, ref LotContainerID);
                                    var lotcontainerterminaldao = new LotContainerTerminalDao(context, localizer);
                                    lstSensorIdOld.ForEach(s =>
                                    {
                                        message = terminalDao.DeleteParentIdBySensorId(LotContainerID, s);
                                        message = lotcontainerterminaldao.Delete(LotContainerID, s);
                                        //sensordataDao.DeleteByLotContainerIdAndTerminalId(LotContainerID, s);
                                    });
                                    //var containerDao = new ContainerDao(dbContext);

                                    //containerDao.DeleteContainerId(ContainerID);

                                    //DeleteByLotContainerId(LotContainerID);

                                }
                                message = daoLotContainer.UpdateLocation(entityLotContainer.LotId, entityLotContainer.LocationId);
                                if (message != string.Empty)
                                {
                                    return message;
                                }

                            }

                        }
                        //Add Lot
                        else
                        {
                            Lot entityLot = new Lot();
                            entityLot.Code = model.LotCode;
                            entityLot.FactotyId = model.FactoryID;
                            entityLot.MaterialId = model.RiceId;
                            entityLot.StartDate = model.StartDate;
                            entityLot.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                            entityLot.CreatedById = UserInfoModel.Id;
                            entityLot.MaterialClassification = model.KubunId;
                            entityLot.RicePolishingRatioName = model.SemaibuaiId;
                            entityLot.RicePolishingRatio = model.RicePolishingRatio;

                            string lotID = string.Empty;
                            message = daoLot.Add(entityLot, ref lotID);
                            if (message != "")
                            {
                                return message;
                            }

                            LotContainer entityLotContainer = new LotContainer();
                            entityLotContainer.LotId = lotID;
                            entityLotContainer.LocationId = model.LocationID;
                            entityLotContainer.StartDate = model.StartDate;
                            entityLotContainer.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Utils.Tokyo_Standard_Time);
                            entityLotContainer.CreatedById = UserInfoModel.Id;


                            if (model.TankListId != null)
                            {
                                List<string> lstLotContainerID = new List<string>();
                                List<string> lstContainerID = new List<string>();
                                model.TankListId.ForEach(s =>
                                {
                                    lstContainerID.Add(s.Id);
                                });
                                message = daoLotContainer.Add(entityLotContainer, model.TankListId, ref lstLotContainerID);

                                //lstLotContainerID.ForEach(s =>
                                //{
                                //    AddLotContainerTerminalForTank(lstContainerID, s);
                                //});
                                for (var i = 0; i < lstLotContainerID.Count; i++)
                                {
                                    AddLotContainerTerminalForTank(lstContainerID[i], lstLotContainerID[i], authCookie.UserInfo);
                                }
                            }

                            if (model.SensorListId != null)
                            {
                                string ContainerID = string.Empty;
                                string LotContainerID = string.Empty;
                                daoLotContainer.GetlstConIdAndlstLotConIdbyLotId(model.LotId, ref ContainerID, ref LotContainerID);
                                if (ContainerID == string.Empty)
                                {
                                    //create Container
                                    var containerdao = new ContainerDao(context, localizer);
                                    Container entityContainer = new Container();
                                    entityContainer.Code = lotID;
                                    entityContainer.Type = semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU;
                                    entityContainer.LocationId = model.LocationID;
                                    containerdao.AddContainerTermiald(entityContainer, ref ContainerID);

                                    //add ContainerID and LotID into LotContainer
                                    entityLotContainer.TempMin = model.MinTempSeigiku;
                                    entityLotContainer.TempMax = model.MaxTempSeigiku;
                                    message = daoLotContainer.AddforTerminal(entityLotContainer, ContainerID, ref LotContainerID);
                                }

                                //Change Parent and add LotContainerTeminal
                                var terminaldao = new TerminalDao(context, localizer);
                                var lotcontainerterminaldao = new LotContainerTerminalDao(context, localizer);
                                model.SensorListId.ForEach(s =>
                                {
                                    terminaldao.UpdateParenID(s.Id, ContainerID, null, string.Empty);
                                    lotcontainerterminaldao.AddByLot(LotContainerID, s.Id, userID, s.StartDate);
                                });

                            }
                            else
                            {
                                string ContainerID = string.Empty;
                                string LotContainerID = string.Empty;
                                daoLotContainer.GetlstConIdAndlstLotConIdbyLotId(model.LotId, ref ContainerID, ref LotContainerID);
                                if (ContainerID == string.Empty)
                                {
                                    //create Container
                                    var containerdao = new ContainerDao(context, localizer);
                                    Container entityContainer = new Container();
                                    entityContainer.Code = lotID;
                                    entityContainer.Type = semigura.Commons.Properties.CONTAINER_TYPE_SEIGIKU;
                                    entityContainer.LocationId = model.LocationID;
                                    containerdao.AddContainerTermiald(entityContainer, ref ContainerID);

                                    //add ContainerID and LotID into LotContainer
                                    entityLotContainer.TempMin = model.MinTempSeigiku;
                                    entityLotContainer.TempMax = model.MaxTempSeigiku;
                                    message = daoLotContainer.AddforTerminal(entityLotContainer, ContainerID, ref LotContainerID);
                                }
                            }

                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        _logger.Error(ex.Message);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }

            return message;
        }

        /* Function: Delete
         * 1. Delete LotContainer
         * 2. Delete LotContainerTerminal
         * 3. Delete Notification
         * 4. Reset Terminal
         * 5. Delete Container for Sensor
         * */
        public string Delete(S03001ViewModel model)
        {
            string message = string.Empty;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var lotdao = new LotDao(context, localizer);
                    message = lotdao.Delete(model.LotId);

                    if (message != string.Empty)
                    {
                        return message;
                    }

                    List<string> lotcontainerID = new List<string>();
                    List<string> containerID = new List<string>();
                    var lotcontainer = new LotContainerDao(context, localizer);
                    message = lotcontainer.Delete(model.LotId, ref lotcontainerID, ref containerID);
                    if (message != string.Empty)
                    {
                        return message;
                    }

                    lotcontainerID.ForEach(s =>
                    {
                        DeleteLotContainerTerminalByLotContainerId(s);
                        DeleteByLotContainerId(s);
                    });

                    var terminalDao = new TerminalDao(context, localizer);
                    containerID.ForEach(s =>
                    {
                        terminalDao.DeleteByContainerId(s);
                    });

                    var containerDao = new ContainerDao(context, localizer);
                    containerID.ForEach(s =>
                    {
                        containerDao.DeleteContainerId(s);
                    });

                    transaction.Commit();

                    // NASのイメージを削除
                    lotcontainerID.ForEach(s =>
                    {
                        NASUtil.DeleteFolder(DeviceConnectionManagement.NAS_ROOT_DIR_PATH, s);
                    });
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

        public List<string> CheckListTankInUse(List<string> lstTank)
        {
            List<string> Result = new List<string>();
            try
            {
                var lotcontainer = new LotContainerDao(context, localizer);
                lstTank.ForEach(s =>
                {
                    if (lotcontainer.CheckListTankInUse(s))
                    {
                        Result.Add(s);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return Result;
        }

        public List<string> CheckListSensorInUse(List<string> lstSensor)
        {
            List<string> Result = new List<string>();
            try
            {
                var lotcontainerterminal = new LotContainerTerminalDao(context, localizer);
                lstSensor.ForEach(s =>
                {
                    if (lotcontainerterminal.CheckListSensorInUse(s))
                    {
                        Result.Add(s);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            return Result;
        }

        public void ModifyListLotContainerTerminal(List<string> lstContainerID, string LotContainerID, UserInfoModel UserInfoModel)
        {
            List<string> Result = new List<string>();
            string userID = UserInfoModel.Id;
            try
            {
                var terminaldao = new TerminalDao(context, localizer);
                var lotcontainerterminaldao = new LotContainerTerminalDao(context, localizer);
                List<string> listSensor = new List<string>();

                lstContainerID.ForEach(s =>
                {

                    listSensor = terminaldao.getListTerminalID(s);
                    listSensor.ForEach(x =>
                    {

                        lotcontainerterminaldao.Delete(LotContainerID, x);

                    });
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public void AddLotContainerTerminalForTank(string ContainerID, string LotContainerID, UserInfoModel UserInfoModel)
        {
            List<string> Result = new List<string>();
            string userID = UserInfoModel.Id;
            try
            {
                var terminaldao = new TerminalDao(context, localizer);
                var lotcontainerterminaldao = new LotContainerTerminalDao(context, localizer);
                List<string> listSensor = new List<string>();

                listSensor = terminaldao.getListTerminalID(ContainerID);
                listSensor.ForEach(s =>
                {
                    lotcontainerterminaldao.Add(LotContainerID, s, userID);
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public void DeleteLotContainerTerminalByLotContainerId(string lotcontainerId)
        {
            try
            {
                var lotcontainerterminaldao = new LotContainerTerminalDao(context, localizer);
                lotcontainerterminaldao.DeleteByLotContainerId(lotcontainerId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public void DeleteByLotContainerId(string lotcontainerId)
        {
            try
            {
                var notificationDao = new NotificationDao(context, localizer);
                var sensordataDao = new SensorDataDao(context, localizer);
                var dataentryDao = new DataEntryDao(context, localizer);
                var mediaDao = new MediaDao(context, localizer);

                notificationDao.DeleteByLotContainerId(lotcontainerId);
                sensordataDao.DeleteByLotContainerId(lotcontainerId);
                dataentryDao.DeleteByLotContainerId(lotcontainerId);
                mediaDao.DeleteByLotContainerId(lotcontainerId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public bool CheckFinishDateLotId(string lotId)
        {
            try
            {
                var lotcontainerDao = new LotContainerDao(context, localizer);
                return lotcontainerDao.AllChildDone(lotId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public string GetLocationIdforSegiku(string lotId)
        {
            try
            {
                var lotcontainerDao = new LotContainerDao(context, localizer);
                return lotcontainerDao.GetLocationIdforSegiku(lotId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public LotContainer getLotContainerByContainerId(string ContainerId)
        {
            try
            {
                var lotcontainerDao = new LotContainerDao(context, localizer);
                return lotcontainerDao.getLotContainerByContainerId(ContainerId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public LotContainer getLotContainerByTerminalId(string TerminalId)
        {
            try
            {
                var lotcontainerDao = new LotContainerDao(context, localizer);
                return lotcontainerDao.getLotContainerByTerminalId(TerminalId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public String getLotCodeByLotContainerId(string lotContainerId)
        {
            try
            {
                var lotcontainerDao = new LotContainerDao(context, localizer);
                return lotcontainerDao.getLotCodeByLotContainerId(lotContainerId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }

        public String getLCTidByLCTidTid(string lotContainerId, string terminalId)
        {
            try
            {
                var lotcontainerterminaldao = new LotContainerTerminalDao(context, localizer);
                return lotcontainerterminaldao.getLCTidByLCTidTid(lotContainerId, terminalId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}