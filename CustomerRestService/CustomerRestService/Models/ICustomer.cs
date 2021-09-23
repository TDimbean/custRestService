namespace CustomerRestService.Models
{
    public interface ICustomer
    {
        string Address { get; set; }
        string CNP { get; set; }
        string CustomerCode { get; set; }
        int ID { get; set; }
        string Name { get; set; }
    }
}