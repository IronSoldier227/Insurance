// BLL/Services/CatalogService.cs
using Interfaces.Services;
using Core.Entities;
using DAL.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly InsuranceDbContext _context;

        public CatalogService(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            return await _context.Brands.OrderBy(b => b.Name).ToListAsync();
        }

        public async Task<IEnumerable<Model>> GetModelsByBrandIdAsync(int brandId)
        {
            return await _context.Models
                .Where(m => m.BrandId == brandId)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public Task<IEnumerable<string>> GetCategoriesAsync()
        {
            var categories = new List<string>
            {
                "A", "B", "C", "D", "BE", "CE", "DE"
            };
            return Task.FromResult(categories.AsEnumerable());
        }
    }
}