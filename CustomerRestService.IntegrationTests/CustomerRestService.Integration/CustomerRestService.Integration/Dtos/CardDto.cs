using System;

namespace CustomerRestService.Integration.Dtos
{
    public class CardDto
    {
        public string CustomerCode { get; set; }
        public string UniqueNumber { get; set; }
        public string CVVNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
