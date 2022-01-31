using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using OpenIddict.Server;
using Shared;
using Shared.Services;

namespace VaccinationCertificateIssuerWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder applicationBuilder, IWebHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
            {
                applicationBuilder.UseDeveloperExceptionPage();
                applicationBuilder.UseForwardedHeaders();
            }
            else
            {
                applicationBuilder.UseExceptionHandler("/Home/Error");
                applicationBuilder.UseForwardedHeaders();
                applicationBuilder.UseHsts();
            }

            //applicationBuilder.UseHttpsRedirection();
            applicationBuilder.UseStaticFiles();
            applicationBuilder.UseCookiePolicy();
            applicationBuilder.UseRouting();
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseAuthorization();

            applicationBuilder.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<AppSettingsModel>()
                .Configure(options =>
                {
                    Configuration.Bind("AppSettings", options);
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.HandleSameSiteCookieCompatibility();
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

            services.AddAuthentication()
                .AddCookie(IdentityConstants.ApplicationScheme)
                .AddMicrosoftIdentityWebApp(Configuration, "AzureAdB2C");

            services.AddControllersWithViews();

            services.AddOpenIddict()
                .AddServer(serverBuilder =>
                {
                    serverBuilder.SetAuthorizationEndpointUris("/oauth2/authorize")
                        .SetTokenEndpointUris("/oauth2/token")
                        .SetUserinfoEndpointUris("/oauth2/userinfo");

                    serverBuilder.AllowAuthorizationCodeFlow();

                    serverBuilder.AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey();

                    serverBuilder.EnableDegradedMode();

                    serverBuilder.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableUserinfoEndpointPassthrough();

                    serverBuilder.AddEventHandler<OpenIddictServerEvents.ValidateAuthorizationRequestContext>(serverEventBuilder =>
                    {
                        serverEventBuilder.UseInlineHandler(serverEventHandler =>
                        {
                            return default;
                        });
                    });

                    serverBuilder.AddEventHandler<OpenIddictServerEvents.ValidateTokenRequestContext>(serverEventBuilder =>
                    {
                        serverEventBuilder.UseInlineHandler(serverEventHandler =>
                        {
                            return default;
                        });
                    });
                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            services.AddTransient<IVerifiableCredentialsRequestService, VerifiableCredentialsRequestService>();

            services.AddSingleton(serviceProvider =>
            {
                var appSettingsAccessor = serviceProvider.GetRequiredService<IOptions<AppSettingsModel>>();
                var appSettings = appSettingsAccessor.Value;

                return ConfidentialClientApplicationBuilder.Create(appSettings.ClientId)
                    .WithClientSecret(appSettings.ClientSecret)
                    .WithAuthority(new Uri(appSettings.Authority))
                    .Build();
            });
        }
    }
}
