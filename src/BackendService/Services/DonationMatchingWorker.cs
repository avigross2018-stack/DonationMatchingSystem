using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Models;

namespace BackendService.Services
{
    public class DonationMatchingWorker : BackgroundService
    {
        private readonly MatchingEventQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DonationMatchingWorker> _logger;

        public DonationMatchingWorker(
            MatchingEventQueue queue,
            IServiceScopeFactory scopeFactory,
            ILogger<DonationMatchingWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var matchingEvent in _queue.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await using var scope =
                        _scopeFactory.CreateAsyncScope();

                    var matchingService = scope.ServiceProvider
                        .GetRequiredService<DonationMatchingService>();

                    _logger.LogInformation(
                        "Received matching event: {EventType}, EntityId: {EntityId}",
                        matchingEvent.Type,
                        matchingEvent.EntityId);

                    switch (matchingEvent.Type)
                    {
                        case MatchingEventType.RecipientRegistered:
                            if (matchingEvent.EntityId is int recipientId)
                            {
                                var count = await matchingService.MatchRecipientAsync(
                                    recipientId,
                                    stoppingToken);

                                _logger.LogInformation(
                                    "Created {Count} matches for recipient {RecipientId}",
                                    count,
                                    recipientId);
                            }
                            break;

                        case MatchingEventType.DonationCreated:
                        case MatchingEventType.DonationUpdated:
                            if (matchingEvent.EntityId is int donationId)
                            {
                                var count = await matchingService.MatchDonationAsync(
                                    donationId,
                                    stoppingToken);

                                _logger.LogInformation(
                                    "Created {Count} matches for donation {DonationId}",
                                    count,
                                    donationId);
                            }
                            break;

                        case MatchingEventType.FullScan:
                            {
                                var count = await matchingService.ScanAllAsync(
                                stoppingToken);

                                _logger.LogInformation(
                                    "Full matching scan completed. Created {Count} matches",
                                    count);
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed processing matching event: {EventType}",
                        matchingEvent.Type);
                }
            }
        }
    }
}