using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces.DTO
{
    public class Insurance
    {
        public int Id { get; set; }

        public int VehicleId { get; set; }

        public int TypeId { get; set; }

        public int StatusId { get; set; }

        public int? CancelledBy { get; set; }

        public string PolicyNumber { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public double BasePrice { get; set; }

        public double PowerCoefficient { get; set; }

        public double ExperienceCoefficient { get; set; }

        public double BonusMalusCoefficient { get; set; }

        public double TotalPrice => BasePrice * PowerCoefficient * ExperienceCoefficient * BonusMalusCoefficient;
        public string TypeName { get; set; } = null!;
        public string VehiclePlateNum { get; set; } = null!; 
        public string VehicleBrand { get; set; } = null!;
        public string VehicleModel { get; set; } = null!;
        public string StatusName { get; set; } = null!;

        public VehicleDto Vehicle { get; set; }
    }
}
