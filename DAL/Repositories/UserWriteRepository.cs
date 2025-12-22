using DAL.Context;
using Core.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

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
                }

                // Логируйте также EntityValidationErrors, если они есть
                foreach (var entry in dbEx.Entries)
                {
                    Debug.WriteLine($"Entity Type: {entry.Entity.GetType().Name}");
                    Debug.WriteLine($"Entity State: {entry.State}");
                }

                throw; 
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

        public async Task AddManagerAsync(Manager manager)
        {
            try
            {
                await _context.Managers.AddAsync(manager);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения Manager: {ex.Message}");
                throw;
            }
        }
    }
}