# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj(s) and restore as distinct layers
COPY *.sln .
# Copy project files
COPY MedManagerApi/*.csproj ./MedManagerApi/
COPY Tools/SeedData/*.csproj ./Tools/SeedData/

RUN dotnet restore "MedManagerApi/MedManagerApi.csproj"

# Copy everything and build
COPY . .
RUN dotnet publish "MedManagerApi/MedManagerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "MedManagerApi.dll"]
