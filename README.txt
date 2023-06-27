Using environment variables
===========================

Benefit of using App Sercrets:
------------------------------
The app secrets are associated with a specific project or shared across several projects. 
The app secrets aren't checked into source control.

1. Clone this project

2. Change into the project folder and run:
dotnet user-secrets init

The preceding command adds a UserSecretsId element within a PropertyGroup of the project csproj file. 
By default, the inner text of UserSecretsId is a GUID. 
The inner text is arbitrary, but is unique to the project.

3. Define an app secret consisting of a key and its value.
dotnet user-secrets set "PgSql:DbPassword" "password"
NOTE:Use the usual password we've been using to access PGAdmin.

The preceding command creates %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json folder and file.
Note that this folder is on your local machine and is not it the project folder so it wont be checked into source control.
To access the hidden APPDATA folder, you can copy paste the following into file explorer address bar: %APPDATA%\Microsoft\UserSecrets\
You should see a GUID folder mathcing the UserSecretsId in the project csproj file.

4. Check your entries
dotnet user-secrets list

5. Once you have performed the above, use the Settings.user file provided by Fred.

6. This project has the following routes:

GET
---
/
> Returns this page.


/users
> Returns all users.



/user/tateclinton
>  Returns a specific user. Supply the loginname.

/user?loginname=fredkhan
> Same as above but using query parameters.



/checkanswer/10/d
> Returns true or false depending on the supplied question id and option chosen.

/checkanswer?questionid=22&optionname=b
> Same as above but using query parameters


/generatequiz
> Returns the OpenAI generated quiz content as JSON but does not store it anywhere.

/generateandstorequiz
> Stores the OpenAI generated quiz content into Postgres and then returns it as JSON.
Requires the following to be set: dotnet user-secrets set "OpenAI:APIKey" "Your-API-Key"


POST
----
Use Thuunder Client for testing the POST routes

/recordanswer/fredkhan/19/a
> Inserts the values into history then returns true or false depending on the supplied question id and option chosen.

/recordanswer?loginname=fredkhan&questionid=29&optionname=b
> Same as above but using query parameters.

/adduser?loginname=anhnguyen&firstname=Anh&lastname=nguyen
> Adds 1 user using query parameters and returns the values that was added to quiz_users table.