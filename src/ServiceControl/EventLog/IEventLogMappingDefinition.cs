﻿namespace ServiceControl.EventLog
{
    using ServiceControl.Infrastructure.DomainEvents;


    public interface IEventLogMappingDefinition
    {
        EventLogItem Apply(IDomainEvent @event);
    }
}
