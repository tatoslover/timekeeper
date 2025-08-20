using System;
using System.Threading.Tasks;
using Timekeeper.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;

namespace Timekeeper.Services
{
    /// <summary>
    /// Service that manages the timer functionality for Toastmasters speeches.
    /// </summary>
    public class TimerService : INotifyPropertyChanged
    {
        private readonly System.Timers.Timer _timer;
        private DateTime _startTime;
        private DateTime? _pausedTime;
        private TimeSpan _elapsedTimeWhenPaused;
        private TimerConfig _currentConfig;
        private TimerState _currentState = TimerState.Start;
        private bool _isRunning = false;
        private Dictionary<TimerState, DateTime> _stateTimestamps = new Dictionary<TimerState, DateTime>();

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<TimerState>? StateChanged;

        /// <summary>
        /// Gets the current configuration of the timer.
        /// </summary>
        public TimerConfig CurrentConfig
        {
            get => _currentConfig;
            private set
            {
                _currentConfig = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the current state of the timer.
        /// </summary>
        public TimerState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    var oldState = _currentState;
                    _currentState = value;

                    // Record the timestamp when this state was reached
                    _stateTimestamps[_currentState] = DateTime.Now;

                    OnPropertyChanged();
                    StateChanged?.Invoke(this, _currentState);
                }
            }
        }

        /// <summary>
        /// Gets whether the timer is currently running.
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the elapsed time since the timer started.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get
            {
                if (!IsRunning && _pausedTime.HasValue)
                {
                    return _elapsedTimeWhenPaused;
                }

                if (!IsRunning && !_pausedTime.HasValue)
                {
                    return TimeSpan.Zero;
                }

                return DateTime.Now - _startTime;
            }
        }

        /// <summary>
        /// Gets the elapsed time in seconds.
        /// </summary>
        public int ElapsedSeconds => (int)ElapsedTime.TotalSeconds;

        /// <summary>
        /// Gets a formatted string representation of the elapsed time.
        /// </summary>
        public string ElapsedTimeFormatted => ElapsedTime.ToString(@"mm\:ss");

        /// <summary>
        /// Constructor for the TimerService.
        /// </summary>
        public TimerService()
        {
            _timer = new System.Timers.Timer(100); // Update every 100ms for smoother UI updates
            _timer.Elapsed += OnTimerElapsed!;

            // Set a default config
            _currentConfig = TimerConfig.CreatePreset(SpeechPresetType.Custom);
        }

        /// <summary>
        /// Sets the timer configuration.
        /// </summary>
        /// <param name="config">The configuration to use for the timer.</param>
        public void SetConfig(TimerConfig config)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Cannot change configuration while timer is running.");
            }

            var validationErrors = config.Validate();
            if (validationErrors.Count > 0)
            {
                throw new ArgumentException($"Invalid timer configuration: {string.Join(", ", validationErrors)}");
            }

            CurrentConfig = config;
            CurrentState = TimerState.Start;

            // Update the LastUsed timestamp
            config.LastUsed = DateTime.Now;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            if (_pausedTime.HasValue)
            {
                // Resume from pause
                var pauseDuration = DateTime.Now - _pausedTime.Value;
                _startTime = _startTime.Add(pauseDuration);
                _pausedTime = null;
            }
            else
            {
                // Fresh start
                _startTime = DateTime.Now;
                _elapsedTimeWhenPaused = TimeSpan.Zero;
                _stateTimestamps.Clear();
                _stateTimestamps[TimerState.Start] = DateTime.Now;
                CurrentState = TimerState.Blank;
            }

            _timer.Start();
            IsRunning = true;
        }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public void Pause()
        {
            if (!IsRunning)
            {
                return;
            }

            _timer.Stop();
            _pausedTime = DateTime.Now;
            _elapsedTimeWhenPaused = ElapsedTime;
            IsRunning = false;
        }

        /// <summary>
        /// Resets the timer.
        /// </summary>
        public void Reset()
        {
            _timer.Stop();
            _startTime = DateTime.Now;
            _pausedTime = null;
            _elapsedTimeWhenPaused = TimeSpan.Zero;
            _stateTimestamps.Clear();
            IsRunning = false;
            CurrentState = TimerState.Start;
            OnPropertyChanged(nameof(ElapsedTime));
            OnPropertyChanged(nameof(ElapsedTimeFormatted));
            OnPropertyChanged(nameof(ElapsedSeconds));
        }

        /// <summary>
        /// Gets the timestamp when a specific state was reached.
        /// </summary>
        /// <param name="state">The timer state</param>
        /// <returns>The timestamp when the state was reached, or null if not reached yet</returns>
        public DateTime? GetStateTimestamp(TimerState state)
        {
            if (_stateTimestamps.TryGetValue(state, out DateTime timestamp))
            {
                return timestamp;
            }
            return null;
        }

        /// <summary>
        /// Gets the time difference between when two states were reached.
        /// </summary>
        /// <param name="startState">The starting state</param>
        /// <param name="endState">The ending state</param>
        /// <returns>The time difference, or null if either state wasn't reached</returns>
        public TimeSpan? GetTimeBetweenStates(TimerState startState, TimerState endState)
        {
            var startTime = GetStateTimestamp(startState);
            var endTime = GetStateTimestamp(endState);

            if (startTime.HasValue && endTime.HasValue)
            {
                return endTime.Value - startTime.Value;
            }
            return null;
        }

        /// <summary>
        /// Event handler for timer ticks.
        /// </summary>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            UpdateState();

            // Notify UI about time changes
            OnPropertyChanged(nameof(ElapsedTime));
            OnPropertyChanged(nameof(ElapsedTimeFormatted));
            OnPropertyChanged(nameof(ElapsedSeconds));
        }

        /// <summary>
        /// Updates the timer state based on the elapsed time and configuration.
        /// </summary>
        private void UpdateState()
        {
            var seconds = ElapsedSeconds;

            // Auto-stop if a finish time is configured and we've reached it
            if (CurrentConfig.FinishTime.HasValue && seconds >= CurrentConfig.FinishTime.Value && IsRunning)
            {
                _timer.Stop();
                IsRunning = false;
                CurrentState = TimerState.Finish;
                return;
            }

            // Update the state based on the elapsed time
            TimerState newState;
            if (seconds >= CurrentConfig.RedTime)
            {
                newState = TimerState.Red;
            }
            else if (seconds >= CurrentConfig.OrangeTime)
            {
                newState = TimerState.Orange;
            }
            else if (seconds >= CurrentConfig.GreenTime)
            {
                newState = TimerState.Green;
            }
            else
            {
                newState = TimerState.Blank;
            }

            // Only update if the state has changed
            if (CurrentState != newState)
            {
                CurrentState = newState;
            }
        }

        /// <summary>
        /// Helper method to raise the PropertyChanged event.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
