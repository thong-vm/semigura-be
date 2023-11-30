using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using semigura.DBContext.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace semigura.Commons
{
    public class TokenManager
    {
        private static string Secret = "ZTNlYTBlMDYtYWRjZi00ZTNlLTg5NDktYThlMzEzM2UyMzUz";//Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));

        private class JWT
        {
            public string Issuer;
            public string Audience;
        }
        static JWT _jwt = new JWT
        {
            Issuer = "semigura.com",
            Audience = "semigura.com"
        };

        public static string GenerateToken(UserInfoModel userInfo)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[] {
                    new Claim(type: ClaimTypes.Name, value: userInfo.Username),
                    new Claim(type: ClaimTypes.Role, value: "userInfo.Role"),
                    new Claim(type: Properties.USER_INFO, value: JsonConvert.SerializeObject(userInfo)),
                }),
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                Expires = DateTime.UtcNow.AddMinutes(Properties.TOKEN_TIMEOUT),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);

            return handler.WriteToken(token);
        }

        public static string GenerateToken(User user)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[] {
                    new Claim(type: ClaimTypes.Name, value: user.Account),
                    new Claim(type: ClaimTypes.Role, value: user.Role),
                }),
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                Expires = DateTime.UtcNow.AddMinutes(Properties.TOKEN_TIMEOUT),
                SigningCredentials = new SigningCredentials(securityKey, algorithm: SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);

            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadJwtToken(token);

                if (jwtToken == null) return null;

                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = false,
                    ValidateIssuer = true,
                    ValidIssuer = _jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);

                return principal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        public static UserInfoModel ValidateToken(string token)
        {
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
            {
                return null;
            }

            ClaimsIdentity identity;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }

            //Claim claim = identity.FindFirst(type: ClaimTypes.Name);
            Claim claimUserInfo = identity.FindFirst(type: Properties.USER_INFO);

            var userInfo = JsonConvert.DeserializeObject<UserInfoModel>(claimUserInfo.Value);

            return userInfo;
        }

        //private RefreshToken GenerateRefreshToken(string ipAddress, string userId)
        //{
        //    using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
        //    {
        //        var randomBytes = new byte[64];
        //        rngCryptoServiceProvider.GetBytes(randomBytes);
        //        return new RefreshToken
        //        {
        //            Token = Convert.ToBase64String(randomBytes),
        //            ExpiryOn = DateTime.UtcNow.AddDays(jwtBearerTokenSettings.RefreshTokenExpiryInDays),
        //            CreatedOn = DateTime.UtcNow,
        //            CreatedByIp = ipAddress,
        //            UserId = userId
        //        };
        //    }
        //}
    }
}