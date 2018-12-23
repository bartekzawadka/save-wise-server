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

        public async Task<IList<Expense>> GetAsync(string planId)
        {
            var planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            var plan = await planRepository.GetByIdAsync(planId);
            return plan.Expenses;
        }

        public async Task<Expense> GetOneAsync(string planId, string expenseId)
        {
            var planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            var plan = await planRepository.GetByIdAsync(planId);
            return plan.Expenses?.Where(e => string.Equals(e.Id, expenseId)).SingleOrDefault();
        }

        public async Task UpdateAsync(string planId, Expense expense)
        {
            var planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            var plan = await planRepository.GetByIdAsync(planId);
            if (plan.Expenses == null)
            {
                plan.Expenses = new List<Expense>();
            }

            var expenses = plan.Expenses.Where(e => !string.Equals(e.Id, expense.Id)).ToList();
            expenses.Add(expense);
            plan.Expenses = expenses;

            await planRepository.UpdateAsync(planId, plan);
        }

        public async Task InsertAsync(string planId, Expense expense)
        {
            var planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            var plan = await planRepository.GetByIdAsync(planId);
            if (plan.Expenses == null)
            {
                plan.Expenses = new List<Expense>();
            }

            expense.Id = Guid.NewGuid().ToString();
            plan.Expenses.Add(expense);
            
            await planRepository.UpdateAsync(planId, plan);
        }

        public async Task DeleteAsync(string planId, string expenseId)
        {
            var planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            var plan = await planRepository.GetByIdAsync(planId);
            if (plan.Expenses == null)
            {
                return;
            }

            var expenses = plan.Expenses.Where(e => !string.Equals(e.Id, expenseId)).ToList();
            plan.Expenses = expenses;

            await planRepository.UpdateAsync(planId, plan);
        }
    }
}