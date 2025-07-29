var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.GolfAllWeb>("webfrontend")
    .WithExternalHttpEndpoints();

builder.Build().Run();