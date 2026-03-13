FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["indy-scca-discord-bot.csproj", "./"]
RUN dotnet restore "indy-scca-discord-bot.csproj"

COPY . .
RUN dotnet publish "indy-scca-discord-bot.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "indy-scca-discord-bot.dll"]

