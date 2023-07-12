using Npgsql;
internal partial class Program
{
    public static async Task ActivateUserStatus(string LoginId, string connectionString)
    {
        try
        {
            if (string.IsNullOrEmpty(LoginId))
            {
                ErrorHandler ("Login ID cannot be empty.");
            }
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string SqlStatement = "UPDATE quiz_users SET user_status = TRUE WHERE login_id = @LoginId";

                using (NpgsqlCommand command = new NpgsqlCommand(SqlStatement, connection))
                {
                    command.Parameters.AddWithValue("@LoginId", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (NpgsqlException ex)
        {
            // Handle PostgreSQL-related exceptions
            // Log the exception, display an error message, or take appropriate actions based on your application's requirements
            ErrorHandler($"An error occurred while disabling the user: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            // Log the exception, display an error message, or take appropriate actions based on your application's requirements
            ErrorHandler($"An unexpected error occurred while disabling the user: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
    }
}
