public class ResponseMessage(string message)
{
    public string Message { get; set; } = message;

    public static ResponseMessage GetErrorMessage(string errorMessage)
    {
        return new ResponseMessage(errorMessage);
    }

    public static ResponseMessage GetUserUnauthorized()
    {
        return new ResponseMessage("Insufficient privileges to perform this action");
    }

    public static ResponseMessage GetUserNotFound()
    {
        return new ResponseMessage("User not found");
    }

    public static ResponseMessage GetOwnerNotFound()
    {
        return new ResponseMessage("Owner not found");
    }

    public static ResponseMessage GetTeacherNotFound()
    {
        return new ResponseMessage("Teacher not found");
    }

    public static ResponseMessage GetClassNotFound()
    {
        return new ResponseMessage("Class not found");
    }

    public static ResponseMessage GetBookingNotFound()
    {
        return new ResponseMessage("Booking not found"); 
    }

    public static ResponseMessage GetSelectedVCenterNotFound()
    {
        return new ResponseMessage("Selected VCenter not found");
    }

    public static ResponseMessage GetVCenterNotFound()
    {
        return new ResponseMessage("VCenter not found");
    }

    public static ResponseMessage GetEsxiHostNotFound()
    {
        return new ResponseMessage("EsxiHost not found");
    }

    public static ResponseMessage GetHostNotValid()
    {
        return new ResponseMessage("EsxiHost dto not valid");
    }

    public static ResponseMessage GetHostIdUnprocessable()
    {
        return new ResponseMessage("Coudn't find any hosts on that id");
    }

    public static ResponseMessage GetVCenterNotValid()
    {
        return new ResponseMessage("VCenter dto not valid");
    }

    public static ResponseMessage GetIpIsEmpty()
    {
        return new ResponseMessage("Ip is empty");
    }

    public static ResponseMessage GetUserDetailsEmpty()
    {
        return new ResponseMessage("Username or password is empty");
    }

    public static ResponseMessage GetHostAlreadyExists()
    {
        return new ResponseMessage("EsxiHost with this Ip already exists");
    }

    public static ResponseMessage GetVCenterAlreadyExists()
    {
        return new ResponseMessage("VCenter already exists");
    }
}
