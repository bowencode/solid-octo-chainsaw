Start-Process dotnet -ArgumentList 'run --project ".\src\Identity\IdentityServer\IdentityServer.csproj"'
Start-Sleep -Seconds 2

Start-Process dotnet -ArgumentList 'run --project ".\src\Web\Demo.Notes.Web\Demo.Partner.ExternalApi.Host\Demo.Partner.ExternalApi.Host.csproj"'
Start-Sleep -Seconds 2
Start-Process dotnet -ArgumentList 'run --project ".\src\Web\Demo.Notes.Web\Demo.Notes.Web.AdminApi.Host\Demo.Notes.Web.AdminApi.Host.csproj"'
Start-Sleep -Seconds 2
Start-Process dotnet -ArgumentList 'run --project ".\src\Web\Demo.Notes.Web\Demo.Notes.Web.UserApi.Host\Demo.Notes.Web.UserApi.Host.csproj"'
Start-Sleep -Seconds 2

Start-Process dotnet -ArgumentList 'run --project ".\src\Web\Demo.Notes.Web\Demo.Notes.Web.Host\Demo.Notes.Web.Host.csproj"'
Start-Sleep -Seconds 2
Start-Process dotnet -ArgumentList 'run --project ".\src\Web\Demo.Notes.Web\Demo.Notes.Web.Blazor\Server\Demo.Notes.Web.Blazor.Server.csproj"'
Start-Sleep -Seconds 2

