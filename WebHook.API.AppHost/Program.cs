var builder = DistributedApplication.CreateBuilder(args);


var database = builder.AddPostgres("postgress").WithDataVolume().WithPgAdmin().AddDatabase("webhooks");


builder.AddProject<Projects.WebHook_API>("webhook-api").WithReference(database).WaitFor(database);

builder.Build().Run();
