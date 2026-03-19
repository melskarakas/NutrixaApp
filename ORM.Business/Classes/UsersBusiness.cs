using ORM.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Business
{
    public class UsersBusiness
    {
        /// <summary>
        /// users.is_active = 1: Kullanıcı Adı ve Şifre Doğru, kullanıcı "aktif" ise 
        /// users.is_active = 0: Kullanıcı Adı ve Şifre, kullanıcı "pasif" ise
        /// null 3: Kullanıcı Adı veya Şifresi yanlış ise
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public users UserLogin(string username, string password)
        {
            users u;
            try
            {
                string md5Password = ORM.Shared.Base.GenerateDoubleMD5Password(password);

                var parameters = new Dapper.DynamicParameters();
                parameters.Add(nameof(users.user_name), username);
                parameters.Add(nameof(users.password), md5Password);
                parameters.Add(nameof(users.is_active), true);
                parameters.Add(nameof(users.is_deleted), false);
                //User u = new Business.Classes.GenericBusiness<User>().GetList(parameters).FirstOrDefault();
                u = new Business.GenericBusiness<users>().GetAllByCustomQuery(parameters).FirstOrDefault();

                return u;
            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }

        }

        public users GetUserById(Guid id, bool isActive)
        {
            users u;
            try
            {
                var parameters = new Dapper.DynamicParameters();
                parameters.Add(nameof(users.id), id);
                parameters.Add(nameof(users.is_active), isActive);
                parameters.Add(nameof(users.is_deleted), false);
                //User u = new Business.Classes.GenericBusiness<User>().GetList(parameters).FirstOrDefault();
                u = new Business.GenericBusiness<users>().GetAllByCustomQuery(parameters).FirstOrDefault();

                return u;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public bool UpdateLastLoginTime(Guid yonetici_id)
        {
            try
            {
                string sql = $"UPDATE {nameof(users)} SET {nameof(users.last_login_time)} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE {nameof(users.id)} = '{yonetici_id}'";
                var x = new GenericBusiness<users>().GetScalar(sql);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
