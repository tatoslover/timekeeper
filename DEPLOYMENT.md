# Deployment Guide for Toastmasters Timer

This document outlines the deployment process for the Toastmasters Timer application, a Blazor WebAssembly project hosted on Azure Static Web Apps.

## Overview

The application is deployed using the Azure Static Web Apps CLI via GitHub Actions. This method has proven to be more reliable than other deployment approaches for Blazor WebAssembly applications.

## Prerequisites

- Azure account with an active subscription
- GitHub account
- Azure Static Web Apps resource
- Deployment token from Azure

## Deployment Architecture

The application uses the following components:

1. **Blazor WebAssembly**: Client-side .NET code that runs entirely in the browser
2. **Azure Static Web Apps**: Hosting platform for static web applications
3. **GitHub Actions**: CI/CD pipeline for automated deployments
4. **Azure Static Web Apps CLI**: Command-line tool for deploying to Azure Static Web Apps

## Deployment Configuration

### 1. Azure Static Web Apps Configuration

The `staticwebapp.config.json` file contains essential configuration for Azure Static Web Apps to properly serve Blazor WebAssembly content:

- **MIME Types**: Ensures correct content types for .wasm, .dll, and other Blazor-specific files
- **Routes**: Configures how requests are handled, particularly for SPA routing
- **Navigation Fallback**: Ensures client-side routing works correctly
- **Caching**: Optimizes performance for static assets

### 2. GitHub Actions Workflow

The deployment workflow is defined in `.github/workflows/swa-cli.yml` and includes:

- Setting up .NET and Node.js environments
- Caching dependencies for faster builds
- Building the Blazor WebAssembly application
- Deploying to Azure Static Web Apps using the SWA CLI

## Deployment Process

When code is pushed to the main branch, the GitHub Actions workflow automatically:

1. Builds the application with `dotnet publish -c Release`
2. Verifies the presence of `staticwebapp.config.json`
3. Deploys the application using the Azure Static Web Apps CLI

## Troubleshooting

### Common Issues

1. **MIME Type Errors**:
   - Ensure `staticwebapp.config.json` contains correct MIME type mappings
   - Verify that `Content-Type` headers are properly set in the routes section

2. **Routing Issues**:
   - Check the `navigationFallback` configuration in `staticwebapp.config.json`
   - Ensure SPA routing is properly configured

3. **Authentication Problems**:
   - Verify the deployment token is correctly set in GitHub secrets
   - Regenerate the token if necessary

4. **File Loading Failures**:
   - Check browser console for 404 errors on specific files
   - Verify the `output_location` parameter in the deployment workflow

## Manual Deployment

If needed, you can deploy manually using the Azure Static Web Apps CLI:

```bash
# Install the CLI
npm install -g @azure/static-web-apps-cli

# Build the application
dotnet publish -c Release

# Deploy using the CLI
swa deploy ./bin/Release/net8.0/publish/wwwroot --deployment-token <your-token> --env production
```

## Monitoring Deployments

You can monitor deployment status:

1. In GitHub Actions: See the workflow runs under the "Actions" tab in your repository
2. In Azure Portal: Check the "Deployment history" section of your Static Web App resource

## Best Practices

1. **Keep Configuration Updated**: When updating the Blazor framework, check if MIME types need updating
2. **Version Control**: Commit `staticwebapp.config.json` to source control
3. **Test Locally**: Test the application with `swa start` before deploying
4. **Secure Tokens**: Never commit deployment tokens to source control

## References

- [Azure Static Web Apps Documentation](https://docs.microsoft.com/en-us/azure/static-web-apps/)
- [Blazor WebAssembly Hosting Documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/host-and-deploy/webassembly)
- [Azure Static Web Apps CLI Documentation](https://github.com/Azure/static-web-apps-cli)