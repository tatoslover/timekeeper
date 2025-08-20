// timer.js - JavaScript functions for the Toastmasters Timer application

// Dictionary to hold our audio elements
let audioElements = {};

// Initialize the sound system when the document is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Create and preload audio elements
    initSounds();
});

/**
 * Initialize the sound system by creating audio elements for each timer state
 */
function initSounds() {
    // Define the sounds for each state
    const sounds = {
        'green': {
            frequency: 800,
            duration: 500,
            type: 'sine'
        },
        'orange': {
            frequency: 600,
            duration: 500,
            type: 'triangle'
        },
        'red': {
            frequency: 400,
            duration: 800,
            type: 'sawtooth'
        },
        'finish': {
            frequency: 700,
            duration: 1000,
            type: 'sine'
        }
    };

    // Create an audio context
    const audioContext = new (window.AudioContext || window.webkitAudioContext)();

    // Generate audio for each sound type
    for (const [name, config] of Object.entries(sounds)) {
        const audioBuffer = createAudioBuffer(audioContext, config);
        audioElements[name] = audioBuffer;
    }
}

/**
 * Create an audio buffer for a sound with the given configuration
 * @param {AudioContext} audioContext - The audio context to use
 * @param {Object} config - The sound configuration
 * @returns {Object} An object containing the audio context and buffer
 */
function createAudioBuffer(audioContext, config) {
    return {
        context: audioContext,
        config: config
    };
}

/**
 * Play a sound for the given timer state
 * @param {string} soundName - The name of the sound to play ('green', 'orange', 'red', 'finish')
 */
function playSound(soundName) {
    const sound = audioElements[soundName];
    if (!sound) {
        console.error(`Sound '${soundName}' not found`);
        return;
    }

    // Create oscillator
    const oscillator = sound.context.createOscillator();
    const gainNode = sound.context.createGain();

    // Configure oscillator
    oscillator.type = sound.config.type;
    oscillator.frequency.value = sound.config.frequency;

    // Set up gain node for fade out
    gainNode.gain.setValueAtTime(0.5, sound.context.currentTime);
    gainNode.gain.exponentialRampToValueAtTime(0.01, sound.context.currentTime + sound.config.duration / 1000);

    // Connect nodes
    oscillator.connect(gainNode);
    gainNode.connect(sound.context.destination);

    // Play sound
    oscillator.start();

    // Stop after duration
    setTimeout(() => {
        oscillator.stop();
    }, sound.config.duration);
}

/**
 * Vibrate the device (if supported)
 * @param {number} duration - The duration to vibrate in milliseconds
 */
function vibrate(duration) {
    if ('vibrate' in navigator) {
        navigator.vibrate(duration);
    }
}

/**
 * Show a browser notification (if supported and allowed)
 * @param {string} title - The notification title
 * @param {string} body - The notification body
 */
async function showNotification(title, body) {
    if (!("Notification" in window)) {
        return;
    }

    if (Notification.permission === "granted") {
        new Notification(title, { body });
    } else if (Notification.permission !== "denied") {
        const permission = await Notification.requestPermission();
        if (permission === "granted") {
            new Notification(title, { body });
        }
    }
}

/**
 * Request permissions for notifications when the app starts
 */
function requestPermissions() {
    // Request notification permission
    if ("Notification" in window && Notification.permission !== "granted" && Notification.permission !== "denied") {
        Notification.requestPermission();
    }
}

// Export functions for use from Blazor
window.playSound = playSound;
window.vibrate = vibrate;
window.showNotification = showNotification;
window.requestPermissions = requestPermissions;
