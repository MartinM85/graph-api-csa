using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    public class UsersService : IUsersService
    {
        private readonly GraphServiceClient graphServiceClient;
        public UsersService(GraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient;
        }

        public async Task<User> AddSecurityAttribute(string userId, string attributeSetName, string secAttributeName, object value)
        {
            return await AddSecurityAttribute(userId, attributeSetName, new List<(string, object)> { (secAttributeName, value) });
        }

        public async Task<User> AddSecurityAttribute(string userId, string attributeSetName, List<(string secAttributeName, object secAttributeValue)> secAttributes)
        {
            var body = new User
            {
                CustomSecurityAttributes = new CustomSecurityAttributeValue
                {
                    AdditionalData = new Dictionary<string, object>
                    {
                        {
                            $"{attributeSetName}" , SecurityAttributesHelper.CreateAttributeSetWithSecurityAttributes(secAttributes)
                        }
                    }
                }
            };
            return await UpdateUser(userId, body);
        }

        private async Task<User> UpdateUser(string userId, User requestBody)
        {
            try
            {
                return await graphServiceClient.Users[userId].PatchAsync(requestBody);
            }
            catch (ODataError ex)
            {
                return null;
            }
        }

        public async Task<Dictionary<string, object>> GetSecurityAttributes(string userId, string attributeSetName)
        {
            try
            {
                var user = await graphServiceClient.Users[userId].GetAsync(rc =>
                {
                    rc.QueryParameters.Select = ["customSecurityAttributes"];
                });

                return SecurityAttributesHelper.GetAttributes(user.CustomSecurityAttributes, attributeSetName);
            }
            catch (ODataError ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
