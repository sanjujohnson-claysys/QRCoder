using Cronos;

namespace QRCode
{
   

        public class CronJobService : BackgroundService
        {
            private readonly ILogger<CronJobService> _logger;
            private readonly CronExpression _cronEveryTwoMinutes;
            private readonly CronExpression _cronSpecificTime;

            public CronJobService(ILogger<CronJobService> logger)
            {
                _logger = logger;
                _cronEveryTwoMinutes = CronExpression.Parse("*/2 * * * *"); // Every two minutes
                _cronSpecificTime = CronExpression.Parse("10 16 * * *"); // 3:00 PM daily
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    DateTime now = DateTime.UtcNow;
                    DateTime? nextEveryTwoMinutesOccurrence = _cronEveryTwoMinutes.GetNextOccurrence(now);
                    DateTime? nextSpecificTimeOccurrence = _cronSpecificTime.GetNextOccurrence(now);

                    if (nextEveryTwoMinutesOccurrence.HasValue && now >= nextEveryTwoMinutesOccurrence.Value.AddSeconds(-1) && now <= nextEveryTwoMinutesOccurrence.Value.AddSeconds(1))
                    {
                        _logger.LogInformation($"Job executed every two minutes at {now}");
                    }

                    if (nextSpecificTimeOccurrence.HasValue && now >= nextSpecificTimeOccurrence.Value.AddSeconds(-1) && now <= nextSpecificTimeOccurrence.Value.AddSeconds(1))
                    {
                        _logger.LogInformation($"Job executed at specific time at {now}");
                    }

                    await Task.Delay(1000, stoppingToken);
                }
            }
        }




    }

