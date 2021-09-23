using System.ComponentModel.DataAnnotations;

namespace CustomerRestService.Models
{
    public class Customer : ICustomer
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string CustomerCode { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [RegularExpression(@"(.{13})")]
        public string CNP { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }
}