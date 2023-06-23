using Newtonsoft.Json;
using Npgsql;
internal partial class Program
{

    public static string? GetUserById(int userId, string connectionString)
    {           

       
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString)) // Establish a connection to the database
        {
           
            connection.Open(); // Open the database connection

            if (userId <= 0) // Check ID greater than 0
            {
                connection.Close();
                return "ERROR: Must be greater than 0";
            }


            using (NpgsqlCommand command = new NpgsqlCommand("SELECT id, first_name, last_name FROM quiz_users WHERE id=@userId", connection))
            {
                command.Parameters.AddWithValue("@userId", NpgsqlTypes.NpgsqlDbType.Integer, userId);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);

                        // Create a JSON object
                        var userObject = new
                        {
                            Id = id,
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