using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project2.Areas.Identity.Data;
using Project2.Data;

[assembly: HostingStartup(typeof(Project2.Areas.Identity.IdentityHostingStartup))]
namespace Project2.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<Project2DbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("Project2DbContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options => 
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                })
                    .AddEntityFrameworkStores<Project2DbContext>();
            });
        }
    }
}