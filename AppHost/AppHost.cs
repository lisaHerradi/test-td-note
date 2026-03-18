var builder = DistributedApplication.CreateBuilder(args);

var mongoUser = builder.AddParameter("mongo-username", "filmapi");
var mongoPassword = builder.AddParameter("mongo-password", "filmapi", secret: true);

var mongo = builder.AddMongoDB("mongo", userName: mongoUser, password: mongoPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(port: 27017, targetPort: 27017, name: "mongo-external");

var mongodb = mongo.AddDatabase("mongodb");

builder.AddProject<Projects.FilmApi>("filmApi")
    .WaitFor(mongodb)
    .WithReference(mongodb)
    .WithUrl("/swagger"); // Trust launchSettings for 1803

builder.AddProject<Projects.SeedFilms>("seed-50k")
    .WaitFor(mongodb)
    .WithReference(mongodb)
    .WithArgs("50000")
    .WithExplicitStart();

builder.AddProject<Projects.SeedFilms>("seed-500k")
    .WaitFor(mongodb)
    .WithReference(mongodb)
    .WithArgs("500000")
    .WithExplicitStart();

var influx = builder.AddContainer("influxdb", "influxdb", "2.7-alpine")
    .WithHttpEndpoint(8086, 8086)
    .WithEnvironment("DOCKER_INFLUXDB_INIT_MODE", "setup")
    .WithEnvironment("DOCKER_INFLUXDB_INIT_ORG", "k6")
    .WithEnvironment("DOCKER_INFLUXDB_INIT_USERNAME", "admin")
    .WithEnvironment("DOCKER_INFLUXDB_INIT_PASSWORD", "admin1234")
    .WithEnvironment("DOCKER_INFLUXDB_INIT_BUCKET", "k6")
    .WithEnvironment("DOCKER_INFLUXDB_INIT_ADMIN_TOKEN", "k6-influxdb-token")
    .WithEnvironment("DOCKER_INFLUXDB_INIT_RETENTION", "7d");

builder.AddContainer("grafana", "grafana/grafana", "12.4.1")
    .WithHttpEndpoint(3000, 3000)
    .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "true")
    .WithEnvironment("GF_AUTH_ANONYMOUS_ORG_ROLE", "Admin")
    .WithEnvironment("GF_AUTH_DISABLE_LOGIN_FORM", "true")
    .WithUrl("/d/k6-load-testing/k6-load-testing")
    .WithBindMount("../grafana/provisioning", "/etc/grafana/provisioning")
    .WaitFor(influx);

builder.Build().Run();