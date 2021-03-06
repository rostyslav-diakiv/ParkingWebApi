﻿namespace MS.Common.Utils
{
    using System.IO;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.PlatformAbstractions;
    
    using Swashbuckle.AspNetCore.Swagger;

    /// <summary>
    /// The swagger configuration.
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Configures the swagger.
        /// </summary>
        /// <param name="services">
        /// The service collection.
        /// </param>
        /// <param name="configs">
        /// The configs.
        /// </param>
        /// <param name="microserviceName">
        /// The microservice Name.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configs)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Description = "Parking. Hometask №3",
                    Title = "Parking",
                    Version = "v1",
                    TermsOfService = "Knock yourself out",
                    Contact = new Contact
                    {
                        Name = configs["Contacts:fullName"],
                        Email = configs["Contacts:email"],
                        Url = configs["Contacts:webSite"]
                    },
                    License = new License
                    {
                        Name = "Apache 2.0",
                        Url = "http://www.apache.org/licenses/LICENSE-2.0.html"
                    }
                });

                options.DescribeAllEnumsAsStrings();

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Parking.WebApi.xml");

                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        /// <summary>
        /// The use configured swagger.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="microserviceName">
        /// The microservice name.
        /// </param>
        /// <returns>
        /// The <see cref="IApplicationBuilder"/>.
        /// </returns>
        public static IApplicationBuilder UseConfiguredSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "api.doc";
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Parking Web Api");

                    c.DefaultModelExpandDepth(2);
                    c.DefaultModelRendering(ModelRendering.Example);
                    c.DefaultModelsExpandDepth(-1);
                    c.DisplayOperationId();
                    c.DisplayRequestDuration();
                    c.DocExpansion(DocExpansion.None);
                    c.EnableDeepLinking();
                    c.EnableFilter();
                    c.MaxDisplayedTags(10);
                    c.ShowExtensions();
                    c.SupportedSubmitMethods(SubmitMethod.Get,
                        SubmitMethod.Post,
                        SubmitMethod.Put,
                        SubmitMethod.Delete,
                        SubmitMethod.Head,
                        SubmitMethod.Options,
                        SubmitMethod.Patch,
                        SubmitMethod.Trace);
                });

            return app;
        }
    }
}
