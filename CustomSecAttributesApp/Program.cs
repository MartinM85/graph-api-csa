using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSecAttributesApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await ManageAttributes();

            await AssignAttributesToUsers();

            await AssignAttributesToServicePrincipals();
        }

        private static async Task ManageAttributes()
        {
            var tenantId = "<tenant_id>";
            var clientId = "<client_id>";
            var secret = "<client_secret>";
            var credential = new ClientSecretCredential(tenantId, clientId, secret);
            var client = new GraphServiceClient(credential);

            var attributeSetsService = new AttributeSetsService(client);
            var secAttributeService = new SecurityAttributesService(client);

            // create attribute set
            var attributeSet = "Business";
            await attributeSetsService.CreateAttributeSetAsync(attributeSet, "Attributes for business department", 10);

            // add definition
            await secAttributeService.AddDefinition(attributeSet, "Project", "Active projects", "String", "Available", true, true, false, null);

            // create attribute set
            attributeSet = "EmployeeCompetencies";
            await attributeSetsService.CreateAttributeSetAsync(attributeSet, "Competencies to access resource", 10);

            // update attribute set
            await attributeSetsService.UpdateAttributeSetAsync(attributeSet, "Employees competencies to access resources", 30);

            // add definitions
            await secAttributeService.AddDefinition(attributeSet, "English", "Level of english", "String", "Available", false, true, true, ["A1", "A2", "B1", "B2", "C1", "C2"]);

            await secAttributeService.AddDefinition(attributeSet, "Microsoft365", "number of years of hands-on experience with Microsoft 365", "Integer", "Available", false, true, false, null);

            await secAttributeService.AddDefinition(attributeSet, "SecurityCertification", "Whether the employee is authorized to work with sensitive data", "Boolean", "Available", false, true, false, null);

            await secAttributeService.AddDefinition(attributeSet, "EntraRoles", "Entra roles that can be assigned to the user", "String", "Available", true, true, true, ["User Administrator", "Application Administrator", "SharePoint Administrator", "Global Reader"]);

            await secAttributeService.AddDefinition(attributeSet, "Projects", "Projects per year", "Integer", "Available", true, true, false, null);

            PrintDefinitions(await secAttributeService.GetDefinitions());

            PrintDefinitions(await secAttributeService.GetDefinitions(attributeSetName: attributeSet));

            PrintDefinitions(await secAttributeService.GetDefinitions(attributeSetName: attributeSet, type: "String"));

            PrintDefinitions(await secAttributeService.GetDefinitions(status: "Deprecated"));

            PrintDefinitions(await secAttributeService.GetDefinitions(type: "Integer", status: "Deprecated"));

            void PrintDefinitions(IEnumerable<CustomSecurityAttributeDefinition> definitions)
            {
                foreach (var definition in definitions)
                {
                    Console.WriteLine($"{definition.AttributeSet} - {definition.Name} - {definition.Description} - {definition.Type} - {definition.Status}");
                }
            }
        }

        private static async Task AssignAttributesToUsers()
        {
            var tenantId = "<tenant_id>";
            var clientId = "<client_id>";
            var secret = "<client_secret>";
            var credential = new ClientSecretCredential(tenantId, clientId, secret);
            var client = new GraphServiceClient(credential);

            var attributeSet = "EmployeeCompetencies";
            var usersService = new UsersService(client);
            var userId = "<user_id>";
            await usersService.AddSecurityAttribute(userId,
                attributeSet,
                [
                    ("English", "B2"),
                    ("Microsoft365", 8),
                    ("SecurityCertification", true),
                    ("EntraRoles", new List<string> { "Application Administrator", "User Administrator" }),
                    ("Projects", new List<int> { 2, 5, 4, 8 })
                ]);

            var data = await usersService.GetSecurityAttributes(userId, attributeSet);

            foreach (var item in data)
            {
                if (item.Value is IEnumerable<object> collection)
                {
                    Console.WriteLine($"{item.Key}: {string.Join(", ", collection)}");
                }
                else
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }
        }

        private static async Task AssignAttributesToServicePrincipals()
        {
            var tenantId = "<tenant_id>";
            var clientId = "<client_id>";
            var secret = "<client_secret>";
            var credential = new ClientSecretCredential(tenantId, clientId, secret);
            var client = new GraphServiceClient(credential);

            var attributeSet = "Business";
            var servicePrincipalsService = new ServicePrincipalsService(client);
            var servicePrincipalId = "<service_principal_id>";
            await servicePrincipalsService.AddSecurityAttribute(servicePrincipalId,
                attributeSet,
                [
                    ("Project", new List<string> { "Project 1", "Project 2" })
                ]);

            var data = await servicePrincipalsService.GetSecurityAttributes(servicePrincipalId, attributeSet);

            foreach (var item in data)
            {
                if (item.Value is IEnumerable<object> collection)
                {
                    Console.WriteLine($"{item.Key}: {string.Join(", ", collection)}");
                }
                else
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }
        }
    }
}
