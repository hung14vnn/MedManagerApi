#!/bin/bash
set -e

# Wait for Postgres
until psql "$ConnectionStrings__DefaultConnection" -c "select 1" >/dev/null 2>&1; do
  echo "Waiting for Postgres..."
  sleep 1
done

# Run EF migrations if any (ignore if dotnet-ef not available)
if dotnet ef database update --no-build 2>/dev/null; then
  echo "Migrations applied"
else
  echo "dotnet-ef not available or no migrations to apply"
fi

# Seed database using SeedData project if present
if [ -d "Tools/SeedData" ]; then
  dotnet run --project Tools/SeedData/SeedData.csproj --no-build --configuration Release || true
fi

exec "$@"
