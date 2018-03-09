using System.Threading.Tasks;

namespace EVisionExercise
{
    public interface IAccountService
    {
        double GetAccountAmount(int accountId);
        Task<double> GetAccountAmountAsync(int accountId);
    }
}
