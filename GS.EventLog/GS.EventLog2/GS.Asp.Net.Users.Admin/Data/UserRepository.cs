using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GS.Asp.Net.Users.Admin.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly UserStore<IdentityUser> _store;
        private readonly UserManager<IdentityUser> _manager;

        public UserRepository()
        {
            _store = new UserStore<IdentityUser>();
            _manager = new UserManager<IdentityUser>(_store);
        }

        public async Task<IdentityUser> GetUserByNameAsync(string username)
        {
            return await _store.FindByNameAsync(username);
        }
        public async Task<IdentityUser> GetUserByIdAsync(string id)
        {
            return await _store.FindByIdAsync(id);
        }

        public async Task<IEnumerable<IdentityUser>> GetAllUsersAsync()
        {
            return await _store.Users.ToArrayAsync();
        }

        public async Task CreateAsync(IdentityUser user, string password)
        {
            await _manager.CreateAsync(user, password);
        }

        public async Task DeleteAsync(IdentityUser user)
        {
            await _manager.DeleteAsync(user);
        }

        public async Task UpdateAsync(IdentityUser user)
        {
            await _manager.UpdateAsync(user);
        }

        public bool VerifyUserPassword(string hashedPassword, string providedPassword)
        {
            return _manager.PasswordHasher.VerifyHashedPassword(hashedPassword, providedPassword) ==
                   PasswordVerificationResult.Success;
        }

        public string HashPassword(string password)
        {
            return _manager.PasswordHasher.HashPassword(password);
        }

        public async Task AddUserToRoleAsync(IdentityUser user, string role)
        {
            await _manager.AddToRoleAsync(user.Id, role);
        }

        public async Task<IEnumerable<string>> GetRolesForUserAsync(IdentityUser user)
        {
            return await _manager.GetRolesAsync(user.Id);
        }

        public async Task RemoveUserFromRoleAsync(IdentityUser user, params string[] roleNames)
        {
            await _manager.RemoveFromRolesAsync(user.Id, roleNames);
        }

        public async Task<IdentityUser> GetLoginUserAsync(string username, string password)
        {
            return await _manager.FindAsync(username, password);
        }

        public async Task<ClaimsIdentity> CreateIdentityAsync(IdentityUser user)
        {
            return await _manager.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        private bool _disposed;
        public void Dispose()
        {
            if (!_disposed)
            {
                _manager.Dispose();
                _store.Dispose();
            }

            _disposed = true;
        }
    }
}