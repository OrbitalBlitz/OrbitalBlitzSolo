public static class APIConfig
{
    public static string ApiUrl;

    static APIConfig()
    {
        Initialize();
    }

    private static void Initialize()
    {
        #if UNITY_EDITOR
        ApiUrl = "https://api-dev.example.com/";
        #else
        ApiUrl = "https://api.example.com/";
        #endif
    }
}