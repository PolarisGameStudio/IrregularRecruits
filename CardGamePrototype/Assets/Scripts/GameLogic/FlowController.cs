using System;
using System.Collections.Generic;

public static class FlowController
{
    public static Queue<Action> ActionQueue = new Queue<Action>();
    public static bool ReadyForInput { get; private set; }

    //TODO: maybe use a enum or class for different event types
    public static void AddEvent(Action p)
    {
        ReadyForInput = false;

        ActionQueue.Enqueue(p);
    }

    public static void TriggerNextAction()
    {
        ActionQueue.Dequeue().Invoke();
        if (ActionQueue.Count == 0)
            ReadyForInput = true;
    }

    public static void ResolveAllActions()
    {
        while (!ReadyForInput)
            TriggerNextAction();
    }
}