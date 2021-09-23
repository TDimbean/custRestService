using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerRestService.Models
{
    public class Card : ICard
    {
        public int ID { get; set; }

        [RegularExpression(@"(.{15})")]
        public string CardCode { get; set; }

        public int CustomerID { get; set; }

        [StringLength(50)]
        public string UniqueNumber { get; set; }

        [StringLength(15)]
        public string CVVNumber { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}