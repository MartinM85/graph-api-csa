using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    public class SecurityAttributesService : ISecurityAttributesService
    {
        private readonly GraphServiceClient graphServiceClient;

        public SecurityAttributesService(GraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient;
        }

        public async Task<CustomSecurityAttributeDefinition> AddDefinition(string attributeSetName, string secAttributeName, string description, string type, string status,
            bool isCollection, bool isSearchable, bool useOnlyPredefinedValues, List<string> predefinedValues)
        {
            var body = new CustomSecurityAttributeDefinition
            {
                AttributeSet = attributeSetName,
                Name = secAttributeName,
                Description = description,
                IsCollection = isCollection,
                IsSearchable = isSearchable,
                UsePreDefinedValuesOnly = useOnlyPredefinedValues,
                Type = type,
                Status = status
            };

            if (predefinedValues != null)
            {
                body.AllowedValues = new List<AllowedValue>();
                foreach (var predefinedValue in predefinedValues)
                {
                    body.AllowedValues.Add(new AllowedValue
                    {
                        Id = predefinedValue,
                        IsActive = true
                    });
                }
            }

            try
            {
                return await graphServiceClient.Directory.CustomSecurityAttributeDefinitions.PostAsync(body);
            }
            catch (ODataError ex)
            {
                return null;
            }
        }

        public async Task<List<CustomSecurityAttributeDefinition>> GetDefinitions(string attributeSetName = null, string secAttributeName = null, string type = null, string status = null, bool includeAllowedValues = false)
        {
            var filters = new List<string>();
            if (!string.IsNullOrEmpty(attributeSetName))
            {
                filters.Add($"attributeSet eq '{attributeSetName}'");
            }

            if (!string.IsNullOrEmpty(secAttributeName))
            {
                filters.Add($"name eq '{secAttributeName}'");
            }

            if (!string.IsNullOrEmpty(type))
            {
                filters.Add($"type eq '{type}'");
            }

            if (!string.IsNullOrEmpty(status))
            {
                filters.Add($"status eq '{status}'");
            }

            var response = await graphServiceClient.Directory.CustomSecurityAttributeDefinitions.GetAsync(rc =>
            {
                if (filters.Count > 0)
                {
                    rc.QueryParameters.Filter = string.Join(" and ", filters);
                }

                if (includeAllowedValues)
                {
                    rc.QueryParameters.Expand = ["allowedValues"];
                }
            });

            var definitions = new List<CustomSecurityAttributeDefinition>();
            var pageIterator = PageIterator<CustomSecurityAttributeDefinition, CustomSecurityAttributeDefinitionCollectionResponse>.CreatePageIterator(graphServiceClient, response, (definition) =>
            {
                definitions.Add(definition);
                return true;
            });

            await pageIterator.IterateAsync();

            return definitions;
        }

        public async Task<CustomSecurityAttributeDefinition> UpdateDefinition(string attributeSetName, string secAttributeName, string description, string status,
            bool? useOnlyPredefinedValues, List<string> predefinedValues)
        {
            return await UpdateDefinition($"{attributeSetName}_{secAttributeName}", description, status, useOnlyPredefinedValues, predefinedValues);
        }

        public async Task<CustomSecurityAttributeDefinition> UpdateDefinition(string secAttributeId, string description, string status,
            bool? useOnlyPredefinedValues, List<string> predefinedValues)
        {
            var body = new CustomSecurityAttributeDefinition();

            if (!string.IsNullOrEmpty(description))
            {
                body.Description = description;
            }

            if (!string.IsNullOrEmpty(status))
            {
                body.Status = status;
            }

            if (useOnlyPredefinedValues.HasValue)
            {
                body.UsePreDefinedValuesOnly = useOnlyPredefinedValues.Value;
            }

            if (predefinedValues != null)
            {
                body.AllowedValues = new List<AllowedValue>();
                foreach (var predefinedValue in predefinedValues)
                {
                    body.AllowedValues.Add(new AllowedValue
                    {
                        Id = predefinedValue,
                        IsActive = true
                    });
                }
            }

            try
            {
                return await graphServiceClient.Directory.CustomSecurityAttributeDefinitions[secAttributeId].PatchAsync(body);
            }
            catch (ODataError ex)
            {
                return null;
            }
        }
    }
}
