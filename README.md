Using environment variables
===========================

Benefit of using App Sercrets:
------------------------------
The app secrets are associated with a specific project or shared across several projects. 
The app secrets aren't checked into source control.

1. Create your project with dotnew<br>

2. In the project folder run:<br>
dotnet user-secrets init<br>

The preceding command adds a UserSecretsId element within a PropertyGroup of the project csproj file. 
By default, the inner text of UserSecretsId is a GUID. 
The inner text is arbitrary, but is unique to the project.

3. Define an app secret consisting of a key and its value.<br>
dotnet user-secrets set "OpenAI:APIKey" "aaaaaaaaaaaa"<br>
dotnet user-secrets set "PgSql:DbPassword" "postgres"<br>

The preceding command creates %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json folder and file.
Note that this folder is on your local machine and is not it the project folder so it wont be checked into source control.
To access the hidden APPDATA folder, you can copy paste the following into file explorer address bar: %APPDATA%\Microsoft\UserSecrets\
You should see a GUID folder mathcing the UserSecretsId in the project csproj file.

4. Check your entries<br>
dotnet user-secrets list<br>
