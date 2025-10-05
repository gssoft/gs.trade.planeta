using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Applications.DataBase.Model
{

    // Many to Many Model
    public class Application
    {
        public int ID { get; set; }
        [StringLength(128)]
        public string Code { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        public DateTime? ModifiedDT { get; set; }
        public ICollection<User> Users { get; set; }

        public Application()
        {
            ModifiedDT = DateTime.Now;
            Users = new HashSet<User>();
        }

    }

    public class User
    {
        public User()
        {
            ModifiedDT = DateTime.Now;
            Applications = new HashSet<Application>();
        }

        public int ID { get; set; }
        [StringLength(128)]
        public string  Guid { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        public DateTime? ModifiedDT { get; set; }
        public ICollection<Application> Applications { get; set; }
    }
}
