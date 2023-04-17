using TRIMS.Monitor.Entity;

namespace TRIMS.Monitor.Repository
{
    public interface ITransactionRepository
    {
        public Task<List<TransactionReport>> GetTransactionReport(EnvironmentType environment, DateTime from, DateTime to);
    }
}
