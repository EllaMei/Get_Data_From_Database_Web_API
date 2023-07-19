using Newtonsoft.Json;
using Npgsql;
using BCrypt.Net;
internal partial class Program
{
    public static async Task<string?> AddUser(string LoginId, string FirstName, string LastName, string Password, string connectionString)
    {
        // Convert the loginid to lowercase
        LoginId = LoginId.ToLower();

        // Capitalize the first letter of firstname and convert the rest to lowercase
       FirstName = char.ToUpper(FirstName[0]) + (FirstName.Substring(1)).ToLower();

        // Capitalize the first letter of lastname and convert the rest to lowercase
        LastName = char.ToUpper(LastName[0]) + (LastName.Substring(1)).ToLower();

        // Establish a connection to the database using the provided connection string
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            // Open the database connection asynchronously
            await connection.OpenAsync();

            // Check if the login_id already exists in the quiz_users table
            string SqlStatement = "SELECT COUNT(*) FROM quiz_users WHERE login_id ILIKE @loginid";

            // Create a new NpgsqlCommand object with the check SQL statement and the database connection
            using (NpgsqlCommand checkCommand = new NpgsqlCommand(SqlStatement, connection))
            {
                // Add the loginid as a parameter to the command
                checkCommand.Parameters.AddWithValue("@loginID", NpgsqlTypes.NpgsqlDbType.Text, LoginId);

                // Execute the command and retrieve the data reader
                using (NpgsqlDataReader reader = checkCommand.ExecuteReader())
                {
                    int count = 0;
                    // Read the first row of the result
                    if (reader.Read())
                    {
                        // Get the count from the first column of the result
                        count = reader.GetInt32(0);
                    }
                    // If the count is greater than 0, it means the login_id already exists, so return an error message
                    if (count > 0)
                    {
                        return "Login ID already exists.";
                    }
                }
            }

            //Hash the password using bcrypt
            string hashedPassword =BCrypt.Net.BCrypt.HashPassword(Password, BCrypt.Net.BCrypt.GenerateSalt(12));

            // Insert the new user record into the quiz_users table
            SqlStatement = @"INSERT INTO quiz_users (login_id, first_name, last_name, password_hash) VALUES (@loginid, @firstname, @lastname, @password)";

            // Create a new NpgsqlCommand object with the insert SQL statement and the database connection
            using (NpgsqlCommand command = new NpgsqlCommand(SqlStatement, connection))
            {
                // Add parameters to the command to prevent SQL injection
                command.Parameters.AddWithValue("@loginid", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);
                command.Parameters.AddWithValue("@firstname", NpgsqlTypes.NpgsqlDbType.Varchar, FirstName);
                command.Parameters.AddWithValue("@lastname", NpgsqlTypes.NpgsqlDbType.Varchar, LastName);
                command.Parameters.AddWithValue("@password", NpgsqlTypes.NpgsqlDbType.Varchar, hashedPassword);

                // Execute the command and retrieve the scalar result asynchronously
                await command.ExecuteScalarAsync();
            }

            // Create an anonymous object to store the response data
            var responseObject = new
            {
                LoginId,
                FirstName,
                LastName
            };

            // Serialize the response object to JSON
            string jsonResponse = JsonConvert.SerializeObject(responseObject);

            // Return the JSON response
            return jsonResponse;
        }
    }

    
}