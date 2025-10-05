using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;

namespace GS.DataBase.Entities
{
    public abstract class SimpleEntity
    {
        public long Id { get; set; }

        [Required]
        public string Code { get; set; }
        public abstract string Key { get; }

    }
    public abstract class SimpleEntity2
    {
        public long Id { get; set; }

        [Required]
        public string Code { get; set; }

        public abstract string Key { get; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDT { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedDT { get; set; }
        protected SimpleEntity2()
        {
            CreatedDT = ModifiedDT = DateTime.Now;
        }
    }
    public abstract class SimpleTimeEntity
    {
        public long Id { get; set; }

        [Required]
        public DateTime? DT { get; set; }

        protected SimpleTimeEntity()
        {
            DT = DateTime.Now;
        }

    }

    public abstract class Entity : IHaveKey<string>
    {
        public long Id { get; set; }
        [Required]
        public string Code { get; set; }
        public string Name { get; set; }
        // public string Alias { get; set; }
        public string Description { get; set; }
        public abstract string Key { get; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedDT { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ModifiedDT { get; set; }

        protected Entity()
        {
            CreatedDT = ModifiedDT = DateTime.Now;
        }
    }
    public abstract class AliasEntity : Entity
    {
            [Required]
            public string Alias { get; set; }

    }



    public abstract class TimeScoreEntity : Entity
    {
        public DateTime FirstDT { get; set; }
        public DateTime LastDT { get; set; }

        protected TimeScoreEntity() : base()
        {
        }

        public int Days
        {
            get
            {
                return 1 + (LastDT.Date - FirstDT.Date).Days;
            }
        }

        public long Count { get; set; }
    }
}
