using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Timekeeper.Models;

namespace Timekeeper.Services
{
    /// <summary>
    /// Service that manages the saving and loading of timer presets using browser local storage.
    /// </summary>
    public class PresetService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "toastmasters_timer_presets";

        /// <summary>
        /// Constructor for the PresetService.
        /// </summary>
        /// <param name="jsRuntime">The JS runtime for interacting with browser local storage.</param>
        public PresetService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Gets all saved presets from local storage.
        /// </summary>
        /// <returns>A list of saved timer configurations.</returns>
        public async Task<List<TimerConfig>> GetPresetsAsync()
        {
            var presetsJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);

            if (string.IsNullOrEmpty(presetsJson))
            {
                return new List<TimerConfig>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<TimerConfig>>(presetsJson) ?? new List<TimerConfig>();
            }
            catch (JsonException)
            {
                // If there's an error deserializing, return an empty list
                return new List<TimerConfig>();
            }
        }

        /// <summary>
        /// Saves a preset to local storage.
        /// </summary>
        /// <param name="config">The timer configuration to save.</param>
        /// <returns>A task that completes when the operation is done.</returns>
        public async Task SavePresetAsync(TimerConfig config)
        {
            var presets = await GetPresetsAsync();

            // Check if this config already exists (by ID)
            var existingIndex = presets.FindIndex(p => p.Id == config.Id);

            if (existingIndex >= 0)
            {
                // Update existing preset
                presets[existingIndex] = config;
            }
            else
            {
                // Add new preset
                presets.Add(config);
            }

            var presetsJson = JsonSerializer.Serialize(presets);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, presetsJson);
        }

        /// <summary>
        /// Deletes a preset from local storage.
        /// </summary>
        /// <param name="id">The ID of the preset to delete.</param>
        /// <returns>A task that completes when the operation is done.</returns>
        public async Task DeletePresetAsync(string id)
        {
            var presets = await GetPresetsAsync();
            var preset = presets.FirstOrDefault(p => p.Id == id);

            if (preset != null)
            {
                presets.Remove(preset);
                var presetsJson = JsonSerializer.Serialize(presets);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, presetsJson);
            }
        }

        /// <summary>
        /// Gets a specific preset by ID.
        /// </summary>
        /// <param name="id">The ID of the preset to retrieve.</param>
        /// <returns>The timer configuration if found, otherwise null.</returns>
        public async Task<TimerConfig?> GetPresetByIdAsync(string id)
        {
            var presets = await GetPresetsAsync();
            return presets.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Creates the default presets if none exist.
        /// </summary>
        /// <returns>A task that completes when the operation is done.</returns>
        public async Task InitializeDefaultPresetsAsync()
        {
            var presets = await GetPresetsAsync();

            if (presets.Count == 0)
            {
                // No presets exist, create the defaults
                var defaultPresets = new List<TimerConfig>
                {
                    TimerConfig.CreatePreset(SpeechPresetType.TableTopics),
                    TimerConfig.CreatePreset(SpeechPresetType.IceBreaker),
                    TimerConfig.CreatePreset(SpeechPresetType.EvaluationSpeech),
                    TimerConfig.CreatePreset(SpeechPresetType.PreparedSpeech)
                };

                var presetsJson = JsonSerializer.Serialize(defaultPresets);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, presetsJson);
            }
        }
    }
}
