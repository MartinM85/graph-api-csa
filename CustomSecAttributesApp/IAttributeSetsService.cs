using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    interface IAttributeSetsService
    {
        Task<AttributeSet> CreateAttributeSetAsync(string name, string description, int? maxAttributes);

        Task<List<AttributeSet>> GetAttributeSetsAsync();

        Task<AttributeSet> GetAttributeSet(string name);

        Task<AttributeSet> UpdateAttributeSetAsync(string name, string description, int? maxAttributes);
    }
}
