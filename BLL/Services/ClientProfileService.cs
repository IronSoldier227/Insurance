using Interfaces.DTO;
using Interfaces.Repository;
using Interfaces.Services;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ClientProfileService : IClientProfileService
    {
        private readonly IRepository<Core.Entities.ClientProfile> _clientProfileRepository;

        public ClientProfileService(IRepository<Core.Entities.ClientProfile> clientProfileRepository)
        {
            _clientProfileRepository = clientProfileRepository;
        }

        public async Task<ClientProfileDto?> GetByUserIdAsync(int userId)
        {
            var entity = await _clientProfileRepository.GetByIdAsync(userId); // Id клиента = Id пользователя
            if (entity == null) return null;

            return new ClientProfileDto
            {
                Id = entity.Id,
                Passport = entity.Passport,
                DriverLicense = entity.DriverLicense,
                DrivingExperience = entity.DrivingExperience
            };
        }
    }
}