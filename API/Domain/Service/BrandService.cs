﻿using API.Domain.DTOs;
using API.Domain.Extensions;
using API.Domain.Request.BrandRequest;
using API.Domain.Service.IService;
using DAL_Empty.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Domain.Service
{
    public class BrandService : IBrandService
    {
        private readonly DbContextApp _context;

        public BrandService(DbContextApp context)
        {
            _context = context;
        }

        public async Task<List<BrandDto>> GetAllAsync()
        {
            var brands = await _context.Brands.ToListAsync();
            return brands.ConvertAll(b => b.ToDto());
        }

        public async Task<BrandDto?> GetByIdAsync(Guid id)
        {
            var brand = await _context.Brands.FindAsync(id);
            return brand?.ToDto();
        }

        public async Task<BrandDto> CreateAsync(CreateBrandRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Code và Name không được để trống.");

            // Kiểm tra trùng code
            bool exists = await _context.Brands.AnyAsync(b => b.Code == request.Code);
            if (exists)
                throw new ArgumentException("Mã thương hiệu đã tồn tại.");

            var brand = new Brand
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Name = request.Name,
                CreatedAt = DateTime.Now
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return brand.ToDto();
        }

        public async Task<BrandDto> UpdateAsync(Guid id, UpdateBrandRequest request)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
                throw new KeyNotFoundException("Không tìm thấy thương hiệu.");

            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Code và Name không được để trống.");

            // Kiểm tra code đã tồn tại cho thương hiệu khác
            bool exists = await _context.Brands.AnyAsync(b => b.Code == request.Code && b.Id != id);
            if (exists)
                throw new ArgumentException("Mã thương hiệu đã tồn tại cho một thương hiệu khác.");

            brand.Code = request.Code;
            brand.Name = request.Name;
            brand.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return brand.ToDto();
        }
    }
}
