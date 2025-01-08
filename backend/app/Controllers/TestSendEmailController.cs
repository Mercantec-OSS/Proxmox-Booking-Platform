[ApiController]
[Route("email_test")]
public class TestSendEmailController(Config config) : ControllerBase
{
    private readonly EmailService _emailService = new(config);

    [HttpGet("user-creation")]
    public ActionResult SendFakeUserCreation(string emailToSendToo = "")
    {
        Random random = new();
        User user = new()
        {
            Email = !string.IsNullOrEmpty(emailToSendToo) ? emailToSendToo : "kkla.skp@edu.mercantec.dk",
            Name = "admin",
            Surname = "test",
            CreatedAt = DateTime.UtcNow,
            Role = Enum.GetNames(typeof(User.UserRoles))[random.Next(Enum.GetNames(typeof(User.UserRoles)).Length)]
        };

        _emailService.SendUserCreation(user);

        return Ok();
    }

    [HttpGet("fake-user-update")]
    public ActionResult FakeUserUpdate(string? emailToSendToo = "")
    {
        User recipient = new()
        {
            Name = "admin",
            Surname = "admin",
            Email = !string.IsNullOrEmpty(emailToSendToo) ? emailToSendToo : "kkla.skp@edu.mercantec.dk",
            Role = Enum.GetNames(typeof(User.UserRoles))[new Random().Next(Enum.GetNames(typeof(User.UserRoles)).Length)],
            UpdatedAt = DateTime.UtcNow,
        };

        User modifier = new()
        {
            Name = "Bob",
            Surname = "boss",
            Email = "kkla.skp@edu.mercantec.dk",
        };

        // _emailService.SendUserUpdate(recipient, modifier);
        return Ok();
    }

    [HttpGet("booking")]
    public ActionResult SendFakeEmails(string emailToSendToo = "")
    {
        User owner = new()
        {
            Id = 1,
            Name = "Owner",
            Surname = "Fake",
            Email = !string.IsNullOrEmpty(emailToSendToo) ? emailToSendToo : "kkla.skp@edu.mercantec.dk",
        };
        User teacher = new()
        {
            Id = 2,
            Name = "Teacher",
            Surname = "Fake",
            Email = !string.IsNullOrEmpty(emailToSendToo) ? emailToSendToo : "kkla.skp@edu.mercantec.dk",
        };
        VmBooking booking = new()
        {
            OwnerId = owner.Id,
            AssignedId = teacher.Id,
            Type = "Windows10Pro",
            Name = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            ExpiredAt = DateTime.UtcNow.AddDays(10),
        };

        _emailService.SendVmBookingCreate(booking);
        _emailService.SendVmBookingToAccept(booking);
        _emailService.SendVmBookingaceepted(booking);
        _emailService.SendVmBookingExpired(booking);
        _emailService.SendVmBookingUpdated(booking);

        return Ok();
    }


}
