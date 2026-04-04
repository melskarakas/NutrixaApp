using ORM.Business;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ORM.Services.IServices;
using ORM.Services.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using ORM.Models.Models;
using ORM.Business.Classes;
using Newtonsoft.Json.Linq;

namespace ORM.Services.Services
{
    public class UserService : IUserService
    {
        private readonly string _secret;

        public UserService(string secret)
        {
            _secret = secret ?? string.Empty;
        }

        public async Task<users?> AddUser(users user)
        {
            try
            {
                if (user == null || string.IsNullOrWhiteSpace(user.email))
                    return null;

                // Check by email or username
                if (!string.IsNullOrWhiteSpace(user.email) && await ExistsByEmail(user.email))
                    return null;


                user.id = Guid.NewGuid();
                user.password = ORM.Shared.Base.GenerateDoubleMD5Password(user.password);

                var res = new GenericBusiness<users>().Add(user);
                return res ? user : null;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[AddUser] Kullanýcý eklenirken bir  hata oluþtu.", ex);
                string json = JsonHelper.SerializeObject(user);
                NutrixaLogger.LogInfo($"[AddUser] Hata oluþan kullanýcý verisi: {json}");
                return null;
            }

        }


        public async Task<bool> ExistsByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return false;
                string where = $" AND {nameof(users.email)} = '{email}'";
                var existing = new GenericBusiness<users>().GetAllByCustomQuery(where).FirstOrDefault();
                return existing != null;
            }
            catch (Exception ex)
            {

                NutrixaLogger.LogError($"[ExistsByEmail]  E-posta adresine göre kullanýcý aranýrken bir  hata oluþtu. E-posta Adresi: {email} ", ex);
                return false;
            }

        }


        public async Task<users?> GetById(object id)
        {
            try
            {
                return new GenericBusiness<users>().GetById(id);

            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[UserService-GetById] Kullanýcý getirirlirken bir  hata oluþtu.Kullanýcý ID: {id}", ex);
                return new users();
            }
        }

        public async Task<List<users>> GetAll()
        {
            try
            {
                return new GenericBusiness<users>().GetAll().ToList();

            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[UserService-GetAll] Kullanýcý Listesi getirirlirken bir  hata oluþtu.", ex);
                return new List<users>();
            }
        }

        public async Task<bool> Update(users user)
        {
            try
            {
                return new GenericBusiness<users>().Update(user);
            }
            catch (Exception ex)
            {

                NutrixaLogger.LogError("[UserService-Update] Kullanýcý güncellenirken bir  hata oluþtu.", ex);
                string json = JsonHelper.SerializeObject(user);
                NutrixaLogger.LogInfo($"[UserService-Update] Hata oluþan kullanýcý verisi: {json}");
                return false;
            }
        }

        public async Task<bool> Delete(object id)
        {
            try
            {
                return new GenericBusiness<users>().Delete(id);

            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[UserService-Delete] Kullanýcý silinirken bir  hata oluþtu. Kullanýcý ID: {id}", ex);
                return false;
            }
        }

        public async Task<bool> DeleteUser(users userObj)
        {
            try
            {
                userObj.is_active = false;
                userObj.is_deleted = true;
                userObj.modified_date = DateTime.Now;
                return new GenericBusiness<users>().Update(userObj);
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[DeleteUser] Kullanýcý aktiflik durumu deðiþtirilirken bir  hata oluþtu.", ex);
                string json = JsonHelper.SerializeObject(userObj);
                NutrixaLogger.LogInfo($"[DeleteUser] Hata oluþan kullanýcý verisi: {json}");
                return false;
            }

        }

        public async Task<users> GetUser(object id)
        {
            try
            {
                return new GenericBusiness<users>().GetById(id);
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[GetUser] Kullanýcý getirirlirken bir  hata oluþtu.Kullanýcý ID:{id}", ex);
                return new users();
            }
        }

        public async Task<users?> AddAndUpdateUser(users userObj)
        {
            try
            {
                bool isSuccess = false;
                if (userObj.id != Guid.Empty)
                {
                    var obj = new GenericBusiness<users>().GetById(userObj.id);
                    if (obj != null)
                    {
                        isSuccess = new GenericBusiness<users>().Update(userObj);
                    }
                }
                else
                {
                    string where = $" AND {nameof(users.email)} = '{userObj.email}'";
                    var user = new GenericBusiness<users>().GetAllByCustomQuery(where).FirstOrDefault();
                    if (user == null)
                    {
                        userObj.id = Guid.NewGuid();
                        userObj.password = ORM.Shared.Base.GenerateDoubleMD5Password(userObj.password);
                        isSuccess = new GenericBusiness<users>().Add(userObj);
                    }
                    else
                    {
                        return null;
                    }
                }

                return isSuccess ? userObj : null;
            }
            catch (Exception ex)
            {

                NutrixaLogger.LogError("[AddAndUpdateUser] Kullanýcý eklenirken/güncellenirken bir  hata oluþtu.", ex);
                string json = JsonHelper.SerializeObject(userObj);
                NutrixaLogger.LogInfo($"[AddAndUpdateUser] Hata oluþan kullanýcý verisi: {json}");
                return new users();
            }

        }

        public async Task<AuthenticationResult?> AuthenticateByEmail(string email, string password, bool isPasswordMd5)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return null;

                string identifier = email.Trim();

                string md5Password = "";
                if (!isPasswordMd5)
                {
                    md5Password = ORM.Shared.Base.GenerateDoubleMD5Password(password);
                }
                else
                {
                    md5Password = password;
                }

                // Allow login by either email or user_name
                string where = $" AND ( {nameof(users.email)} = '{identifier}' OR {nameof(users.email)} = '{identifier}' ) AND {nameof(users.password)} = '{md5Password}' AND {nameof(users.is_active)} = true AND {nameof(users.is_deleted)} = false";
                var user = new GenericBusiness<users>().GetAllByCustomQuery(where).FirstOrDefault();

                if (user == null) return null;

                // generate token and include is_registration
                var token = await isAuthenticate(user);

                return new AuthenticationResult { Token = token, IsRegistration = true };
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError($"[AuthenticateByEmail] Email e göre kimlik doðrulama oluþturulurken bir  hata oluþtu. E-posta adresi: {email}, Þifre: {password}, MD5'li mi: {isPasswordMd5}", ex);
                return new AuthenticationResult { Token = "", IsRegistration = false };
            }

        }

        private async Task<string> isAuthenticate(users user)
        {
            try
            {
                string w = $" AND {nameof(authtoken.user_id)}='{user.id}' AND {nameof(authtoken.expire_date)}>'{DateTime.Now}'";
                authtoken _token = new GenericBusiness<authtoken>().GetAllByCustomQuery(w).FirstOrDefault();
                string _jwtToken = "";
                if (_token == null)
                {
                    authtoken _authtoken = new authtoken
                    {
                        id = Guid.NewGuid(),
                        token = await generateJwtToken(user),
                        user_id = user.id,
                        created_date = DateTime.Now,
                        expire_date = DateTime.Now.AddDays(1)
                    };
                    bool res = new GenericBusiness<authtoken>().Add(_authtoken);
                    _jwtToken = _authtoken.token;
                }
                else
                {
                    _token.expire_date = _token.expire_date.AddHours(3);
                    bool res = new GenericBusiness<authtoken>().Update(_token);
                    _jwtToken = _token.token;
                }
                return _jwtToken;
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[isAuthenticate] Kimlik doðrulama sýrasýnda bir  hata oluþtu.", ex);
                string json = JsonHelper.SerializeObject(user);
                NutrixaLogger.LogInfo($"[isAuthenticate] Hata oluþan kullanýcý verisi: {json}");
                return string.Empty;
            }

        }

        private async Task<string> generateJwtToken(users user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                if (string.IsNullOrEmpty(_secret) || Encoding.UTF8.GetBytes(_secret).Length < 32)
                {
                    throw new InvalidOperationException("JWT secret is not configured or too short. Set AppSettings:Secret with at least 32 bytes length (e.g. 32+ characters).");
                }
                var key = Encoding.UTF8.GetBytes(_secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("id", user.id.ToString()) }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                NutrixaLogger.LogError("[generateJwtToken] Token oluþturulurken bir  hata oluþtu.", ex);
                string json = JsonHelper.SerializeObject(user);
                NutrixaLogger.LogInfo($"[generateJwtToken] Hata oluþan kullanýcý verisi: {json}");
                return string.Empty;
            }
        }
    }
}
