ModelValidation
===================

This package allows for model validation to be implemented with the following goals in mind:

1. The models being validated do not need to have attributes or implement a specific interface.
2. Model validation can occur in the ASP.NET pipeline
3. Model validation can be injected as a service and occur anywhere in the application

## Table of Contents

- [Available String Extensions](#available-string-extensions)
- [Reference Model](#reference-model)
- [Creating Model Validators](#creating-model-validators)
- [Specifying Different Types of Validation](#specifying-different-types-of-validation)
- [Validator Composition Using Dependency Injection](#validator-composition-using-dependency-injection)
- [Registering Model Validators With Dependency Injection Container](#registering-model-validators-with-dependency-injection-container)
- [Registering Automatic Model Validation](#registering-automatic-model-validation)
- [Using Manual Model Validation](#using-manual-model-validation)

## Available String Extensions

This package exports some string extensions that can be used for common validation, and makes use of the validation attributes of the same kind provided by `System.ComponentModel.DataAnnotations`. Each extension returns a boolean value indicating whether the string is valid.

- `IsEmailAddress`
    - alias for `System.ComponentModel.DataAnnotations.EmailAddressAttribute.IsValid`
    - validates whether the string is a valid email address
- `IsPhoneNumber`
    - alias for `System.ComponentModel.DataAnnotations.PhoneAttribute.IsValid`
    - validates wheter the string is a valid phone number
- `IsUrl`
    - alias for `System.ComponentModel.DataAnnotations.UrlAttribute.IsValid`
    - validates whether the string is a valid url

## Reference Model

The code examples below all use the following model:

```csharp
public class SampleRequest
{
    public bool AutoRedirect { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string Phone { get; set; }

    public string Website { get; set; }
}
```

The business rules for this sample model are:
1. `Name` is a required field
2. `Phone` is optional, but if present must be a valid phone number.
3. `Email` is optional, but if present must be a valid email address.
4. `Website` is optional, but if present must be a valid url.
5. The model is required to have either `Email` or `Phone` (can have both).
6. If `AutoRedirect` is true, `Webiste` must be present.

## Creating Model Validators

Create a model validator by extending the generic base class `ModelValidatorBase<T>` and implementing the `Validate` method.

> `ModelValidatorBase<T>` implements `IModelValidator<T>`. If you do not create the model validator using the abstract base class, you will need to implement the other methods on the interface.

The model validator below will validate the rules for the `SampleRequest` model outlined above.

> The recommended approach to model validation is to do field level validations first, and only do class level validations if all of the field level validations pass.

```csharp
public class SampleRequestValidator : ModelValidatorBase<SampleRequest>
{
    public override IEnumerable<ValidationResult> Validate(SampleRequest model, int scenario = 0)
    {
        var allFieldsAreValid = true;
        var hasPhoneOrEmail = !(string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Phone));
        var hasWebsite = !string.IsNullOrWhiteSpace(model.Website);

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            allFieldsAreValid = false;
            yield return new ValidationResult("Name is a required value", new[] { nameof(SampleRequest.Name) });
        }

        if (!string.IsNullOrWhiteSpace(model.Email) && !model.Email.IsEmailAddress())
        {
            allFieldsAreValid = false;
            yield return new ValidationResult("Not a valid email address", new[] { nameof(SampleRequest.Email) });
        }

        if (!string.IsNullOrWhiteSpace(model.Phone) && !model.Phone.IsPhoneNumber())
        {
            allFieldsAreValid = false;
            yield return new ValidationResult("Not a valid phone number", new[] { nameof(SampleRequest.Phone) });
        }

        if (!string.IsNullOrWhiteSpace(model.Website) && !model.Website.IsUrl())
        {
            allFieldsAreValid = false;
            yield return new ValidationResult("Invalid website url", new[] { nameof(SampleRequest.Website) });
        }

        if (allFieldsAreValid)
        {
            if (!hasPhoneOrEmail)
            {
                yield return new ValidationResult("Either an email address or a phone number is required", new[] { nameof(SampleRequest.Email), nameof(SampleRequest.Phone) });
            }

            if (model.AutoRedirect && !hasWebsite)
            {
                yield return new ValidationResult("A website url is required to use auto redirect", new[] { nameof(SampleRequest.Website) });
            }
        }
    }
}
```

## Specifying Different Types of Validation

You can specify different validation scenarios using the optional integer `scenario` parameter. Keep in mind that when validation is used in the ASP.NET pipeline, the default value (in this case, 0) is passed for the `scenario` parameter.

> When doing model validation in the ASP.NET pipeline, it is recommended to limit the validation to determining whether the format of the data is correct and whether the required set of fields have values. Deeper validations - e.g. detecting whether or not a specific id exists in the database - should not be done during the ASP.NET pipeline validation.

```csharp
public class SampleRequestValidator : ModelValidatorBase<SampleRequest>
{
    public const int DefaultValidation = 0;
    public const int ScenarioA = 1;
    public const int ScenarioB = 2;

    private readonly IMyService _myService;

    public SampleRequestValidator(IMyService myService)
    {
        this._myService = myService;
    }

    public override IEnumerable<ValidationResult> Validate(SampleRequest model, int scenario = 0)
    {
        switch (scenario)
        {
            case ScenarioA:
                return this.ScenarioAValidation(model);
            case ScenarioB:
                return this.ScenarioBValidation(model);
            case DefaultValidation:
            default:
                return this.DefaultValidation(model)
        }
    }

    private IEnumerable<ValidationResult> DefaultValidation(SampleRequest model)
    {
        // Here we ensure that the name field is populated.
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            yield return new ValidationResult("Name is a required value", new[] { nameof(SampleRequest.Name) });
        }
    }

    private IEnumerable<ValidationResult> ScenarioAValidation(SampleRequest model)
    {
        // Here we make use of the service that was injected to ensure
        // that the name provided is available for use.
        if (!this._myService.IsNameAvailable(model.Name))
        {
            yield return new ValidationResult($"Name {model.Name} is already in use", new[] { nameof(SampleRequest.Name) });
        }
    }

    private IEnumerable<ValidationResult> ScenarioBValidation(SampleRequest model)
    {
        // Do other validations here
        return Enumerable.Empty<ValidationResult>();
    }
}
```

## Validator Composition Using Dependency Injection

> Model validators can take advantage of dependency injection.

Using the sample request above, in a scenario where we have a request with the same payload and business rules, but with an additional field, we don't want to have to write and maintain that validation in two different places. In those intstaces, we can use inheritance and dependency injection to reuse validation code.

If we extend the `SampleRequest` class and make our additions:

```csharp
public class AnotherSampleRequest : SampleRequest
{
    public Guid Id { get; set; }
}
```

Then we can reuse the existing validator for `SampleRequest`:

```csharp
public class AnotherSampleRequestValidator : ModelValidatorBase<AnotherSampleRequest>
{
    private readonly IModelValidator<SampleRequest> _sampleRequestValidator;

    public AnotherSampleRequestValidator(IModelValidator<SampleRequest> sampleRequestValidator)
    {
        this._sampleRequestValidator = sampleRequestValidator;
    }

    public override IEnumerable<ValidationResult> Validate(AnotherSampleRequest model, int scenario = 0)
    {
        if (model.Id == Guid.Empty)
        {
            yield return new ValidationResult("Id is a required field", new[] { nameof(AnotherSampleRequest.Id) });
        }

        foreach (var result in this._sampleRequestValidator.Validate(model))
        {
            yield return result;
        }
    }
}
```

The important thing to remember is that changes are limited to being additive.

## Registering Model Validators With Dependency Injection container

Registering validators to the service collection should be done using one of the `IServiceCollection` discovery extension methods provided by the package.

- Only one registration method should be used.
- Each method takes an **optional** `ServiceLifetime` parameter
- The default `ServiceLifetime` if none is provided is `ServiceLifetime.Singleton`

Each method scans an assembly for classes that implement `IModelValidator<T>` and registers them to the service collection with the specified (or default) service lifetime.

- `services.AddModelValidators()` will scan the calling assembly.
- `services.AddModelValidatorsFromAssemblyContaining<T>()` will scan the assembly containing the marker class `T`.
- `services.AddModelValidatorsFromAssembly(Assembly assembly)` will scan the assembly specified.

All of these methods will also regsiter an implementation of `IModelValidatorService` using the same service lifetime. This service is used during automatic model validation and can also be injected into other services to provide access to your model validators.

> If multiples of the extension methods above are used, only the first instance discovered is registered, using the service lifetime specified by that discovery method.

## Registering Automatic Model Validation

Adding model validation to the ASP.NET request pipeline is optional, and is done using the `IServiceCollection` extension method `services.UseAutomaticModelValidation()`.

During model validation in the ASP.NET request pipeline, if no validator is found for the model type being validated, the validation is considered a success. If a validator is found and the model fails validation, an `UnprocessableEntityObjectResult` is returned with the model state. This will automatically integrate with any problem details middleware to return a `ProblemDetails` response.

## Using Manual Model Validation

Models can be validated anywhere in the project by injecting the `IModelValidatorService`. There are two methods that can be used to do model validation.

> Using the `IModelValidatorService` is recommended over injecting each `IModelValidator<T>` class that might be needed, especially in scenarios where one might not exist.

```csharp
public class SampleService
{
    private readonly IModelValidatorService _modelValidatorService;

    public SampleService(IModelValidatorService modelValidatorService)
    {
        this._modelValidatorService = modelValidatorService
    }

    public void SampleUsage(SampleModel model)
    {
        // If a model validator for the model does not exist, an exception of type
        // MissingModelValidatorException will be thrown.
        //
        // If the returned result set is empty, the model validated successfully.
        //
        // If the returned result set is not empty, the model validation failed.
        var results = this._modelValidatorService.Validate(model);

        // If a model validator for the model does not exist, an exception of type
        // MissingModelValidatorException will be thrown.
        //
        // If model validation failes, an exception of type ModelValidationException
        // will be thrown. This exception has a public ModelStateDictionary property
        // that will contain all of the model validation errors.
        this._modelValidatorService.ValidateAndThrow(model);
    }
}
```