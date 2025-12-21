// Interfaces/Services/ICurrentUserService.cs
using Interfaces.DTO;

namespace Interfaces.Services
{
    public interface ICurrentUserService
    {
        UserDto? CurrentUser { get; }
        bool IsAuthenticated { get; }

        void SetCurrentUser(UserDto user);
        void ClearCurrentUser(); // переименуем для ясности
        UserDto? GetCurrentUser();
    }
}