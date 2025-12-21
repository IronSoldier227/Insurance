using Interfaces.Services;
using Interfaces.DTO;

namespace BLL.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public UserDto? CurrentUser { get; private set; }

        public bool IsAuthenticated => CurrentUser != null;

        public void SetCurrentUser(UserDto user)
        {
            CurrentUser = user;
        }

        public void ClearCurrentUser()
        {
            CurrentUser = null;
        }

        public UserDto? GetCurrentUser()
        {
            return CurrentUser;
        }
    }
}