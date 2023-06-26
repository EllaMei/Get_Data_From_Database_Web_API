using Newtonsoft.Json;
using Npgsql;
internal partial class Program
{
    public static string? GetUsers(string connectionString)
    {           
         using (NpgsqlConnection connection = new NpgsqlConnection(connectionString)) // Establish a connection to the database
        {
            connection.Open(); // Open the database connection

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT login_name, first_name, last_name FROM quiz_users", connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    List<object> userList = new List<object>(); // Create a list to store user objects

                    while (reader.Read()) // Iterate through each row
                    {
                        string loginname = reader.GetString(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);

                        // Create a JSON object for each row
                        var userObject = new
                        {
                            LoginName = loginname,
                            FirstName = firstName,
                            LastName = lastName
                        };

                        userList.Add(userObject); // Add the user object to the list
                    }
                    // Serialize the list of user objects to a JSON array
                    string jsonResponse = JsonConvert.SerializeObject(userList);
                    return jsonResponse;
                }
            }
        }
    }
}