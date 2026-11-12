using Microsoft.Extensions.Configuration;

namespace ServerUNO
{
    public static class AppConfig
    {
        public static IConfiguration Configuration { get; private set; } = null!;

        public static void Load()
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>(optional: true)  // dev: читает secrets.json
                .AddEnvironmentVariables()                // prod/docker: ConnectionStrings__UnoDb
                .Build();
        }

        public static string ConnectionString =>
            Configuration.GetConnectionString("UnoDb")
            ?? throw new InvalidOperationException(
                "Не задана строка подключения 'UnoDb'. " +
                "Выполните: dotnet user-secrets set \"ConnectionStrings:UnoDb\" \"...\" --project ServerUNO/ServerUNO.csproj");
    }
}