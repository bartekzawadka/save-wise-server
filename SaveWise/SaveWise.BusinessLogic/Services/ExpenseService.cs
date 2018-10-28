using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;

namespace SaveWise.BusinessLogic.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ExpenseService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task UpsertExpense(string planId, Expense expense)
        {
            var planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            var plan = await planRepository.GetByIdAsync(planId);
            if (plan.Expenses == null)
            {
                plan.Expenses = new List<Expense>();
            }

            if (string.IsNullOrEmpty(expense.Id))
            {
                expense.Id = new Guid().ToString();
                plan.Expenses.Add(expense);
            }
            else
            {
                var expenses = plan.Expenses.Where(e => !string.Equals(e.Id, expense.Id)).ToList();
                expenses.Add(expense);
                plan.Expenses = expenses;
            }
            
            await planRepository.UpdateAsync(planId, plan);
        }
    }
}