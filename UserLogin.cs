using Npgsql;
using BCrypt.Net;


internal partial class Program
{
    public static async Task<dynamic> UserLogin(string LoginId, string Password, string connectionString)
    {
        if(string.IsNullOrEmpty(LoginId) || string.IsNullOrEmpty(Password))
            {
                return "Login ID or Password is missing.";
            }
        try
        {
            //check if the user status is false
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string SqlStatement = "SELECT COUNT(*) As count FROM quiz_users WHERE NOT user_status  AND login_id ILIKE @loginid ";

                using (NpgsqlCommand Command = new(SqlStatement, connection))   
                {
                    Command.Parameters.AddWithValue("@loginid", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);  
                    int count = Convert.ToInt32(await Command.ExecuteScalarAsync());
                    if (count == 1) 
                    {
                        return "Login ID is not active.";
                    }
                }
            }
            // At this point means the user is allowed to login or user_status is TRUE
            using(NpgsqlConnection connection= new(connectionString))
            {
                await connection.OpenAsync();
                string SqlStatement = "SELECT password_hash, user_status FROM quiz_users WHERE login_id ILIKE @LoginId AND user_status";

                using (NpgsqlCommand command = new (SqlStatement, connection))
                {
                    command.Parameters.AddWithValue("@LoginId", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            string? hashedPassword = reader["password_hash"] as string;

                            if (BCrypt.Net.BCrypt.Verify(Password, hashedPassword))
                            {
                                return Results.Ok("Login Successful.");
                            }
                            else
                            {
                               return Results.Ok("Your username or Password is incorrect.");
                            }
                        }
                    }
                }
            }
            return Results.Ok("No Record is Returned.");         
        }
         // End of UserLogin
         catch (NpgsqlException ex)
            {
                // Handle PostgreSQL-related exceptions
                return ErrorHandler($"An error occurred during login: {ex.Message}");
            }
    }
}