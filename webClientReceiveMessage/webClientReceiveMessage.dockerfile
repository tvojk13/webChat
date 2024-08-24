FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["webClientReceiveMessage/webClientReceiveMessage.csproj", "webClientReceiveMessage/"]

RUN dotnet restore "webClientReceiveMessage/webClientReceiveMessage.csproj"
COPY . .
WORKDIR "/src/webClientReceiveMessage"
RUN dotnet build "webClientReceiveMessage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "webClientReceiveMessage.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "webClientReceiveMessage.dll"]
