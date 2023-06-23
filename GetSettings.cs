internal partial class Program
{   
    public static dynamic GetSettings()
    {

        // Setting file name
        string settingsFile = "Settings.user";
        string fileNotFoundError = $">>> ERROR: {settingsFile} not found.\nPlease create {settingsFile} file and enter all required information.";


        // Check that the file exists
        if (!File.Exists(settingsFile)) 
        {
            Console.WriteLine(fileNotFoundError);
            return fileNotFoundError;
        }

        // Read configuration parameters from settings file
        var settings = File.ReadAllLines(settingsFile)
                    .Select(l => l.Split(new[] { '=' }))
                    .ToDictionary(s => s[0].Trim(), s => s[1].Trim());
        
        return settings;

    } // End GetSettings

}