using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class EnemyBrain : MonoBehaviour
{
    protected Enemy enemy;

    private EnemyState initialState;
    private EnemyState current;

    private readonly Dictionary<EnemyState, List<EnemyTransition>> transitions = new Dictionary<EnemyState, List<EnemyTransition>>();
    private readonly List<EnemyTransition> anyTransitions = new List<EnemyTransition>();

    protected virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
        Build();
    }

    protected abstract void Build();

    public EnemyState Current
    {
        get { return current; }
    }

    public void Begin()
    {
        current = initialState;
        current.Enter();
    }

    public void Tick()
    {
        EnemyTransition triggered = GetTriggered();
        if (triggered != null)
        {
            SwitchTo(triggered.Target);
        }

        current.Update();
    }

    public void FixedTick()
    {
        current.FixedUpdate();
    }

    protected void SetInitial(EnemyState state)
    {
        initialState = state;
    }

    protected void AddTransition(EnemyState from, EnemyState to, Func<bool> condition)
    {
        if (!transitions.ContainsKey(from))
        {
            transitions[from] = new List<EnemyTransition>();
        }

        transitions[from].Add(new EnemyTransition(to, condition));
    }

    protected void AddAnyTransition(EnemyState to, Func<bool> condition)
    {
        anyTransitions.Add(new EnemyTransition(to, condition));
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

    private void SwitchTo(EnemyState next)
    {
        if (next == current)
        {
            return;
        }

        current.Exit();
        current = next;
        current.Enter();
    }
}
