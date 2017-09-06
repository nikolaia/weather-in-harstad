using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLib;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddEntityFrameworkNpgsql().AddDbContext<BloggingContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BloggingContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // var blog = new Blog
            // {
            //     Url = "http://blogs.msdn.com/dotnet",
            //     Posts = new List<Post>
            //     {
            //         new Post { Title = "Intro to C#" },
            //         new Post { Title = "Intro to VB.NET" },
            //         new Post { Title = "Intro to F#" }
            //     }
            // };

            // context.Blogs.Add(blog);
            // context.SaveChanges();

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
