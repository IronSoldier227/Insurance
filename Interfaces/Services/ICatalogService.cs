// Interfaces/Services/ICatalogService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Interfaces.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<Brand>> GetAllBrandsAsync();
        Task<IEnumerable<Model>> GetModelsByBrandIdAsync(int brandId);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<IEnumerable<string>> GetPolicyTypesAsync();
        Task<int> GetPolicyTypeIdByNameAsync(string typeName);
    }
}