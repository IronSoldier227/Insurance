// BLL/Services/VehicleService.cs
using Core.Entities;
using DAL.Repositories;
using Interfaces.DTO;
using Interfaces.Repository;
using Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IPolicyRepository _policyRepository;

        public VehicleService(IVehicleRepository vehicleRepository, IPolicyRepository policyRepository)
        {
            _vehicleRepository = vehicleRepository;
            _policyRepository = policyRepository;
        }

        // VehicleService.cs
        public async Task<IEnumerable<VehicleDto>> GetVehiclesByClientIdAsync(int clientId)
        {
            var vehicles = await _vehicleRepository.GetByClientIdAsync(clientId);

            var vehicleDtos = new List<VehicleDto>();
            foreach (var vehicle in vehicles)
            {
                var activePolicies = (await _policyRepository.GetByClientIdAsync(clientId))
                    .Where(p => p.VehicleId == vehicle.Id && p.StatusId == 1);

                vehicleDtos.Add(new VehicleDto
                {
                    Id = vehicle.Id,
                    ModelId = vehicle.ModelId,
                    Brand = vehicle.Model?.Brand?.Name,      // ✅ Получаем имя марки
                    Model = vehicle.Model?.Name,            // ✅ Получаем имя модели
                    ClientId = vehicle.ClientId,
                    Color = vehicle.Color,
                    YearOfProduction = vehicle.YearOfProduction,
                    Vin = vehicle.Vin,
                    PlateNum = vehicle.PlateNum,
                    Category = vehicle.Category,
                    PowerHp = vehicle.PowerHp,
                    IsInsured = activePolicies.Any()
                });
            }

            return vehicleDtos;
        }

        public async Task<VehicleDto?> GetVehicleByIdAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return null;

            var activePolicies = (await _policyRepository.GetByClientIdAsync(vehicle.ClientId))
                .Where(p => p.VehicleId == vehicle.Id && p.StatusId == 1);

            return new VehicleDto
            {
                Id = vehicle.Id,
                ModelId = vehicle.ModelId,
                Brand = vehicle.Model?.Brand?.Name,      // ✅
                Model = vehicle.Model?.Name,            // ✅
                ClientId = vehicle.ClientId,
                Color = vehicle.Color,
                YearOfProduction = vehicle.YearOfProduction,
                Vin = vehicle.Vin,
                PlateNum = vehicle.PlateNum,
                Category = vehicle.Category,
                PowerHp = vehicle.PowerHp,
                IsInsured = activePolicies.Any()
            };
        }

        public async Task<int> AddVehicleAsync(VehicleCreateDto dto)
        {
            var vehicle = new Vehicle
            {
                ModelId = dto.ModelId,
                ClientId = dto.ClientId,
                Color = dto.Color,
                YearOfProduction = dto.YearOfProduction,
                Vin = dto.Vin,
                PlateNum = dto.PlateNum,
                Category = dto.Category,
                PowerHp = dto.PowerHp
            };

            await _vehicleRepository.AddAsync(vehicle);

            if (_vehicleRepository is Repository<Vehicle> repo)
            {
                await repo.SaveChangesAsync();
            }
            else if (_vehicleRepository is VehicleRepository vehicleRepo)
            {
                await vehicleRepo.SaveChangesAsync(); // Если есть такой метод
            }

            return vehicle.Id;
        }

        // BLL/Services/VehicleService.cs
        // ...
        public async Task UpdateVehicleAsync(VehicleUpdateDto dto)
        {
            System.Diagnostics.Debug.WriteLine($"VehicleService.UpdateVehicleAsync вызван. DTO Id={dto.Id}, Color='{dto.Color}', PlateNum='{dto.PlateNum}', PowerHp={dto.PowerHp}");
            var vehicle = await _vehicleRepository.GetByIdAsync(dto.Id);
            if (vehicle == null)
            {
                System.Diagnostics.Debug.WriteLine($"VehicleService: Транспортное средство с Id={dto.Id} не найдено.");
                return;
            }
            System.Diagnostics.Debug.WriteLine($"VehicleService: Найдено транспортное средство. Id={vehicle.Id}, Color='{vehicle.Color}', PlateNum='{vehicle.PlateNum}', PowerHp={vehicle.PowerHp}");

            // Обновляем только те поля, которые пришли в DTO
            if (!string.IsNullOrEmpty(dto.Color))
            {
                vehicle.Color = dto.Color;
                System.Diagnostics.Debug.WriteLine($"VehicleService: Обновлён цвет на '{dto.Color}'");
            }
            if (!string.IsNullOrEmpty(dto.PlateNum))
            {
                vehicle.PlateNum = dto.PlateNum;
                System.Diagnostics.Debug.WriteLine($"VehicleService: Обновлён гос. номер на '{dto.PlateNum}'");
            }
            if (dto.PowerHp > 0) // Если мощность передана и положительна
            {
                vehicle.PowerHp = dto.PowerHp;
                System.Diagnostics.Debug.WriteLine($"VehicleService: Обновлена мощность на {dto.PowerHp}");
            }

            System.Diagnostics.Debug.WriteLine($"VehicleService: Сущность Vehicle после обновления: Id={vehicle.Id}, Color='{vehicle.Color}', PlateNum='{vehicle.PlateNum}', PowerHp={vehicle.PowerHp}");
            _vehicleRepository.Update(vehicle); 

            var changes = await _vehicleRepository.SaveChangesAsync();
            System.Diagnostics.Debug.WriteLine($"VehicleService: Сохранение изменений завершено. Изменено сущностей: {changes}");
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return;

            _vehicleRepository.Remove(vehicle);
            await _vehicleRepository.SaveChangesAsync();
        }

        public async Task<bool> CanInsureVehicleAsync(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null) return false;

            // Проверяем, есть ли активная страховка
            var policies = await _policyRepository.GetByClientIdAsync(vehicle.ClientId);
            return !policies.Any(p => p.VehicleId == vehicleId && p.StatusId == 1); // StatusId = 1 (Active)
        }
    }
}