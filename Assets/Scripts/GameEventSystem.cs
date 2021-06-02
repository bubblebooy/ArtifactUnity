using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    public delegate void EventListener(IGameEventInfo e);
    public delegate void EventListener<T>(T e);

    private static Dictionary<Type, List<EventListener>> eventListeners;

    public static (Type, EventListener) Register<T>(EventListener<T> listener)
    {
        Type t = typeof(T);
        if(eventListeners == null)
        {
            eventListeners = new Dictionary<Type, List<EventListener>>();
        }

        if(!eventListeners.ContainsKey(t) || eventListeners[t] == null)
        {
            eventListeners[t] = new List<EventListener>();
        }

        EventListener eventListener = (e) => { listener((T)e); };
        eventListeners[t].Add(eventListener);
        return (t, eventListener);

    }


    public static void Unregister<T>(EventListener listener)
    {
        Unregister(typeof(T), listener);
    }
    public static void Unregister(Type t, EventListener listener)
    {
        eventListeners[t].Remove(listener);
    }

    public static void Unregister(List<(Type, EventListener)> listeners)
    {
        foreach ((System.Type t, EventListener listener) in listeners)
        {
            Unregister(t, listener);
        }
        listeners.Clear();
    }

    public static void Event(IGameEventInfo eventInfo)
    {
        Type t = eventInfo.GetType();
        if (eventListeners == null || !eventListeners.ContainsKey(t) || eventListeners[t] == null)
        {
            return;
        }

        foreach( EventListener listener in eventListeners[eventInfo.GetType()].ToArray())
        {
            if(listener != null) // This did not work. I should figure out how to do this.
            {
                listener(eventInfo);
            }
            else
            {
                print("listener is null, Unregistering listener");
                eventListeners[eventInfo.GetType()].Remove(listener);
            }
            
        }
        
    }

}

//public abstract class EventListener
//{
//    internal abstract Type GetCallbackType();
//}

//public class EventListener<T> : EventListener
//{

//    internal override Type GetCallbackType()
//    {
//        return typeof(T);
//    }
//}