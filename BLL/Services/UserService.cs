using Interfaces.Services;
using Interfaces.DTO;
using Interfaces.Repository;
using System.Threading.Tasks;
using Core.Entities;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Diagnostics; 

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

        public async Task<int> RegisterAsync(UserCreateDto dto)
        {
            var existing = await _userReadRepository.GetByLoginAsync(dto.Login);
            if (existing != null)
            {
                throw new System.InvalidOperationException("ѕользователь с данным логином уже существует");
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
                IsClient = dto.IsClient 
            };

            var id = await _userWriteRepository.AddUserAsync(user);

            if (dto.IsClient)
            {
                var profile = new ClientProfile
                {
                    Id = id, 
                    Passport = dto.Passport,
                    DriverLicense = dto.DriverLicense,
                    DrivingExperience = dto.DrivingExperience
                };

                await _userWriteRepository.AddClientProfileAsync(profile);
            }
            else
            {
                var manager = new Manager
                {
                    Id = id 
                };

                await _userWriteRepository.AddManagerAsync(manager); 
            }

            return id;
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var userEntity = await _userReadRepository.GetByIdAsync(id); 
            if (userEntity == null) return null;

            return new UserDto
            {
                Id = userEntity.Id,
                Login = userEntity.Login,
                IsClient = userEntity.IsClient,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                MiddleName = userEntity.MiddleName,
                PhoneNumber = userEntity.PhoneNumber
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