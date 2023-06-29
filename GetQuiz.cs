using Npgsql;

internal partial class Program
{

    public static string? GetQuiz(string connectionString)
    {
        // Establish a connection to the PostgreSQL database using the provided connection string
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open(); // Open the database connection

            // Define the SQL query to retrieve quiz data from the database
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT quiz_questions.id, quiz_questions.question_text, quiz_options.option_name, quiz_options.option_text
                                                            FROM quiz_questions
                                                            INNER JOIN quiz_options ON quiz_questions.id = quiz_options.id;", connection))
            {
                // Send the command to execute and return the json format
                return FormatQuiz(command);
            }
        }
    }
}