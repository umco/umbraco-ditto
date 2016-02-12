using System;
using Umbraco.Core;

namespace Our.Umbraco.Ditto
{
    internal class DittoDisposableTimer : DisposableTimer
    {
        public DittoDisposableTimer(Action<long> callback)
            : base(callback)
        { }

        public static new DisposableTimer DebugDuration<T>(string startMessage)
        {
            if (Ditto.IsDebuggingEnabled)
            {
                return DisposableTimer.DebugDuration<T>(startMessage);
            }

            return new DittoDisposableTimer((x) => { });
        }
    }
}