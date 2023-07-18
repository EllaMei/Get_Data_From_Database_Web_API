using Npgsql;
using BCrypt.Net;

internal partial class Program
{
    public static async Task<bool> UserLogin(string LoginId, string Password, string connectionString)
    {
        try
        {
            
            using NpgsqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            string SqlStatement = "SELECT password_hash, user_status FROM quiz_users WHERE login_id ILIKE @LoginId AND user_status = TRUE";

            using (NpgsqlCommand command = new NpgsqlCommand(SqlStatement, connection))
            {
                command.Parameters.AddWithValue("@LoginId", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        string? hashedPassword = reader["password_hash"] as string;
                        bool UserStatus = Convert.ToBoolean(reader["user_status"]);
                        

                        if (!UserStatus)
                        {
                            return  false;
                        }

                        if (hashedPassword != null && BCrypt.Net.BCrypt.Verify(Password, hashedPassword))
                        {
                            // Login successful
                            return true;
                        }
                    }
                    //login ID does not exist
                    else
                    {
                        return false;
                    }
                }
            }
        }
        catch (NpgsqlException ex)
        {
            // Handle PostgreSQL-related exceptions
            return ErrorHandler($"An error occurred during login: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            return ErrorHandler($"An unexpected error occurred during login: {ex.Message}");
        }

        // Login failed
        return false;
    }
}
