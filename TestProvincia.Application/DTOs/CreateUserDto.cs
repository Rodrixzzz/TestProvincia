namespace TestProvincia.Application.DTOs;

public class CreateUserDto
{
    public string? Name { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;
    public string? Province { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
}
