using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GS.Asp.Net.Users.Admin.Data
{
    public interface IUserRepository : IDisposable
    {

        Task<IdentityUser> GetUserByNameAsync(string username);
        Task<IdentityUser> GetUserByIdAsync(string id);
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
        Task CreateAsync(IdentityUser user, string password);
        Task DeleteAsync(IdentityUser user);
        Task UpdateAsync(IdentityUser user);
        bool VerifyUserPassword(string hashedPassword, string providedPassword);
        string HashPassword(string password);

        Task AddUserToRoleAsync(IdentityUser newUser, string p);

        Task<IEnumerable<string>> GetRolesForUserAsync(IdentityUser user);

        Task RemoveUserFromRoleAsync(IdentityUser user, params string[] roleNames);

        Task<IdentityUser> GetLoginUserAsync(string username, string password);

        Task<ClaimsIdentity> CreateIdentityAsync(IdentityUser user);
    }
}