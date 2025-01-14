using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    public interface IServicePrincipalsService
    {
        Task<ServicePrincipal> AddSecurityAttribute(string servicePrincipalId, string attributeSetName, string secAttributeName, object secAttributeValue);

        Task<ServicePrincipal> AddSecurityAttribute(string servicePrincipalId, string attributeSetName, List<(string secAttributeName, object secAttributeValue)> secAttributes);

        Task<Dictionary<string, object>> GetSecurityAttributes(string servicePrincipalId, string attributeSetName);
    }
}
