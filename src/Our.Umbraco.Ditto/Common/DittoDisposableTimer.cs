using System;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    /// <summary>
    /// Ditto's internal disposable timer for debugging and profiling purposes.
    /// </summary>
    internal class DittoDisposableTimer : DisposableTimer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DittoDisposableTimer"/> class.
        /// </summary>
        /// <param name="callback">A callback method.</param>
        public DittoDisposableTimer(Action<long> callback)
            : base(callback)
        { }

        /// <summary>
        /// Profiles and tracks how long a code fragment takes, until it is disposed.
        /// </summary>
        /// <typeparam name="T">The type of the calling class.</typeparam>
        /// <param name="startMessage">The starting message for the profiler.</param>
        /// <returns>Returns an instance of the disposable timer.</returns>
        public new static DisposableTimer DebugDuration<T>(string startMessage)
        {
            return DebugDuration(typeof(T), startMessage);
        }

        /// <summary>
        /// Profiles and tracks how long a code fragment takes, until it is disposed.
        /// </summary>
        /// <param name="loggerType">The type of the calling class.</param>
        /// <param name="startMessage">The starting message for the profiler.</param>
        /// <returns>Returns an instance of the disposable timer.</returns>
        public static DisposableTimer DebugDuration(Type loggerType, string startMessage)
        {
            if (Ditto.IsProfilingEnabled() && ApplicationContext.Current?.ProfilingLogger != null)
            {
                return ApplicationContext.Current.ProfilingLogger.DebugDuration(loggerType, startMessage, "Complete");
            }

            return new DittoDisposableTimer((x) => { });
        }
    }
}