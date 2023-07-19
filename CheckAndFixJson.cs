using Newtonsoft.Json.Linq;

internal partial class Program
{
    public static string CheckAndFixJson(string jsonString)
    {

        // Check that the JSON string is not null or empty
        if (String.IsNullOrEmpty(jsonString)) return jsonString;

        // Parse the JSON string into a JObject
        JObject obj = JObject.Parse(jsonString);

        // Iterate over the properties of the JObject
        foreach (JProperty property in obj.Properties())
        {
            // Check if the property value is a nested JObject
            JObject? nestedObj = property.Value as JObject;
            if (nestedObj != null)
            {
                // Check if the nested object has a property named "Options"
                JProperty? optionsProperty = nestedObj.Property("Options");
                if (optionsProperty != null)
                {
                    // Convert the "Options" property value to an array
                    JArray optionsArray = new JArray();
                    foreach (JProperty optionProperty in optionsProperty.Value)
                    {
                         // Create a new JObject for each option
                        JObject optionObj = new JObject();
                        // Add "OptionName" and "OptionText" properties to the option object
                        optionObj.Add("OptionName", optionProperty.Name);
                        optionObj.Add("OptionText", optionProperty.Value);
                        // Add the option object to the options array
                        optionsArray.Add(optionObj);
                    }

                    // Replace the "Options" property with the fixed array
                    optionsProperty.Value.Replace(optionsArray);
                }
            }
        }
        // Return the fixed JSON as a string
        return obj.ToString();

    }
}