﻿# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app
COPY --from=build /app/out ./

# Copy SQLite database file into runtime image
COPY VerseCraft.db ./

# Expose port 80
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "VerseCraft.dll"]
