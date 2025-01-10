public class EmailService() {
    public async Task SendAsync(Email email)
    {
        // create message
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("Mercantec VM booking system", Config.SMTP_USER));
        message.To.Add(new MailboxAddress("", email.Recipient));
        message.Subject = email.Subject;
        message.Body = new TextPart("HTML") { Text = email.Body };

        // send message
        using (var smtpClient = new SmtpClient())
        {
            try
            {
                await smtpClient.ConnectAsync(Config.SMTP_ADDRESS, int.Parse(Config.SMTP_PORT), true);
                await smtpClient.AuthenticateAsync(Config.SMTP_USER, Config.SMTP_PASSWORD);

                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}