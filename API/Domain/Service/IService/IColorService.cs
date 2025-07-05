public interface IColorService
{
    Task<List<ColorDto>> GetAllAsync();
    Task<ColorDto?> GetByIdAsync(Guid id);
    Task<ColorDto> CreateAsync(CreateColorRequest request);
    Task<ColorDto> UpdateAsync(UpdateColorRequest request);
}
