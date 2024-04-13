using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? SSN { get; set; }
        public DateTime? DOB { get; set; }
        public Address? HomeAddress { get; set; }
        public Address? OfficeAddress { get; set; }
        public int? Age { get; set; }
    }

    public class Address
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
    }
}
