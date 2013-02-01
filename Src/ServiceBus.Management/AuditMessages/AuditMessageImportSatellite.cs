﻿namespace ServiceBus.Management.AuditMessages
{
    using NServiceBus;
    using NServiceBus.Satellites;
    using Raven.Client;

    public class AuditMessageImportSatellite : ISatellite
    {
        public IDocumentStore Store { get; set; }

        public bool Handle(TransportMessage message)
        {
            using (var session = Store.OpenSession())
            {
               
                var auditMessage = session.Load<Message>(message.IdForCorrelation);

                if (auditMessage == null)
                {
                    auditMessage = new Message(message)
                        {
                            Status = MessageStatus.Successfull
                        };
                }
                else
                {
                    if (auditMessage.Status == MessageStatus.Successfull)
                        return true;//duplicate

                    auditMessage.FailureDetails.ResolvedAt =
                        DateTimeExtensions.ToUtcDateTime(message.Headers[Headers.ProcessingEnded]);

                    auditMessage.Status = MessageStatus.Successfull;
                }

                auditMessage.Statistics = GetStatistics(message);

                session.Store(auditMessage);

                session.SaveChanges();
            }

            return true;
        }

        MessageStatistics GetStatistics(TransportMessage message)
        {
            return new MessageStatistics
                {
                    CriticalTime =
                        DateTimeExtensions.ToUtcDateTime(message.Headers[Headers.ProcessingEnded]) -
                        DateTimeExtensions.ToUtcDateTime(message.Headers[Headers.TimeSent]),
                    ProcessingTime =
                        DateTimeExtensions.ToUtcDateTime(message.Headers[Headers.ProcessingEnded]) -
                        DateTimeExtensions.ToUtcDateTime(message.Headers[Headers.ProcessingStarted])
                };
        }


        public void Start()
        {

        }

        public void Stop()
        {

        }

        public Address InputAddress { get { return Address.Parse("audit"); } }

        public bool Disabled { get { return false; } }
    }
}