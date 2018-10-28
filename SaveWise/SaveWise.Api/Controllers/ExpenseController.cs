using Microsoft.AspNetCore.Mvc;
using SaveWise.BusinessLogic.Services;

namespace SaveWise.Api.Controllers
{
    [Route("api/[controller]")]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }
        
        // GET
        public string Index()
        {
            return "";
        }
    }
}