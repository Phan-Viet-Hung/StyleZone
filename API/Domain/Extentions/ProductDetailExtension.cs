using DAL_Empty.Models;

public static class ProductDetailExtensions
{
    public static ProductDetailDto ToDto(this ProductDetail p)
    {
        return new ProductDetailDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Quantity = p.Quantity,

            ProductId = p.ProductId,
            ProductName = p.Product?.Name ?? string.Empty,

            ColorId = p.ColorId,
            ColorName = p.Color?.Name ?? string.Empty,

            SizeId = p.SizeId,
            SizeName = p.Size?.Name ?? string.Empty,

            MaterialId = p.MaterialId,
            MaterialName = p.Material?.Name ?? string.Empty,



            OriginId = p.OriginId,
            OriginName = p.Origin?.Name ?? string.Empty,

            SupplierId = p.SupplierId,
            SupplierName = p.Supplier?.Name ?? string.Empty,

            Status = p.Status
        };
    }
}
