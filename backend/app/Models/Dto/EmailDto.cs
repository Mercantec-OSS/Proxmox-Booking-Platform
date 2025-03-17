namespace Dto;

public class EmailDto
{
    public string Recipient { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";

    public static EmailDto GetUserCreation(User recipient)
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

        return new EmailDto(){Recipient=recipient.Email, Subject=subject, Body=htmlContent};
    }

    public static EmailDto GetUserRoleUpdate(User recipient, string earlierRole)
    {
        string Subject = $"{recipient.Name} {recipient.Surname} your role has been changed.";
        string Body = $"Your role has changed from {earlierRole} to {recipient.Role}.";

        return new EmailDto(){Recipient=recipient.Email, Subject=Subject, Body=Body};
    }

    public static EmailDto GetUserUpdate(User recipient, User modifier)
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

        return new EmailDto(){Recipient=recipient.Email, Subject=subject, Body=htmlContent};
    }

    public static EmailDto GetVmBookingCreate(VmBooking booking)
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

        return new EmailDto(){Recipient=booking.Owner.Email, Subject=subject, Body=htmlContent};
    }

    public static EmailDto GetVmBookingToAccept(VmBooking booking)
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
        return new EmailDto(){Recipient=booking.Assigned.Email, Subject=subject, Body=htmlContent};
    }

    public static EmailDto GetVmBookingAccepted(VmBooking booking)
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

        return new EmailDto(){Recipient=booking.Owner.Email, Subject=subject, Body=htmlContent};
    }

    public static EmailDto GetVmBookingExpired(VmBooking booking)
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

        return new EmailDto(){Recipient=booking.Owner.Email, Subject=subject, Body=htmlContent};
    }

    public static EmailDto GetVmBookingUpdated(VmBooking booking)
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

        return new EmailDto(){Recipient=booking.Owner.Email, Subject=subject, Body=htmlContent};
    }

    public static EmailDto GetInviteLink(string recipientEmail, string inviteKey, string userRole) {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "InviteLink.html"));

        htmlContent = ReplaceFromString(htmlContent, new()
        {
            { "##INVITE_KEY##", inviteKey },
            { "##USER_ROLE##", userRole },
            { "##CREATED_AT##", DateTime.UtcNow.ToString("ddd, dd MMM yyy HH:mm:ss") },
        });

        string subject = "Invite link";

        return new EmailDto(){Recipient=recipientEmail, Subject=subject, Body=htmlContent};
    }

    public static EmailDto GetResetPassword(string recipientEmail, string token)
    {
        string htmlContent = File.ReadAllText(Path.Combine(Config.EMAIL_TEMPLATES_PATH, "ResetPassword.html"));

        htmlContent = ReplaceFromString(htmlContent, new()
        {
            { "##TOKEN##", token },
            { "##CREATED_AT##", DateTime.UtcNow.ToString("ddd, dd MMM yyy HH:mm:ss") },
        });

        string subject = "Reset password";

        return new EmailDto(){Recipient=recipientEmail, Subject=subject, Body=htmlContent};
    }

    private static string ReplaceFromString(string stringToReplaceFrom, Dictionary<string, string> replacements)
    {
        return replacements.Aggregate(stringToReplaceFrom, (current, replacment) =>
            current.Replace(replacment.Key, replacment.Value));
    }
}
