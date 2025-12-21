// BLL/Services/UserService.cs
using Interfaces.Services;
using Interfaces.DTO;
using Interfaces.Repository;
using System.Threading.Tasks;
using Core.Entities;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Diagnostics; // Добавим для Debug.WriteLine

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly Interfaces.Repository.IUserRepository _userReadRepository;
        private readonly DAL.Repositories.UserWriteRepository _userWriteRepository;

        public UserService(Interfaces.Repository.IUserRepository userReadRepository, DAL.Repositories.UserWriteRepository userWriteRepository)
        {
            _userReadRepository = userReadRepository;
            _userWriteRepository = userWriteRepository;
        }

        public async Task<UserDto?> AuthenticateAsync(string login, string password)
        {
            var userDto = await _userReadRepository.GetByLoginAsync(login);
            if (userDto == null) return null;

            // For demo: use same hashing approach as AuthService
            var hash = ComputeHash(password);
            var provided = Encoding.UTF8.GetBytes(hash);
            if (userDto.PasswordHash == null) return null;
            if (provided.Length != userDto.PasswordHash.Length) return null;
            for (int i = 0; i < provided.Length; i++)
            {
                if (provided[i] != userDto.PasswordHash[i]) return null;
            }

            return userDto;
        }

        // BLL/Services/UserService.cs
        // ...
        public async Task<int> RegisterAsync(UserCreateDto dto)
        {
            // Check if login already exists
            var existing = await _userReadRepository.GetByLoginAsync(dto.Login);
            if (existing != null)
            {
                throw new System.InvalidOperationException("Login already exists");
            }

            var passwordHashString = ComputeHash(dto.Password);
            var user = new User
            {
                Login = dto.Login,
                PasswordHash = Encoding.UTF8.GetBytes(passwordHashString),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                PhoneNumber = dto.PhoneNumber,
                IsClient = dto.IsClient // <-- Устанавливаем тип
            };

            var id = await _userWriteRepository.AddUserAsync(user);

            if (dto.IsClient)
            {
                // Создаём ClientProfile
                var profile = new ClientProfile
                {
                    Id = id, // Id совпадает с Id User
                    Passport = dto.Passport,
                    DriverLicense = dto.DriverLicense,
                    DrivingExperience = dto.DrivingExperience
                };

                await _userWriteRepository.AddClientProfileAsync(profile);
            }
            else
            {
                // Создаём Manager
                var manager = new Manager
                {
                    Id = id // Id совпадает с Id User
                            // Другие поля Manager, если есть, заполняем здесь
                            // Например, может быть поле Position, но в текущей схеме только Id
                };

                await _userWriteRepository.AddManagerAsync(manager); // <-- Нужно добавить этот метод
            }

            return id;
        }
        // ...

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var userEntity = await _userReadRepository.GetByIdAsync(id); // Предположим, у вас есть такой метод в IUserRepository
            if (userEntity == null) return null;

            // Маппим в DTO
            return new UserDto
            {
                Id = userEntity.Id,
                Login = userEntity.Login,
                IsClient = userEntity.IsClient,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                MiddleName = userEntity.MiddleName,
                PhoneNumber = userEntity.PhoneNumber
                // PasswordHash обычно не передаётся в DTO для отображения
            };
        }

        private static string ComputeHash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}