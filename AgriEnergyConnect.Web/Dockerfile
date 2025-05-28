# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AgriEnergyConnect.Web/AgriEnergyConnect.Web.csproj", "AgriEnergyConnect.Web/"]
RUN dotnet restore "AgriEnergyConnect.Web/AgriEnergyConnect.Web.csproj"
COPY . .
WORKDIR "/src/AgriEnergyConnect.Web"
RUN dotnet build "AgriEnergyConnect.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AgriEnergyConnect.Web.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "AgriEnergyConnect.Web.dll"]