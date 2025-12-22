using Core.Entities;
using DAL.Context;
using Interfaces.DTO;
using Interfaces.Repository;
using Interfaces.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly InsuranceDbContext _context;

        public PolicyService(IPolicyRepository policyRepository, InsuranceDbContext context)
        {
            _policyRepository = policyRepository;
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
                    TotalPrice  = policyDto.TotalPrice
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
                        
                        if (sqlEx.Number == 8152) 
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
                TotalPrice = p.TotalPrice,
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
                BonusMalusCoefficient = p.BonusMalusCoefficient,
                TotalPrice = p.TotalPrice
            };
        }

        public async Task CancelPolicyAsync(int policyId, int cancelledByManagerId)
        {
            var p = await _policyRepository.GetByIdAsync(policyId);
            if (p == null) return;
            p.StatusId = 4; 
            p.CancelledBy = cancelledByManagerId;
            _policyRepository.Update(p);
        }

        public async Task<IEnumerable<Insurance>> GetActivePoliciesByVehicleIdAsync(int vehicleId)
        {
            const int ACTIVE_STATUS_ID = 1;

            var policies = await _policyRepository.GetAllWithIncludesAsync(
                p => p.Type,      
                p => p.Status,    
                p => p.Vehicle   
            );

            var activePoliciesForVehicle = policies.Where(p => p.VehicleId == vehicleId && p.StatusId == ACTIVE_STATUS_ID);

            var dtos = new List<Insurance>();
            foreach (var policy in activePoliciesForVehicle)
            {
                dtos.Add(new Insurance
                { 
                    Id = policy.Id,
                    VehicleId = policy.VehicleId,
                    TypeId = policy.TypeId,
                    TypeName = policy.Type.Name,
                    StatusId = policy.StatusId,
                    StatusName = policy.Status.Name,
                    PolicyNumber = policy.PolicyNumber,
                    StartDate = policy.StartDate,
                    EndDate = policy.EndDate,
                    BasePrice = policy.BasePrice,
                    PowerCoefficient = policy.PowerCoefficient,
                    ExperienceCoefficient = policy.ExperienceCoefficient,
                    BonusMalusCoefficient = policy.BonusMalusCoefficient,
                    TotalPrice = policy.TotalPrice
                });
            }

            return dtos;
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
                TotalPrice = p.TotalPrice,
            });
        }

        public async Task<IEnumerable<Insurance>> GetByClientIdAsync(int clientId)
        {
            var list = await _policyRepository.GetByClientIdAsync(clientId);
            List<InsurancePolicy> expiredPolicies = new List<InsurancePolicy>();
            foreach(var policy in list)
            { 
                if (policy.EndDate < DateTime.Now && policy.StatusId != 3)
                {
                    policy.StatusId = 2;
                    expiredPolicies.Add(policy);
                }
            }

            if (expiredPolicies.Any())
            {
                foreach(var policy in expiredPolicies)
                {
                    _policyRepository.Update(policy);
                }

            }

            list = await _policyRepository.GetByClientIdAsync(clientId);

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
                TypeName = GetTypeName(p.TypeId), 
                VehiclePlateNum = p.Vehicle.PlateNum, 
                VehicleBrand = p.Vehicle.Model.Brand.Name, 
                VehicleModel = p.Vehicle.Model.Name, 
                StatusName = GetStatusName(p.StatusId), 
                TotalPrice = p.TotalPrice,
            });
        }

        private string GetTypeName(int typeId)
        {
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
            return statusId switch
            {
                1 => "Активен",
                2 => "Завершён",
                3 => "Отменён",
                _ => "Неизвестно"
            };
        }
    }
}
