namespace Bigode.Models;

[Serializable]
public class RenderModel : Dictionary<string, RenderValue>
{
    public RenderModel() : this(0, null) { }

    public RenderModel(int capacity) : this(capacity, null) { }

    public RenderModel(IEqualityComparer<string>? comparer) : this(0, comparer) { }

    public RenderModel(int capacity, IEqualityComparer<string>? comparer) : base(capacity, comparer) { }

    public RenderModel(IDictionary<string, RenderValue> dictionary) : this(dictionary, null) { }

    public RenderModel(IDictionary<string, RenderValue> dictionary, IEqualityComparer<string>? comparer) :
        base(dictionary?.Count ?? 0, comparer)
    { }

    public RenderModel(IEnumerable<KeyValuePair<string, RenderValue>> collection) : this(collection, null) { }

    public RenderModel(IEnumerable<KeyValuePair<string, RenderValue>> collection, IEqualityComparer<string>? comparer) :
        base((collection as ICollection<KeyValuePair<string, RenderValue>>)?.Count ?? 0, comparer)
    { }
}