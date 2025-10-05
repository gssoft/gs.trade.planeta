using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Data;

namespace GS.Asp.Net.Users.Admin.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleStore<IdentityRole> _store;
        private readonly RoleManager<IdentityRole> _manager;

        public RoleRepository()
        {
            _store = new RoleStore<IdentityRole>();
            _manager = new RoleManager<IdentityRole>(_store);
        }
        public async Task<IdentityRole> GetRoleByIdAsync(string id)
        {
            return await _store.FindByIdAsync(id);
        }

        public async Task<IdentityRole> GetRoleByNameAsync(string name)
        {
            return await _store.FindByNameAsync(name);
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
        {
            return await _store.Roles.ToArrayAsync();
        }

        public async Task CreateAsync(IdentityRole role)
        {
            await _manager.CreateAsync(role);
        }
        public async Task DeleteAsync(IdentityRole role)
        {
            await _manager.DeleteAsync(role);
        }

        public async Task<IEnumerable<IdentityUserRole>> GetUserRoles(string id)
        {
            var role = await GetRoleByIdAsync(id);
            if (role == null)
                return Enumerable.Empty<IdentityUserRole>();
            return role.Users.ToList();
        }

        private bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                _store.Dispose();
                _manager.Dispose();
            }

            _disposed = true;
        }
    }
}