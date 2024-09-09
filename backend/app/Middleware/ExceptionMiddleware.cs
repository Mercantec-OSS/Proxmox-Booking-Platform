public class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpException ex) 
        {
            await HandleExceptionAsync(context, ex.Message, (int) ex.StatusCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await HandleExceptionAsync(context, ex.Message, (int) HttpStatusCode.InternalServerError);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new { message });
        return context.Response.WriteAsync(result);
    }
}