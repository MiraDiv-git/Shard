namespace Engine;

public abstract class Script
{
    public virtual void Ready() { }
    public virtual void Update(double dt) { }
    public virtual void Shutdown() { }
}