﻿namespace Particular.HealthMonitoring.Uptime
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Particular.HealthMonitoring.Uptime.Api;
    using ServiceControl.Infrastructure.DomainEvents;

    public class UptimeMonitoringDependencies
    {
        public IPersistEndpointUptimeInformation Persister { get; set; }
        public IDomainEvents DomainEvents { get; set; }
    }

    public class UptimeMonitoring : IComponent<UptimeMonitoringDependencies>
    {
        EndpointInstanceMonitoring monitoring;

        public UptimeMonitoring()
        {
            monitoring = new EndpointInstanceMonitoring();
        }
        
        public IEnumerable<object> CreateParts()
        {
            yield return new UptimeApiModule(monitoring);
            yield return new HeartbeatFailureDetector(monitoring);
            yield return new HeartbeatProcessor(monitoring);
            yield return new EndpointDetectingProcessor(monitoring);
        }

        public Task Initialize(UptimeMonitoringDependencies dependencies)
        {
            return monitoring.Initialize(dependencies.Persister, dependencies.DomainEvents);
        }

        public Task TearDown()
        {
            return Task.FromResult(0);
        }
    }
}