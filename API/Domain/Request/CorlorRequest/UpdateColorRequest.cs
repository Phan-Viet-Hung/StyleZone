using System.ComponentModel.DataAnnotations;

public class UpdateColorRequest : CreateColorRequest
{
    [Required]
    public Guid Id { get; set; }
}
