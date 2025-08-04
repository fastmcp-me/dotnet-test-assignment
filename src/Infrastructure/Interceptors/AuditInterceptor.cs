using Domain.Common.Auth;
using Domain.Common.Times;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    #region DI
    private readonly ICurrentUserContext _currentUserService;
    private readonly ITimeProvider _timeProvider;

    public AuditInterceptor(ICurrentUserContext currentUserService, ITimeProvider timeProvider)
    {
        _currentUserService = currentUserService;
        _timeProvider = timeProvider;
    }
    #endregion

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null)
            return base.SavingChanges(eventData, result);

        SetAuditFields(context);

        return base.SavingChanges(eventData, result);
    }

    private void SetAuditFields(DbContext context)
    {
        var entries = context.ChangeTracker.Entries<IEntity>().Where(e => e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            entry.Entity.CreatedBy = _currentUserService.UserId; // 0 = System
            entry.Entity.CreatedAt = _timeProvider.Now;
        }
    }
}