using Npgsql;

internal partial class Program
{

public static string? GetaQuiz(string connectionString)
{
    try
    {
        // Establish a connection to the PostgreSQL database using the provided connection string
        using (NpgsqlConnection connection = new(connectionString))
        {
            connection.Open(); // Open the database connection

            // Define the SQL statement to retrieve quiz data from the database
            string sqlstatement =@"SELECT q.question_id, q.question_text, o.option_name, o.option_text
                                    FROM (
                                        SELECT question_id, question_text
                                        FROM quiz_questions
                                        WHERE NOT duplicate
                                        ORDER BY RANDOM()
                                        LIMIT 1
                                        ) AS q
                                        INNER JOIN quiz_options AS o ON q.question_id = o.question_id ";

            // Create a new NpgsqlCommand object with the SQL statement and the database connection
            using (NpgsqlCommand command = new(sqlstatement, connection))
            {
                // Send the command to execute and return the JSON format of the quiz data
                return FormatQuizToJson(command);
            }
        }
    }
    catch (NpgsqlException ex)
        {
             // Handle PostgreSQL-related exceptions
            return ErrorHandler($"An error occurred during getting a question: {ex.Message}");
        }
}

}