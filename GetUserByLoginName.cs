using Newtonsoft.Json;
using Npgsql;
internal partial class Program
{

    public static string? GetUserByLoginName(string loginname, string connectionString)
    {           

       
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString)) // Establish a connection to the database
        {
           
            connection.Open(); // Open the database connection

            if (string.IsNullOrEmpty(loginname)) // Check that a value is supplied
            {
                connection.Close();
                return "ERROR: Must be greater than 0";
            }


            using (NpgsqlCommand command = new NpgsqlCommand("SELECT login_name, first_name, last_name FROM quiz_users WHERE login_name=@loginName", connection))
            {
                command.Parameters.AddWithValue("@loginName", NpgsqlTypes.NpgsqlDbType.Text, loginname);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string loginName = reader.GetString(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);

                        // Create a JSON object
                        var userObject = new
                        {
                            LoginName = loginName,
                            FirstName = firstName,
                            LastName = lastName
                        };

                        // Serialize the JSON object to a string
                        string jsonResponse = JsonConvert.SerializeObject(userObject);

                        return jsonResponse;
                    }
                }
            }

        }

        return null;
    }


}