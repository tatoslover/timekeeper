namespace Timekeeper.Models
{
    /// <summary>
    /// Represents the different states of the Toastmasters timer.
    /// </summary>
    public enum TimerState
    {
        /// <summary>
        /// Initial configuration state before the timer starts.
        /// </summary>
        Start,

        /// <summary>
        /// Timer is running but has not yet reached the Green state.
        /// </summary>
        Blank,

        /// <summary>
        /// Timer has reached the minimum time (Green light).
        /// </summary>
        Green,

        /// <summary>
        /// Timer is approaching the maximum time (Orange/Yellow light).
        /// </summary>
        Orange,

        /// <summary>
        /// Timer has reached the maximum time (Red light).
        /// </summary>
        Red,

        /// <summary>
        /// Timer has been stopped, either manually or automatically.
        /// </summary>
        Finish
    }
}
