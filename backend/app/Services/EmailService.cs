using NuGet.DependencyResolver;
using Org.BouncyCastle.Cms;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class EmailService
{
    private readonly Config _config;
    private static readonly string emailLocaltion = _config.EMAIL_TEMPLATES_PATH;

    public EmailService(Config config)
    {
        _config = config;
    }

    public async Task SendEmail(string recipientAddress, string subject, string body)
    {
        if (_config.TEST_MODE)
        {
            Console.WriteLine($"TEST MODE: EMAIL NOT SEND! Mail to: {recipientAddress}, Subject: {subject}");
            return;
        }

        MimeMessage message = CreateEmail(recipientAddress, subject, body);
        await SendEmailAsync(message);
    }

    private MimeMessage CreateEmail(string recipientAddress, string subject, string body)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("Mercantec VM booking system", _config.SMTP_USER));
        message.To.Add(new MailboxAddress("", recipientAddress));
        message.Subject = subject;
        message.Body = new TextPart("HTML") { Text = body };

        return message;
    }

    private async Task SendEmailAsync(MimeMessage message)
    {
        using (var smtpClient = new SmtpClient())
        {
            try
            {
                await smtpClient.ConnectAsync(_config.SMTP_ADDRESS, int.Parse(_config.SMTP_PORT), true);
                await smtpClient.AuthenticateAsync(_config.SMTP_USER, _config.SMTP_PASSWORD);

                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public async void SendUserCreation(User recipient)
    {
        BodyBuilder bodyBuilder = new BodyBuilder();

        string htmlContent = File.ReadAllText(emailLocaltion + "UserCreation.html");

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

        await SendEmail(recipient.Email, subject, bodyBuilder.HtmlBody = htmlContent);
    }

    public async Task SendUserRoleUpdate(User recipient, string earlierRole)
    {
        string Subject = $"{recipient.Name} {recipient.Surname} your role has been changed.";
        string Body = $"Your role has changed from {earlierRole} to {recipient.Role}.";

        await SendEmail(recipient.Email, Subject, Body);
    }

    public async Task sendUserUpdate(User recipient, User modifier)
    {
        BodyBuilder bodyBuilder = new BodyBuilder();

        string htmlContent = File.ReadAllText(emailLocaltion + "UpdateUser.html");


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

        bodyBuilder.HtmlBody = htmlContent;
        string subject = $"{recipient.Name} {recipient.Surname} your account has been changed.";

        await SendEmail(recipient.Email, subject, bodyBuilder.HtmlBody = htmlContent);
    }

    public async void SendVmBookingCreate(VmBooking booking)
    {
        BodyBuilder bodyBuilder = new BodyBuilder();

        string htmlContent = File.ReadAllText(emailLocaltion + "BookingRequestCreate.html");


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

        await SendEmail(booking.Owner.Email, subject, bodyBuilder.HtmlBody = htmlContent);
    }

    public async void SendVmBookingToAccept(VmBooking booking)
    {
        BodyBuilder bodyBuilder = new BodyBuilder();

        string htmlContent = File.ReadAllText(emailLocaltion + "BookingRequestToAccept.html");

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
        await SendEmail(booking.Assigned.Email, subject, bodyBuilder.HtmlBody = htmlContent);
    }

    public async void SendVmBookingaceepted(VmBooking booking)
    {
        BodyBuilder bodyBuilder = new BodyBuilder();

        string htmlContent = File.ReadAllText(emailLocaltion + "BookingRequestAccepted.html");


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

        await SendEmail(booking.Owner.Email, subject, bodyBuilder.HtmlBody = htmlContent);
    }

    public async void SendVmBookingExpired(VmBooking booking)
    {
        BodyBuilder bodyBuilder = new BodyBuilder();

        string htmlContent = File.ReadAllText(emailLocaltion + "BookingRequestExpired.html");

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

        await SendEmail(booking.Owner.Email, subject, bodyBuilder.HtmlBody = htmlContent);
    }

    public async void SendVmBookingUpdated(VmBooking booking)
    {
        BodyBuilder bodyBuilder = new BodyBuilder();

        string htmlContent = File.ReadAllText(emailLocaltion + "BookingRequestUpdated.html");


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

        await SendEmail(booking.Owner.Email, subject, bodyBuilder.HtmlBody = htmlContent);
    }

    public async void SendClusterBookingCreate(ClusterBooking Clusterbooking)
    {
        BodyBuilder bodyBuilder = new BodyBuilder();

        string htmlBody = File.ReadAllText(emailLocaltion + "BookingCreate.html");
        string vcenterPrefab = File.ReadAllText(emailLocaltion + "VCenter.html");
        string esxiPrefab = File.ReadAllText(emailLocaltion + "EsxiHosts.html");

                string vcenterAndHosts = "";
        foreach (VCenter vCenter in Clusterbooking.VCenters)
        {
            List<EsxiHost> vcenterHosts = vCenter.EsxiHosts.FindAll(h => h.VCenterId == vCenter.Id);
            Dictionary<string, string> replacements = new()
            {
                { "##amoutOfEsxiHosts##", vcenterHosts.Count.ToString() },
                { "##VCenterIP##", vCenter.Ip.ToString() },
                { "##VCenterUser##", vCenter.UserName },
                { "##VCenterPassword##", vCenter.Password },
            };

            string esxiHost = "";
            foreach (EsxiHost host in vcenterHosts)
            {
                string esxi = ReplaceFromString(esxiPrefab, new()
                {
                    { "##hostIp##", host.Ip.ToString()},
                    { "##hostUsername##", host.UserName},
                    { "##hostPassword##", host.Password},
                });

                if (!replacements.TryGetValue("##firstHost##", out _))
                {
                    replacements.Add("##firstHost##", esxi.Substring("<tr>".Length, esxi.Length - "</tr>".Length));
                    continue;
                }

                esxiHost += esxi;
            }

            replacements.Add("##hosts##", esxiHost);
            vcenterAndHosts += ReplaceFromString(vcenterPrefab, replacements);
        }

        htmlBody = ReplaceFromString(htmlBody, new()
        {
            { "##name##", Clusterbooking.Owner.Name },
            { "##surname##", Clusterbooking.Owner.Surname },
            { "##email##", Clusterbooking.Owner.Email },
            { "##role##", Clusterbooking.Owner.Role },
            { "##bookingId##", Clusterbooking.Id.ToString()},
            { "##bookingAoumtStudents##", Clusterbooking.AmountStudents.ToString() },
            { "##vcentersAndHost##", vcenterAndHosts },
            { "##created##", Clusterbooking.CreatedAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
            { "##expires##", Clusterbooking.ExpiredAt.ToString("ddd, dd MMM yyy HH:mm:ss") },
        });
        string subject = "Cluster booking created";

        await SendEmail(Clusterbooking.Owner.Email, subject, bodyBuilder.HtmlBody = $"{htmlBody + vcenterPrefab + esxiPrefab}");
    }

    private static string ReplaceFromString(string stringToReplaceFrom, Dictionary<string, string> replacements)
    {
        return replacements.Aggregate(stringToReplaceFrom, (current, replacment) =>
            current.Replace(replacment.Key, replacment.Value));
    }

}
