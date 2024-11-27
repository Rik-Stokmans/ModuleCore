using LogicLayer.Core;

namespace Authenticator;

public static class Authenticator
{
    public static bool IsAuthenticated(Dictionary<string, string> headers)
    {
        if (headers.TryGetValue("ApiKey", out var apiKey))
        {
            return Core.ValidateApiKey(apiKey);
        }
        
        if (headers.TryGetValue("HashedSerialNumber", out var hashedSerialNumber))
        {
            return Core.ValidateSerialNumber(hashedSerialNumber);
        }

        return false;
    }
}