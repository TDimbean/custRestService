using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerRestService.Integration.Entities
{
    public class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.Cards = new HashSet<Card>();
        }

        public long ID { get; set; }

        [StringLength(50)]
        [Required]
        public string CustomerCode { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [RegularExpression(@"(.{13})")]
        public string CNP { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Card> Cards { get; set; }
    }
}
