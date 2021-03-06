namespace ServiceControlInstaller.Engine.Instances
{
    using System;

    public class ServiceControlUpgradeOptions
    {
        public bool? OverrideEnableErrorForwarding { get; set; }
        public TimeSpan? ErrorRetentionPeriod { get; set; }
        public TimeSpan? AuditRetentionPeriod { get; set; }
        public bool SkipQueueCreation { get; set; }

        public void ApplyChangesToInstance(ServiceControlInstance instance)
        {
            if (OverrideEnableErrorForwarding.HasValue)
            {
                instance.ForwardErrorMessages = OverrideEnableErrorForwarding.Value;
            }

            if (ErrorRetentionPeriod.HasValue)
            {
                instance.ErrorRetentionPeriod = ErrorRetentionPeriod.Value;
            }

            if (AuditRetentionPeriod.HasValue)
            {
                instance.AuditRetentionPeriod = AuditRetentionPeriod.Value;
            }

            instance.SkipQueueCreation = SkipQueueCreation;

            instance.ApplyConfigChange();
        }
    }
}