using LogicLayer.Interfaces;

namespace LogicLayer.Core;

public static partial class Core
{
    public static bool ValidateApiKey(string apiKey)
    {
        return GetService<IAuthenticationService>().ApiKeyIsAuthenticated(apiKey);
    }
    
    public static bool ValidateSerialNumber(string hashedSerialNumber)
    {
        var serialNumbers = GetService<IAuthenticationService>().GetAllSerialNumbers();
        
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