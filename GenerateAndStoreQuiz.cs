using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

internal partial class Program
{
    public static async Task<dynamic> GenerateAndStoreQuiz(string apiKey, string apiURL, string prompt, string model, string connectionString)
    {
        string? getQuizContent = string.Empty; // Variable to store the generated quiz content
        string? responseStatusCode = string.Empty; // Variable to store the response status code

        var headers = new AuthenticationHeaderValue("Bearer", apiKey); // Create an AuthenticationHeaderValue with the API key

        var data = new
        {
            prompt,
            model,
            max_tokens = 1000,
            temperature = 0.5
        };
        // Data object to hold prompt, model, max_tokens, and temperature

        string json = JsonConvert.SerializeObject(data); // Serialize the data object to JSON
            
        using (var client = new HttpClient()) // Create a new HttpClient
        {
            // Set the Authorization header
            client.DefaultRequestHeaders.Authorization = headers; 

            // Send a POST request to the OpenAI API with the JSON data
            var response = await client.PostAsync(apiURL, new StringContent(json, Encoding.UTF8, "application/json"));

            // Get the response status code
            responseStatusCode = response.StatusCode.ToString(); 

            if (response.IsSuccessStatusCode) // If the response is successful
            {
                string responseContent =  await response.Content.ReadAsStringAsync(); // Read the response content as string

                try // Try to deserialise the response content
                {
                    dynamic result = JsonConvert.DeserializeObject(responseContent) ?? new System.Dynamic.ExpandoObject();
                    // Deserialize the response content as dynamic object, or use a new ExpandoObject if null

                    if (result.choices != null && result.choices.Count > 0 && result.choices[0].text != null) // Check generated quiz is present in result
                    {
                        getQuizContent = result.choices[0].text; // Get the generated quiz content
                    }

                }
                catch (Exception ex) // Handle the exception during deserialisation
                {
                    ErrorHandler($"\nERROR: An error occurred during deserialization from OpenAI: {ex.Message}");
                }
            }
            else // Response statuscocde indicates an error. Display the statuscode
            {
                ErrorHandler($"\nERROR: There was a problem communicating with the API. Status code: {responseStatusCode}");
            }
        }

        // Parse the JSON output and add it to Postgres
        AddQuiz(getQuizContent, prompt, connectionString);

        //return the Quiz Content
        return getQuizContent;
    }
}