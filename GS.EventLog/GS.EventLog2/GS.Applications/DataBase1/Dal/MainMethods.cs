using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Applications.DataBase1.Model;

namespace GS.Applications.DataBase1.Dal
{
    public partial class ApplicationsContext1
    {
        public User RegisterUser(string name, string guid)
        {
            var u = Users.FirstOrDefault( us => us.Name == name);
            if (u == null)
            {
                u = new User {Name = name, Guid = guid};
                Users.Add(u);
                SaveChanges();
                return u;
            }
            if (u.Guid == guid)
                return u;
            u.Guid = guid;
            SaveChanges();
            return u;
        }
        public async Task<User> RegisterUserAsync(string name, string guid)
        {
            var u = await Users.FirstOrDefaultAsync(us => us.Name == name);
            if (u == null)
            {
                u = new User { Name = name, Guid = guid };
                Users.Add(u);
                await SaveChangesAsync();
                return u;
            }
            if (u.Guid == guid)
                return u;
            u.Guid = guid;
            await SaveChangesAsync();
            return u;
        }

        public IEnumerable<Application> GetAppsByUserName(string userName)
        {
            var u = Users.FirstOrDefault(us => us.Name == userName);
            if (u == null)
                return null;
            var l = u.Applications.ToList();
            return l;
        }
    }
}
