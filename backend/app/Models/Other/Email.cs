namespace Models;

public class Email
{
    public string Recipient { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";

    public static Email GetEmail(string recipient, string subject, string body)
    {
        return new Email
        {
            Recipient = recipient,
            Subject = subject,
            Body = body
        };
    }

    public static Email GetUserCreation(User recipient)
    {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "UserCreation.html"));

        htmlContent = ReplaceFromString(htmlContent, new()
        {
            { "##FirstName##", recipient.Name },
            { "##Surname##", recipient.Surname },
            { "##Email##", recipient.Email },
            { "##UserRole##", recipient.Role },
            { "##CreatedAt##", recipient.CreatedAt.ToString("ddd, dd MMM yyy HH:mm:ss")
                ?? "Your Account hasn't been created properly, if you get this message contact tech support." },
        });

        string subject = $"Welcome to VM Ware {recipient.Name} {recipient.Surname}.";

        return GetEmail(recipient.Email, subject, htmlContent);
    }

    public static Email GetUserRoleUpdate(User recipient, string earlierRole)
    {
        string Subject = $"{recipient.Name} {recipient.Surname} your role has been changed.";
        string Body = $"Your role has changed from {earlierRole} to {recipient.Role}.";

        return GetEmail(recipient.Email, Subject, Body);
    }

    public static Email GetUserUpdate(User recipient, User modifier)
    {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "UpdateUser.html"));

        htmlContent = ReplaceFromString(htmlContent, new()
        {
                { "##FirstName##", recipient.Name },
                { "##Surname##", recipient.Surname },
                { "##Email##", recipient.Email },
                { "##UserRole##", recipient.Role },
                { "##ModifierFirstName##", modifier.Name},
                { "##ModifierSurname##", modifier.Surname },
                { "##ModifierEmail##", modifier.Email },
                { "##UpdatedAt##", recipient.UpdatedAt.ToString("ddd, dd MMM yyy HH:mm:ss")
                    ?? "Your Account hasn't been updated properly, if you get this message contact tech support." },
        });

        string subject = $"{recipient.Name} {recipient.Surname} your account has been changed.";

        return GetEmail(recipient.Email, subject, htmlContent);
    }

    public static Email GetVmBookingCreate(VmBooking booking)
    {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "BookingRequestCreate.html"));


        htmlContent = ReplaceFromString(htmlContent, new()
        {
                { "##FirstName##", booking.Owner.Name },
                { "##Surname##", booking.Owner.Surname },
                { "##TeacherFirstName##", booking.Assigned.Name },
                { "##TeacherSurname##", booking.Assigned.Surname },
                { "##TeacherEmail##", booking.Assigned.Email },
                { "##Type##", booking.Type.ToString() },
                { "##MachineUuid##", booking.Name },
                { "##Created##", booking.CreatedAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
                { "##Expires##", booking.ExpiredAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
        });


        string subject = "Booking request created";

        return GetEmail(booking.Owner.Email, subject, htmlContent);
    }

    public static Email GetVmBookingToAccept(VmBooking booking)
    {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "BookingRequestToAccept.html"));

        htmlContent = ReplaceFromString(htmlContent, new()
        {
                { "##FirstName##", booking.Assigned.Name },
                { "##Surname##", booking.Assigned.Surname },
                { "##StudentFirstName##", booking.Owner.Name },
                { "##StudentSurname##", booking.Owner.Surname },
                { "##StudentEmail##", booking.Owner.Email },
                { "##Type##", booking.Type.ToString() },
                { "##MachineUuid##", booking.Name },
                { "##Created##", booking.CreatedAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
                { "##Expires##", booking.ExpiredAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
        });


        string subject = $"{booking.Owner.Name} {booking.Owner.Surname} requests a booking";
        return GetEmail(booking.Assigned.Email, subject, htmlContent);
    }

    public static Email GetVmBookingAccepted(VmBooking booking)
    {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "BookingRequestAccepted.html"));

        htmlContent = ReplaceFromString(htmlContent, new()
        {
            { "##FirstName##", booking.Owner.Name },
            { "##Surname##", booking.Owner.Surname },
            { "##AcceptedTime##", DateTime.UtcNow.ToString("ddd, dd MMM yyy HH:mm:ss") },
            { "##TeacherFirstName##", booking.Assigned.Name },
            { "##TeacherSurname##", booking.Assigned.Surname },
            { "##TeacherEmail##", booking.Assigned.Email },
            { "##Type##", booking.Type.ToString() },
            { "##MachineUuid##", booking.Name },
            { "##Created##", booking.CreatedAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
            { "##Expires##", booking.ExpiredAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
    });


        string subject = "Booking accepted";

        return GetEmail(booking.Owner.Email, subject, htmlContent);
    }

    public static Email GetVmBookingExpired(VmBooking booking)
    {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "BookingRequestExpired.html"));

        htmlContent = ReplaceFromString(htmlContent, new()
        {
                { "##FirstName##", booking.Owner.Name },
                { "##Surname##", booking.Owner.Surname },
                { "##Type##", booking.Type.ToString() },
                { "##MachineUuid##", booking.Name },
                { "##Created##", booking.CreatedAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
                { "##Expires##", booking.ExpiredAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
        });

        string subject = "Booking expired";

        return GetEmail(booking.Owner.Email, subject, htmlContent);
    }

    public static Email GetVmBookingUpdated(VmBooking booking)
    {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "BookingRequestUpdated.html"));

        htmlContent = ReplaceFromString(htmlContent, new()
        {
                { "##FirstName##", booking.Owner.Name },
                { "##Surname##", booking.Owner.Surname },
                { "##TeacherFirstName##", booking.Assigned.Name },
                { "##TeacherSurname##", booking.Assigned.Surname },
                { "##TeacherEmail##", booking.Assigned.Email },
                { "##Type##", booking.Type.ToString() },
                { "##MachineUuid##", booking.Name },
                { "##Created##", booking.CreatedAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
                { "##Expires##", booking.ExpiredAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
        });

        string subject = "Booking updated";

        return GetEmail(booking.Owner.Email, subject, htmlContent);
    }

    private static string ReplaceFromString(string stringToReplaceFrom, Dictionary<string, string> replacements)
    {
        return replacements.Aggregate(stringToReplaceFrom, (current, replacment) =>
            current.Replace(replacment.Key, replacment.Value));
    }
}
