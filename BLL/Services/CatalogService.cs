// BLL/Services/CatalogService.cs
using Interfaces.Services;
using Interfaces.Repository;
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
        private readonly IRepository<TypeOfPolicy> _typeOfPolicyRepository;

        public CatalogService(InsuranceDbContext context, IRepository<TypeOfPolicy> repo)
        {
            _context = context;
            _typeOfPolicyRepository = repo;
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

        public async Task<IEnumerable<string>> GetPolicyTypesAsync()
        {
            // Получаем все типы из БД
            var types = await _typeOfPolicyRepository.GetAllAsync();
            // Возвращаем только имена
            return types.Select(t => t.Name).ToList();
        }

        public async Task<int> GetPolicyTypeIdByNameAsync(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentException("Название типа не может быть пустым.", nameof(typeName));
            }

            // Получаем все типы
            var types = await _typeOfPolicyRepository.GetAllAsync();
            // Ищем по имени (без учёта регистра)
            var type = types.FirstOrDefault(t => string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase));

            if (type == null)
            {
                throw new InvalidOperationException($"Тип полиса с именем '{typeName}' не найден в справочнике.");
            }

            return type.Id;
        }
    }
}