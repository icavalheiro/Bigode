namespace Bigode.Models;

public class RenderValue
{
    public readonly bool IsString;
    public readonly bool IsModel;
    public readonly bool IsLambda;
    public readonly bool IsArray;
    public readonly bool IsBool;

    private readonly string? stringValue;
    private readonly RenderModel? mulitpleValue;
    private readonly Func<string, Task<string>>? lambdaValue;
    private readonly RenderModel[]? arrayValue;
    private readonly bool? boolValue = null;

    public RenderValue(string value)
    {
        IsString = true;
        IsModel = false;
        IsLambda = false;
        IsArray = false;
        IsBool = false;
        stringValue = value;
    }

    public RenderValue(RenderModel value)
    {
        IsString = false;
        IsModel = true;
        IsLambda = false;
        IsArray = false;
        IsBool = false;
        mulitpleValue = value;
    }

    public RenderValue(RenderModel[] value)
    {
        IsString = false;
        IsModel = false;
        IsLambda = false;
        IsArray = true;
        IsBool = false;
        arrayValue = value;
    }

    public RenderValue(Func<string, Task<string>> value)
    {
        IsString = false;
        IsModel = false;
        IsLambda = true;
        IsArray = false;
        IsBool = false;
        lambdaValue = value;
    }

    public RenderValue(bool value)
    {
        IsString = false;
        IsModel = false;
        IsLambda = false;
        IsArray = false;
        IsBool = true;
        boolValue = value;
    }

    public RenderModel GetModel()
    {
        if (mulitpleValue is null)
            throw new Exception("RenderValue is not a model");

        return mulitpleValue;
    }

    public bool GetBoolValue()
    {
        if (boolValue is null)
            throw new Exception("RenderValue is not a bool");

        return (bool)boolValue; // c# compiler is dumb, bool is never null but go figure...
    }

    public string GetStringValue()
    {
        if (stringValue is null)
            throw new Exception("RenderValue is not a string");

        return stringValue;
    }

    public Func<string, Task<string>> GetLambda()
    {
        if (lambdaValue is null)
            throw new Exception("RenderValue is not a lambda");

        return lambdaValue;
    }

    public RenderModel[] GetArray()
    {
        if (arrayValue is null)
            throw new Exception("RenderValue is not an array");

        return arrayValue;
    }
}