using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.BusinessLogic.Services
{
    public class ExpenseCategoryService : Service<ExpenseCategory>, IExpenseCategoryService
    {
        public ExpenseCategoryService(IRepositoryFactory repositoryFactory) : base(repositoryFactory)
        {
        }

        public override async Task InsertAsync(ExpenseCategory document)
        {
            IGenericRepository<ExpenseCategory> repo = RepositoryFactory.GetGenericRepository<ExpenseCategory>();
            bool categoryExists = (await repo.GetAsync(new Filter<ExpenseCategory>
            {
                FilterExpression = category => string.Equals(category.Name, document.Name)
            })).Any();

            if (categoryExists || string.IsNullOrWhiteSpace(document.Name))
            {
                return;
            }

            await repo.InsertAsync(document);
        }

        public async Task InsertTypeAsync(string categoryId, ExpenseType expenseType)
        {
            IGenericRepository<ExpenseCategory> repo = RepositoryFactory.GetGenericRepository<ExpenseCategory>();
            ExpenseCategory category = await repo.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new Exception("Wskazana kategoria wydatków nie została odnaleziona");
            }

            List<ExpenseType> types = category.Types?.ToList();
            if (types?.Any(type => string.Equals(type.Name, expenseType.Name)) != true)
            {
                if (types == null)
                {
                    types = new List<ExpenseType>();
                }

                types.Add(expenseType);
                category.Types = types;
                await repo.UpdateAsync(categoryId, category);
            }
        }
    }
}