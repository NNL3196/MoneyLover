using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Be.Api
{
    /// <summary>
    ///     Configures the Swagger generation options.
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly ILogger<ConfigureSwaggerOptions> _logger;
        private readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigureSwaggerOptions" /> class.
        /// </summary>
        /// <param name="provider">
        ///     The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger
        ///     documents.
        /// </param>
        /// <param name="logger">The logger used for logging Swagger operations.</param>
        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider,
            ILogger<ConfigureSwaggerOptions> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        /// <summary>
        ///     Applies the Swagger generation options.
        /// </summary>
        /// <param name="options">The Swagger generation options.</param>
        public void Configure(SwaggerGenOptions options)
        {
            // Add a swagger document for each discovered API version
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                _logger.LogInformation("Creating Swagger document for version: {GroupName}", description.GroupName);

                var info = new OpenApiInfo
                {
                    Title = "API",
                    Version = description.ApiVersion.ToString(),
                    Description = description.IsDeprecated
                        ? "This API version has been deprecated."
                        : "API documentation"
                };

                if (description.IsDeprecated)
                {
                    info.Description += " This API version has been deprecated.";
                }

                options.SwaggerDoc(description.GroupName, info);
            }
        }
    }
}