using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelValidation
{
    public interface IModelValidator
    {
        IEnumerable<ValidationResult> Validate(ValidationContext context);
    }

    public interface IModelValidator<T> : IModelValidator where T : class
    {
        /// <summary>
        /// Returns an enumeration of validation results for the model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="scenario"></param>
        /// <returns>If the enumeration is empty, model validation succeeded.</returns>
        IEnumerable<ValidationResult> Validate(T model, int scenario = 0);

        /// <summary>
        /// If model validation fails, throws an ModelValidationException exception.
        /// </summary>
        /// <param name="model"></param>
        void ValidateAndThrow(T model);
    }
}