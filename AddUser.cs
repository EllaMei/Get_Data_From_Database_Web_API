using Newtonsoft.Json;
using Npgsql;
using BCrypt.Net;
internal partial class Program
{
    public static async Task<string?> AddUser(string loginid, string firstname, string lastname, string Password, string connectionString)
    {
        // Convert the loginid to lowercase
        loginid = loginid.ToLower();

        // Capitalize the first letter of firstname and convert the rest to lowercase
        firstname = char.ToUpper(firstname[0]) + (firstname.Substring(1)).ToLower();

        // Capitalize the first letter of lastname and convert the rest to lowercase
        lastname = char.ToUpper(lastname[0]) + (lastname.Substring(1)).ToLower();

        // Establish a connection to the database using the provided connection string
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            // Open the database connection asynchronously
            await connection.OpenAsync();

            // Check if the login_id already exists in the quiz_users table
            string sqlstatement = "SELECT COUNT(*) FROM quiz_users WHERE login_id ILIKE @loginid";

            // Create a new NpgsqlCommand object with the check SQL statement and the database connection
            using (NpgsqlCommand checkCommand = new NpgsqlCommand(sqlstatement, connection))
            {
                // Add the loginid as a parameter to the command
                checkCommand.Parameters.AddWithValue("@loginID", NpgsqlTypes.NpgsqlDbType.Text, loginid);

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
            sqlstatement = @"INSERT INTO quiz_users (login_id, first_name, last_name, password_hash) VALUES (@loginid, @firstname, @lastname, @password)";

            // Create a new NpgsqlCommand object with the insert SQL statement and the database connection
            using (NpgsqlCommand command = new NpgsqlCommand(sqlstatement, connection))
            {
                // Add parameters to the command to prevent SQL injection
                command.Parameters.AddWithValue("@loginid", NpgsqlTypes.NpgsqlDbType.Varchar, loginid);
                command.Parameters.AddWithValue("@firstname", NpgsqlTypes.NpgsqlDbType.Varchar, firstname);
                command.Parameters.AddWithValue("@lastname", NpgsqlTypes.NpgsqlDbType.Varchar, lastname);
                command.Parameters.AddWithValue("@password", NpgsqlTypes.NpgsqlDbType.Varchar, hashedPassword);

                // Execute the command and retrieve the scalar result asynchronously
                await command.ExecuteScalarAsync();
            }

            // Create an anonymous object to store the response data
            var responseObject = new
            {
                LoginID = loginid,
                FirstName = firstname,
                LastName = lastname
            };

            // Serialize the response object to JSON
            string jsonResponse = JsonConvert.SerializeObject(responseObject);

            // Return the JSON response
            return jsonResponse;
        }
    }

    
}