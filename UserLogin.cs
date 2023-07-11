using Npgsql;
using BCrypt.Net;
internal partial class Program
{
    public static async Task<bool> UserLogin(string LoginId, string Password, string connectionString)
    {

        try
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlStatement = "SELECT password_hash FROM quiz_users WHERE login_id = @LoginId";

                using (NpgsqlCommand command = new NpgsqlCommand(SqlStatement, connection))
                {
                    command.Parameters.AddWithValue("@LoginId", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);

                    string? hashedPassword = await command.ExecuteScalarAsync() as string;

                   // if(string.IsNullOrEmpty(hashedPassword) && (Password, hashedPassword))
                    if (hashedPassword != null && BCrypt.Net.BCrypt.Verify(Password, hashedPassword))
                    {
                        // Login successful
                        return true;
                    }
                    else
                    {
                        // Login failed
                        return false;
                    }
                }
            }
        }
        catch (NpgsqlException ex)
        {
            // Handle PostgreSQL-related exceptions
            ErrorHandler($"An error occurred during login: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            ErrorHandler($"An unexpected error occurred during login: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
    }
}