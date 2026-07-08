using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Base state-machine brain for enemies. Derived brains create states in Build(),
/// register transition predicates, then Enemy calls Tick and FixedTick each frame.
/// </summary>
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
    
    /// <summary>
    /// Optional animation event hook for enemies with animation-driven action timing.
    /// </summary>
    public virtual void AnimEvent_Teleport()
    {
    }

    /// <summary>
    /// Optional animation event hook used when a vanish animation has reached its completion point.
    /// </summary>
    public virtual void AnimEvent_CompleteVanish()
    {
    }

    /// <summary>
    /// Optional animation event hook used when an appear animation has completed.
    /// </summary>
    public virtual void AnimEvent_CompleteAppear()
    {
    }

    /// <summary>
    /// Enters the initial state after Enemy has finished gathering module references.
    /// </summary>
    public virtual void Begin()
    {
        current = initialState;
        lastTransition = "Begin";
        if (current != null)
        {
            current.Enter();
        }
    }

    /// <summary>
    /// Evaluates transitions first, then updates the active state.
    /// </summary>
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

    /// <summary>
    /// Forwards physics-step behavior to the active state.
    /// </summary>
    public virtual void FixedTick()
    {
        if (current == null)
        {
            return;
        }

        current.FixedUpdate();
    }

    /// <summary>
    /// Sets the first state used by Begin().
    /// </summary>
    protected void SetInitial(EnemyState state)
    {
        initialState = state;
    }

    /// <summary>
    /// Registers a transition that is only checked while the source state is active.
    /// </summary>
    protected void AddTransition(EnemyState from, EnemyState to, Func<bool> condition, string name = null)
    {
        if (!transitions.ContainsKey(from))
        {
            transitions[from] = new List<EnemyTransition>();
        }

        transitions[from].Add(new EnemyTransition(TransitionName(from, to, name), to, condition));
    }

    /// <summary>
    /// Registers a transition that can fire from any current state except its own target.
    /// </summary>
    protected void AddAnyTransition(EnemyState to, Func<bool> condition, string name = null)
    {
        anyTransitions.Add(new EnemyTransition(TransitionName(null, to, name), to, condition));
    }

    /// <summary>
    /// Finds the first valid transition, giving global transitions priority over state-local transitions.
    /// </summary>
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

    /// <summary>
    /// Leaves the current state, records the transition name for debugging, then enters the next state.
    /// </summary>
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
