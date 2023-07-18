using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ModelValidation
{
    public interface IModelValidatorService
    {
        /// <summary>
        /// Returns a model validator for the specified class, or null if one does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        IModelValidator<T> GetModelValidator<T>() where T : class;

        /// <summary>
        /// Returns an enumeration of VaidationResult for the model specified in the ValidationContext. If no validator for the given model type is found, returns an empty enumeration.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<ValidationResult> Validate(ValidationContext context);

        /// <summary>
        /// Returns an enumeration of ValidationResult for the given model. If no validator for the given model type is found, throws a MissingModelValidatorException.
        /// </summary>
        /// <exception cref="MissingModelValidatorException">Thrown when no model validator can be found.</exception>
        /// <typeparam name="ValidationResult"></typeparam>
        IEnumerable<ValidationResult> Validate<T>(T model) where T : class;

        /// <summary>
        /// Validates the model specified and throws a ModelValidationException if there are any model validation errors.
        /// </summary>
        /// <exception cref="ModelValidationException">Thrown when the model fails to validate.</exception>
        /// <exception cref="MissingModelValidatorException">Thrown when no model validator can be found.</exception>
        /// <typeparam name="T"></typeparam>
        void ValidateAndThrow<T>(T model) where T : class;
    }
}