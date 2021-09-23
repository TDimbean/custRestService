using System;

namespace CustomerRestService.Models
{
    public interface ICard
    {
        string CardCode { get; set; }
        int CustomerID { get; set; }
        string CVVNumber { get; set; }
        DateTime EndDate { get; set; }
        int ID { get; set; }
        DateTime StartDate { get; set; }
        string UniqueNumber { get; set; }
    }
}