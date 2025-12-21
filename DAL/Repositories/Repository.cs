using Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using DAL.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly InsuranceDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(InsuranceDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // --- Реализация нового метода ---
        public async Task<IEnumerable<T>> GetByVehicleIdAsync(int vehicleId)
        {
            // Проверяем, есть ли у сущности свойство VehicleId
            if (typeof(T).GetProperty("VehicleId") != null)
            {
                // Используем LINQ для фильтрации
                var query = _dbSet.Where(entity => EF.Property<int>(entity, "VehicleId") == vehicleId);
                return await query.ToListAsync();
            }
            else
            {
                // Если свойство VehicleId не найдено, возвращаем пустую коллекцию
                return new List<T>();
            }
        }
    }
}