using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using System.Collections.Generic;

using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using StackExchange.Redis;

namespace AspNetCoreRateLimitWithRedis
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNetCoreRateLimitWithRedis", Version = "v1" });
            });

            services.Configure<IpRateLimitOptions>(options =>               // Defini o limite de cota por IP de Origem
            {
                options.GeneralRules = new List<RateLimitRule>()            // Regra de limite de requisição
                {
                    new()
                    {
                        Endpoint = ":/pedido",                              // Expressão regular para filtrar o recurso http a ser monitorado
                        Period = "1m",                                      // Período (s:segundo, m: minuto, h:hora e d:dia
                        Limit = 5,                                         // Total de requisições permitidas (para o período)
                        QuotaExceededResponse = new QuotaExceededResponse   //Padronizaçãop da resposta
                        {
                            Content = "Too Many Requests in 1m",            //Resposta
                            ContentType = "application/text",               // Tipo da resposta
                            StatusCode = 429                                //Codigo Http de retorno de estado
                        },
                    }
                };
                options.EnableEndpointRateLimiting = true;                  // Ativa cota de limite para endpoint customizado
                options.EnableRegexRuleMatching = true;                     // Habilita Regex
            });

            //Redis (Connection String)
            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("127.0.0.1"));
            services.AddRedisRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            //Necessário adicionar
            services.AddMvc();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCoreRateLimitWithRedis v1"));
            }

            // Ativa Middleware de RateLimit
            app.UseIpRateLimiting();

             app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
