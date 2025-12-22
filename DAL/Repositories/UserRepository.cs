using DAL.Context;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Interfaces.DTO;

namespace DAL.Repositories
{
    public class UserRepository : Interfaces.Repository.IUserRepository
    {
        private readonly InsuranceDbContext _context;

        public UserRepository(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> GetByLoginAsync(string login)
        {
            var user = await _context.Users.Include(u => u.ClientProfile).FirstOrDefaultAsync(u => u.Login == login);
            if (user == null) return null;

            return new UserDto { Id = user.Id, Login = user.Login, IsClient = user.IsClient, PasswordHash = user.PasswordHash };
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id); 
        }
    }
}
