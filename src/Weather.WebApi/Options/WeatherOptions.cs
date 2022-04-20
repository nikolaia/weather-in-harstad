namespace Weather.WebApi.Options
{
    public record WeatherOptions
    {
        public string KeyVaultUri { get; init; } = string.Empty;
        public string StormSecretName { get; init; } = string.Empty;
    }
}