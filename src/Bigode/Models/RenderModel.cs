namespace Bigode.Models;

/// <summary>
/// A RenderModel, a basic Dictionary(string, RenderValue) to be used for rendering
/// bigode pages with AOT compliance.
/// </summary>
[Serializable]
public class RenderModel : Dictionary<string, RenderValue>
{
    /// <summary>
    /// 
    /// </summary>
    public RenderModel() : this(0, null) { }

    /// <summary>
    /// 
    /// </summary>
    public RenderModel(int capacity) : this(capacity, null) { }

    /// <summary>
    /// 
    /// </summary>
    public RenderModel(IEqualityComparer<string>? comparer) : this(0, comparer) { }

    /// <summary>
    /// 
    /// </summary>
    public RenderModel(int capacity, IEqualityComparer<string>? comparer) : base(capacity, comparer) { }

    /// <summary>
    /// 
    /// </summary>
    public RenderModel(IDictionary<string, RenderValue> dictionary) : this(dictionary, null) { }

    /// <summary>
    /// 
    /// </summary>
    public RenderModel(IDictionary<string, RenderValue> dictionary, IEqualityComparer<string>? comparer) :
        base(dictionary?.Count ?? 0, comparer)
    { }

    /// <summary>
    /// 
    /// </summary>
    public RenderModel(IEnumerable<KeyValuePair<string, RenderValue>> collection) : this(collection, null) { }


    /// <summary>
    /// 
    /// </summary>
    public RenderModel(IEnumerable<KeyValuePair<string, RenderValue>> collection, IEqualityComparer<string>? comparer) :
        base((collection as ICollection<KeyValuePair<string, RenderValue>>)?.Count ?? 0, comparer)
    { }
}