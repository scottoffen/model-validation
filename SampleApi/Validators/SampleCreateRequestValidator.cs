using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ModelValidation;
using ModelValidation.Extensions;
using SampleApi.Models;

namespace SampleApi.Validators
{
    public class SampleCreateRequestValidator : ModelValidatorBase<SampleCreateRequest>
    {
        public override IEnumerable<ValidationResult> Validate(SampleCreateRequest model, int scenario = 0)
        {
            var allFieldsValidated = true;
            var hasPhoneOrEmail = !(string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Phone));
            var hasWebsite = string.IsNullOrWhiteSpace(model.Website);

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                allFieldsValidated = false;
                yield return new ValidationResult("Name is a required value", new[] { nameof(SampleCreateRequest.Name) });
            }

            if (!string.IsNullOrWhiteSpace(model.Email) && !model.Email.IsEmailAddress())
            {
                allFieldsValidated = false;
                yield return new ValidationResult("Not a valid email address", new[] { nameof(SampleCreateRequest.Email) });
            }

            if (!string.IsNullOrWhiteSpace(model.Phone) && !model.Phone.IsPhoneNumber())
            {
                allFieldsValidated = false;
                yield return new ValidationResult("Not a valid phone number", new[] { nameof(SampleCreateRequest.Phone) });
            }

            if (!string.IsNullOrWhiteSpace(model.Website) && !model.Website.IsUrl())
            {
                allFieldsValidated = false;
                yield return new ValidationResult("Invalid website url", new[] { nameof(SampleCreateRequest.Website) });
            }

            if (allFieldsValidated)
            {
                if (!hasPhoneOrEmail)
                {
                    yield return new ValidationResult("Either an email address or a phone number is required", new[] { nameof(SampleCreateRequest.Email), nameof(SampleCreateRequest.Phone) });
                }

                if (model.AutoRedirect && !hasWebsite)
                {
                    yield return new ValidationResult("A website url is required to use auto redirect", new[] { nameof(SampleCreateRequest.Website) });
                }
            }
        }
    }
}