using Application.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence.Repositories;
using System.Text;

namespace Persistence.Contexts

{
    public static class ServiceRegistration
    {

        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrganizationDbContext>(options => options.UseSqlServer(Configuration.ConnectionString), ServiceLifetime.Singleton);
            services.AddScoped<IMemberReadRepository, MemberReadRepository>();
            services.AddScoped<IMemberWriteRepository, MemberWriteRepository>();
            services.AddScoped<IEventReadRepository, EventReadRepository>();
            services.AddScoped<IEventWriteRepository, EventWriteRepository>();
            services.AddScoped<ITicketReadRepository, TicketReadRepository>();
            services.AddScoped<ITicketWriteRepository, TicketWriteRepository>();
            services.AddScoped<ITicketCompanyWriteRepository, TicketCompanyWriteRepository>();
            services.AddScoped<ITicketCompanyReadRepository, TicketCompanyReadRepository>();
            services.AddScoped<IEventParticipantWriteRepository, EventParticipantWriteRepository>();
            services.AddScoped<IEventParticipantReadRepository, EventParticipantReadRepository>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                TokenOption tokenOption = configuration.GetSection("TokenOption").Get<TokenOption>();
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    
                    ValidIssuer = tokenOption.Issuer,
                    ValidAudience = tokenOption.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOption.SecretKey))
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin");
                });
                options.AddPolicy("User", policy =>
                {
                    policy.RequireRole("User");
                });
                options.AddPolicy("Company", policy =>
                {
                    policy.RequireRole("Company");
                });
            });

        }
    }
}
