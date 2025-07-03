using DAL_Empty.Models;

public static class ColorExtensions
{
    public static ColorDto ToDto(this Color color)
    {
        return new ColorDto
        {
            Id = color.Id,
            Code = color.Code,
            Name = color.Name ?? "",
            CreatedAt = color.CreatedAt,
            UpdatedAt = color.UpdatedAt
        };
    }
}
