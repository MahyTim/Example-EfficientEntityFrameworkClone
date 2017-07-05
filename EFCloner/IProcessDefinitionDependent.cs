namespace EFCloner
{
    public interface IProcessDefinitionDependent : IEntity
    {
        ProcessDefinition ProcessDefinition { get; set; }
    }
}