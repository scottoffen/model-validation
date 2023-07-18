using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ModelValidation.Extensions
{
    public static class AutomaticModelValidationExtension
    {
        /// <summary>
        /// Enables automatic model validation in the ASP.NET pipeline using the Wsr.ModelValidation library.
        /// </summary>
        /// <param name="services"></param>
        /// <remarks>
        ///     <para>For automatic validation to work, it is requred that the validators have been registered in the service container.</para>
        ///     <para>It is recommended to use one or more of the following extension methods to automatically register validators:</para>
        ///     <list type="bullet">
        ///         <item><see cref="ModelValidationRegistrationExtensions.AddModelValidators(IServiceCollection, ServiceLifetime)">services.AddModelValidators(ServiceLifetime)</see></item>
        ///         <item><see cref="ModelValidationRegistrationExtensions.AddModelValidatorsFromAssembly(IServiceCollection, System.Reflection.Assembly, ServiceLifetime)">services.AddModelValidatorsFromAssembly(Assembly, ServiceLifetime)</see></item>
        ///         <item><see cref="ModelValidationRegistrationExtensions.AddModelValidatorsFromAssemblyContaining{T}(IServiceCollection, ServiceLifetime)">services.AddModelValidatorsFromAssemblyContaining{T}(ServiceLifetime)</see></item>
        ///     </list>
        /// </remarks>
        public static IServiceCollection UseAutomaticModelValidation(this IServiceCollection services)
        {
            if (!services.Any(s => s.ServiceType == typeof(IModelValidatorService)))
            {
                throw new InvalidOperationException($"Unable to resovle {nameof(IModelValidatorService)}. You may have forgotten to register model validators before calling this method.");
            }

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<ModelValidationActionFilter>();
            });

            return services;
        }
    }
}