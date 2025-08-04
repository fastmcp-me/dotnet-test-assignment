using Domain.Interfaces;

namespace Domain;

public class BaseEntity : IEntity
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}