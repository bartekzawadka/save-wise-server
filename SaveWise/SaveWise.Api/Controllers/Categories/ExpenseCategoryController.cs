using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaveWise.BusinessLogic.Services;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.Api.Controllers.Categories
{
    [Route("api/category/expense")]
    public class ExpenseCategoryController : ControllerBase
    {
        private readonly IService<ExpenseCategory> _categoryService;

        public ExpenseCategoryController(IService<ExpenseCategory> categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _categoryService.GetAsync<Filter<ExpenseCategory>>(null);
            return Ok(categories);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }
            
            var value = await _categoryService.GetByIdAsync(id);
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ExpenseCategory category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            await _categoryService.InsertAsync(category);
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] ExpenseCategory category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            await _categoryService.UpdateAsync(id, category);
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            var result = await _categoryService.DeleteAsync(id);
            return Ok(new {result});
        }
    }
}