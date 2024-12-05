using LogicLayer.Authentication.Interfaces;

namespace LogicLayer.Authentication;

public static class AuthenticationCore
{
    public static bool ValidateApiKey(string apiKey)
    {
        return Core.GetService<IAuthenticationService>().ApiKeyIsAuthenticated(apiKey);
    }
    
    public static bool ValidateSerialNumber(string hashedSerialNumber)
    {
        var serialNumbers = LogicLayer.Core.GetService<IAuthenticationService>().GetAllSerialNumbers();
        
        var isValid = false;
        
        serialNumbers.ForEach(sn => {
            try
            {
                if (BCrypt.Net.BCrypt.Verify(sn, hashedSerialNumber))
                {
                    isValid = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });

        return isValid;
    }
    
}