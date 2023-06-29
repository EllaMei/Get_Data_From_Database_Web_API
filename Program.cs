internal partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Read the OpenAi Api Key and Database Password from environment variable that you set with dotnet user-secrets set command.
        string openAiApiKey = builder.Configuration["OpenAI:APIKey"] ?? String.Empty;
        string pgsqlDbPassword = builder.Configuration["PGSQL:DbPassword"] ?? String.Empty;
        
        Dictionary<string, string>? settings = GetSettings(); // Declare a dictionary variable to load and store the settings from Settings.user file

        // Create Connection String for NPGSQL
        string connectionString = $"Host={settings["HOST"]};Username={settings["USERNAME"]};Password={pgsqlDbPassword};Database={settings["DATABASE"]}"; 

        var app = builder.Build();

        // Display README.txt eg: http://localhost:5000/
        app.MapGet("/", () =>
        {
            string readmeContents = File.ReadAllText("README.txt");
            return readmeContents;
        });  
     
        app.MapGet("/users", () => GetUsers(connectionString)); // Display all the users in the table in JSON format eg: http://localhost:5000/users

        // Display the OpenAI generated Quiz as JSON but does not store it anywhere. Refreshing the page will re-generate new quiz.
        app.MapGet("/generatequiz", () =>
        {
            return GenerateQuiz(
        openAiApiKey, 
        settings["APIURL"], 
        settings["PROMPT"], 
        settings["MODEL"]);
        });

        // Add the OpenAI generated Quiz to database and return it as JSON as well
        app.MapGet("/generateandstorequiz", () =>
        {
            return GenerateAndStoreQuiz(
        openAiApiKey, 
        settings["APIURL"], 
        settings["PROMPT"], 
        settings["MODEL"],
        connectionString);
        });

        //  Display specific user info. Using route parameters in the URL eg: http://localhost:5000/user/tateclinton
        app.MapGet("/user/{loginname}", (string loginname) => GetUserByLoginName(loginname, connectionString));

        // Display specific user info. Using query parameters in the URL eg: http://localhost:5000/user?loginname=tateclinton
        app.MapGet("/user", (string loginname) => GetUserByLoginName(loginname, connectionString));

 
        //  Insert user to quiz users table. Using query parameters in the URL eg: http://localhost:5000/adduser?loginname=anhnguyen&firstname=anh&lastname=nguyen
        app.MapPost("/adduser", async (context) =>
        {
            string? loginname = context.Request.Query["loginname"];
            string? firstname = context.Request.Query["firstname"];
            string? lastname = context.Request.Query["lastname"];

            string? result = null;
            if (!string.IsNullOrEmpty(loginname) && !string.IsNullOrEmpty(firstname) && !string.IsNullOrEmpty(lastname))
            {

                result = await AddUser(loginname, firstname, lastname, connectionString);
            }

            await context.Response.WriteAsync(result ?? string.Empty);
        });



        // Return json of all questions and options eg: http://localhost:5000/getquiz
        app.MapGet("/getquiz", () => GetQuiz(connectionString));

        // Return json of unattempted questions and options eg: http://localhost:5000/getnewquiz?loginname=fredkhan
        app.MapGet("/getnewquiz", (string loginname) => GetNewQuiz(loginname, connectionString));

        // Return json of attempted questions and options eg: http://localhost:5000/getnewquiz?loginname=fredkhan
        app.MapGet("/getoldquiz", (string loginname) => GetOldQuiz(loginname, connectionString));


 
        //  Display True or False. Using route parameters in the URL eg: http://localhost:5000/checkanswer/21/b
        app.MapGet("/checkanswer/{questionid}/{optionname}", (int questionid, char optionname) => CheckAnswer(questionid, Char.ToUpper(optionname), connectionString));

        // Display True or False. Using query parameters in the url eg: http://localhost:5000/checkanswer?questionid=21&optionname=b
        app.MapGet("/checkanswer", (int questionid, char optionname) => CheckAnswer(questionid, Char.ToUpper(optionname), connectionString));



        // Insert record to quiz history table and returns true or false. Using route parameters in the URL eg: http://localhost:5000/recordanswer/anhnguyen/11/d
        // The route spells out as follows: user / the question ID / the answer given for that question ID
        app.MapPost("/recordanswer/{loginname}/{questionid}/{optionname}", async (context) =>
        {
            string? loginname = context.Request.RouteValues["loginname"] as string;

            //int questionid = int.Parse(context.Request.RouteValues["questionid"] as string);
            string? questionIdString = context.Request.RouteValues["questionid"] as string;
            int questionid = 0;
            int.TryParse(questionIdString, out questionid);
            
            //char optionname = char.Parse(context.Request.RouteValues["optionname"] as string);
            string? optionNameString = context.Request.RouteValues["optionname"] as string;
            char optionname = !string.IsNullOrEmpty(optionNameString) ? optionNameString[0] : '\0';            

            //string? result = RecordAnswer(loginname, questionid, char.ToUpper(optionname), connectionString);
            string? result = null;
            if (loginname != null)
            {
                result = await RecordAnswer(loginname, questionid, char.ToUpper(optionname), connectionString);
            }
            
            //await context.Response.WriteAsync(result);
            await context.Response.WriteAsync(result ?? string.Empty);
        });


        //  Insert record to quiz history table and returns true or false. Using query parameters in the URL eg: http://localhost:5000/recordanswer?loginname=anhnguyen&questionid=11&optionname=d
        app.MapPost("/recordanswer", async (context) =>
        {
            string? loginname = context.Request.Query["loginname"];
            //int questionid = int.Parse(context.Request.Query["questionid"]);
            int questionid;
            if (!int.TryParse(context.Request.Query["questionid"], out questionid))
            {
                questionid = 0; // Assign a default value of 0 in case no question ID was supplied.
            }

            string? optionname = context.Request.Query["optionname"].FirstOrDefault();

            string? result = null;
            if (!string.IsNullOrEmpty(loginname))
            {
                char optionChar = !string.IsNullOrEmpty(optionname) ? char.ToUpper(optionname[0]) : '\0';
                result = await RecordAnswer(loginname, questionid, optionChar, connectionString);
            }

            await context.Response.WriteAsync(result ?? string.Empty);
        });



        app.Run();
    }
}