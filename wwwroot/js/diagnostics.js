// Simple diagnostics script for Blazor WebAssembly loading issues
console.log("Diagnostics script loaded - checking for Blazor loading issues");

window.addEventListener('load', function() {
  console.log("Page load event fired");

  // Check if Blazor is defined
  setTimeout(function() {
    if (typeof Blazor === 'undefined') {
      console.error("ERROR: Blazor is not defined after page load");

      // Check if the blazor.webassembly.js file loaded
      const blazorScript = document.querySelector('script[src*="blazor.webassembly.js"]');
      if (!blazorScript) {
        console.error("ERROR: No blazor.webassembly.js script tag found in document");
      } else {
        console.log("Blazor script tag is present in the document");
      }

      // Try to load the script directly as a backup
      console.log("Attempting to load Blazor script directly");
      const script = document.createElement('script');
      script.src = '/_framework/blazor.webassembly.js';
      script.onerror = function(err) {
        console.error("Failed to load Blazor script:", err);
        logNetworkRequests();
      };
      script.onload = function() {
        console.log("Successfully loaded Blazor script directly");
        if (typeof Blazor !== 'undefined') {
          console.log("Blazor is now defined, attempting to start it");
          try {
            Blazor.start();
          } catch (e) {
            console.error("Error starting Blazor:", e);
          }
        }
      };
      document.body.appendChild(script);
    } else {
      console.log("SUCCESS: Blazor is defined correctly");
    }
  }, 2000);
});

// Log any failed network requests that might be relevant
function logNetworkRequests() {
  const frameworkFiles = [
    '/_framework/blazor.webassembly.js',
    '/_framework/dotnet.js',
    '/_framework/dotnet.wasm',
    '/_framework/dotnet.timezones.blat',
    '/_framework/Timekeeper.dll',
    '/_framework/Microsoft.AspNetCore.Components.dll'
  ];

  console.log("Checking framework files availability:");

  frameworkFiles.forEach(file => {
    fetch(file)
      .then(response => {
        if (!response.ok) {
          console.error(`Failed to load ${file}: ${response.status} ${response.statusText}`);
        } else {
          console.log(`Successfully fetched ${file}`);
        }
      })
      .catch(error => {
        console.error(`Error fetching ${file}:`, error);
      });
  });
}

// Add this script to the console output for debugging
console.log("Diagnostics script configuration:", {
  url: window.location.href,
  userAgent: navigator.userAgent,
  timestamp: new Date().toISOString()
});
