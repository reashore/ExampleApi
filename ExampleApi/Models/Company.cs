
namespace ExampleApi.Models
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string Ein { get; set; }
        public bool IsGovernmentAgency { get; set; }
    }
}
