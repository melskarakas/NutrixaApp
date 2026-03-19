using System.Collections.Generic;
using System.Threading.Tasks;
using ORM.Services.Models;
using ORM.Models.Models;

namespace ORM.Services.IServices
{
    public interface IUserService
    {
        Task<AuthenticationResult?> AuthenticateByEmail(string email, string password, bool isPasswordMd5);
        Task<users?> AddAndUpdateUser(users userObj);
        Task<users?> AddUser(users user);
        Task<users?> GetById(object id);
        Task<List<users>> GetAll();
        Task<bool> Update(users user);
        Task<bool> Delete(object id);
        Task<bool> DeleteUser(users userObj);
        Task<users> GetUser(object id);
        Task<users?> AddAndUpdateUser(object userObj);
        Task<bool> ExistsByEmail(string email);
        Task<bool> ExistsByUserName(string userName);
        Task<user_inf?> AddUserInfo(user_inf info);
    }
}
