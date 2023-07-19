using Npgsql;
internal partial class Program
{
    public static async Task<string?> ActivateUserStatus(string LoginId, string connectionString)
    {
        try
        {
            if (string.IsNullOrEmpty(LoginId))
            {
                return"Login ID cannot be empty.";
            }
            using(NpgsqlConnection connection = new(connectionString))
            {
                await connection.OpenAsync();
                string SqlStatement = "SELECT COUNT(*) As count FROM quiz_users WHERE login_id ILIKE @loginid";

                using NpgsqlCommand Command = new(SqlStatement, connection);
                Command.Parameters.AddWithValue("@loginid", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);


                int count = Convert.ToInt32(await Command.ExecuteScalarAsync());
                
                // If no rows are found, return the "Login ID Does Not Exist" message
                if (count == 0)
                {
                    return "Login ID Does Not Exist";
                }
            }
            using (NpgsqlConnection connection = new(connectionString))
            {
                await connection.OpenAsync();

                string SqlStatement = "UPDATE quiz_users SET user_status = TRUE WHERE login_id ILIKE @LoginId";

                using (NpgsqlCommand command = new(SqlStatement, connection))
                {
                    command.Parameters.AddWithValue("@LoginId", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return string.Empty;
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
