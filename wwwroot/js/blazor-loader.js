// blazor-loader.js - Custom script to ensure Blazor WebAssembly loads properly

// Immediately-invoked function expression to avoid polluting global namespace
(function () {
    // Function to load Blazor script with correct content type
    function loadBlazorScript() {
        console.log('Attempting to load Blazor WebAssembly runtime...');

        // Create a new script element
        const script = document.createElement('script');
        script.src = '/_framework/blazor.webassembly.js';
        script.type = 'text/javascript';

        // Add error handler
        script.onerror = function(error) {
            console.error('Failed to load Blazor WebAssembly:', error);
            showErrorMessage('Failed to load the application. Please try refreshing the page.');
        };

        // Add load handler
        script.onload = function() {
            console.log('Blazor WebAssembly script loaded successfully');
        };

        // Append to document
        document.body.appendChild(script);
    }

    // Function to show error message to user
    function showErrorMessage(message) {
        const appElement = document.getElementById('app');
        if (appElement) {
            appElement.innerHTML = `
                <div style="padding: 20px; text-align: center; font-family: sans-serif;">
                    <h2 style="color: #dc3545;">Application Error</h2>
                    <p>${message}</p>
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

    // Function to check if browser supports WebAssembly
    function checkWebAssemblySupport() {
        try {
            if (typeof WebAssembly === 'object' &&
                typeof WebAssembly.instantiate === 'function') {
                const module = new WebAssembly.Module(new Uint8Array([
                    0x0, 0x61, 0x73, 0x6d, 0x01, 0x00, 0x00, 0x00
                ]));
                if (module instanceof WebAssembly.Module) {
                    return new WebAssembly.Instance(module) instanceof WebAssembly.Instance;
                }
            }
        } catch (e) {
            return false;
        }
        return false;
    }

    // Main initialization function
    function init() {
        if (!checkWebAssemblySupport()) {
            showErrorMessage('Your browser does not support WebAssembly. Please use a modern browser like Chrome, Firefox, Safari, or Edge.');
            return;
        }

        // Check if the script already exists
        if (!document.querySelector('script[src*="blazor.webassembly.js"]')) {
            loadBlazorScript();
        }
    }

    // Initialize when DOM is fully loaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
