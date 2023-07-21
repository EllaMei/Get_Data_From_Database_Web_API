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

        // Display specific user info. Using query parameters in the URL eg: http://localhost:5000/user?loginname=
        app.MapGet("/user", (string loginname) => GetUserByLoginName(loginname, connectionString));

 
     //  Insert user to quiz users table. Using query parameters in the URL eg: http://localhost:5000/adduser?loginid=&firstname=&lastname=
        app.MapPost("/adduser",(string LoginId, string FirstName, string LastName, string Password) => AddUser(LoginId ?? string.Empty, FirstName ?? string.Empty, LastName ?? string.Empty, Password ?? string.Empty, connectionString) );      

        //Return all questions, options and user's answers for completed questions
        app.MapGet("/getalloldquiz", (string loginname) => GetAllOldQuiz(loginname, connectionString));

        // Return json of all questions and options eg: http://localhost:5000/getquiz
        app.MapGet("/getquiz", () => GetQuiz(connectionString));

        // Return json of unattempted questions and options eg: http://localhost:5000/getnewquiz?loginname=
        app.MapGet("/getnewquiz", (string loginname) => GetNewQuiz(loginname, connectionString));

        // Return json of attempted questions and options eg: http://localhost:5000/getnewquiz?loginname=
        app.MapGet("/getoldquiz", (string loginname) => GetOldQuiz(loginname, connectionString));


 
        //  Display True or False. Using route parameters in the URL eg: http://localhost:5000/checkanswer/21/b
        app.MapGet("/checkanswer/{question_id}/{optionname}", (int question_id, char optionname) => CheckAnswer(question_id, Char.ToUpper(optionname), connectionString));

        // Display True or False. Using query parameters in the url eg: http://localhost:5000/checkanswer?questionid=&optionname=
        app.MapGet("/checkanswer", (int question_id, char optionname) => CheckAnswer(question_id, Char.ToUpper(optionname), connectionString));



        // Insert record to quiz history table and returns true or false. Using route parameters in the URL eg: http://localhost:5000/recordanswer/anhnguyen/11/d
        // The route spells out as follows: user / the question ID / the answer given for that question ID
       //  Insert record to quiz history table and returns true or false. Using query parameters in the URL eg: http://localhost:5000/addhistory?loginid=anhnguyen&questionid=11&optionname=d
        app.MapPost("/recordanswer", async (context) =>
        {
        // Get the value of the "loginid" parameter from the request query string.
            string? loginid = context.Request.Query["loginid"];

        // Try to parse the value of the "questionid" parameter from the request query string to an integer.
        // If successful, assign the parsed value to the "questionid" variable.
        // If parsing fails, assign a default value of 0 to "questionid".
            int questionid;
            if (!int.TryParse(context.Request.Query["questionid"], out questionid)) questionid = 0;

        // Get the value of the "optionname" parameter from the request query string.
        // If there are multiple values for "optionname", only the first value will be assigned to "optionname".
            string? optionname = context.Request.Query["optionname"].FirstOrDefault();

        // Convert the first character of "optionname" to uppercase and assign it to "optionChar".
        // If "optionname" is null or empty, assign '\0' (null character) to "optionChar".
            char optionChar = !string.IsNullOrEmpty(optionname) ? char.ToUpper(optionname[0]) : '\0';

        // Check that all parameters are provided and question id was successfully parsed and valid
            if (string.IsNullOrEmpty(loginid) || questionid <= 0  || optionChar == '\0')
            {
            // Set the HTTP response status code to 400 (Bad Request).
                context.Response.StatusCode = 400;

            // Write an error message to the response.
                await context.Response.WriteAsync("One or more parameters are missing.");

            // Exit the function early.
                return;
            }

        // Call the "RecordAnswer" method, passing the "loginid", "questionid", "optionChar", and "connectionString" parameters.
        // Await the method's asynchronous execution and assign the returned value to "result".
            string? result = await RecordAnswer(loginid, questionid, optionChar, connectionString);

        // Write the value of "result" to the response, or an empty string if "result" is null.
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




        //  Update user to quiz users table. Using query parameters in the URL eg: http://localhost:5000/updateuser?loginid=&firstname=&lastname=&password=
        app.MapPut("/updateuser",(string LoginId, string FirstName, string LastName, string Password) => UpdateUser(LoginId ?? string.Empty, FirstName ?? string.Empty, LastName ?? string.Empty, Password ?? string.Empty, connectionString)); 

        //Activate users to get access to quiz http://localhost:5000/activateUserStatus?loginid=
             app.MapPut("/activateuserstatus", (string LoginId) => ActivateUserStatus (LoginId ?? string.Empty,connectionString));

        //Deactivate users to get access to quiz http://localhost:5000/deactivateuserstatus?login_id=
        app.MapPut("/deactivateuserstatus", (string Login_Id) => DeactivateUserStatus(Login_Id ?? string.Empty,connectionString));   

           
           //Login endpoint URL eg: http://localhost:5000/userlogin?login_id=&password=
            app.MapPost("/userlogin", (string Login_Id,string Password) => UserLogin(Login_Id ?? string.Empty, Password ?? string.Empty, connectionString));


             

        app.Run();
    }
}