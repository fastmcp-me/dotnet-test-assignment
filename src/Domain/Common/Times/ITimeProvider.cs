namespace Domain.Common.Times;

public interface ITimeProvider
{
    DateTimeOffset Now { get; }
}

public class DefaultTimeProvider : ITimeProvider
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}