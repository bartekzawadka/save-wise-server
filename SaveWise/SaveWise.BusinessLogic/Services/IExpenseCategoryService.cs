using System.Collections.Generic;
using System.Threading.Tasks;
using SaveWise.DataLayer.Models;

namespace SaveWise.BusinessLogic.Services
{
    public interface IExpenseCategoryService : IService<ExpenseCategory>
    {
        Task InsertTypeAsync(string categoryId, ExpenseType expenseType);
    }
}