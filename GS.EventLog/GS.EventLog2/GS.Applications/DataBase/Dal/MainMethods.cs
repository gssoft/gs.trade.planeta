using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Applications.DataBase.Model;

namespace GS.Applications.DataBase.Dal
{
    public partial class ApplicationsContext
    {
        public User RegisterUser(string name, string guid)
        {
            var u = Users.FirstOrDefault(us => us.Name == name);
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
    }
}
