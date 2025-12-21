using Core.Entities;
using DAL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class TypeOfPolicyRepository
    {
        private readonly InsuranceDbContext _context;

        public TypeOfPolicyRepository(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TypeOfPolicy>> GetAllAsync()
        {
            return await _context.TypeOfPolicies.ToListAsync();
        }
    }
}
