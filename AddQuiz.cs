using System.Text.Json;
using Npgsql;

internal partial class Program
{

    public static void AddQuiz(string getQuizContent, string prompt_Text, string connectionString)
    {

        // Declare the document variable
        JsonDocument? document = null;

        try
        {
            // Parse the JSON string into a JsonDocument object
            document = JsonDocument.Parse(getQuizContent);
        }
        catch (JsonException ex)
        {
            // Handle the JSON parsing exception
            Console.WriteLine($"\nERROR: An error occurred during JsonDocument parsing: {ex.Message}\nExecution halted.\nPlease re-run and try again.");
            return;
        }

        // Establish a connection to the database
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            // Open the database connection
            connection.Open();

            // Check if the document is null before proceeding
            if (document is null)
            {
                Console.WriteLine("\nERROR: Failed to generate the quiz. The JSON document is null.");
                connection.Close();
                return;
            }

            // Declare variable to hold the prompt ID generated from inserting this prompt
            int prompt_ID = 0;

            // Insert Prompt Text into quiz_prompt table and retrieve the ID generated
            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO quiz_prompts (prompt_text) VALUES (@promptText) RETURNING prompt_id", connection))
            {
                // Check if the prompt text is not null
                if (prompt_Text is not null)
                {
                    command.Parameters.AddWithValue("@promptText", NpgsqlTypes.NpgsqlDbType.Text, prompt_Text);
                    // Execute the command and retrieve the generated ID
                    object result = command.ExecuteScalar()!;
                    prompt_ID = (int)result;
                }
            }



            // Iterate through each property in the JSON document and insert into tables
            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                // Declare variable to hold the question ID generated from inserting this question
                int question_ID = 0;

                // Insert Questions into questions table and retrieve the ID generated
                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO quiz_questions (question_text, prompt_id) VALUES (@questionText, @promptID) RETURNING question_id", connection))
                {
                    // Get Question value from JSON
                    JsonElement question = property.Value.GetProperty("Question");

                    // Check if the question value is not null
                    if (question.ValueKind != JsonValueKind.Null)
                    {
                        string questionText = question.GetString() ?? string.Empty;
                        string question_text = questionText.Replace("'", "\"");
                        command.Parameters.AddWithValue("@questionText", NpgsqlTypes.NpgsqlDbType.Text, question_text);
                        command.Parameters.AddWithValue("@promptID", NpgsqlTypes.NpgsqlDbType.Integer, prompt_ID);
                        // Execute the command and retrieve the generated ID
                        object result = command.ExecuteScalar()!;
                        question_ID = (int)result;
                    }
                }



                // Insert Options into options table
                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO quiz_options (question_id, option_name, option_text) VALUES (@questionid, @optionName, @optionText)", connection))
                {
                    // Get the value of the "Options" property from the JSON
                    JsonElement options = property.Value.GetProperty("Options");

                    // Iterate through options and insert them into the options table
                    foreach (JsonProperty option in options.EnumerateObject())
                    {
                        string option_name = option.Name ?? "";
                        string option_text = option.Value.GetString()?.Replace("'", "\"") ?? "";
                        // Set the parameter values for the option ID, option name, and option text
                        command.Parameters.AddWithValue("@questionid", NpgsqlTypes.NpgsqlDbType.Integer, question_ID);
                        command.Parameters.AddWithValue("@optionName", NpgsqlTypes.NpgsqlDbType.Char, option_name);
                        command.Parameters.AddWithValue("@optionText", NpgsqlTypes.NpgsqlDbType.Text, option_text);
                        // Execute the command to insert the option
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }

                // Insert Answer into answers table
                using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO quiz_answers (question_id, answer_name) VALUES (@questionid, @answerName)", connection))
                {
                    // Get the value of the "Answer" property from the JSON
                    JsonElement answer = property.Value.GetProperty("Answer");

                    string answer_name = answer.GetString() ?? "";
                    // Set the parameter values for the answer ID and answer name
                    command.Parameters.AddWithValue("@questionid", NpgsqlTypes.NpgsqlDbType.Integer, question_ID);
                    command.Parameters.AddWithValue("@answerName", NpgsqlTypes.NpgsqlDbType.Char, answer_name);
                    // Execute the command to insert the answer
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }

            // Close the database connection
            connection.Close();
        }
    } // End AddQuiz()

}