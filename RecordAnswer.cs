using Npgsql;

internal partial class Program
{
    public static async Task<string?> RecordAnswer(string loginname, int questionid, char optionname, string connectionString)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO quiz_history (login_name, question_id, option_name) 
                                                                VALUES (@loginname, @questionid, @optionname)", connection))
            {
                command.Parameters.AddWithValue("@loginname", NpgsqlTypes.NpgsqlDbType.Varchar, loginname);
                command.Parameters.AddWithValue("@questionid", NpgsqlTypes.NpgsqlDbType.Integer, questionid);
                command.Parameters.AddWithValue("@optionname", NpgsqlTypes.NpgsqlDbType.Char, optionname);
                await command.ExecuteScalarAsync();
            }

            return await CheckAnswer(questionid, optionname, connectionString);
        }
    }
    
}