# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (optimizes caching)
COPY *.csproj ./
RUN dotnet restore

# Copy remaining code and publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Replace with your actual project DLL name
ENTRYPOINT ["dotnet", "Todo_App.dll"]