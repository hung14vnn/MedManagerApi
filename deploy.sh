#!/bin/bash

# MedManager API - Production Deployment Script
# This script uses docker-compose.prod.yml and requires .env file

set -e  # Exit on error

echo "🚀 MedManager API - Production Deployment"
echo "=========================================="
echo ""

# Check if .env file exists
if [ ! -f .env ]; then
    echo "❌ Error: .env file not found!"
    echo ""
    echo "Please create .env file with production settings:"
    echo "  1. cp .env.example .env"
    echo "  2. nano .env"
    echo "  3. Update all values (JWT key, email, passwords)"
    echo ""
    echo "See CONFIGURATION_ENVIRONMENTS.md for details."
    exit 1
fi

echo "✅ Found .env file"

# Validate required environment variables
echo "🔍 Validating environment variables..."
source .env

required_vars=("POSTGRES_PASSWORD" "JWT_KEY" "EMAIL_SMTP_HOST" "EMAIL_SMTP_PASSWORD" "SUPERADMIN_PASSWORD")
missing_vars=()

for var in "${required_vars[@]}"; do
    if [ -z "${!var}" ]; then
        missing_vars+=("$var")
    fi
done

if [ ${#missing_vars[@]} -ne 0 ]; then
    echo "❌ Error: Missing required environment variables in .env:"
    for var in "${missing_vars[@]}"; do
        echo "  - $var"
    done
    echo ""
    echo "Please update your .env file."
    exit 1
fi

echo "✅ All required variables present"
echo ""

# Pull latest code (if using git)
echo "📥 Pulling latest code..."
if git rev-parse --git-dir > /dev/null 2>&1; then
    git pull origin master || echo "⚠️  No changes or unable to pull"
else
    echo "⚠️  Not a git repository"
fi
echo ""

# Stop existing containers
echo "🛑 Stopping existing containers..."
docker-compose -f docker-compose.prod.yml down
echo ""

# Build new images
echo "🔨 Building Docker images (this may take a few minutes)..."
docker-compose -f docker-compose.prod.yml build --no-cache
echo ""

# Start containers
echo "▶️  Starting containers..."
docker-compose -f docker-compose.prod.yml up -d
echo ""

# Wait for database to be ready
echo "⏳ Waiting for database to be ready..."
sleep 10

# Check if containers are running
echo "🔍 Checking container status..."
docker-compose -f docker-compose.prod.yml ps
echo ""

# Run migrations
echo "🗄️  Running database migrations..."
if docker-compose -f docker-compose.prod.yml exec -T app dotnet ef database update; then
    echo "✅ Migrations completed successfully"
else
    echo "⚠️  Migration failed or database already up to date"
fi
echo ""

# Show deployment summary
echo "=========================================="
echo "✅ Deployment Complete!"
echo "=========================================="
echo ""
echo "📊 Container Status:"
docker-compose -f docker-compose.prod.yml ps
echo ""
echo "🌐 Application should be available"

# Show logs
docker-compose -f docker-compose.prod.yml logs -f app
