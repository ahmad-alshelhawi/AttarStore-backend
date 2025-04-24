using AttarStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Services
{

    
        public interface IRefreshTokenRepository
        {
            Task CreateAsync(RefreshToken token);
            Task<RefreshToken> GetByTokenAsync(string token);
            Task UpdateAsync(RefreshToken token);
        }
    
}
