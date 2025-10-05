using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Applications.DataBase1.Model
{
    public class Application
    {
        public int ID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public int UserID { get; set; }

        [StringLength(128)]
        public string Code { get; set; }
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
        public DateTime? ModifiedDT { get; set; }

        public Application()
        {
            ModifiedDT = DateTime.Now;
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
        public string Name { get; set; }
        [StringLength(128)]
        public string  Guid { get; set; }

        public DateTime? ModifiedDT { get; set; }
        public virtual ICollection<Application> Applications { get; set; }
    }
}
