using System.ComponentModel.DataAnnotations;

namespace Domain.Interfaces;

public interface IEntity : ICreatable
{
    [Key] public int Id { get; }
}