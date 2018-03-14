using System;
using System.Threading;
using System.Threading.Tasks;

namespace CakeBuildSample
{
    public class AccountInfo
    {
        private readonly int _accountId;
        private readonly IAccountService _accountService;
        static readonly SemaphoreSlim _sem = new SemaphoreSlim(1);

        public double Amount { get; private set; }

        public AccountInfo(int accountId, IAccountService accountService)
        {
            _accountId = accountId;
            _accountService = accountService;
        }

        public void RefreshAmount()
        {
            Amount = _accountService.GetAccountAmount(_accountId);
        }

        public async Task RefreshAmountAsync()
        {
            try
            {
                await _sem.WaitAsync();
                Amount = await _accountService.GetAccountAmountAsync(_accountId);
            }
            catch (Exception ex)
            {
                //maybe a more proper logging here
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                _sem.Release();
            }
        }
    }
}
