using TRIMS.Monitor.Entity;
using TRIMS.Monitor.Repository;

namespace TRIMS.Monitor.Manager
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ITransactionRepository _repository;
        public TransactionManager(ITransactionRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<TransactionReport>> GetTransactionReport(DateTime from, DateTime to)
        {
            return await _repository.GetTransactionReport(from, to);
        }
    }
}
