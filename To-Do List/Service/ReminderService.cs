namespace To_Do_List.Service
{
    public class ReminderService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public ReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Set the timer to run every minute
            _timer = new Timer(SendReminders, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void SendReminders(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TaskContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                var now = DateTime.Now;
                var tasksWithReminders = context.Tasks
                    .Where(t => t.ReminderTime.HasValue && t.ReminderTime.Value <= now && !t.IsCompleted)
                    .ToList();

                foreach (var task in tasksWithReminders)
                {
                    emailService.SendEmail("manev.yavor@gmail.com", $"Task Reminder: {task.Title}", $"Reminder: The task '{task.Title}' is scheduled for now.");
                    task.ReminderTime = null;
                    context.SaveChanges();
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
}
