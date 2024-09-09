namespace Services;

public class ActivityService : IHostedService
{
    private static readonly List<Activity> activities = [];
    private const int HOURS_TO_EXPIRE = 6;
    private readonly System.Timers.Timer timer = new()
    {
        Interval = 5000,
        AutoReset = true,
        Enabled = false
    };

    public Activity CreateActivity(int userId, int bookingId, Activity.ActivityEvent activityAction, Activity.ActivityType activityType)
    {
        Activity activity = new()
        {
            OwnerId = userId,
            BookingId = bookingId,
            Action = activityAction,
            Type = activityType,
        };

        activities.Add(activity);
        return activity;
    }

    public List<Activity> GetByUserId(int userId)
    {
        return activities.FindAll(t => t.OwnerId == userId);
    }

    public List<Activity> GetAll()
    {
        return activities;
    }

    public void Delete(string uuid)
    {
        Activity? activity = activities.FirstOrDefault(t => t.Uuid == uuid);
        if (activity != null)
        {
            activities.Remove(activity);
        }
    }

    public void DeleteByBookingId(int bookingId)
    {
        activities.RemoveAll(activity => activity.BookingId == bookingId);
    }

    public void DeleteByUser(int userId)
    {
        activities.RemoveAll(activity => activity.OwnerId == userId);
    }

    public void DeleteAll()
    {
        activities.Clear();
    }

    private static void ThreadAction()
    {
        foreach (Activity activity in activities.FindAll(a =>
            DateTime.UtcNow - a.CreatedAt > TimeSpan.FromHours(HOURS_TO_EXPIRE)))
        {
            activities.Remove(activity);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer.Elapsed += (sender, e) => ThreadAction();
        timer.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer.Stop();
        return Task.CompletedTask;
    }
}