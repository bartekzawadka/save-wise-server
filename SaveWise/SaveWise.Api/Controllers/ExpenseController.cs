using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaveWise.BusinessLogic.Services;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Models.Filters;

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
            IDictionary<string, List<Expense>> expenses = await _expenseService.GetAsync(planId, filter);
            return Ok(expenses);
        }

        [HttpGet("{planId}/{expenseId}")]
        public async Task<IActionResult> GetById(string planId, string expenseId)
        {
            Expense expense = await _expenseService.GetOneAsync(planId, expenseId);
            return Ok(expense);
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

        [HttpDelete("{planId}/{expenseId}")]
        public async Task<IActionResult> Delete(string planId, string expenseId)
        {
            await _expenseService.DeleteAsync(planId, expenseId);
            return Ok();
        }
    }
}