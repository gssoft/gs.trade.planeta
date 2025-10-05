using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Applications.DataBase1.Dal;
using GS.Applications.DataBase1.Model;

namespace GS.Applications
{
    public static class GSApplications
    {
        public static IEnumerable<ApplicationDto> GetAppsByUserName(string userName)
        {
            using (var db = new ApplicationsContext1())
            {
                var u = db.Users.FirstOrDefault(us => us.Name == userName);
                if (u == null)
                    return null;
                var l = u.Applications.Select(a => new ApplicationDto
                {
                    ID = a.ID,
                    Code = a.Code,
                    Name = a.Name,
                    Description = a.Description,
                    UserID = a.UserID,
                    UserName = u.Name
                }).ToList();

                return l;
            }
        }
        public static async Task<IEnumerable<ApplicationDto>> GetAppsByUserNameAsync(string userName)
        {
            using (var db = new ApplicationsContext1())
            {
                var u = await db.Users.FirstOrDefaultAsync(us => us.Name == userName);
                if (u == null)
                    return null;
                var l = u.Applications.Select(a => new ApplicationDto
                {
                    ID = a.ID,
                    Code = a.Code,
                    Name = a.Name,
                    Description = a.Description,
                    UserID = a.UserID,
                    UserName = u.Name
                }).ToList();

                return l;
            }
        }

        public static async Task<User> RegisterUserAsync(string name, string guid)
        {
            using (var db = new ApplicationsContext1())
            {
                return await db.RegisterUserAsync(name, guid);
            }
        }
        public static User RegisterUser(string name, string guid)
        {
            using (var db = new ApplicationsContext1())
            {
                return  db.RegisterUser(name, guid);
            }
        }
    }
}
