using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;

namespace SaveWise.BusinessLogic.Services
{
    public interface IExpenseService
    {
        Task<IList<Expense>> GetAsync(string planId);

        Task<Expense> GetOneAsync(string planId, string expenseId);
        
        Task UpdateAsync(string planId, Expense expense);

        Task InsertAsync(string planId, Expense expense);

        Task DeleteAsync(string planId, string expenseId);
    }
}