# Build stage - Use SDK that matches your project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy just the project file first for efficient layer caching
COPY ["AgriEnergyConnect.API/AgriEnergyConnect.API.csproj", "AgriEnergyConnect.API/"]
RUN dotnet restore "AgriEnergyConnect.API/AgriEnergyConnect.API.csproj"

# Copy remaining source files
COPY . .

# Build the project
WORKDIR "/src/AgriEnergyConnect.API"
RUN dotnet build "AgriEnergyConnect.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AgriEnergyConnect.API.csproj" -c Release -o /app/publish

# Runtime stage - Use ASP.NET runtime that matches your SDK
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "AgriEnergyConnect.API.dll"]