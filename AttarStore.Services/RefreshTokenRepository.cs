using AttarStore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AttarStore.Services.RefreshTokenRepository;

namespace AttarStore.Services
{

        public class RefreshTokenRepository : IRefreshTokenRepository
        {
            private readonly AppDbContext _dbcontext;
            public RefreshTokenRepository(AppDbContext db) => _dbcontext = db;

            public async Task CreateAsync(RefreshToken token)
            {
                _dbcontext.RefreshTokens.Add(token);
                await _dbcontext.SaveChangesAsync();
            }

            public async Task<RefreshToken> GetByTokenAsync(string token) =>
                await _dbcontext.RefreshTokens
                         .SingleOrDefaultAsync(rt => rt.Token == token);

            public async Task UpdateAsync(RefreshToken token)
            {
                _dbcontext.RefreshTokens.Update(token);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }

