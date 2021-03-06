﻿using System.Threading.Tasks;

namespace CakeBuildSample
{
    public interface IAccountService
    {
        double GetAccountAmount(int accountId);
        Task<double> GetAccountAmountAsync(int accountId);
    }
}
