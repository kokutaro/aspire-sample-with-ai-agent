var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("postgresdb");
    //.WithPgAdmin();

var redis = builder.AddRedis("redis");

var apiservice = builder.AddProject<Projects.MyAspireApp_ApiService>("apiservice")
    .WithReference(postgresdb)
    .WithReference(redis);

var frontend = builder.AddNpmApp("frontend", "../../myaspireapp-frontend")
    .WithReference(apiservice)
    .WithHttpEndpoint(targetPort: 5173, isProxied: true)
    .WithEnvironment("VITE_API_SERVICE_URL", apiservice.GetEndpoint("http"));

builder.AddProject<Projects.MyAspireApp_Web>("webproxy")
    .WithReference(apiservice)
    .WithReference(frontend);

builder.Build().Run();
