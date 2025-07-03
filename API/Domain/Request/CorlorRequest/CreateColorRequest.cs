using System.ComponentModel.DataAnnotations;

public class CreateColorRequest
{
    [MaxLength(10)]
    public string? Code { get; set; }

    [MaxLength(50)]
    public string? Name { get; set; } = string.Empty;
}
