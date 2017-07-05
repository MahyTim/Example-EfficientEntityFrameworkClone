using System;

namespace EFCloner
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}