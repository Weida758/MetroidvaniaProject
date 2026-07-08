using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Enemy))]
public abstract class EnemyBrain : MonoBehaviour
{
    protected Enemy enemy;

    private EnemyState initialState;
    private EnemyState current;
    private string lastTransition = "None";

    private readonly Dictionary<EnemyState, List<EnemyTransition>> transitions = new Dictionary<EnemyState, List<EnemyTransition>>();
    private readonly List<EnemyTransition> anyTransitions = new List<EnemyTransition>();

    protected virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
        Build();
    }

    protected virtual void Build()
    {
    }

    public EnemyState Current
    {
        get { return current; }
    }

    [ShowInInspector, ReadOnly, BoxGroup("State Debug")]
    public string CurrentState
    {
        get { return current != null ? current.GetType().Name : "None"; }
    }

    [ShowInInspector, ReadOnly, BoxGroup("State Debug")]
    public string InitialState
    {
        get { return initialState != null ? initialState.GetType().Name : "None"; }
    }

    [ShowInInspector, ReadOnly, BoxGroup("State Debug")]
    public string LastTransition
    {
        get { return lastTransition; }
    }

    public virtual string DebugStateName
    {
        get { return current != null ? current.GetType().Name : GetType().Name; }
    }
    
    // For enemies that have the ability to teleport
    public virtual void AnimEvent_Teleport()
    {
    }

    public virtual void AnimEvent_CompleteVanish()
    {
    }

    public virtual void AnimEvent_CompleteAppear()
    {
    }

    public virtual void Begin()
    {
        current = initialState;
        lastTransition = "Begin";
        if (current != null)
        {
            current.Enter();
        }
    }

    public virtual void Tick()
    {
        if (current == null)
        {
            return;
        }

        EnemyTransition triggered = GetTriggered();
        if (triggered != null)
        {
            SwitchTo(triggered);
        }

        current.Update();
    }

    public virtual void FixedTick()
    {
        if (current == null)
        {
            return;
        }

        current.FixedUpdate();
    }

    protected void SetInitial(EnemyState state)
    {
        initialState = state;
    }

    protected void AddTransition(EnemyState from, EnemyState to, Func<bool> condition, string name = null)
    {
        if (!transitions.ContainsKey(from))
        {
            transitions[from] = new List<EnemyTransition>();
        }

        transitions[from].Add(new EnemyTransition(TransitionName(from, to, name), to, condition));
    }

    protected void AddAnyTransition(EnemyState to, Func<bool> condition, string name = null)
    {
        anyTransitions.Add(new EnemyTransition(TransitionName(null, to, name), to, condition));
    }

    private EnemyTransition GetTriggered()
    {
        foreach (EnemyTransition transition in anyTransitions)
        {
            if (transition.Target != current && transition.Condition())
            {
                return transition;
            }
        }

        if (transitions.TryGetValue(current, out List<EnemyTransition> list))
        {
            foreach (EnemyTransition transition in list)
            {
                if (transition.Condition())
                {
                    return transition;
                }
            }
        }

        return null;
    }

    private void SwitchTo(EnemyTransition transition)
    {
        EnemyState next = transition.Target;
        if (next == current)
        {
            return;
        }

        current.Exit();
        lastTransition = transition.Name;
        current = next;
        current.Enter();
    }

    private static string TransitionName(EnemyState from, EnemyState to, string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        string fromName = from != null ? from.GetType().Name : "Any";
        string toName = to != null ? to.GetType().Name : "None";
        return $"{fromName} -> {toName}";
    }
}
