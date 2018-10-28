using System.Threading.Tasks;
using SaveWise.DataLayer.Models;

namespace SaveWise.BusinessLogic.Services
{
    public interface IExpenseService
    {
        Task UpsertExpense(string planId, Expense expense);
    }
}