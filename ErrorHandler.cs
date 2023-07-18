using Newtonsoft.Json;

internal partial class Program
{   
    public static dynamic ErrorHandler(string errorMessage)
    {
        // Create a new anonymous object to represent the error message
        var errorObject = new
        {
            Error = errorMessage
        };

        // Serialize the error object to a JSON string
        string jsonResponse = JsonConvert.SerializeObject(errorObject);

        // Return the JSON response as dynamic type
        return jsonResponse;
    }

}