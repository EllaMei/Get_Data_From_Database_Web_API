using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

internal partial class Program
{

    public static async Task<dynamic> GenerateQuiz(string apiKey, string apiURL, string prompt, string model)
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
        System.Console.WriteLine(json); // Print the JSON string

        using (var client = new HttpClient()) // Create a new HttpClient
        {
            client.DefaultRequestHeaders.Authorization = headers; // Set the Authorization header

            var response = await client.PostAsync(apiURL, new StringContent(json, Encoding.UTF8, "application/json"));
            // Send a POST request to the OpenAI API with the JSON data

            responseStatusCode = response.StatusCode.ToString(); // Get the response status code

            if (response.IsSuccessStatusCode) // If the response is successful check and fix the JSON first before deserialising
            {
                string responseContent =  await response.Content.ReadAsStringAsync(); // Read the response content as string

                try // Try to deserialise the response content
                {
                    dynamic result = JsonConvert.DeserializeObject(responseContent) ?? new System.Dynamic.ExpandoObject();
                    // Deserialize the response content as dynamic object, or use a new ExpandoObject if null
                    ErrorHandler("-------------------------------");
                    ErrorHandler($"Status code: {responseStatusCode}");

                    if (result.choices != null && result.choices.Count > 0 && result.choices[0].text != null) // Check generated quiz is present in result
                    {
                        getQuizContent = result.choices[0].text; // Get the generated quiz content
                    }
                    else
                    {
                       return ErrorHandler("ERROR: Failed to generate the quiz. The response does not contain the expected/any quiz content.");
                    }
                }
                catch (Exception ex) // Handle the exception during deserialisation
                {
                    return ErrorHandler($"ERROR: An error occurred during deserialization from OpenAI: {ex.Message}");
                }
            }
            else // Response statuscocde indicates an error. Display the statuscode
            {
                return ErrorHandler($"ERROR: There was a problem communicating with the API. Status code: {responseStatusCode}");
            }
        }

        string?[] generatedQuiz = { getQuizContent, responseStatusCode, prompt }; // Array to hold the generated quiz content and response status code

        //return generatedQuiz; // Return the generated quiz array
        return getQuizContent;
    }

}