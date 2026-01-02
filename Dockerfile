# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy entire repo into the container (ensures project and source files are colocated)
COPY . .

# Restore and publish the project (project file is at repository root)
RUN dotnet restore "MedManagerApi.csproj"
RUN dotnet publish "MedManagerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "MedManagerApi.dll"]
