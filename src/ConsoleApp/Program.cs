using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PostgresBackupTool.ConsoleApp;
using PostgresBackupTool.ConsoleApp.Extensions;
using PostgresBackupTool.ConsoleApp.Models;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddSlimHostLifetime();

builder.Configuration
    .AddJsonFile(
        path: "appsettings.json",
        optional: true,
        reloadOnChange: true
    )
    .AddJsonFile(
        path: $"appsettings.{builder.Environment}.json",
        optional: true,
        reloadOnChange: true
    )
    .AddEnvironmentVariables()
    .AddCommandLine(
        args: args,
        switchMappings: RootCommandLineMappings.SwitchMappings
    );

builder.Services
    .AddMainService(
        options =>
        {
            options.Host = builder.Configuration.GetValue<string>("DATABASE_HOST") ?? throw new NullReferenceException("DATABASE_HOST or --host is required");
            options.Port = builder.Configuration.GetValue<int>("DATABASE_PORT") == 0 ? 5432 : builder.Configuration.GetValue<int>("DATABASE_PORT");
            options.Username = builder.Configuration.GetValue<string>("DATABASE_USERNAME") ?? throw new NullReferenceException("DATABASE_USERNAME or --username is required");
            options.Password = builder.Configuration.GetValue<string>("DATABASE_PASSWORD") ?? throw new NullReferenceException("DATABASE_PASSWORD or --password is required");
            options.Database = builder.Configuration.GetValue<string>("DATABASE_NAME") ?? throw new NullReferenceException("DATABASE_NAME or --database is required");
            options.OutputPath = builder.Configuration.GetValue<string>("OUTPUT_PATH") ?? throw new NullReferenceException("OUTPUT_PATH or --output-path is required");

            string? backupLocationSettingValue = builder.Configuration.GetValue<string>("BACKUP_LOCATION");
            BackupLocation backupLocation = backupLocationSettingValue is not null
                ? Enum.Parse<BackupLocation>(backupLocationSettingValue, ignoreCase: true)
                : BackupLocation.Local;

            options.BackupLocation = backupLocation;

            if (backupLocation == BackupLocation.AzureBlobStorage)
            {
                options.AzureBlobStorageConfig = new AzureBlobStorageConfig
                {
                    EndpointUri = builder.Configuration.GetValue<Uri>("AZURE_BLOB_STORAGE_ENDPOINT_URI") ?? throw new NullReferenceException("AZURE_BLOB_STORAGE_ENDPOINT_URI is required"),
                    ContainerName = builder.Configuration.GetValue<string>("AZURE_BLOB_STORAGE_CONTAINER_NAME") ?? throw new NullReferenceException("AZURE_BLOB_STORAGE_CONTAINER_NAME is required")
                };
            }
        }
    );

var app = builder.Build();

await app.RunAsync();
