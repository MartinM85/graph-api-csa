using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    public interface IUsersService
    {
        Task<User> AddSecurityAttribute(string userId, string attributeSetName, string secAttributeName, object secAttributeValue);

        Task<User> AddSecurityAttribute(string userId, string attributeSetName, List<(string secAttributeName, object secAttributeValue)> secAttributes);

        Task<Dictionary<string, object>> GetSecurityAttributes(string userId, string attributeSetName);
    }
}
