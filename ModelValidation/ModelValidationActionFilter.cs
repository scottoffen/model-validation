using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ModelValidation
{
    public class ModelValidationActionFilter : IActionFilter
    {
        private readonly IModelValidatorService _modelValidatorService;

        public ModelValidationActionFilter(IModelValidatorService modelValidatorService)
        {
            this._modelValidatorService = modelValidatorService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // no-op
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // If the model state is invalid, then some other validation has already happened
            // Don't try to validate an already validated model.
            if (context.ModelState.IsValid)
            {
                // Iterate over each argument to the controller method
                foreach (var item in context.ActionArguments)
                {
                    // Create a validation context for the argument
                    var validationContext = new ValidationContext(item.Value, context.HttpContext.RequestServices, null);

                    // Iterate over the results and add them to the model state
                    foreach (var result in _modelValidatorService.Validate(validationContext))
                    {
                        if (result.MemberNames.Any())
                        {
                            foreach (var memberName in result.MemberNames)
                            {
                                context.ModelState.AddModelError(memberName, result.ErrorMessage);
                            }
                        }
                        else
                        {
                            context.ModelState.AddModelError(item.Value.GetType().Name, result.ErrorMessage);
                        }
                    }
                }

                // If any errors were found during model validation, return a 402
                if (!context.ModelState.IsValid)
                {
                    context.Result = new UnprocessableEntityObjectResult(context.ModelState);
                }
            }
        }
    }
}