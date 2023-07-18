using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using ModelValidation.Exceptions;

namespace ModelValidation
{
    public class ModelValidatorService : IModelValidatorService
    {
        private static readonly Type _interfaceType = typeof(IModelValidator<>);

        private static readonly string _missingModelValidatorMessage = "The service provider does not contain a registered implementation for IModelValidator<$0>. Either it has not been created or it has not been register in the services collection.";

        private readonly IServiceProvider _provider;

        public ModelValidatorService(IServiceProvider provider)
        {
            this._provider = provider;
        }

        public IModelValidator<T> GetModelValidator<T>() where T : class
        {
            var validator = this._provider.GetService<IModelValidator<T>>();
            return validator;
        }

        public IEnumerable<ValidationResult> Validate<T>(T model) where T : class
        {
            var validator = this._provider.GetService<IModelValidator<T>>();

            if (validator != null)
            {
                return validator.Validate(model);
            }
            else
            {
                throw new MissingModelValidatorException(string.Format(_missingModelValidatorMessage, model.GetType().Name));
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var genericType = _interfaceType.MakeGenericType(context.ObjectType);

            var validator = this._provider.GetService(genericType) as IModelValidator;

            if (validator != null)
            {
                return validator.Validate(context);
            }

            return Enumerable.Empty<ValidationResult>();
        }

        public void ValidateAndThrow<T>(T model) where T : class
        {
            var validator = this._provider.GetRequiredService<IModelValidator<T>>();

            if (validator != null)
            {
                validator.ValidateAndThrow(model);
            }
            else
            {
                throw new MissingModelValidatorException(string.Format(_missingModelValidatorMessage, model.GetType().Name));
            }
        }
    }
}