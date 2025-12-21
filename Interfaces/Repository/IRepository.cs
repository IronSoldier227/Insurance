using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
        // --- Новый метод ---
        Task<IEnumerable<T>> GetByVehicleIdAsync(int vehicleId); // <-- Добавляем
    }
}