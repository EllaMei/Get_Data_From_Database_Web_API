using Newtonsoft.Json;
using Npgsql;
using BCrypt.Net;
internal partial class Program
{
    public static async Task<string?> UpdateUser( string LoginId, string FirstName, string LastName, string Password, string connectionString)
    {

         // Validate input parameters
        if (string.IsNullOrEmpty(LoginId) || string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Password))
        {
            return "One or more parameters are missing";
        }



        //convert the loginid to lowercase
        LoginId = LoginId.ToLower();


        //Capitalize the first letter of FirstName and Convert the rest to lowercase
        FirstName = char.ToUpper(FirstName[0]) + (FirstName.Substring(1)).ToLower();

         //Capitalize the first letter of LastName and Convert the rest to lowercase
        LastName = char.ToUpper(LastName[0]) + (LastName.Substring(1)).ToLower();

        //Hash the password using bcrypt
        string hashedPassword =BCrypt.Net.BCrypt.HashPassword(Password, BCrypt.Net.BCrypt.GenerateSalt(12)); 

        using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();


            string SqlStatement = "UPDATE quiz_users SET  first_name = @FirstName, last_name = @LastName, password_hash = @Password WHERE login_id ILIKE @LoginId";


            // Create a new NpgsqlCommand object with the insert SQL statement and the database connection
            using (NpgsqlCommand command = new NpgsqlCommand(SqlStatement, connection))
            {
                // Add parameters to the command to prevent SQL injection
                command.Parameters.AddWithValue("@LoginId", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);
                command.Parameters.AddWithValue("@FirstName", NpgsqlTypes.NpgsqlDbType.Varchar, FirstName);
                command.Parameters.AddWithValue("@LastName", NpgsqlTypes.NpgsqlDbType.Varchar, LastName);
                command.Parameters.AddWithValue("@Password", NpgsqlTypes.NpgsqlDbType.Varchar, hashedPassword);

                // Execute the command and retrieve the scalar result asynchronously
                await command.ExecuteNonQueryAsync();
            }

            // Create an anonymous object to store the response data
            var responseObject = new
            {
                LoginID = LoginId,
                FirstName,
                LastName,
                
            };

            // Serialize the response object to JSON
            string jsonResponse = JsonConvert.SerializeObject(responseObject);

            // Return the JSON response
            return jsonResponse;

        }
    }
}