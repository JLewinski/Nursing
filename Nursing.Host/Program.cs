var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Nursing_API>("API")
    .WithExternalHttpEndpoints();

builder.AddNpmApp("Svelte", "../Nursing.Svelte", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpsEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
