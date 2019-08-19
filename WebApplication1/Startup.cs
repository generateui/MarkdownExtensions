using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkdownExtension.Excel;
using MarkdownExtensions;
using MarkdownExtensions.Extensions.FolderList;
using MarkdownExtensions.Extensions.FolderFromDisk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace WebApplication1
{
    public class Startup
    {
        private Container container = new Container();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var scope = new ThreadScopedLifestyle();
            container.Options.DefaultScopedLifestyle = scope;
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            //container.Register<IMarkdownConverter, MarkdownExtensionConverter>(Lifestyle.Scoped);
            //container.Register<Func<IMarkdownConverter>>(() => container.GetInstance<IMarkdownConverter>, Lifestyle.Scoped);
            //container.Register<Ea.ObjectText>(scope);
            //Ea.Plugin.Register(container);
            //container.Collection.Register<IMarkdownExtension>(
            //    typeof(FolderFromDisk),
            //    typeof(Folder),
            //    //typeof(GitGraph),
            //    //typeof(GitHistory),
            //    //typeof(Ea.ObjectText),
            //    //typeof(Ea.DiagramImage),
            //    //typeof(Ea.TableNotes),
            //    //typeof(Ea.RequirementsTable),
            //    //typeof(PanZoomImage),
            //    //typeof(Snippet),
            //    //typeof(MsSqlTable),
            //    //typeof(NestedBlockExample),
            //    //typeof(NestedInlineExample),
            //    typeof(ExcelTable)
            ////typeof(KeyboardKeys)
            //);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(options =>
            {
                options.AddPolicy("Access-Control-Allow-Origin",
                builder =>
                {
                    builder.WithOrigins("http://localhost")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
            });

            services.AddSimpleInjector(container, options =>
            {
                // AddAspNetCore() wraps web requests in a Simple Injector scope.
                options.AddAspNetCore()
                    // Ensure activation of a specific framework type to be created by
                    // Simple Injector instead of the built-in configuration system.
                    .AddControllerActivation()
                    .AddViewComponentActivation()
                    .AddPageModelActivation()
                    .AddTagHelperActivation();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // UseSimpleInjector() enables framework services to be injected into
            // application components, resolved by Simple Injector.
            app.UseSimpleInjector(container, options =>
            {
                // Add custom Simple Injector-created middleware to the ASP.NET pipeline.

                // Optionally, allow application components to depend on the
                // non-generic Microsoft.Extensions.Logging.ILogger abstraction.
                options.UseLogging();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseCors("Access-Control-Allow-Origin");
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
