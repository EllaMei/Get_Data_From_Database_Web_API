using Npgsql;

internal partial class Program
{

    public static string? GetNewQuiz(string loginname, string connectionString)
    {
        // Establish a connection to the PostgreSQL database using the provided connection string
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open(); // Open the database connection
            
            // Define the SQL query to retrieve quiz data from the database
            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT quiz_questions.id, quiz_questions.question_text, quiz_options.option_name, quiz_options.option_text
                                                            FROM (quiz_questions LEFT JOIN (SELECT question_id FROM quiz_history WHERE login_name = @loginName)
                                                            AS quiz_history ON quiz_questions.id = quiz_history.question_id) 
                                                            INNER JOIN quiz_options ON quiz_questions.id = quiz_options.id
                                                            WHERE (((quiz_history.question_id) Is Null) AND qq.duplicate IS NOT TRUE);", connection))
            {
                command.Parameters.AddWithValue("@loginName", NpgsqlTypes.NpgsqlDbType.Text, loginname);
                // Send the command to execute and return the json format
                return FormatQuiz(command);
            }
        }
    }

}