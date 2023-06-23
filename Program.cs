internal partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        
        // Read the OpenAi Api Key and Database Password from environment variable that you set with dotnet user-secrets set command.
        string openAiApiKey = builder.Configuration["OpenAI:APIKey"] ?? String.Empty;
        string pgsqlDbPassword = builder.Configuration["PGSQL:DbPassword"] ?? String.Empty;

        // Declare a dictionary variable to load and store the settings from Settings.user file
        Dictionary<string, string>? settings = GetSettings(); 

        // Create Connection String for NPGSQL
        string connectionString = $"Host={settings["HOST"]};Username={settings["USERNAME"]};Password={pgsqlDbPassword};Database={settings["DATABASE"]}"; 

        var app = builder.Build();

        app.MapGet("/", () => connectionString);

        app.MapGet("/users", () => GetUsers(connectionString));

        app.MapGet("/user/{userid}", (int userId) => GetUserById(userId, connectionString));

        app.Run();
    }
}