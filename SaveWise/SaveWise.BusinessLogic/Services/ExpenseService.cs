using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Filters;
using SaveWise.DataLayer.Sys.Exceptions;

namespace SaveWise.BusinessLogic.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IRepositoryFactory _repositoryFactory;

        public ExpenseService(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public async Task<IDictionary<string, List<Expense>>> GetAsync(string planId, ExpenseFilter filter)
        {
            IGenericRepository<Plan> planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            Plan plan = await planRepository.GetByIdAsync(planId);

            ExpenseFilter expenseFilter = filter ?? new ExpenseFilter();
            IEnumerable<Expense> expenses = plan.Expenses.Where(item => item.Amount > 0.0f);
            if (!string.IsNullOrWhiteSpace(expenseFilter.Category))
            {
                expenses = expenses.Where(item => !string.IsNullOrWhiteSpace(item.Category) &&
                                                  string.Equals(item.Category.Trim().ToLower(),
                                                      expenseFilter.Category.Trim().ToLower()));
            }

            if (expenseFilter.DateTo != null)
            {
                expenses = expenses.Where(item => item.Date != null && item.Date <= expenseFilter.DateTo);
            }

            if (expenseFilter.DateFrom != null)
            {
                expenses = expenses.Where(item => item.Date != null && item.Date >= expenseFilter.DateFrom);
            }

            expenses = expenses
                .Skip(expenseFilter.PageIndex * expenseFilter.PageSize)
                .Take(expenseFilter.PageSize);

            Dictionary<string, List<Expense>> dict = expenses
                .GroupBy(x => x.Category, x => x)
                .ToDictionary(expense => expense.Key, expense => expense.ToList());

            return dict;
        }

        public async Task<Expense> GetOneAsync(string planId, string expenseId)
        {
            if (string.IsNullOrWhiteSpace(planId) || string.IsNullOrWhiteSpace(expenseId))
            {
                throw new DocumentNotFoundException("Brak identyfikatora budżetu i/lub identyfikatora wydatku");
            }

            IGenericRepository<Plan> planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            Plan plan = await planRepository.GetByIdAsync(planId);
            return plan.Expenses?.Where(e => string.Equals(e.Id, expenseId)).SingleOrDefault();
        }

        public async Task UpdateAsync(string planId, string expenseId, Expense expense)
        {
            if (string.IsNullOrWhiteSpace(planId) || string.IsNullOrWhiteSpace(expenseId))
            {
                throw new DocumentNotFoundException("Brak identyfikatora budżetu i/lub identyfikatora wydatku");
            }

            IGenericRepository<Plan> planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            Plan plan = await planRepository.GetByIdAsync(planId);
            if (plan.Expenses == null)
            {
                plan.Expenses = new List<Expense>();
            }

            List<Expense> expenses = plan.Expenses.Where(e => !string.Equals(e.Id, expense.Id)).ToList();
            expenses.Add(expense);
            plan.Expenses = expenses;

            await planRepository.UpdateAsync(planId, plan);
        }

        public async Task InsertAsync(string planId, Expense expense)
        {
            IGenericRepository<Plan> planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            Plan plan = await planRepository.GetByIdAsync(planId);
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
            IGenericRepository<Plan> planRepository = _repositoryFactory.GetGenericRepository<Plan>();
            Plan plan = await planRepository.GetByIdAsync(planId);
            if (plan.Expenses == null)
            {
                return;
            }

            List<Expense> expenses = plan.Expenses.Where(e => !string.Equals(e.Id, expenseId)).ToList();
            plan.Expenses = expenses;

            await planRepository.UpdateAsync(planId, plan);
        }
    }
}