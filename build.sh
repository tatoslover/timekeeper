#!/bin/bash
set -e

# Build script for Blazor WebAssembly Toastmasters Timer app

echo "Starting build process for Toastmasters Timer..."

# Navigate to the timekeeper directory
cd "$(dirname "$0")"

# Clean any previous build artifacts
echo "Cleaning previous build artifacts..."
rm -rf bin obj

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Build the application
echo "Building the application..."
dotnet build -c Release

# Publish the application
echo "Publishing the application..."
dotnet publish -c Release -o published

# Verify the published output
if [ -d "published/wwwroot" ]; then
    echo "Build completed successfully!"
    echo "The application has been published to: $(pwd)/published/wwwroot"
else
    echo "Error: Build failed to produce the expected output directory."
    exit 1
fi

# Create a .deployment file to help Azure identify the published directory
echo "[config]" > published/.deployment
echo "appdir=wwwroot" >> published/.deployment

echo "Build process completed successfully."
