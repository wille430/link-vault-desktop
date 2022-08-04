
using System;
using System.Collections.Generic;
using Avalonia.Threading;

namespace LinkVault.Services
{
    public class messageBusService
    {
        Dictionary<string, List<Action<object>>> events = new Dictionary<string, List<Action<object>>>();

        public void RegisterEvents(string name, Action<object> eventHandler)
        {
            List<Action<object>> eventActions = new List<Action<object>>();
            if (events.ContainsKey(name))
            {
                eventActions = events[name];
            }
            eventActions.Add(eventHandler);
            events[name] = eventActions;
        }

        object sequence = "";
        public void Emit(string eventName, object argument)
        {
            lock (sequence)
            {
                if (events.ContainsKey(eventName))
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        var eventActions = events[eventName];
                        eventActions.ForEach(x => x.Invoke(argument));
                    });
                }
            }
        }
    }
}