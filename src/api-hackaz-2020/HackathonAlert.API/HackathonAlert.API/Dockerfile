FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["HackathonAlert.API/HackathonAlert.API.csproj", "HackathonAlert.API/"]
RUN dotnet restore "HackathonAlert.API/HackathonAlert.API.csproj"
COPY . .
WORKDIR "/src/HackathonAlert.API"
RUN dotnet build "HackathonAlert.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HackathonAlert.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HackathonAlert.API.dll"]