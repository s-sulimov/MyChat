**ABOUT:**
<br />Pet project contains simple chat server and simple client for manual testing.

**REQUIREMENTS**
1. Docker Descktop 4.16 and up.
2. Visual Studio 2022 (web development workload).

**DEBUG**
1. Run MS SQLServer in Docker:
<br />`docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=S3cur3P@ssW0rd!" -p 1433:1433 -d -v my-chat-server_sqlData:/var/opt/mssql mcr.microsoft.com/mssql/server:2022-latest`
2. Open `src\MyChatServer\MyChatServer.sln` in Visual Studio.
3. Set connection string in `appsetings.Development.json`:
<br />`Server=localhost;Database=myChat;User Id=sa;Password=S3cur3P@ssW0rd!;TrustServerCertificate=True;`
4. Press `F5` to run in debug mode.

**BUILD**
<br />In `src` folder run
<br />`docker compose build`

**RUN**
<br />In `src` folder run
<br />`docker compose up`
