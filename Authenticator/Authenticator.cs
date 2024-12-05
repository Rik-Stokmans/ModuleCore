using LogicLayer.Authentication;

namespace Authenticator;

public static class Authenticator
{
    public static bool IsAuthenticated(Dictionary<string, string> headers)
    {
        if (headers.TryGetValue("ApiKey", out var apiKey))
        {
            return AuthenticationCore.ValidateApiKey(apiKey);
        }
        
        if (headers.TryGetValue("HashedSerialNumber", out var hashedSerialNumber))
        {
            return AuthenticationCore.ValidateSerialNumber(hashedSerialNumber);
        }

        return false;
    }
}