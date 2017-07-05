using System;

namespace EFCloner
{
    public class Order : IProcessDefinitionDependent
    {
        public Guid Id { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual ProcessDefinition ProcessDefinition { get; set; }
        public string Name { get; set; }
    }
}