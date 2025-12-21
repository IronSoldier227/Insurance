// DAL/Repositories/UserWriteRepository.cs
using DAL.Context;
using Core.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.SqlClient; // или System.Data.SqlClient, в зависимости от версии
using System.Diagnostics; // Добавим для Debug.WriteLine

namespace DAL.Repositories
{
    public class UserWriteRepository
    {
        private readonly InsuranceDbContext _context;

        public UserWriteRepository(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task AddClientProfileAsync(ClientProfile profile)
        {
            try
            {
                Debug.WriteLine($"Попытка сохранить ClientProfile с Id={profile.Id}, Passport='{profile.Passport}', DriverLicense='{profile.DriverLicense}', DrivingExperience={profile.DrivingExperience}");
                await _context.ClientProfiles.AddAsync(profile);
                await _context.SaveChangesAsync();
                Debug.WriteLine("ClientProfile успешно сохранен.");
            }
            catch (DbUpdateException dbEx)
            {
                Debug.WriteLine("--- DbUpdateException в AddClientProfileAsync ---");
                Debug.WriteLine($"Сообщение: {dbEx.Message}");
                Debug.WriteLine($"Тип InnerException: {dbEx.InnerException?.GetType()}");
                Debug.WriteLine($"Сообщение InnerException: {dbEx.InnerException?.Message}");

                if (dbEx.InnerException is SqlException sqlEx)
                {
                    Debug.WriteLine($"Номер ошибки SQL: {sqlEx.Number}");
                    Debug.WriteLine($"Сообщение SQL: {sqlEx.Message}");
                    // 8152: String or binary data would be truncated
                    // 2601: Cannot insert duplicate key row
                    // 2627: Violation of PRIMARY KEY constraint
                    // 547: The INSERT statement conflicted with the FOREIGN KEY constraint
                }

                // Логируйте также EntityValidationErrors, если они есть
                foreach (var entry in dbEx.Entries)
                {
                    Debug.WriteLine($"Entity Type: {entry.Entity.GetType().Name}");
                    Debug.WriteLine($"Entity State: {entry.State}");
                    // Note: GetValidationResult might not be available directly on Entry
                    // We can check if the entity is valid against its own rules if needed,
                    // but EF usually shows validation errors in the exception or via State.
                    // The main issue is likely DB constraint violation, not entity validation.
                }

                throw; // Перебросить исключение дальше
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"--- Общее исключение в AddClientProfileAsync ---");
                Debug.WriteLine($"Сообщение: {ex.Message}");
                Debug.WriteLine($"Тип: {ex.GetType()}");
                Debug.WriteLine($"InnerException: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}