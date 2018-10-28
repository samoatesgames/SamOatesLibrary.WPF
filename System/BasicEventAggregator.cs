using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Threading;
using SamOatesLibrary.WPF.System.EventAggregator;

namespace SamOatesLibrary.WPF.System
{
    public class BasicEventAggregator
    {
        /// <summary>
        /// 
        /// </summary>
        private class EventCallback
        {
            public Delegate Callback { get; }
            public Dispatcher ExecutingDispatcher { get; }

            public EventCallback(Delegate callback, Dispatcher executingDispatcher)
            {
                Callback = callback;
                ExecutingDispatcher = executingDispatcher;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class EventSubscription : Dictionary<ISubscribable, EventCallback>
        {
        }

        /// <summary>
        /// A mapping between all registered events and their callbacks.
        /// </summary>
        private readonly Dictionary<Type, EventSubscription> m_handlers = new Dictionary<Type, EventSubscription>();

            /// <summary>
        /// Subscribe an object to a specified event, when the event is published
        /// all callbacks will be executed on the specified dispatcher.
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="callback"></param>
        /// <param name="executingDispatch"></param>
        public bool Subscribe<TEventType>(ISubscribable subscriber, Action<TEventType> callback, Dispatcher executingDispatch = null) where TEventType : IEventAggregatorEvent
        {
            if (!m_handlers.TryGetValue(typeof(TEventType), out var subscriberToActionMap))
            {
                subscriberToActionMap = new EventSubscription();
                m_handlers[typeof(TEventType)] = subscriberToActionMap;
            }

            // We are already subscribed to the specified event.
            if (subscriberToActionMap.ContainsKey(subscriber))
            {
                return false;
            }

            // Store the subscription
            subscriberToActionMap[subscriber] = new EventCallback(callback, executingDispatch);
            return true;
        }

        /// <summary>
        /// Unsubscribe an object from a specified event.
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool UnSubscribe<TEventType>(ISubscribable subscriber) where TEventType : IEventAggregatorEvent
        {
            if (!m_handlers.TryGetValue(typeof(TEventType), out var subscriberToActionMap))
            {
                // Nothing is subscribed to this event.
                return false;
            }

            if (!subscriberToActionMap.ContainsKey(subscriber))
            {
                // The subscriber isn't subscribed to this event type
                return false;
            }

            // Remove the subscription
            subscriberToActionMap.Remove(subscriber);
            return true;
        }

        /// <summary>
        /// Unsubscribe an object from all events.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool UnSubscribeAll(ISubscribable subscriber)
        {
            var anyRemoved = false;

            foreach (var handler in m_handlers.Values)
            {
                if (!handler.ContainsKey(subscriber))
                {
                    // The subscriber isn't subscribed to this event type
                    continue;
                }

                handler.Remove(subscriber);
                anyRemoved = true;
            }

            return anyRemoved;
        }

        /// <summary>
        /// Publish a specified event to all listeners
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="eventToPublish"></param>
        /// <returns></returns>
        public bool Publish<TEventType>(TEventType eventToPublish) where TEventType : IEventAggregatorEvent
        {
            if (!m_handlers.TryGetValue(typeof(TEventType), out var subscriberToActionMap))
            {
                // Nothing is subscribed to this event.
                return false;
            }

            // Call the delegates on all subscribers
            foreach (var subscriber in subscriberToActionMap.Values)
            {
                if (!(subscriber.Callback is Action<TEventType> callback))
                {
                    continue;
                }

                if (subscriber.ExecutingDispatcher != null)
                {
                    subscriber.ExecutingDispatcher.Invoke(() => { callback.Invoke(eventToPublish); });
                }
                else
                {
                    callback.Invoke(eventToPublish);
                }
            }

            return true;
        }

        /// <summary>
        /// Publish a specified event to all listeners asynchronously
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="eventToPublish"></param>
        /// <returns></returns>
        public async Task<bool> PublishAsync<TEventType>(TEventType eventToPublish)
            where TEventType : IEventAggregatorEvent
        {
            return await Task.Run(() => Publish(eventToPublish));
        }
    }
}
