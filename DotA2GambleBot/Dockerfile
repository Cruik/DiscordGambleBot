#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DotA2GambleBot/DotA2GambleBot.csproj", "DotA2GambleBot/"]
COPY ["src/Bot.Model/Bot.Model.csproj", "src/Bot.Model/"]
COPY ["src/DotA2GamblingMachine/DotA2GamblingMachine.csproj", "src/DotA2GamblingMachine/"]
COPY ["src/DotA2.Gambling.Model/DotA2.Gambling.Model.csproj", "src/DotA2.Gambling.Model/"]
COPY ["src/Bot.Services/Bot.Services.csproj", "src/Bot.Services/"]
COPY ["src/DotA2.Gambling.Context/DotA2.Gambling.Context.csproj", "src/DotA2.Gambling.Context/"]
COPY ["src/Bot.Modules/Bot.Modules.csproj", "src/Bot.Modules/"]
RUN dotnet restore "DotA2GambleBot/DotA2GambleBot.csproj"
COPY . .
WORKDIR "/src/DotA2GambleBot"
RUN dotnet build "DotA2GambleBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotA2GambleBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotA2GambleBot.dll"]