# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and csproj files for restore
COPY OrderProcessing.sln ./
COPY src/OrderProcessing.Contracts/OrderProcessing.Contracts.csproj src/OrderProcessing.Contracts/
COPY src/OrderProcessing.Infrastructure/OrderProcessing.Infrastructure.csproj src/OrderProcessing.Infrastructure/
COPY src/OrderProcessing.AdminApi/OrderProcessing.AdminApi.csproj src/OrderProcessing.AdminApi/
RUN dotnet restore src/OrderProcessing.AdminApi/OrderProcessing.AdminApi.csproj

# Copy all sources
COPY src/ ./src/

# Publish
RUN dotnet publish src/OrderProcessing.AdminApi/OrderProcessing.AdminApi.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "OrderProcessing.AdminApi.dll"]
