using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVisionExercise
{
    public class AccountInfo
    {
        private readonly int _accountId;
        private readonly IAccountService _accountService;
        static SemaphoreSlim _sem = new SemaphoreSlim(1);
        public AccountInfo(int accountId, IAccountService accountService)
        {
            _accountId = accountId;
            _accountService = accountService;
        }
        public double Amount { get; private set; }

        public void RefreshAmount()
        {
            Amount = _accountService.GetAccountAmount(_accountId);
        }

        public async void RefreshAmountAsync()
        {
            _sem.Wait();
            Amount = await _accountService.GetAccountAmountAsync(_accountId);
            _sem.Release();
        }
    }
}
