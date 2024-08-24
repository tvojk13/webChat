FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["webClientSendMessage/webClientSendMessage.csproj", "webClientSendMessage/"]

RUN dotnet restore "webClientSendMessage/webClientSendMessage.csproj"
COPY . .
WORKDIR "/src/webClientSendMessage"
RUN dotnet build "webClientSendMessage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "webClientSendMessage.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "webClientSendMessage.dll"]
