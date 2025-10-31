namespace NodeTrees.Server
{
    internal static class NLogConfiguration
    {
        public static void ConfigureNLog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
        }
    }
}
