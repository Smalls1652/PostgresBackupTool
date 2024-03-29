using System.Text.Json.Serialization;
using PostgresBackupTool.ConsoleApp.Models;

namespace PostgresBackupTool.ConsoleApp;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Default,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    WriteIndented = true
)]
[JsonSerializable(typeof(BackupAppSettings))]
internal partial class CoreJsonContext : JsonSerializerContext
{}