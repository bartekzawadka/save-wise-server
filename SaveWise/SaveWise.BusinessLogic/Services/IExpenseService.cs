using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Filters;

namespace SaveWise.BusinessLogic.Services
{
    public interface IExpenseService
    {
        Task<IDictionary<string, List<Expense>>> GetAsync(string planId, ExpenseFilter filter);

        Task<Expense> GetOneAsync(string planId, string expenseId);

        Task UpdateAsync(string planId, string expenseId, Expense expense);

        Task InsertAsync(string planId, Expense expense);

        Task DeleteAsync(string planId, string expenseId);
    }
}