var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.GolfAllApi>("apiservice");
//.WithHttpHealthCheck("/health");

builder.AddProject<Projects.GolfAllWeb>("webfrontend")
    .WithExternalHttpEndpoints()
    //.WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();