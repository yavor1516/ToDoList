using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using To_Do_List.Service;
using To_Do_List;

public class DailyTaskReminder : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;

    public DailyTaskReminder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(SendDailyReminders, null, TimeSpan.Zero, TimeSpan.FromDays(1));
        return Task.CompletedTask;
    }

    private void SendDailyReminders(object state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TaskContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

            var tasksDueToday = context.Tasks
                .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == DateTime.Now.Date && !t.IsCompleted)
                .ToList();

            foreach (var task in tasksDueToday)
            {
                emailService.SendEmail("manev.yavor@gmail.com", $"Task Reminder: {task.Title}", $"Your task '{task.Title}' is due today.");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
