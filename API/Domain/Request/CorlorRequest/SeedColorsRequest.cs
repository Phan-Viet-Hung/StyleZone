using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Domain.Request.CorlorRequest
{
    public class SeedColorsRequest
    {
        public static async Task SeedColorsAsync(DbContextApp context)
        {
            if (await context.Colors.AnyAsync())
                return;

            var colors = new List<Color>
        {
            new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Code = "#FF0000", Name = "Red" },
            new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Code = "#FFA500", Name = "Orange" },
            new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Code = "#FFFF00", Name = "Yellow" },
            new() { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Code = "#008000", Name = "Green" },
            new() { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Code = "#0000FF", Name = "Blue" },
            new() { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Code = "#4B0082", Name = "Indigo" },
            new() { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Code = "#EE82EE", Name = "Violet" },
            new() { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Code = "#FFFFFF", Name = "White" },
            new() { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Code = "#000000", Name = "Black" },
        };

            await context.Colors.AddRangeAsync(colors);
            await context.SaveChangesAsync();
        }
    }
}
