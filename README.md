# Toastmasters Timer

[![GitHub Repo](https://img.shields.io/badge/GitHub-Repo-blue?logo=github)](https://github.com/tatoslover/timekeeper)

[![Live Demo – Azure SWA](https://img.shields.io/badge/Live%20Demo-Azure%20SWA-blue?logo=microsoftazure&logoColor=white)](https://green-water-06a334700.2.azurestaticapps.net)

A clean, simple timer application specifically designed for Toastmasters speeches. This application helps timekeepers track speech times with visual color indicators and provides detailed reports after each speech.

## Features

- **Simple Visual Interface**: Background color changes based on timing thresholds
- **Configurable Timing**: Set custom timing parameters for different speech types
- **Preset Timings**: Built-in presets for standard Toastmasters speeches
- **Detailed Reports**: Get a summary of timing results after each speech
- **Responsive Design**: Works on desktop and mobile devices

## How to Use

1. **Configure the Timer**:
   - Select a preset speech type or create your own
   - Set the green, orange, and red timing thresholds

2. **Run the Timer**:
   - Press "Start Timer" when the speaker begins
   - The background changes color based on timing thresholds:
     - White: Keep going (under minimum time)
     - Green: Minimum time met
     - Orange: Approaching maximum time
     - Red: Maximum time reached

3. **End the Speech**:
   - Press "Stop" when the speaker finishes
   - View the timing report
   - Press "Reset" to prepare for the next speaker

## Technology

- Built with C# and Blazor WebAssembly (.NET 8.0)
- Single page application (SPA) that runs entirely in the browser
- No server-side code required
- Deployable to any static hosting service

## Development

### Prerequisites

- .NET 8.0 SDK or later

### Building and Running Locally

```bash
# Clone the repository
git clone https://github.com/tatoslover/timekeeper.git
cd timekeeper

# Build and run the application
dotnet build
dotnet run
```

## Deployment

This project is deployed on Azure Static Web Apps using the Azure SWA CLI method. To deploy your own instance:

1. Fork this repository
2. Create an Azure Static Web App resource in the Azure Portal
3. Generate a deployment token in the Azure Portal:
   - Go to your Static Web App resource
   - Navigate to "Manage deployment token"
   - Copy the token
4. Add the token as a GitHub Secret named `AZURE_STATIC_WEB_APPS_API_TOKEN_YOUR_APP_NAME`
5. The GitHub workflow will automatically build and deploy the application

The deployment uses GitHub Actions with the Azure Static Web Apps CLI, which provides more reliable deployment for Blazor WebAssembly applications.

### Troubleshooting Deployment

If you encounter MIME type errors or other issues:
- Make sure `staticwebapp.config.json` is properly configured
- Verify that the deployment token is correctly set in your GitHub secrets
- Check the GitHub Actions logs for detailed error messages

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
