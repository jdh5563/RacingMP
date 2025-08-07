namespace packageBase.eventManagement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using UnityEngine;

	public class EventManager : MonoBehaviour
	{
		// Singleton pattern
		public static EventManager Instance { get; private set; }

		// Maps types of events to a list of everyone who listens for that event
		private readonly Dictionary<Type, List<WeakReference>> eventSubscribers_ = new Dictionary<Type, List<WeakReference>>();

		// Thread safety
		private readonly object lockDict_ = new object();

		// Singleton pattern
		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				Instance = this;
			}

			DontDestroyOnLoad(gameObject);
		}

		// Called my other classes to subscribe to a type of event
		public void SubscribeEvent(Type eventType, object subscriber)
		{
			lock (lockDict_)
			{
				WeakReference newSub = new WeakReference(subscriber);
				List<WeakReference> currentSubs = null;
				bool exists = eventSubscribers_.TryGetValue(eventType, out currentSubs);
				// This is the first subscriber to this event type
				if (!exists)
				{
					currentSubs = new List<WeakReference>();
					eventSubscribers_.Add(eventType, currentSubs);
				}
				currentSubs.Add(newSub);
			}
		}

		// Called by other classes to publish an event
		// All subscribers to that event will be invoked
		public void PublishEvent<TEventType>(in TEventType eventToPublish)
		{
			List<WeakReference> deadSubs = new List<WeakReference>();
			List<WeakReference> subs = null;

			// Find all subscribers to this event type
			bool exists = eventSubscribers_.TryGetValue(typeof(TEventType), out subs);
			if (exists)
			{
				foreach (WeakReference sub in subs)
				{
					if (sub.IsAlive)
					{
						ISubscriber<TEventType> subObj = (ISubscriber<TEventType>)sub.Target;
						// Execute the appropriate callback of each sub
						InvokeSubscriberEvent(eventToPublish, subObj);
					}
					// The subscriber does not exist anymore
					else
					{
						deadSubs.Add(sub);
					}
				}

				// Cleanup
				if (deadSubs.Any())
				{
					lock (lockDict_)
					{
						foreach (WeakReference deadSub in deadSubs)
						{
							subs.Remove(deadSub);
						}
					}
				}
			}
		}

		// Helper to call the callback function of a given subscriber
		private void InvokeSubscriberEvent<TEventType>(TEventType eventToPublish, ISubscriber<TEventType> subscriber)
		{
			subscriber.OnEventHandler(eventToPublish);
		}

	}
}