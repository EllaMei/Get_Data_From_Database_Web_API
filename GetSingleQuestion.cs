using Npgsql;

internal partial class Program
{

public static string? GetaQuiz(string connectionString)
{
    try
    {
        // Establish a connection to the PostgreSQL database using the provided connection string
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open(); // Open the database connection

            // Define the SQL statement to retrieve quiz data from the database
            string sqlstatement = @"SELECT quiz_questions.question_id, quiz_questions.question_text, quiz_options.option_name, quiz_options.option_text
                                    FROM quiz_questions
                                    INNER JOIN quiz_options ON quiz_questions.question_id = quiz_options.question_id
                                    WHERE NOT duplicate ORDER BY RANDOM() LIMIT 1";

            // Create a new NpgsqlCommand object with the SQL statement and the database connection
            using (NpgsqlCommand command = new NpgsqlCommand(sqlstatement, connection))
            {
                // Send the command to execute and return the JSON format of the quiz data
                return FormatQuizToJson(command);
            }
        }
    }
    catch (NpgsqlException ex)
        {
             // Handle PostgreSQL-related exceptions
            return ErrorHandler($"An error occurred during login: {ex.Message}");
        }
}

}