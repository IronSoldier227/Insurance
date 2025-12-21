using System.Collections.Generic;

namespace Interfaces.DTO
{
    public class ClientDetails
    {
        public Client Client { get; set; } = null!;
        public IEnumerable<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();
        public IEnumerable<Insurance> Policies { get; set; } = new List<Insurance>();
        public IEnumerable<Claim> Claims { get; set; } = new List<Claim>();
    }
}
