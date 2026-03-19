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
            if (user == null || string.IsNullOrWhiteSpace(user.email))
                return null;

            // Check by email or username
            if (!string.IsNullOrWhiteSpace(user.email) && await ExistsByEmail(user.email))
                return null;

            if (!string.IsNullOrWhiteSpace(user.user_name) && await ExistsByUserName(user.user_name))
                return null;

            user.id = Guid.NewGuid();
            user.password = ORM.Shared.Base.GenerateDoubleMD5Password(user.password);
            user.created_date = DateTime.Now;
            user.modified_date = DateTime.Now;
            user.is_active = true;
            user.is_deleted = false;

            var res = new GenericBusiness<users>().Add(user);
            return res ? user : null;
        }

        public async Task<user_inf?> AddUserInfo(user_inf info)
        {
            if (info == null || info.user_id == Guid.Empty) return null;

            info.id = Guid.NewGuid();
            info.created_date = DateTime.Now;
            info.modified_date = DateTime.Now;
            info.is_active = true;
            info.is_deleted = false;

            var res = new GenericBusiness<user_inf>().Add(info);
            return res ? info : null;
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            string where = $" AND {nameof(users.email)} = '{email}'";
            var existing = new GenericBusiness<users>().GetAllByCustomQuery(where).FirstOrDefault();
            return existing != null;
        }

        public async Task<bool> ExistsByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;
            string where = $" AND {nameof(users.user_name)} = '{userName}'";
            var existing = new GenericBusiness<users>().GetAllByCustomQuery(where).FirstOrDefault();
            return existing != null;
        }

        public async Task<users?> GetById(object id)
        {
            return new GenericBusiness<users>().GetById(id);
        }

        public async Task<List<users>> GetAll()
        {
            return new GenericBusiness<users>().GetAll().ToList();
        }

        public async Task<bool> Update(users user)
        {
            return new GenericBusiness<users>().Update(user);
        }

        public async Task<bool> Delete(object id)
        {
            return new GenericBusiness<users>().Delete(id);
        }

        public async Task<bool> DeleteUser(users userObj)
        {
            userObj.is_active = false;
            userObj.is_deleted = true;
            userObj.modified_date = DateTime.Now;
            return new GenericBusiness<users>().Update(userObj);
        }

        public async Task<users> GetUser(object id)
        {
            return new GenericBusiness<users>().GetById(id);
        }

        public async Task<users?> AddAndUpdateUser(users userObj)
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
                string where = $" AND {nameof(users.user_name)} = '{userObj.user_name}'";
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

        public async Task<AuthenticationResult?> AuthenticateByEmail(string email, string password, bool isPasswordMd5)
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
            string where = $" AND ( {nameof(users.email)} = '{identifier}' OR {nameof(users.user_name)} = '{identifier}' ) AND {nameof(users.password)} = '{md5Password}' AND {nameof(users.is_active)} = true AND {nameof(users.is_deleted)} = false";
            var user = new GenericBusiness<users>().GetAllByCustomQuery(where).FirstOrDefault();

            if (user == null) return null;

            // generate token and include is_registration
            var token = await isAuthenticate(user);

            return new AuthenticationResult { Token = token, IsRegistration = user.is_registration };
        }

        public async Task<users?> AddAndUpdateUser(object userObj)
        {
            return await Task.FromResult<users?>(null);
        }

        private async Task<string> isAuthenticate(users user)
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

        private async Task<string> generateJwtToken(users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (string.IsNullOrEmpty(_secret) || Encoding.UTF8.GetBytes(_secret).Length < 32)
            {
                throw new InvalidOperationException("JWT secret is not configured or too short. Set AppSettings:Secret with at least 32 bytes length (e.g. 32+ characters)." );
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
    }
}
