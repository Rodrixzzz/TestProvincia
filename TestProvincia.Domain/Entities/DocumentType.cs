namespace TestProvincia.Domain.Entities;

public class DocumentType
{
    public int Id { get; set; }
    public string Desc { get; set; } = string.Empty;
    public bool Active { get; set; }
}
