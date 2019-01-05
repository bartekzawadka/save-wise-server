using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaveWise.BusinessLogic.Services;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.Api.Controllers
{
    [Authorize]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeCategoryService _incomeCategoryService;

        public IncomeController(IIncomeCategoryService incomeCategoryService)
        {
            _incomeCategoryService = incomeCategoryService;
        }
        
        [HttpGet("list")]
        public async Task<IActionResult> Get()
        {
            var incomeCategories = await _incomeCategoryService.GetAsync<Filter<IncomeCategory>>(null);
            return Ok(incomeCategories);
        }
    }
}