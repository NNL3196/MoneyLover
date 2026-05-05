using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Be.Api
{
    /// <summary>
    ///     Represents the OpenAPI/Swagger operation filter used to document information for API versions.
    /// </summary>
    public class SwaggerDefaultValues : IOperationFilter
    {
        private readonly ILogger<SwaggerDefaultValues> _logger;

        public SwaggerDefaultValues(ILogger<SwaggerDefaultValues> logger)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            _logger.LogDebug("Processing operation: {RelativePath}", apiDescription.RelativePath);

            // Mark operation as deprecated if needed
            operation.Deprecated |= apiDescription.IsDeprecated();

            // Set operation ID
            if (operation.OperationId == null)
            {
                operation.OperationId = context.MethodInfo.Name;
            }

            // Set API version parameter value
            if (operation.Parameters != null)
            {
                foreach (var parameter in operation.Parameters)
                {
                    var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);

                    if (description == null)
                    {
                        continue;
                    }

                    if (parameter.Description == null)
                    {
                        parameter.Description = description.ModelMetadata?.Description;
                    }

                    // Note: IOpenApiSchema.Default is read-only in the current version of the OpenAPI library
                    // Default values cannot be set through the operation filter
                    // if (parameter.Schema != null && parameter.Schema.Default == null && description.DefaultValue != null)
                    // {
                    //     // Default value assignment not supported
                    // }

                    //parameter.Required |= description.IsRequired;
                }
            }
        }
    }
}