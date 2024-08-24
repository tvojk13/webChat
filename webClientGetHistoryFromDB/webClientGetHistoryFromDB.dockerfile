FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["webClientGetHistoryFromDB/webClientGetHistoryFromDB.csproj", "webClientGetHistoryFromDB/"]

RUN dotnet restore "webClientGetHistoryFromDB/webClientGetHistoryFromDB.csproj"
COPY . .
WORKDIR "/src/webClientGetHistoryFromDB"
RUN dotnet build "webClientGetHistoryFromDB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "webClientGetHistoryFromDB.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "webClientGetHistoryFromDB.dll"]
