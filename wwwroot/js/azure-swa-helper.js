// azure-swa-helper.js - A helper script to work around Content-Type issues in Azure Static Web Apps

// Create a self-executing function to avoid global scope pollution
(function () {
  console.log("Azure SWA Helper: Initializing...");

  // Function to load the Blazor WebAssembly JavaScript file directly
  function loadBlazorWebAssembly() {
    try {
      console.log("Azure SWA Helper: Creating direct script element...");

      // Create a new script element with direct src attribute
      const scriptElement = document.createElement("script");
      scriptElement.type = "text/javascript";
      scriptElement.src = "/_framework/blazor.webassembly.js";

      // Add error handler
      scriptElement.onerror = function (error) {
        console.error(
          "Azure SWA Helper: Error loading Blazor WebAssembly script:",
          error,
        );
        showError(
          "Failed to load the application. Please try refreshing the page.",
        );
      };

      // Add load handler
      scriptElement.onload = function () {
        console.log(
          "Azure SWA Helper: Blazor WebAssembly script loaded successfully",
        );
      };

      // Append the script to the document
      document.body.appendChild(scriptElement);
    } catch (error) {
      console.error(
        "Azure SWA Helper: Error setting up Blazor WebAssembly:",
        error,
      );
      showError(
        "Failed to load the application. Please try refreshing the page.",
      );
    }
  }

  // Function to display an error message to the user
  function showError(message) {
    const appElement = document.getElementById("app");
    if (appElement) {
      appElement.innerHTML = `
                <div style="padding: 20px; text-align: center; font-family: sans-serif;">
                    <h2 style="color: #dc3545;">Application Error</h2>
                    <p>${message}</p>
                    <p>Technical details: MIME type issues with Blazor WebAssembly script.</p>
                    <p>You can try:</p>
                    <ul style="list-style-type: none; padding: 0;">
                        <li>Refreshing the page</li>
                        <li>Clearing your browser cache</li>
                        <li>Using a different browser</li>
                    </ul>
                    <button onclick="location.reload()" style="background-color: #1a365d; color: white; border: none; padding: 8px 16px; border-radius: 4px; cursor: pointer; margin-top: 15px;">
                        Refresh Page
                    </button>
                </div>
            `;
    }
  }

  // Check if we're running in Azure Static Web Apps
  function isAzureStaticWebApps() {
    return (
      window.location.hostname.includes("azurestaticapps.net") ||
      window.location.hostname.includes("azurewebsites.net") ||
      localStorage.getItem("force-swa-helper") === "true"
    );
  }

  // Main initialization function
  function initialize() {
    console.log("Azure SWA Helper: Running workaround for all environments");

    // Find and remove any existing Blazor script tags
    const existingScripts = document.querySelectorAll(
      'script[src*="blazor.webassembly.js"]',
    );
    existingScripts.forEach((script) => script.remove());

    // Add a small delay before loading Blazor
    setTimeout(function () {
      // Load Blazor WebAssembly directly
      loadBlazorWebAssembly();
    }, 100);
  }

  // Run the initialization when the DOM is fully loaded
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initialize);
  } else {
    initialize();
  }
})();
