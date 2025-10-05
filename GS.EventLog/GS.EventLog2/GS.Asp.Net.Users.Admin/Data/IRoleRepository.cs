using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public interface IRoleRepository : IDisposable
    {
        Task<IdentityRole> GetRoleByIdAsync(string id);
        Task<IdentityRole> GetRoleByNameAsync(string name);
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task CreateAsync(IdentityRole role);
        Task DeleteAsync(IdentityRole role);

        Task<IEnumerable<IdentityUserRole>> GetUserRoles(string id);
    }
}