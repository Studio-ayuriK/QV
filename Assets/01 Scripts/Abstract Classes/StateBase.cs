using UnityEngine;

public abstract class StateBase<T>
{
    public StateBase(T owner) { _owner = owner; }

    public virtual void Start() { }
    public virtual void Update() { }

    protected T _owner;
}
