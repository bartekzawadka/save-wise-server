using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SaveWise.BusinessLogic.Services;
using SaveWise.DataLayer.Models;
using SaveWise.DataLayer.Sys;

namespace SaveWise.Api.Controllers
{
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var plans = await _planService.GetAsync<Filter<Plan>>(null);
            return Ok(plans);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            return Ok(await _planService.GetCurrentPlanAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            var document = await _planService.GetByIdAsync(id);
            return Ok(document);
        }

        [HttpGet("new")]
        public async Task<IActionResult> GetNewPlanIncomeCategories()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            var result = await _planService.GetNewPlanAsync();
            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Plan plan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            try
            {
                await _planService.InsertAsync(plan);
            }
            catch (DuplicateNameException e)
            {
                return BadRequest(new ErrorResult(e.Message).ToJson());
            }

            return Ok(plan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Plan plan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }
            
            await _planService.UpdateAsync(id, plan);
            return Ok(plan);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }
            
            var result = await _planService.DeleteAsync(id);
            return Ok(result);
        }
    }
}