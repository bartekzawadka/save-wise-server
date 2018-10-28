using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SaveWise.Api.Controllers
{
    [Route("api/[controller]")]
    public abstract class ControllerBase : Controller
    {
        protected object GetErrorFromModelState()
        {
            var errors = new List<string>();
            foreach (ModelStateEntry modelStateValue in ModelState.Values)
            {
                foreach (ModelError modelError in modelStateValue.Errors)
                {
                    errors.Add(modelError.ErrorMessage);
                }
            }
            
            return new {errors = errors.ToArray()};
        }
    }
}