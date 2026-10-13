using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using BackendService.Models;

namespace BackendService.Services
{
    public class MatchingEventQueue
    {
        private readonly Channel<MatchingEvent> _channel;
        public MatchingEventQueue()
        {
            _channel = Channel.CreateBounded<MatchingEvent>(
                new BoundedChannelOptions(100)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                }
            );
        }
        
        public async Task EnqueueAsync(
            MatchingEvent matchingEvent,
            CancellationToken cancellationToken = default)
        {
            using var timeout = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken);

            timeout.CancelAfter(TimeSpan.FromSeconds(3));

            await _channel.Writer.WriteAsync(
                matchingEvent,
                timeout.Token);
        }


        public IAsyncEnumerable<MatchingEvent> ReadAllAsync(
            CancellationToken cancellationToken = default
        )
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}