using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveWise.DataLayer;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;
using SaveWise.DataLayer.Sys.Exceptions;

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
            ExpenseCategory expenseCategory = (await repo.GetAsync(
                new Filter<ExpenseCategory>()
                    .AppendFilters(category => string.Equals(category.Name, document.Name))
            )).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(document.Name))
            {
                return;
            }

            if (expenseCategory == null)
            {
                await repo.InsertAsync(document);
                return;
            }

            List<string> existingTypeNames = expenseCategory.Types?.Select(x => x.Name).ToList();
            if (existingTypeNames == null)
            {
                if (document.Types?.Any() == true)
                {
                    var newTypes = new List<ExpenseType>();
                    newTypes.AddRange(document.Types);
                    expenseCategory.Types = newTypes;
                    await repo.UpdateAsync(expenseCategory.Id, expenseCategory);
                }
            }
            else
            {
                List<ExpenseType> toBeAdded = document
                    .Types?
                    .Where(x => !existingTypeNames.Contains(x.Name))
                    .ToList();

                if (toBeAdded != null && toBeAdded.Count > 0)
                {
                    List<ExpenseType> types = expenseCategory.Types.ToList();                    
                    types.AddRange(toBeAdded);
                    expenseCategory.Types = types;
                    await repo.UpdateAsync(expenseCategory.Id, expenseCategory);
                }
            }
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

        public override async Task InsertManyAsync(IEnumerable<ExpenseCategory> documents)
        {
            var newCategories = documents?.ToList();
            if (newCategories?.Any() != true)
            {
                return;
            }
            
            IList<ExpenseCategory> expenseCategories = await GetAsync<Filter<ExpenseCategory>>(null);

            Dictionary<string, IEnumerable<ExpenseType>> expenseCategoriesDict = expenseCategories
                .ToDictionary(category => category.Name, category => category.Types);
                
            List<string> expenseCategoriesNames = expenseCategories.Select(ec => ec.Name).ToList();
            
            var categoriesToBeAdded = new List<ExpenseCategory>();

            foreach (ExpenseCategory expenseCategory in newCategories)
            {
                if (!expenseCategoriesNames.Contains(expenseCategory.Name))
                {
                    categoriesToBeAdded.Add(new ExpenseCategory
                    {
                        Name = expenseCategory.Name,
                        Types = expenseCategory.Types
                    });
                    continue;
                }

                List<string> existingTypeNames = expenseCategoriesDict[expenseCategory.Name].Select(x => x.Name).ToList();
                List<ExpenseType> typesToBeAdded = expenseCategory
                    .Types?
                    .Where(x => !existingTypeNames.Contains(x.Name))
                    .ToList();

                if (typesToBeAdded?.Any() != true)
                {
                    continue;
                }

                ExpenseCategory currentCategory = (await GetAsync(
                        new Filter<ExpenseCategory>()
                            .AppendFilters(
                                category => string.Equals(category.Name, expenseCategory.Name)))
                    ).SingleOrDefault();
                
                if (currentCategory == null)
                {
                    throw new DocumentNotFoundException(
                        $"Nie odnaleziono kategorii wydatków o nazwie '{expenseCategory.Name}'");
                }

                List<ExpenseType> currentCategoryTypes = currentCategory.Types?.ToList() ?? new List<ExpenseType>();
                currentCategoryTypes.AddRange(typesToBeAdded);
                currentCategory.Types = currentCategoryTypes;

                await UpdateAsync(currentCategory.Id, currentCategory);
            }

            if (categoriesToBeAdded.Count > 0)
            {
                await base.InsertManyAsync(categoriesToBeAdded);
            }
        }
    }
}