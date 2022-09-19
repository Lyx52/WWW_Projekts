FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80/tcp
EXPOSE 443/tcp

# Build application using
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY [ "WebProject.csproj", "." ]
RUN dotnet restore "WebProject.csproj"
COPY . .
RUN dotnet build "WebProject.csproj" -c Release -o /app/build

# Publish built application
FROM build AS publish
RUN dotnet publish "WebProject.csproj" -c Release -o /app/publish

# Run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .