using Npgsql;
using Newtonsoft.Json;

internal partial class Program
{

    public static string? GetAllOldQuiz(string loginname, string connectionString)
    {
        // Establish a connection to the PostgreSQL database using the provided connection string
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open(); // Open the database connection
            // TODO: Use parameter query for the NOT in not null
            // Define the SQL query to retrieve quiz data from the database
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT quiz_questions.id, quiz_questions.question_text, 
                                                                quiz_options.option_name, quiz_options.option_text, quiz_history.option_name user_answer
                                                                FROM (quiz_questions LEFT JOIN 
                                                                        (SELECT question_id, option_name FROM quiz_history 
                                                                        WHERE login_name = @loginName)
                                                                AS quiz_history ON quiz_questions.id = quiz_history.question_id) 
                                                                INNER JOIN quiz_options ON quiz_questions.id = quiz_options.id
                                                                WHERE (((quiz_history.question_id) Is Not Null));", connection))
            {
                command.Parameters.AddWithValue("@loginName", NpgsqlTypes.NpgsqlDbType.Text, loginname);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    // Create a dictionary to store quiz data using the question ID as the key
                    Dictionary<int, Dictionary<string, string>> quizDict = new Dictionary<int, Dictionary<string, string>>();

                    // Read the data from the database
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0); // Retrieve the question ID from the first column.
                        string questionText = reader.GetString(1); // Retrieve the question text from the second column.
                        string optionName = reader.GetString(2); // Retrieve the option name from the third column.
                        string optionText = reader.GetString(3); // Retrieve the option text from the fourth column.
                        string userAnswer = reader.GetString(4); //Retrieve the user answer for the question.

                        

                        // Check if the question ID already exists in the dictionary
                        if (!quizDict.ContainsKey(id))
                        {
                            // If the question ID doesn't exist, create a new entry in the dictionary
                            quizDict[id] = new Dictionary<string, string>
                            {
                                { "QuestionText", questionText }, // Add the question text to the dictionary under the key "QuestionText"
                                { optionName, optionText }, // Add the option text to the dictionary using the option name as the key
                                { "UserAnswer", userAnswer }
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
                            if (optionEntry.Key != "QuestionText" && optionEntry.Key != "UserAnswer")
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
                            UserAnswer = entry.Value["UserAnswer"],
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

    }

}