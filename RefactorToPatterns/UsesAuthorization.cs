using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorToPatterns
{
    public class UsesAuthorization
    {
        private readonly IAuthorizer authorizer;

        public UsesAuthorization(IAuthorizer authorizer)
        {
            this.authorizer = authorizer;
        }

        public string AccessAuthorizedResource()
        {
            string authToken = Authorization.GetAuthToken();
            //Make a service call, using the auth token
            if (authorizer.IsAuthorized(authToken))
            {
                return "Authorized Resource";
            }
            return "Unauthorized Access";
        }
    }

    public interface IAuthorizer
    {
        bool IsAuthorized(string authToken);
    }
}
