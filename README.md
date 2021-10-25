# AspNetCoreRateLimitWithRedis

## Passo 1: Instalar NuGets

```cs
install-package AspNetCoreRateLimit
install-package AspNetCoreRateLimit.Redis
```

##  Passo 2: Adicionar na classe Startup.cs (ConfigureServices)

```cs
services.Configure<IpRateLimitOptions>(options =>               // Defini o limite de cota por IP de Origem
{
    options.GeneralRules = new List<RateLimitRule>()            // Regra de limite de requisição
    {
        new()
        {
            Endpoint = ":/pedido",                              // Expressão regular para filtrar o recurso http a ser monitorado
            Period = "1d",                                      // Período (s:segundo, m: minuto, h:hora e d:dia
            Limit = 100,                                        // Total de requisições permitidas (para o período)
            QuotaExceededResponse = new QuotaExceededResponse   //Padronizaçãop da resposta
            {
                Content = "Too Many Requests in 1d",            //Resposta
                ContentType = "application/json",               // Tipo da resposta
                StatusCode = 429                                //Codigo Http de retorno de estado
            },
        }
    };
    options.EnableEndpointRateLimiting = true;                  // Ativa cota de limite para endpoint customizado
    options.EnableRegexRuleMatching = true;                     // Habilita Regex
});

//Redis IP - Connection String
services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("127.0.0.1")); 
services.AddRedisRateLimiting();
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
```

## Passo 3: Adicionar na classe Startup.cs (Configure)

```cs
app.UseIpRateLimiting();    // Ativa o uso do Middleware de RateLimit
```
