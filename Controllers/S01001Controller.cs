using Logger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using semigura.Commons;
using semigura.DAL;
using semigura.DBContext.Business;
using semigura.DBContext.Models;
using semigura.Hubs;
using semigura.Models;
using semigura.ProcessHandles;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static Controllers.UsersController;

//******************************************************************************
// システムＩＤ　：SBM
// システム名称　：酒造管理システム
// モジュールＩＤ：S01001
// モジュール名称：ログイン画面
// 機能概要　　　：システムにログイン
//
// 改訂履歴　　　：2021/10/01 新規 sync-partners)Thao
// 　　　　　　　　2021/10/01 修正 sync-partners)Thao  DB定義書(1.0.3版)対応
//
//******************************************************************************

namespace semigura.Controllers
{
    [CExceptionFilter]
    public class S01001Controller : Controller
    {
        private readonly IStringLocalizer localizer;
        private readonly MyConfig _cfg;
        private readonly IConfiguration _configuration;
        private readonly UserRepository _repository;
        private readonly S01001Business business;
        private readonly LogFormater _logger;
        private readonly IChatHubRepository _hubRepo;

        public S01001Controller(
            IOptions<MyConfig> cfg,
            IConfiguration configuration,
            UserRepository repo,
            S01001Business business,
            ILogger<S01001Controller> logger,
            IChatHubRepository _hubRepo,
            IStringLocalizer<S01001Controller> localizer
            )
        {
            this.localizer = localizer;
            _cfg = cfg.Value;
            ENVIRONMENT_ROOT_PATH = _cfg.ContentRootPath;
            _configuration = configuration;
            _repository = repo;
            this.business = business;
            this._logger = new LogFormater(logger);
            this._hubRepo = _hubRepo;
        }

        //private string ENVIRONMENT_ROOT_PATH = HostingEnvironment.MapPath(@"~/");
        private string ENVIRONMENT_ROOT_PATH;
        private string ENVIRONMENT_CONTENT_IMAGE_PATH = $"Content{System.IO.Path.DirectorySeparatorChar}dev{System.IO.Path.DirectorySeparatorChar}asset{System.IO.Path.DirectorySeparatorChar}image";
        private string ENVIRONMENT_IMAGE_SLIDE_PATH = $"Content{System.IO.Path.DirectorySeparatorChar}dev{System.IO.Path.DirectorySeparatorChar}asset{System.IO.Path.DirectorySeparatorChar}slide";

        public ActionResult Index(S01001ViewModel model)
        {
            ModelState.Clear();
            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("Index");
        }

        private (int, string, string) Login(UserDTO user)
        {
            string msg = "Unauthorized!";
            string role = "";
            int statusCodes = StatusCodes.Status401Unauthorized;
            do
            {

                if (user.Account == "admin" && user.Password == "123456")
                {
                    role = "admin";
                    msg = "Default account!";
                    statusCodes = StatusCodes.Status200OK;
                    break;
                }

                var found = _repository.GetAll()
                                .Where(u => u.Account != null && u.Account == user.Account)
                                .Select(u => new { Role = u.Role, HashedPassword = u.HashedPassword })
                                .FirstOrDefault()
                                ;

                if (found == null)
                {
                    msg = "Invalid account!";
                    statusCodes = StatusCodes.Status404NotFound;
                    break;
                }

                if (BCrypt.Net.BCrypt.Verify(user.Password, found.HashedPassword))
                {
                    role = found.Role;
                    statusCodes = StatusCodes.Status200OK;
                }
            } while (false);

            return (statusCodes, msg, role);
        }

