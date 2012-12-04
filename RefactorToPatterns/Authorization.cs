using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorToPatterns
{
    public static class Authorization
    {
        private static string currentToken;
        private static DateTime tokenExpiration;

        public static string GetAuthToken()
        {
            if (IsAboutToExpire(tokenExpiration))
            {
                lock (typeof(Authorization))
                {
                    if (IsAboutToExpire(tokenExpiration))
                    {
                        var newToken = RefreshToken();
                        tokenExpiration = tokenExpiration.AddHours(1);
                        currentToken = newToken;
                    }
                }
            }
            return currentToken;
        }

        private static bool IsAboutToExpire(DateTime expiryTime)
        {
            return DateTime.Now.AddSeconds(10) > expiryTime;
        }

        private static int refreshCount = 0;
        private static string RefreshToken()
        {
            //This is a very long and painful method.
            refreshCount++;
            return "Token" + refreshCount;
        }
    }
}
