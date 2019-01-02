using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SaveWise.Api.Controllers
{
    [Route("api/[controller]")]
    public abstract class ControllerBase : Controller
    {
        protected object GetErrorFromModelState()
        {
            var error = new List<string>();
            foreach (ModelStateEntry modelStateValue in ModelState.Values)
            {
                foreach (ModelError modelError in modelStateValue.Errors)
                {
                    error.Add(modelError.ErrorMessage);
                }
            }

            return new {error = error.ToArray()};
        }

        protected object GetMessageObject(string message)
        {
            return new
            {
                error = message
            };
        }
    }
}