# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and csproj files for restore
COPY OrderProcessing.sln ./
COPY src/OrderProcessing.Contracts/OrderProcessing.Contracts.csproj src/OrderProcessing.Contracts/
COPY src/OrderProcessing.Infrastructure/OrderProcessing.Infrastructure.csproj src/OrderProcessing.Infrastructure/
COPY src/OrderProcessing.Worker/OrderProcessing.Worker.csproj src/OrderProcessing.Worker/
RUN dotnet restore src/OrderProcessing.Worker/OrderProcessing.Worker.csproj

# Copy all sources
COPY src/ ./src/

# Publish
RUN dotnet publish src/OrderProcessing.Worker/OrderProcessing.Worker.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderProcessing.Worker.dll"]
