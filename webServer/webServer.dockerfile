FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["webServer/webServer.csproj", "webServer/"]

RUN dotnet restore "webServer/webServer.csproj"
COPY . .
WORKDIR "/src/webServer"
RUN dotnet build "webServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "webServer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "webServer.dll"]
