using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KeyCloakAuthention
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
            services.AddScoped<IClaimsTransformation, ClaimsTransformer>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "KeyCloakAuthention", Version = "v1" });
            });


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = "http://localhost:8080/auth/realms/my-realm";//Configuration["Jwt:Authority"];
                o.Audience = "demo-app";//Configuration["Jwt:Audience"];
                o.RequireHttpsMetadata = false;
                //o.TokenValidationParameters = new TokenValidationParameters
                //{
                //    RoleClaimType = ClaimTypes.Role
                //};
                //o.Events = new JwtBearerEvents()
                //{
                //    OnAuthenticationFailed = c =>
                //    {
                //        c.NoResult();

                //        c.Response.StatusCode = 500;
                //        c.Response.ContentType = "text/plain";
                //        return c.Response.;
                //    }
                //};
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "KeyCloakAuthention v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This interface IClaimsTransformation is called by method Authorize.
        public class ClaimsTransformer : IClaimsTransformation
        {
            public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
            {
                var claimsIdentity = (ClaimsIdentity)principal.Identity;

                // flatten realm_access because Microsoft identity model doesn't support nested claims
                // by map it to Microsoft identity model, because automatic JWT bearer token mapping already processed here
                if (claimsIdentity.IsAuthenticated && claimsIdentity.HasClaim(claim => claim.Type == "realm_access"))
                {
                    var realmAccessClaim = claimsIdentity.FindFirst(claim => claim.Type == "realm_access");
                    var realmAccessAsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string[]>>(realmAccessClaim.Value);
                    if (realmAccessAsDict["roles"] != null)
                    {
                        foreach (var role in realmAccessAsDict["roles"])
                        {
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                }

                return Task.FromResult(principal);
            }
        }
    }
}
