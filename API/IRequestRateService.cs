using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRate.API
{
    public interface IRequestRateService
    {
        public Task<string> GetRateAsync(string url); 
    }
}