        private async Task SetCookieAuthen(string account, string role, string data = "")
        {
            var identity = new ClaimsIdentity(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, account));
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            identity.AddClaim(new Claim(ClaimTypes.UserData, data));
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                });
        }

        private async Task ResetCookieAuthen()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private string GenerateToken(string account, string role)
        {
            var jwtConfig = _configuration.GetSection("Jwt").Get<JwtConfig>();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                    {
                            new Claim("Id", Guid.NewGuid().ToString()),
                            new Claim(ClaimsIdentity.DefaultNameClaimType, account),
                            new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        }),
                Expires = DateTime.UtcNow.AddMonths(3),
                Issuer = jwtConfig.Issuer,
                Audience = jwtConfig.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key)),
                    SecurityAlgorithms.HmacSha512Signature
                    )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync(S01001ViewModel model)
        {
            try
            {
                if (model != null && ModelState.IsValid)
                {
                    //var passwordEncrypt = AESCryption.Encrypt(model.Password, semigura.Commons.Properties.AES_IV, semigura.Commons.Properties.AES_Key);

                    //var userObj = business.GetUser(model.Username, passwordEncrypt);
                    //if (userObj != null)
                    string userName = model.Username;
                    string password = model.Password;
                    (var statusCode, string msg, string role) = Login(new UserDTO { Account = userName, Password = password });
                    if (statusCode == StatusCodes.Status200OK)
                    {
                        //FormsAuthentication.SetAuthCookie(userObj.Username, model.IsRememberMe);
                        var userInfo = business.UserInfos().Where(u => u.Username == userName).First();
                        var userInfoModel = new UserInfoModel
                        {
                            Id = userInfo.Id,
                            Username = userName,
                            IsSysAdmin = role == "admin"
                        };
                        var authorInfoList = business.GetAuthorInfo(userInfoModel);

                        //var jwtToken = TokenManager.GenerateToken(userObj);

                        //HttpContext.Session.SetString(Commons.Properties.USER_INFO, JsonSerializer.Serialize(userObj));
                        //HttpContext.Session.SetString(Commons.Properties.AUTHOR_INFO, JsonSerializer.Serialize(authorInfoList));

                        //HttpContext.Response.Cookies.Append(Commons.Properties.JWT_TOKEN, jwtToken,
                        //    new CookieOptions()
                        //    {
                        //        Expires = DateTime.UtcNow.AddDays(1),
                        //        Path = "/"
                        //    }
                        //    );
                        await SetCookieAuthen(userName, role, JsonSerializer.Serialize(new UserDataModel { authorInfoModelList = authorInfoList, userInfoModel = userInfoModel }));
                        var jwtToken = GenerateToken(userName, role);

                        if (model.LoginByAjax)
                        {
                            return Json(new { status = true, token = jwtToken });
                        }
                        else
                        {
                            var returnUrl = model.ReturnUrl;
                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }

                            return RedirectToAction("Index", "S01002");
                        }
                    }

                    if (model.LoginByAjax)
                    {
                        return Json(new { status = false, message = localizer["msg_login_fail"].ToString() });
                    }
                    else
                    {
                        ModelState.AddModelError("", localizer["msg_login_fail"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //ログを出力
                _logger.Error(ex.Message, ex);

                if (model != null && model.LoginByAjax)
                {
                    return Json(new { status = false, message = localizer["C01003"].ToString() }/*, JsonRequestBehavior.AllowGet*/);
                }
            }

            return View("Index");
        }

        public async Task<ActionResult> Logout()
        {
            await ResetCookieAuthen();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Regist()
        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Regist(S01001ViewModel userInfo)
        {


            return View(userInfo);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return RedirectToAction("Index", "S01005");
        }

        public ActionResult Error()
        {
            return View();
        }

        public JsonResult ChangeLanguage(string lang)
        {
            //LanguageUtil.SetLanguage(lang);
            Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) }
            );

            return Json(new { status = true }/*, JsonRequestBehavior.AllowGet*/);
        }

        public ActionResult GetImageSlide(string filename, int? width, int? height)
        {
            try
            {
                if (!string.IsNullOrEmpty(filename))
                {
                    var path = System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_IMAGE_SLIDE_PATH, filename);
                    if (width != null || height != null)
                    {
                        var orgImg = Image.FromFile(path);
                        if (width == null) width = orgImg.Width;
                        if (height == null) height = orgImg.Height;
                        var size = new Size((int)width, (int)height);
                        var newImg = ResizeImage(orgImg, size);
                        byte[] imageData = null;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            imageData = ms.ToArray();
                        }

                        return base.File(imageData, "image/jpeg");
                    }
                    return base.File(path, "image/jpeg");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return new EmptyResult();
        }

        [ResponseCache(Duration = 60)]
        //[OutputCache(Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult GetImageFromNAS(string id, int? width, int? height)
        {
            var thumbnailFilePath = System.IO.Path.Combine(ENVIRONMENT_ROOT_PATH, ENVIRONMENT_CONTENT_IMAGE_PATH, "thumbnail.jpg");
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var media = business.GetImage(id);
                    if (media != null)
                    {
                        // ロカールから画像を取得
                        var filePath = System.IO.Path.Combine(DeviceConnectionManagement.ENVIRONMENT_ROOT_PATH, DeviceConnectionManagement.ENVIRONMENT_IMAGE_PATH, media.LotContainerId, media.Path);
                        if (System.IO.File.Exists(filePath))
                        {
                            return base.File(System.IO.File.ReadAllBytes(filePath), "image/jpeg");
                        }

                        // NASから画像を取得
                        var dirPath = DeviceConnectionManagement.NAS_ROOT_DIR_PATH + "/" + media.LotContainerId;
                        var filename = media.Path;
                        var imageData = NASUtil.GetViewer(dirPath, filename);
                        if (imageData != null && imageData.Length != 0)
                        {
                            //byte[] imageData = null;
                            //if (width != null || height != null)
                            //{
                            //    if (width == null) width = orgImg.Width;
                            //    if (height == null) height = orgImg.Height;
                            //    var size = new Size((int)width, (int)height);
                            //    var newImg = ResizeImage(orgImg, size);
                            //    using (MemoryStream ms = new MemoryStream())
                            //    {
                            //        newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //        imageData = ms.ToArray();
                            //    }
                            //    return base.File(imageData, "image/jpeg");
                            //}
                            //using (MemoryStream ms = new MemoryStream())
                            //{
                            //    orgImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            //    imageData = ms.ToArray();
                            //}

                            return base.File(imageData, "image/jpeg");
                        }




                        //var path = Path.Combine(DeviceConnectionManagement.ENVIRONMENT_ROOT_PATH, DeviceConnectionManagement.ENVIRONMENT_IMAGE_PATH, media.LotContainerId, media.Path);
                        //if (System.IO.File.Exists(path))
                        //{
                        //    if (width != null || height != null)
                        //    {
                        //        var orgImg = Image.FromFile(path);
                        //        if (width == null) width = orgImg.Width;
                        //        if (height == null) height = orgImg.Height;
                        //        var size = new Size((int)width, (int)height);
                        //        var newImg = ResizeImage(orgImg, size);
                        //        byte[] imageData = null;
                        //        using (MemoryStream ms = new MemoryStream())
                        //        {
                        //            newImg.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        //            imageData = ms.ToArray();
                        //        }

                        //        return base.File(imageData, "image/jpeg");
                        //    }
                        //    return base.File(path, "image/jpeg");
                        //}
                    }
                }
            }
            catch (System.IO.FileNotFoundException ex)
            {
                _logger.Debug(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.Message, ex);
            }

            return base.File(thumbnailFilePath, "image/jpeg");
        }

        private static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }

        public ActionResult DeleteJunkData()
        {
            try
            {
                _ = Task.Run(() => Process.DeleteJunkData().ConfigureAwait(false));

                return Json(new { status = true, message = "ジャンクデータのクリーンアップ処理が実行中です。 時間がかかる場合があります。" }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = ex.Message }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public ActionResult GetDeviceConfig()
        {
            try
            {
                var data = new JObject();
                data.Add(new JProperty("Interval", semigura.Commons.Properties.TANK_SENSOR_INTERVAL));

                return Json(new { status = true, data = data }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = ex.Message }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        //public ActionResult SyncOndotoriData()
        //{
        //    try
        //    {

        //        _ = Task.Run(() => Process.ReadSensorData().ConfigureAwait(false));

        //        return Json(new { status = true, message = "同期処理中です。 時間がかかる場合があります。" }/*, JsonRequestBehavior.AllowGet*/);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex.Message, ex);

        //        return Json(new { status = false, message = ex.Message }/*, JsonRequestBehavior.AllowGet*/);
        //    }
        //}

        public ActionResult CountDevices()
        {
            try
            {
                var listDeviceCode = new List<string>();
                foreach (KeyValuePair<string, ConnectedClient> obj in DeviceConnectionManagement.Clients)
                {
                    listDeviceCode.Add(obj.Key + "(viewers:" + obj.Value.StreamViewerClientIds.Count + ")");
                }

                return Json(new { status = true, numbers = DeviceConnectionManagement.Clients.Count, devices = string.Join(",", listDeviceCode) }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = ex.Message }/*, JsonRequestBehavior.AllowGet*/);
            }
        }

        public ActionResult CountClients()
        {
            try
            {

                return Json(new { status = true, clients = _hubRepo.ClientsCount }/*, JsonRequestBehavior.AllowGet*/);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                return Json(new { status = false, message = ex.Message }/*, JsonRequestBehavior.AllowGet*/);
            }
        }
    }
}