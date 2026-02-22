namespace Bigode.Models;

/// <summary>
/// Represents the value used in the RenderModel.
/// It can contain multiple types of values depending on how its constructed.
/// </summary>
public class RenderValue
{
    /// <summary>
    /// If this RenderValue is a string.
    /// </summary>
    public readonly bool IsString;

    /// <summary>
    /// If this RenderValue is a RenderModel.
    /// </summary>
    public readonly bool IsModel;

    /// <summary>
    /// If this RenderValue is a lambda.
    /// </summary>
    public readonly bool IsLambda;

    /// <summary>
    /// If this RenderValue is an array.
    /// </summary>
    public readonly bool IsArray;

    /// <summary>
    /// If this RenderValue is a bool.
    /// </summary>
    public readonly bool IsBool;

    private readonly string? stringValue;
    private readonly RenderModel? mulitpleValue;
    private readonly Func<string, Task<string>>? lambdaValue;
    private readonly RenderModel[]? arrayValue;
    private readonly bool? boolValue = null;

    /// <summary>
    /// Constructs a RenderValue that holds a string.
    /// </summary>
    /// <param name="value">Value to be stored</param>
    public RenderValue(string value)
    {
        IsString = true;
        IsModel = false;
        IsLambda = false;
        IsArray = false;
        IsBool = false;
        stringValue = value;
    }

    /// <summary>
    /// Constructs a RenderValue that holds a RenderModel.
    /// </summary>
    /// <param name="value">Value to be stored</param>
    public RenderValue(RenderModel value)
    {
        IsString = false;
        IsModel = true;
        IsLambda = false;
        IsArray = false;
        IsBool = false;
        mulitpleValue = value;
    }

    /// <summary>
    /// Constructs a RenderValue that holds an array.
    /// </summary>
    /// <param name="value">Value to be stored</param>
    public RenderValue(RenderModel[] value)
    {
        IsString = false;
        IsModel = false;
        IsLambda = false;
        IsArray = true;
        IsBool = false;
        arrayValue = value;
    }

    /// <summary>
    /// Constructs a RenderValue that holds a lambda.
    /// </summary>
    /// <param name="value">Value to be stored</param>
    public RenderValue(Func<string, Task<string>> value)
    {
        IsString = false;
        IsModel = false;
        IsLambda = true;
        IsArray = false;
        IsBool = false;
        lambdaValue = value;
    }

    /// <summary>
    /// Constructs a RenderValue that holds a bool.
    /// </summary>
    /// <param name="value">Value to be stored</param>
    public RenderValue(bool value)
    {
        IsString = false;
        IsModel = false;
        IsLambda = false;
        IsArray = false;
        IsBool = true;
        boolValue = value;
    }

    /// <summary>
    /// Gets the value of this RenderValue.
    /// Throws if this RenderValue doesn't hold a value of type RenderModel
    /// </summary>
    /// <returns>The stored value</returns>
    /// <exception cref="Exception"></exception>
    public RenderModel GetModel()
    {
        if (mulitpleValue is null)
            throw new Exception("RenderValue is not a model");

        return mulitpleValue;
    }

    /// <summary>
    /// Gets the value of this RenderValue.
    /// Throws if this RenderValue doesn't hold a value of type boolean
    /// </summary>
    /// <returns>The stored value</returns>
    /// <exception cref="Exception"></exception>
    public bool GetBoolValue()
    {
        if (boolValue is null)
            throw new Exception("RenderValue is not a bool");

        return (bool)boolValue; // c# compiler is dumb, bool is never null but go figure...
    }

    /// <summary>
    /// Gets the value of this RenderValue.
    /// Throws if this RenderValue doesn't hold a value of type string
    /// </summary>
    /// <returns>The stored value</returns>
    /// <exception cref="Exception"></exception>
    public string GetStringValue()
    {
        if (stringValue is null)
            throw new Exception("RenderValue is not a string");

        return stringValue;
    }

    /// <summary>
    /// Gets the value of this RenderValue.
    /// Throws if this RenderValue doesn't hold a value of type lambda
    /// </summary>
    /// <returns>The stored value</returns>
    /// <exception cref="Exception"></exception>
    public Func<string, Task<string>> GetLambda()
    {
        if (lambdaValue is null)
            throw new Exception("RenderValue is not a lambda");

        return lambdaValue;
    }

    /// <summary>
    /// Gets the value of this RenderValue.
    /// Throws if this RenderValue doesn't hold a value of type RenderModel[]
    /// </summary>
    /// <returns>The stored value</returns>
    /// <exception cref="Exception"></exception>
    public RenderModel[] GetArray()
    {
        if (arrayValue is null)
            throw new Exception("RenderValue is not an array");

        return arrayValue;
    }
}