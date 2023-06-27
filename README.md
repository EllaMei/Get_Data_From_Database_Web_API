Using environment variables
===========================

Benefit of using App Sercrets:
------------------------------
The app secrets are associated with a specific project or shared across several projects. 
The app secrets aren't checked into source control.

1. Clone this project<br>

2. Change into the project folder and run:<br>
dotnet user-secrets init<br>

The preceding command adds a UserSecretsId element within a PropertyGroup of the project csproj file. 
By default, the inner text of UserSecretsId is a GUID. 
The inner text is arbitrary, but is unique to the project.

3. Define an app secret consisting of a key and its value.<br>
dotnet user-secrets set "PgSql:DbPassword" "password"<br>
<em>NOTE:</em> Use the usual password we've been using to access PGAdmin.

The preceding command creates %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json folder and file.
Note that this folder is on your local machine and is not it the project folder so it wont be checked into source control.
To access the hidden APPDATA folder, you can copy paste the following into file explorer address bar: %APPDATA%\Microsoft\UserSecrets\
You should see a GUID folder mathcing the UserSecretsId in the project csproj file.

4. Check your entries<br>
dotnet user-secrets list<br>

5. Once you have performed the above, use the Settings.user file provided by Fred.<br>

6. This project has the following routes:<br>
GET<br>
---
/<br>
Returns this page.<br><br>

/users<br>
Returns all users.<br><br>

/user/tateclinton<br>
Returns a specific user. Supply the loginname.<br><br>

/user?loginname=fredkhan<br>
Same as above but using query parameters.<br><br>

/checkanswer/10/d<br>
Returns true or false depending on the supplied question id and option chosen.<br><br>

/checkanswer?questionid=22&optionname=b<br>
Same as above but using query parameters.<br><br>


/generatequiz<br>
Returns the OpenAI generated quiz content as JSON but does not store it anywhere.<br><br>

/generateandstorequiz<br>
Stores the OpenAI generated quiz content into Postgres and then returns it as JSON.Requires the following to be set:<br>
dotnet user-secrets set "OpenAI:APIKey" "Your-API-Key"<br><br>


POST<br>
----
<em>Use Thuunder Client for testing the POST routes</em><br>

/recordanswer/fredkhan/19/a<br>
Inserts the values into history then returns true or false depending on the supplied question id and option chosen.<br><br>

/recordanswer?loginname=fredkhan&questionid=29&optionname=b<br>
Same as above but using query parameters.<br><br>

/adduser?loginname=anhnguyen&firstname=Anh&lastname=nguyen<br>
Adds 1 user using query parameters and returns the values that was added to quiz_users table.<br><br>