using Core.Entities;
using Interfaces.DTO;
using Interfaces.Repository;
using Interfaces.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Context;

namespace BLL.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly InsuranceDbContext _context;

        public PolicyService(IPolicyRepository policyRepository, InsuranceDbContext context)
        {
            _policyRepository = policyRepository;
            _context = context;
        }

        public async Task<int> CreatePolicyAsync(Insurance policyDto)
        {
            Console.WriteLine($"=== СОЗДАНИЕ ПОЛИСА ===");
            Console.WriteLine($"VehicleId: {policyDto.VehicleId}");
            Console.WriteLine($"TypeId: {policyDto.TypeId}");
            Console.WriteLine($"PolicyNumber: {policyDto.PolicyNumber}");
            Console.WriteLine($"BasePrice: {policyDto.BasePrice}");

            try
            {
                var entity = new InsurancePolicy
                {
                    VehicleId = policyDto.VehicleId,
                    TypeId = policyDto.TypeId,
                    StatusId = policyDto.StatusId,
                    PolicyNumber = policyDto.PolicyNumber,
                    StartDate = policyDto.StartDate,
                    EndDate = policyDto.EndDate,
                    BasePrice = policyDto.BasePrice,
                    PowerCoefficient = policyDto.PowerCoefficient,
                    ExperienceCoefficient = policyDto.ExperienceCoefficient,
                    BonusMalusCoefficient = policyDto.BonusMalusCoefficient,
                    // ⚠️ НЕ ПЕРЕДАВАЙТЕ TotalPrice! Он вычисляется автоматически
                };

                Console.WriteLine($"Сохранение полиса...");
                await _policyRepository.AddAsync(entity);

                Console.WriteLine($"=== ПОЛИС СОЗДАН! ID: {entity.Id} ===");
                Console.WriteLine($"Автоматически рассчитанный TotalPrice: {entity.TotalPrice}");

                return entity.Id;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"=== DBUPDATE EXCEPTION ===");
                Console.WriteLine($"Message: {dbEx.Message}");

                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"Inner: {dbEx.InnerException.Message}");

                    if (dbEx.InnerException is SqlException sqlEx)
                    {
                        Console.WriteLine($"SQL Error #{sqlEx.Number}: {sqlEx.Message}");

                        // Добавьте диагностику конкретных ошибок
                        if (sqlEx.Number == 8152) // String or binary data would be truncated
                        {
                            Console.WriteLine("ОШИБКА: Слишком длинная строка для поля!");
                        }
                    }
                }

                throw new InvalidOperationException(
                    $"Ошибка создания полиса: {dbEx.InnerException?.Message ?? dbEx.Message}",
                    dbEx);
            }
        }

        public async Task<IEnumerable<Insurance>> GetClientPoliciesAsync(int clientId)
        {
            var list = await _policyRepository.GetByClientIdAsync(clientId);
            return list.Select(p => new Insurance
            {
                Id = p.Id,
                VehicleId = p.VehicleId,
                TypeId = p.TypeId,
                StatusId = p.StatusId,
                CancelledBy = p.CancelledBy,
                PolicyNumber = p.PolicyNumber,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                BasePrice = p.BasePrice,
                PowerCoefficient = p.PowerCoefficient,
                ExperienceCoefficient = p.ExperienceCoefficient,
                BonusMalusCoefficient = p.BonusMalusCoefficient,
                // TotalPrice не нужно передавать, он вычисляется
            });
        }

        public async Task<Insurance?> GetPolicyByIdAsync(int id)
        {
            var p = await _policyRepository.GetByIdAsync(id);
            if (p == null) return null;
            return new Insurance
            {
                Id = p.Id,
                VehicleId = p.VehicleId,
                TypeId = p.TypeId,
                StatusId = p.StatusId,
                CancelledBy = p.CancelledBy,
                PolicyNumber = p.PolicyNumber,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                BasePrice = p.BasePrice,
                PowerCoefficient = p.PowerCoefficient,
                ExperienceCoefficient = p.ExperienceCoefficient,
                BonusMalusCoefficient = p.BonusMalusCoefficient
            };
        }

        public async Task<AnnualPolicyRevenueReportDto> GetAnnualRevenueReportAsync(int year)
        {
            // Выполняем запрос к БД
            var report = await _context.InsurancePolicies
                .Where(p => p.StartDate.Year == year) // Полис оформлен в году
                .GroupBy(p => 1) // Группируем всё в одну группу для агрегации
                .Select(g => new AnnualPolicyRevenueReportDto
                {
                    Year = year,
                    TotalPoliciesCount = g.Count(), // Количество полисов
                    TotalRevenue = g.Sum(p => p.TotalPrice) // Сумма TotalPrice
                })
                .FirstOrDefaultAsync(); // Получаем первую (и единственную) группу или null

            // Если за год нет полисов, возвращаем DTO с нулями
            return report ?? new AnnualPolicyRevenueReportDto { Year = year, TotalPoliciesCount = 0, TotalRevenue = 0 };
        }

        public async Task CancelPolicyAsync(int policyId, int cancelledByManagerId)
        {
            var p = await _policyRepository.GetByIdAsync(policyId);
            if (p == null) return;
            p.StatusId = 4; // assume 3 = cancelled
            p.CancelledBy = cancelledByManagerId;
            _policyRepository.Update(p);
        }
        public async Task<IEnumerable<Insurance>> GetPoliciesByVehicleId(int vehicleId)
        {
            var policies = await _policyRepository.GetPoliciesByVehicleIdAsync(vehicleId);
            return policies.Select(p => new Insurance
            {
                Id = p.Id,
                VehicleId = p.VehicleId,
                TypeId = p.TypeId,
                StatusId = p.StatusId,
                PolicyNumber = p.PolicyNumber,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                BasePrice = p.BasePrice,
                PowerCoefficient = p.PowerCoefficient,
                ExperienceCoefficient = p.ExperienceCoefficient,
                BonusMalusCoefficient = p.BonusMalusCoefficient,
                // TotalPrice не нужно передавать, он вычисляется
            });
        }

        // BLL/Services/PolicyService.cs
        // ...
        public async Task<IEnumerable<Insurance>> GetByClientIdAsync(int clientId)
        {
            var list = await _policyRepository.GetByClientIdAsync(clientId);
            return list.Select(p => new Insurance
            {
                Id = p.Id,
                VehicleId = p.VehicleId,
                TypeId = p.TypeId,
                StatusId = p.StatusId,
                CancelledBy = p.CancelledBy,
                PolicyNumber = p.PolicyNumber,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                BasePrice = p.BasePrice,
                PowerCoefficient = p.PowerCoefficient,
                ExperienceCoefficient = p.ExperienceCoefficient,
                BonusMalusCoefficient = p.BonusMalusCoefficient,
                // TotalPrice вычисляется автоматически
                // --- Заполняем новые свойства ---
                TypeName = GetTypeName(p.TypeId), // <-- Нужно реализовать
                VehiclePlateNum = p.Vehicle.PlateNum, // <-- Убедитесь, что Vehicle загружен
                VehicleBrand = p.Vehicle.Model.Brand.Name, // <-- Убедитесь, что Brand загружен
                VehicleModel = p.Vehicle.Model.Name, // <-- Убедитесь, что Model загружен
                StatusName = GetStatusName(p.StatusId), // <-- Нужно реализовать
                                                        // ---
            });
        }

        private string GetTypeName(int typeId)
        {
            // Возвращаете имя типа по Id (например, из справочника TypeOfPolicy)
            // Это можно реализовать через ITypeOfPolicyService, или хранить в памяти словарь
            // Пока заглушка:
            return typeId switch
            {
                1 => "ОСАГО",
                2 => "КАСКО",
                3 => "ДСАГО",
                4 => "ОСГОП",
                _ => "Неизвестно"
            };
        }

        private string GetStatusName(int statusId)
        {
            // Возвращаете имя статуса по Id (например, из справочника StatusOfPolicy)
            // Это можно реализовать через IStatusOfPolicyService, или хранить в памяти словарь
            // Пока заглушка:
            return statusId switch
            {
                1 => "Активен",
                2 => "Завершён",
                3 => "Отменён",
                _ => "Неизвестно"
            };
        }
        // ...
    }
}
