﻿using ITOps.ViewModelComposition;
using ITOps.ViewModelComposition.Gateway;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Divergent.CompositionGateway;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRouting();
        services.AddViewModelComposition();
        services.AddCors();

        services.AddOpenTelemetryTracing(config => config
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Divergent.CompositionGateway"))
            .AddZipkinExporter(o =>
            {
                o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
            })
            .AddJaegerExporter(c =>
            {
                c.AgentHost = "localhost";
                c.AgentPort = 6831;
            })
            .AddAspNetCoreInstrumentation()
            .AddSqlClientInstrumentation(opt => opt.SetDbStatementForText = true)
        );
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseCors(policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin();
            policyBuilder.AllowAnyMethod();
            policyBuilder.AllowAnyHeader();
        });

        app.RunCompositionGatewayWithDefaultRoutes();
    }
}