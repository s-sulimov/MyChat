using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sulimov.MyChat.Server.BL.Services;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;
using Sulimov.MyChat.Server.Hubs;
using Sulimov.MyChat.Server.Services;
using System.Text;

namespace Sulimov.MyChat.Server
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        
        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<DataContext>(opt => opt.UseSqlServer(connectionString));

            services
                .AddIdentityCore<DbUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddSignInManager()
                .AddEntityFrameworkStores<DataContext>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidIssuer = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                        )
                    };
                });

            services.AddHttpContextAccessor();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthentificationService, AuthentificationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IUserService, UserService>();

            services.AddSwaggerGen();
            services.AddSignalR();

            services.AddControllers()
                .AddNewtonsoftJson();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                var dbContext = scope?.ServiceProvider.GetRequiredService<DataContext>();
                dbContext?.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpLogging();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("chat-hub");
            });
        }
    }
}
