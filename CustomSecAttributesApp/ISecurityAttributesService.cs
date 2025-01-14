using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    public interface ISecurityAttributesService
    {
        Task<CustomSecurityAttributeDefinition> AddDefinition(string attributeSetName, string secAttributeName, string description, string type, string status, bool isCollection, bool isSearchable, bool useOnlyPredefinedValues, List<string> predefinedValues);

        Task<List<CustomSecurityAttributeDefinition>> GetDefinitions(string attributeSetName = null, string secAttributeName = null, string type = null, string status = null, bool includeAllowedValues = false);

        Task<CustomSecurityAttributeDefinition> UpdateDefinition(string attributeSetName, string secAttributeName, string description, string status, bool? useOnlyPredefinedValues, List<string> predefinedValues);
    }
}
