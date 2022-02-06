FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
# copy sln and csproj files into the image
COPY *.sln .
COPY src/Bot.Model/*.csproj ./src/Bot.Model/
COPY src/Bot.Modules/*.csproj ./src/Bot.Modules/
COPY src/Bot.Services/*.csproj ./src/Bot.Services/
COPY src/DotA2.Gambling.Context/*.csproj ./src/DotA2.Gambling.Context/
COPY src/DotA2.Gambling.Model/*.csproj ./src/DotA2.Gambling.Model/
COPY src/DotA2GamblingMachine/*.csproj ./src/DotA2GamblingMachine/
COPY DotA2GambleBot/*.csproj ./DotA2GambleBot/

COPY tests/Bot.Gamble.Tests/*.csproj ./tests/Bot.Gamble.Tests/

# restore package dependencies for the solution
RUN dotnet restore

COPY . .
WORKDIR "/src/."
RUN dotnet build "DotA2GambleBot/DotA2GambleBot.csproj" -c Release -o /app/build 

FROM build AS publish
RUN dotnet publish "DotA2GambleBot/DotA2GambleBot.csproj" -c Release -o /app/publish

 FROM base AS final
 WORKDIR /app
  COPY --from=publish /app/publish .
 ENTRYPOINT ["dotnet", "DotA2GambleBot.dll"]

# RUN dotnet restore "DiscordBot.csproj"
# COPY . .
# WORKDIR "/src/."
# RUN dotnet build "DiscordBot.csproj" -c Release -o /app/build
# 
# FROM build AS publish
# RUN dotnet publish "DiscordBot.csproj" -c Release -o /app/publish
# 
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "DiscordBot.dll"]