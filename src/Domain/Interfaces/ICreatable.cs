namespace Domain.Interfaces;

public interface ICreatable
{
    DateTimeOffset CreatedAt { get; set; }
    int CreatedBy { get; set; }
}