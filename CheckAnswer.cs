using Newtonsoft.Json;
using Npgsql;
/* 
SQL Command to use on Postgres
SELECT questions.id, answers.answer_name FROM quiz_questions questions INNER JOIN quiz_answers answers ON questions.id = answers.id;
*/

internal partial class Program
{
    public static async Task<string?> CheckAnswer(int question_id, char optionname, string connectionString)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand(@"SELECT questions.id, answers.answer_name 
                                                                FROM quiz_questions questions 
                                                                INNER JOIN quiz_answers answers ON @questionid = answers.id", connection))
            {
                command.Parameters.AddWithValue("@questionid", NpgsqlTypes.NpgsqlDbType.Integer, question_id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        char answerName = reader.GetChar(1);

                        bool result = answerName == optionname;

                        var answerObject = new
                        {
                            Result = result
                        };

                        string jsonResponse = JsonConvert.SerializeObject(answerObject);

                        return jsonResponse;
                    }
                }
            }
        }
        return null;
    }


}