using Newtonsoft.Json;
using Npgsql;

internal partial class Program
{
    public static async Task<string?> AddUser(string loginname, string firstname, string lastname, string connectionString)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand(@"INSERT INTO quiz_users (login_name, first_name, last_name) 
                                                                VALUES (@loginname, @firstname, @lastname)", connection))
            {
                command.Parameters.AddWithValue("@loginname", NpgsqlTypes.NpgsqlDbType.Varchar, loginname);
                command.Parameters.AddWithValue("@firstname", NpgsqlTypes.NpgsqlDbType.Varchar, firstname);
                command.Parameters.AddWithValue("@lastname", NpgsqlTypes.NpgsqlDbType.Varchar, lastname);
                await command.ExecuteScalarAsync();
            }

            var responseObject = new
            {
                LoginName = loginname,
                FirstName = firstname,
                LastName = lastname
            };

            string jsonResponse = JsonConvert.SerializeObject(responseObject);

            return jsonResponse;
        }
    }
    
}