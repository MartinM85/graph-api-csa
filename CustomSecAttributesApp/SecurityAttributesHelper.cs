using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;

namespace CustomSecAttributesApp
{
    public static class SecurityAttributesHelper
    {
        public static Dictionary<string,object> GetAttributes(CustomSecurityAttributeValue csaValue, string attributeSetName)
        {
            var secAttributes = new Dictionary<string, object>();

            var attributeSet = csaValue.AdditionalData[attributeSetName];
            if (attributeSet is UntypedObject untypedObject)
            {
                var securityAttributes = untypedObject.GetValue();
                foreach (var securityAttribute in securityAttributes)
                {
                    if (securityAttribute.Key.ToLowerInvariant().Contains("odata.type"))
                    {
                        continue;
                    }

                    if (securityAttribute.Value is UntypedString untypedString)
                    {
                        secAttributes.Add(securityAttribute.Key, untypedString.GetValue());
                    }
                    else if (securityAttribute.Value is UntypedInteger untypedInteger)
                    {
                        secAttributes.Add(securityAttribute.Key, untypedInteger.GetValue());
                    }
                    else if (securityAttribute.Value is UntypedBoolean untypedBoolean)
                    {
                        secAttributes.Add(securityAttribute.Key, untypedBoolean.GetValue());
                    }
                    else if (securityAttribute.Value is UntypedArray untypedArray)
                    {
                        var values = new List<object>();
                        foreach (var item in untypedArray.GetValue())
                        {
                            if (item is UntypedString untypedStringItem)
                            {
                                values.Add(untypedStringItem.GetValue());
                            }
                            else if (item is UntypedInteger untypedIntegerItem)
                            {
                                values.Add(untypedIntegerItem.GetValue());
                            }
                        }
                        secAttributes.Add(securityAttribute.Key, values);
                    }

                }
            }

            return secAttributes;
        }

        public static UntypedObject CreateAttributeSetWithSecurityAttributes(List<(string secAttributeName, object secAttributeValue)> secAttributes)
        {
            var properties = new Dictionary<string, UntypedNode>
            {
                ["@odata.type"] = new UntypedString("#Microsoft.DirectoryServices.CustomSecurityAttributeValue")
            };

            foreach (var (secAttributeName, secAttributeValue) in secAttributes)
            {
                UntypedNode attributeValue = null;

                if (secAttributeValue is string stringValue)
                {
                    attributeValue = new UntypedString(stringValue);
                }
                else if (secAttributeValue is int intValue)
                {
                    properties[$"{secAttributeName}@odata.type"] = new UntypedString("#Int32");
                    attributeValue = new UntypedInteger(intValue);
                }
                else if (secAttributeValue is bool boolValue)
                {
                    attributeValue = new UntypedBoolean(boolValue);
                }
                else if (secAttributeValue is IEnumerable<string> stringCollection)
                {
                    var values = new List<UntypedNode>();
                    foreach (var item in stringCollection)
                    {
                        values.Add(new UntypedString(item));
                    }
                    attributeValue = new UntypedArray(values);
                }
                else if (secAttributeValue is IEnumerable<int> intCollection)
                {
                    var values = new List<UntypedNode>();
                    foreach (var item in intCollection)
                    {
                        values.Add(new UntypedInteger(item));
                    }
                    attributeValue = new UntypedArray(values);
                }
                properties[secAttributeName] = attributeValue;
            }

            return new UntypedObject(properties);
        }
    }
}
