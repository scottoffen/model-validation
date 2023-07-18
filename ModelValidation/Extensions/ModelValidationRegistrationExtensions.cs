using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ModelValidation.Extensions
{
    public static class ModelValidationRegistrationExtensions
    {
        public const ServiceLifetime DefaultServiceLifetime = ServiceLifetime.Singleton;

        /// <summary>
        /// Scans the executing assembly for model validators and registers them to the dependency injection container using the specified service lifetime.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <remarks>The default service lifetime is singleton.</remarks>
        public static IServiceCollection AddModelValidators(this IServiceCollection services, ServiceLifetime serviceLifetime = DefaultServiceLifetime)
        {
            var assembly = Assembly.GetCallingAssembly();

            services.AddModelValidatorsFromAssembly(assembly, serviceLifetime);
            return services;
        }

        /// <summary>
        /// Scans the assembly containing the specified marker class for model validators and registers them to the dependency injection container using the specified service lifetime.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <typeparam name="T"></typeparam>
        /// <remarks>The default service lifetime is singleton.</remarks>
        public static IServiceCollection AddModelValidatorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime serviceLifetime = DefaultServiceLifetime) where T : class
        {
            services.AddModelValidatorsFromAssembly(typeof(T).Assembly, serviceLifetime);
            return services;
        }

        /// <summary>
        /// Scans the specified assembly for model validators and registers them to the dependency injection container using the specified service lifetime.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="serviceLifetime"></param>
        /// <remarks>The default service lifetime is singleton.</remarks>
        public static IServiceCollection AddModelValidatorsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime serviceLifetime = DefaultServiceLifetime)
        {
            var modelValidatorType = typeof(IModelValidator<>);

            var serviceDescriptors = (
                from type in assembly.GetTypes().Distinct()
                where !type.IsAbstract && !type.IsGenericTypeDefinition
                let interfaces = type.GetInterfaces()
                let genericInterfaces = interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == modelValidatorType)
                let matchingInterface = genericInterfaces.FirstOrDefault()
                where matchingInterface != null
                select new ServiceDescriptor(matchingInterface, type, serviceLifetime)
            );

            foreach (var serviceDescriptor in serviceDescriptors)
            {
                services.TryAdd(serviceDescriptor);
            }

            services.TryAdd(new ServiceDescriptor(typeof(IModelValidatorService), typeof(ModelValidatorService), serviceLifetime));

            return services;
        }
    }
}