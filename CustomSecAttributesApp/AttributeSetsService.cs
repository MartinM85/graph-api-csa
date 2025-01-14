using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    public class AttributeSetsService : IAttributeSetsService
    {
        private readonly GraphServiceClient graphServiceClient;

        public AttributeSetsService(GraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient;
        }

        public async Task<AttributeSet> CreateAttributeSetAsync(string name, string description, int? maxAttributes)
        {
            var attributeSet = new AttributeSet
            {
                Id = name,
                Description = description,
                MaxAttributesPerSet = maxAttributes
            };
            try
            {
                return await graphServiceClient.Directory.AttributeSets.PostAsync(attributeSet);
            }
            catch (ODataError ex)
            {
                return null;
            }
        }

        public async Task<List<AttributeSet>> GetAttributeSetsAsync()
        {
            var attributeSets = new List<AttributeSet>();
            var response = await graphServiceClient.Directory.AttributeSets.GetAsync();
            var pageIterator = PageIterator<AttributeSet, AttributeSetCollectionResponse>.CreatePageIterator(graphServiceClient, response, (attributeSet) =>
            {
                attributeSets.Add(attributeSet);
                return true;
            });

            await pageIterator.IterateAsync();
            return attributeSets;
        }

        public async Task<AttributeSet> GetAttributeSet(string name)
        {
            return await graphServiceClient.Directory.AttributeSets[name].GetAsync();
        }

        public async Task<AttributeSet> UpdateAttributeSetAsync(string name, string description, int? maxAttributes)
        {
            var attributeSet = new AttributeSet
            {
                Description = description,
                MaxAttributesPerSet = maxAttributes
            };
            try
            {
                return await graphServiceClient.Directory.AttributeSets[name].PatchAsync(attributeSet);
            }
            catch (ODataError ex)
            {
                return null;
            }
        }
    }
}
