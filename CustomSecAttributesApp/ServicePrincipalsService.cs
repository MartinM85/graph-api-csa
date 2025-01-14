using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    public class ServicePrincipalsService : IServicePrincipalsService
    {
        private readonly GraphServiceClient graphServiceClient;
        public ServicePrincipalsService(GraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient;
        }

        public async Task<ServicePrincipal> AddSecurityAttribute(string servicePrincipalId, string attributeSetName, string secAttributeName, object value)
        {
            return await AddSecurityAttribute(servicePrincipalId, attributeSetName, new List<(string, object)> { (secAttributeName, value) });
        }

        public async Task<ServicePrincipal> AddSecurityAttribute(string servicePrincipalId, string attributeSetName, List<(string secAttributeName, object secAttributeValue)> secAttributes)
        {
            var body = new ServicePrincipal
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
            return await UpdateServicePrincipal(servicePrincipalId, body);
        }

        

        private async Task<ServicePrincipal> UpdateServicePrincipal(string servicePrincipalId, ServicePrincipal requestBody)
        {
            try
            {
                return await graphServiceClient.ServicePrincipals[servicePrincipalId].PatchAsync(requestBody);
            }
            catch (ODataError ex)
            {
                return null;
            }
        }

        public async Task<Dictionary<string, object>> GetSecurityAttributes(string servicePrincipalId, string attributeSetName)
        {
            try
            {
                var user = await graphServiceClient.ServicePrincipals[servicePrincipalId].GetAsync(rc =>
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
