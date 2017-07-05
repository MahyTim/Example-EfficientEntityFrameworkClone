using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EFCloner
{
    static class DbHelper
    {
        public static ProcessDefinition Clone(this MyDatabase context, ProcessDefinition entity, bool alsoCopyDeletedItems)
        {
            var idMapping = new Dictionary<Guid, Guid>();
            var processDefinitionId = entity.Id;
            var newDefinitionId = Guid.NewGuid();
            using (var temporaryContext = context.CreateNested())
            {
                temporaryContext.Configuration.ValidateOnSaveEnabled = false;
                temporaryContext.Configuration.LazyLoadingEnabled = true;

                var newDefinition = temporaryContext
                    .ProcessDefinitions
                    .Single(pd => pd.Id == processDefinitionId);

                temporaryContext.Entry(newDefinition).State = EntityState.Added;
                newDefinition.Id = newDefinitionId;

                var allDedependants =
                    temporaryContext
                        .AllProcessDefinitionDependents(processDefinitionId)
                        .ToList();

                temporaryContext.Configuration.AutoDetectChangesEnabled = false;

                allDedependants.ForEach(dependant =>
                {
                    temporaryContext.Entry(dependant).State = EntityState.Added;
                    var currentId = dependant.Id;
                    var newId = Guid.NewGuid();
                    idMapping.Add(currentId, newId);
                    dependant.Id = newId;
                    dependant.ProcessDefinition = newDefinition;
                });

                temporaryContext.ChangeTracker.DetectChanges();
                temporaryContext.SaveChanges();
            }

            return context.ProcessDefinitions.Find(newDefinitionId);
        }
    }
}