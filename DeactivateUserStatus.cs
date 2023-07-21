using Npgsql;
internal partial class Program
{
    public static async Task<string> DeactivateUserStatus(string LoginId, string connectionString)
    {
       
        
       if ( string.IsNullOrWhiteSpace( LoginId ) )
            {
            return "Login Id can not be empty";
            }
        try
        {

             using(NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string SqlStatement = "SELECT COUNT(*) As count FROM quiz_users WHERE login_id ILIKE @loginid";

                using (NpgsqlCommand Command= new NpgsqlCommand(SqlStatement, connection))
                {
                     Command.Parameters.AddWithValue("@loginid", NpgsqlTypes.NpgsqlDbType.Varchar, LoginId);

                            
                    int count = Convert.ToInt32(await Command.ExecuteScalarAsync());

                    if (count == 0)
                     {
                        return "Login ID Does Not Exist";
                    }
                }
            }
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string SqlStatement = "UPDATE quiz_users SET user_status = FALSE WHERE login_id ILIKE @LoginId";

                    using (NpgsqlCommand command = new NpgsqlCommand(SqlStatement, connection))
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
            ErrorHandler($"An error occurred while deactivating the user: {ex.Message}");
            throw; // Re-throw the exception if necessary
        }
        
    }

}
