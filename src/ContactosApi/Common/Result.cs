namespace ContactosApi.Common;


//result pattern, no depende de exceptions 
public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Conflict
}


// el servicio comunica al controlador lo ocurrido y se traducen los errores encontrados a estados HTTP
public sealed record Result<T>(
    bool IsSuccess,
    T? Value,
    string? Error,
    ErrorType ErrorType
)
{
    public static Result<T> Success(T value)
        => new(true, value, null, ErrorType.None);

    public static Result<T> Validation(string error)
        => new(false, default, error, ErrorType.Validation);

    public static Result<T> NotFound(string error)
        => new(false, default, error, ErrorType.NotFound);

    public static Result<T> Conflict(string error)
        => new(false, default, error, ErrorType.Conflict);
}