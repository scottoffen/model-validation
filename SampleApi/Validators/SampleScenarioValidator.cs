using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ModelValidation;
using SampleApi.Models;

namespace SampleApi.Validators
{
    public class SampleScenarioValidator : ModelValidatorBase<SampleCreateRequest>
    {
        public const int DefaultScenario = 0;
        public const int ScenarioA = 1;
        public const int ScenarioB = 2;

        public override IEnumerable<ValidationResult> Validate(SampleCreateRequest model, int scenario = 0)
        {
            switch (scenario)
            {
                case ScenarioA:
                    return this.ScenarioAValidation(model);
                case ScenarioB:
                    return this.ScenarioBValidation(model);
                case DefaultScenario:
                default:
                    return this.DefaultScenarioValidation(model);
            }
        }

        private IEnumerable<ValidationResult> DefaultScenarioValidation(SampleCreateRequest model)
        {
            return Enumerable.Empty<ValidationResult>();
        }

        private IEnumerable<ValidationResult> ScenarioAValidation(SampleCreateRequest model)
        {
            return Enumerable.Empty<ValidationResult>();
        }

        private IEnumerable<ValidationResult> ScenarioBValidation(SampleCreateRequest model)
        {
            return Enumerable.Empty<ValidationResult>();
        }
    }
}