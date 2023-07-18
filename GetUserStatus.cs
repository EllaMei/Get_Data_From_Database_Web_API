using System.Net.Http.Headers;
using Npgsql;
internal partial class Program
{
    public static async Task<bool> GetUserStatus(string LoginId, string connectionString)
    {
        try
        {

            if ( string.IsNullOrEmpty( LoginId ) )
            {
                return ErrorHandler("Please supply a Login ID");
            }
            
            using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            string SqlStatement = "SELECT user_status, login_id FROM quiz_users WHERE login_id ILIKE @LoginId";
            using (NpgsqlCommand command = new NpgsqlCommand(SqlStatement, connection))
            {
                command.Parameters.AddWithValue("@LoginId", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        bool UserStatus = Convert.ToBoolean(reader["user_status"]);
                        return UserStatus;
                    }
                }
            }
            

        }
        catch (NpgsqlException ex)
        {
            // Handle PostgreSQL-related exceptions
            ErrorHandler($"An error occurred while retrieving user status: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            ErrorHandler($"An unexpected error occurred while retrieving user status: {ex.Message}");
        }

        return false;
    }
}