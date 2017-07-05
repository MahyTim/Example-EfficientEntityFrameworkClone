using System;
using System.Collections.Generic;

namespace EFCloner
{
    public class Customer : IProcessDefinitionDependent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ProcessDefinition ProcessDefinition { get; set; }
    }
}