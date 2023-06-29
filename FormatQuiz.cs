using Newtonsoft.Json;
using Npgsql;

internal partial class Program
{

    public static string? FormatQuiz(NpgsqlCommand command)
    {
        using (NpgsqlDataReader reader = command.ExecuteReader())
        {
            // Create a dictionary to store quiz data using the question ID as the key
            Dictionary<int, Dictionary<string, string>> quizDict = new Dictionary<int, Dictionary<string, string>>();

            // Read the data from the database
            while (reader.Read())
            {
                int id = reader.GetInt32(0); // Retrieve the question ID from the first column
                string questionText = reader.GetString(1); // Retrieve the question text from the second column
                string optionName = reader.GetString(2); // Retrieve the option name from the third column
                string optionText = reader.GetString(3); // Retrieve the option text from the fourth column

                // Check if the question ID already exists in the dictionary
                if (!quizDict.ContainsKey(id))
                {
                    // If the question ID doesn't exist, create a new entry in the dictionary
                    quizDict[id] = new Dictionary<string, string>
                    {
                        { "QuestionText", questionText }, // Add the question text to the dictionary under the key "QuestionText"
                        { optionName, optionText } // Add the option text to the dictionary using the option name as the key
                    };
                }
                else
                {
                    // If the question ID already exists, add the option text to the existing entry in the dictionary
                    quizDict[id][optionName] = optionText;
                }
            }

            // Create a list to store the quiz objects
            List<object> quizList = new List<object>();

            // Iterate over the entries in the quiz dictionary
            foreach (var entry in quizDict)
            {
                // Create a dictionary to store the options for each question
                var optionsDict = new Dictionary<string, string>();

                // Iterate over the options for the current question
                foreach (var optionEntry in entry.Value)
                {
                    // Exclude the "QuestionText" key when populating the options dictionary
                    if (optionEntry.Key != "QuestionText")
                    {
                        var optionName = optionEntry.Key;
                        var optionText = optionEntry.Value;
                        optionsDict[optionName] = optionText;
                    }
                }

                // Create the quiz object with ID, question text, and options dictionary
                var quizObject = new
                {
                    ID = entry.Key,
                    QuestionText = entry.Value["QuestionText"],
                    Options = optionsDict
                };

                // Add the quiz object to the quiz list
                quizList.Add(quizObject);
            }

            // Serialize the list of quiz objects to a JSON string with indentation
            string jsonResponse = JsonConvert.SerializeObject(quizList, Formatting.Indented);

            // Return the JSON response
            return jsonResponse;
        }
    }


}