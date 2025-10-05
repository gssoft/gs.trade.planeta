using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TimeSeries.General
{
    public abstract class DbElement<TID> : CreateModifyTracking
    {
        [Key]
        public TID ID { get; set; }
        [Required]
        [MaxLength(64)]
        public string Code { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    }

    public abstract class CreateModifyTracking
    {
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        protected CreateModifyTracking()
        {
            Created = Modified = DateTime.Now;
        }
    }
}
