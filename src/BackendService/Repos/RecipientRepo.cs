using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackendService.Data;
using BackendService.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Repos
{
    public class RecipientRepo : IRecipientRepo
    {
        private readonly AppDbContext _appDb;

        public RecipientRepo(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public async Task AddAsync(Recipient recipient)
        {
            await _appDb.Recipients.AddAsync(recipient);
        }

        public async Task<Recipient?> GetByIdAsync(int id)
        {
            return await _appDb.Recipients
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipient?> GetByIdForUpdateAsync(int recipientId, CancellationToken cancellationToken = default)
        {
            var query = _appDb.Recipients
                .FromSqlInterpolated(
                    $"SELECT * FROM `Recipients` WHERE `Id` = {recipientId} FOR UPDATE")
                .AsAsyncEnumerable();

            await foreach (var recipient in
                query.WithCancellation(cancellationToken))
            {
                return recipient;
            }

            return null;
        }

        public async Task<Recipient?> GetByUserIdAsync(int userId)
        {
            return await _appDb.Recipients
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.PopulationType)
                .FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task<List<Recipient>> GetEligibleRecipientsAsync(CancellationToken cancellationToken = default)
        {
            return await _appDb.Recipients
                .AsNoTracking()
                .Where(r =>
                    r.User.IsActive &&
                    r.RemainingEligibility > 0)
                .ToListAsync(cancellationToken);
        }

        public async Task ReloadAsync(Recipient recipient, CancellationToken cancellationToken = default)
        {
            // Recalling and updating entity values on track.
            await _appDb.Entry(recipient)
                .ReloadAsync(cancellationToken);
        }

        public async Task<Recipient?> GetByIdNoTrackingAsync(
            int recipientId,
            CancellationToken cancellationToken = default)
        {
            return await _appDb.Recipients
                .AsNoTracking()
                .Include(r => r.User)
                .FirstOrDefaultAsync(
                    r => r.Id == recipientId,
                    cancellationToken);
        }

        public async Task<bool> IsRecipientActiveAsync(int recipientId)
        {
            return await _appDb.Recipients
                .AnyAsync(r =>
                    r.Id == recipientId &&
                    r.User.IsActive);
        }
        
        public async Task<Recipient?> GetByUserIdForUpdateAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var query = _appDb.Recipients
                .FromSqlInterpolated(
                    $"SELECT * FROM `Recipients` WHERE `UserId` = {userId} FOR UPDATE")
                .AsAsyncEnumerable();

            await foreach (var recipient in
                query.WithCancellation(cancellationToken))
            {
                return recipient;
            }

            return null;
        }
    }
}