using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Timekeeper.Models
{
    /// <summary>
    /// Represents the configuration for a Toastmasters speech timer.
    /// </summary>
    public class TimerConfig
    {
        /// <summary>
        /// Gets or sets the unique identifier for this timer configuration.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the name of the speech type (e.g., "Table Topics", "Prepared Speech", etc.)
        /// </summary>
        [Required(ErrorMessage = "Speech name is required")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time in seconds when the Green light should appear.
        /// </summary>
        [Required(ErrorMessage = "Green time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Green time must be positive")]
        public int GreenTime { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds when the Orange/Yellow light should appear.
        /// </summary>
        [Required(ErrorMessage = "Orange time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Orange time must be positive")]
        public int OrangeTime { get; set; }

        /// <summary>
        /// Gets or sets the time in seconds when the Red light should appear.
        /// </summary>
        [Required(ErrorMessage = "Red time is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Red time must be positive")]
        public int RedTime { get; set; }

        /// <summary>
        /// Gets or sets whether the timer should automatically stop at a certain point after the Red light.
        /// If null, the timer will continue indefinitely.
        /// </summary>
        public int? FinishTime { get; set; }



        /// <summary>
        /// Gets or sets the date and time when this configuration was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the date and time when this configuration was last used.
        /// </summary>
        public DateTime? LastUsed { get; set; }

        /// <summary>
        /// Validates that the timer thresholds are in the correct order.
        /// </summary>
        /// <returns>List of validation errors, or empty list if valid.</returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (GreenTime <= 0)
                errors.Add("Green time must be greater than zero.");

            if (OrangeTime <= GreenTime)
                errors.Add("Orange time must be greater than Green time.");

            if (RedTime <= OrangeTime)
                errors.Add("Red time must be greater than Orange time.");

            if (FinishTime.HasValue && FinishTime.Value <= RedTime)
                errors.Add("Finish time must be greater than Red time.");

            return errors;
        }

        /// <summary>
        /// Creates a preset configuration for a common Toastmasters speech type.
        /// </summary>
        /// <param name="presetType">The type of preset to create.</param>
        /// <returns>A new TimerConfig instance with preset values.</returns>
        public static TimerConfig CreatePreset(SpeechPresetType presetType)
        {
            return presetType switch
            {
                SpeechPresetType.TableTopics => new TimerConfig
                {
                    Name = "Table Topics",
                    GreenTime = 60, // 1 minute
                    OrangeTime = 90, // 1 minute 30 seconds
                    RedTime = 120, // 2 minutes
                    FinishTime = 150 // 2 minutes 30 seconds
                },
                SpeechPresetType.IceBreaker => new TimerConfig
                {
                    Name = "Ice Breaker (4-6 min)",
                    GreenTime = 240, // 4 minutes
                    OrangeTime = 300, // 5 minutes
                    RedTime = 360, // 6 minutes
                    FinishTime = 420 // 7 minutes
                },
                SpeechPresetType.EvaluationSpeech => new TimerConfig
                {
                    Name = "Evaluation (2-3 min)",
                    GreenTime = 120, // 2 minutes
                    OrangeTime = 150, // 2 minutes 30 seconds
                    RedTime = 180, // 3 minutes
                    FinishTime = 210 // 3 minutes 30 seconds
                },
                SpeechPresetType.PreparedSpeech => new TimerConfig
                {
                    Name = "Prepared Speech (5-7 min)",
                    GreenTime = 300, // 5 minutes
                    OrangeTime = 360, // 6 minutes
                    RedTime = 420, // 7 minutes
                    FinishTime = 480 // 8 minutes
                },
                SpeechPresetType.Custom => new TimerConfig
                {
                    Name = "Custom Speech",
                    GreenTime = 180,
                    OrangeTime = 240,
                    RedTime = 300,
                    FinishTime = null
                },
                _ => throw new ArgumentOutOfRangeException(nameof(presetType))
            };
        }
    }

    /// <summary>
    /// Defines the preset types for Toastmasters speeches.
    /// </summary>
    public enum SpeechPresetType
    {
        TableTopics,
        IceBreaker,
        EvaluationSpeech,
        PreparedSpeech,
        Custom
    }
}
