using TestProvincia.Domain.Enums;

namespace TestProvincia.Domain.Entities;



public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
