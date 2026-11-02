namespace Engine;

public sealed class ScriptHost
{
    private static readonly Dictionary<Type, bool> OverridesUpdateCache = new();

    private readonly List<Script> _pending = new();
    private readonly List<Script> _all = new();
    private readonly List<Script> _updatable = new();

    public void Add(Script script) => _pending.Add(script);

    public void Flush()
    {
        while (_pending.Count > 0)
        {
            var batch = _pending.ToArray();
            _pending.Clear();

            foreach (var script in batch)
            {
                _all.Add(script);
                if (OverridesUpdate(script.GetType()))
                    _updatable.Add(script);

                script.Ready();
            }
        }
    }

    public void Update(double dt)
    {
        for (var i = 0; i < _updatable.Count; i++)
            _updatable[i].Update(dt);
    }

    public void Draw(SpriteBatch batch)
    {
        for (var i = 0; i < _all.Count; i++)
            _all[i].Draw(batch);
    }

    public void Shutdown()
    {
        for (var i = _all.Count - 1; i >= 0; i--)
            _all[i].Shutdown();

        _all.Clear();
        _updatable.Clear();
        _pending.Clear();
    }

    private static bool OverridesUpdate(Type type)
    {
        if (OverridesUpdateCache.TryGetValue(type, out var result))
            return result;

        var method = type.GetMethod(nameof(Script.Update), new[] { typeof(double) })!;
        result = method.DeclaringType != typeof(Script);
        OverridesUpdateCache[type] = result;
        return result;
    }
}