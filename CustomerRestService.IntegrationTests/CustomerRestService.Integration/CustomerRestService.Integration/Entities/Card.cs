using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerRestService.Integration.Entities
{


    public class Card
    {
        public long ID { get; set; }

        [Required]
        [StringLength(15)]
        public string CardCode { get; set; }

        public long? CustomerId { get; set; }

        [StringLength(50)]
        public string UniqueNumber { get; set; }

        [StringLength(15)]
        public string CVVNumber { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.DateTime)]

        public DateTime? EndDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
