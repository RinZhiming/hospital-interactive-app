using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public static class FusionHelper
{
	private static Dictionary<NetworkRunner, Dictionary<Type,object>> singletonsRunner = new();
	private static Dictionary<NetworkRunner, Dictionary<Type,List<Action<object>>>> pendingRequestsRunner = new();

	/// <summary>
	/// Get a singleton of a specific type. Will return false if none is available.
	/// </summary>
	/// <param name="runner"></param>
	/// <param name="singleton">Placeholder for requested singleton</param>
	/// <typeparam name="T">Type of singleton to get</typeparam>
	/// <returns>false if none is available</returns>
	public static bool TryGetSingleton<T>(this NetworkRunner runner, out T singleton) where T : MonoBehaviour
	{
		if (singletonsRunner.TryGetValue(runner, out var singletonsByType))
		{
			if (singletonsByType.TryGetValue(typeof(T), out var s))
			{
				if ((T)s)
				{
					singleton = (T) s;
					return true;
				}
				singletonsByType.Remove(typeof(T));
			}
			if (singletonsByType.Count == 0)
				singletonsRunner.Remove(runner);
		}
		singleton = null;
		return false;
	}

	/// <summary>
	/// Wait for a singleton of a specific type to be registered. 
	/// </summary> 
	/// <param name="runner"></param>
	/// <param name="onSingletonResolved">Callback that will be invoked with the singleton instance once it registers</param>
	/// <typeparam name="T">Type of singleton to wait for</typeparam>
	/// <returns>Handle that can be passed to StopWaitingForSingleton if you need to cancel the pending request</returns>
	public static Action<object> WaitForSingleton<T>(this NetworkRunner runner, Action<T> onSingletonResolved) where T : MonoBehaviour
	{
		if (runner.TryGetSingleton(out T singleton))
		{
			onSingletonResolved(singleton);
			return null;
		}

		if (!pendingRequestsRunner.TryGetValue(runner, out Dictionary<Type,List<Action<object>>> pendingByType))
		{
			pendingByType = new Dictionary<Type, List<Action<object>>>();
			pendingRequestsRunner[runner] = pendingByType;
		}

		if (!pendingByType.TryGetValue(typeof(T), out List<Action<object>> pendingRequests))
		{
			pendingRequests = new List<Action<object>>();
			pendingByType[typeof(T)] = pendingRequests;
		}

		Action<object> untypedCallback = resolvedSingleton => { onSingletonResolved((T) resolvedSingleton); };
		pendingRequests.Add( untypedCallback );

		Debug.Log($"Waiting for singleton of type {typeof(T)}");
		return untypedCallback;
	}

	/// <summary>
	/// Cancel a pending request for a singleton.
	/// </summary>
	/// <param name="runner"></param>
	/// <param name="handler">The handler that was previously registered (return value from WaitForSingleton)</param>
	public static void StopWaitingForSingleton(this NetworkRunner runner, Action<object> handler)
	{
		if (pendingRequestsRunner.TryGetValue(runner, out Dictionary<Type, List<Action<object>>> pendingByType))
		{
			foreach (KeyValuePair<Type,List<Action<object>>> keyValuePair in pendingByType)
			{
				if (keyValuePair.Value.Remove(handler))
				{
					if (keyValuePair.Value.Count == 0)
						pendingByType.Remove(keyValuePair.Key);
					return;
				}
			}
		}
	}

	/// <summary>
	/// Remove all registered singletons and pending requests for singletons on a specific runner.
	/// Should be called when the runner shuts down.
	/// </summary>
	/// <param name="runner"></param>
	public static void ClearSingletons(this NetworkRunner runner)
	{
		pendingRequestsRunner.Remove(runner);
		singletonsRunner.Remove(runner);
	}

	/// <summary>
	/// Unregister singleton for all the types that it represents
	/// </summary>
	/// <param name="runner"></param>
	/// <param name="singleton">The singleton instance to unregister</param>
	/// <typeparam name="T">Actual type of the singleton</typeparam>
	public static void RemoveSingleton<T>(this NetworkRunner runner, T singleton) where T: MonoBehaviour
	{
		if (singletonsRunner.TryGetValue(runner, out var singletonsByType))
		{
			Type type = typeof(T);
			while (typeof(MonoBehaviour).IsAssignableFrom(type))
			{
				if (singletonsByType.TryGetValue(type, out var registeredSingleton))
				{
					if (registeredSingleton == singleton)
					{
						Debug.Log($"Remove singleton {singleton} of type {type}");
						singletonsByType.Remove(type);
					}
				}
				type = type.BaseType;
			}
		}
	}

	/// <summary>
	/// Register an object instance as the singleton for its type.
	/// </summary>
	/// <param name="runner"></param>
	/// <param name="singleton">The singleton</param>
	/// <typeparam name="T">The only type for which the singleton is explicitly registered</typeparam>
	public static void AddSingleton<T>(this NetworkRunner runner, T singleton) where T: MonoBehaviour
	{
		Debug.Log($"Add singleton of type {typeof(T)}");
		if (!singletonsRunner.TryGetValue(runner, out var singletonsByType))
		{
			singletonsByType = new Dictionary<Type, object>();
			singletonsRunner[runner] = singletonsByType;
		}

		Type type = typeof(T);
		AddSingleton(runner, singletonsByType, type, singleton);
	}

	/// <summary>
	/// Register an object instance as the singleton for its explicit type and all parent types up to and including the provided base-type.
	/// </summary>
	/// <param name="runner"></param>
	/// <param name="basetype">The top-most type that this singleton represents</param>
	/// <param name="singleton">The singleton</param>
	/// <typeparam name="T">The specific type of the singleton</typeparam>
	public static void AddSingleton<T>(this NetworkRunner runner, Type basetype, T singleton) where T: MonoBehaviour
	{
		Debug.Log($"Add singleton of type {typeof(T)} for basetype {basetype}");
		if (!singletonsRunner.TryGetValue(runner, out var singletonsByType))
		{
			singletonsByType = new Dictionary<Type, object>();
			singletonsRunner[runner] = singletonsByType;
		}

		Type type = typeof(T);
		while (type!=null && basetype.IsAssignableFrom(type))
		{
			Debug.Log($"  Add derived type {type} for base {basetype}");
			AddSingleton(runner, singletonsByType, type, singleton);
			type = type.BaseType;
		}
	}

	private static void AddSingleton<T>(NetworkRunner runner, Dictionary<Type,object> singletonsByType, Type singletonType, T singletonInstance) where T: MonoBehaviour
	{
		if (singletonsByType.TryGetValue(singletonType, out _))
			throw new Exception($"Attempt to add {typeof(T)} twice as a singleton for the same runner!");

		singletonsByType[singletonType] = singletonInstance;

		if (pendingRequestsRunner.TryGetValue(runner, out var pendingRequestsByType)) 
		{
			Debug.Log("Resolving pending requests for Runner!");
			if (pendingRequestsByType.TryGetValue(singletonType, out List<Action<object>> pendingRequests))
			{
				foreach (var pendingRequest in pendingRequests) 
				{
					pendingRequest(singletonInstance);
				}
				pendingRequests.Clear();
				pendingRequestsByType.Remove(singletonType);
				if (pendingRequestsByType.Count == 0)
					pendingRequestsRunner.Remove(runner);
			}
		}
	}
}