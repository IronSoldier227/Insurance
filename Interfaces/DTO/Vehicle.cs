using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.DTO
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int ClientId { get; set; }
        public string? DriverName { get; set; }
        public string? Color { get; set; }
        public int YearOfProduction { get; set; }
        public string? Vin { get; set; }
        public string? PlateNum { get; set; }
        public string? Category { get; set; }
        public int PowerHp { get; set; }
        public bool IsInsured { get; set; } // Добавим флаг застрахованности
    }

    public class VehicleCreateDto
    {
        public int ModelId { get; set; }
        public int ClientId { get; set; }
        public string Color { get; set; } = string.Empty;
        public int YearOfProduction { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string PlateNum { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PowerHp { get; set; }
    }

    public class VehicleUpdateDto
    {
        public int Id { get; set; }
        public string Color { get; set; } = string.Empty;
        public string PlateNum { get; set; } = string.Empty;
        public int PowerHp { get; set; }
    }
}
