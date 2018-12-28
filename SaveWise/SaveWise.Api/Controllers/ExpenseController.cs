using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaveWise.BusinessLogic.Services;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Filters;
using SaveWise.DataLayer.Sys;

namespace SaveWise.Api.Controllers
{
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        private readonly IExpenseCategoryService _expenseCategoryService;

        public ExpenseController(IExpenseService expenseService, IExpenseCategoryService expenseCategoryService)
        {
            _expenseService = expenseService;
            _expenseCategoryService = expenseCategoryService;
        }

        [HttpPost("list/{planId}")]
        public async Task<IActionResult> GetExpenses(string planId, [FromBody] ExpenseFilter filter)
        {
            var expenses = await _expenseService.GetAsync(planId, filter);
            return Ok(expenses);
        }

        [HttpGet("{planId}/{expenseId}")]
        public async Task<IActionResult> GetById(string planId, string expenseId)
        {
            var expense = await _expenseService.GetOneAsync(planId, expenseId);
            return Ok(expense);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _expenseCategoryService.GetAsync<Filter<ExpenseCategory>>(null);
            return Ok(categories);
        }

        [HttpPost("categories/add")]
        public async Task<IActionResult> PostCategory([FromBody] ExpenseCategory category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }
            
            await _expenseCategoryService.InsertAsync(category);
            return Ok();
        }

        [HttpPost("categories/{categoryId}/type")]
        public async Task<IActionResult> PostCategoryType(string categoryId, [FromBody] ExpenseType type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            await _expenseCategoryService.InsertTypeAsync(categoryId, type);
            return Ok();
        }
        
        [HttpPost("{planId}")]
        public async Task<IActionResult> Post(string planId, [FromBody] Expense expense)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            await _expenseService.InsertAsync(planId, expense);
            return Ok();
        }
        
        [HttpPut("{planId}/{expenseId}")]
        public async Task<IActionResult> Put(string planId, string expenseId, [FromBody] Expense expense)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            await _expenseService.UpdateAsync(planId, expenseId, expense);
            return Ok();
        }
    }
}