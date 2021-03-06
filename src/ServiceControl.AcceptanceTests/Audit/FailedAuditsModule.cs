﻿namespace ServiceBus.Management.AcceptanceTests
{
    using System.Threading;
    using Nancy;
    using NServiceBus;
    using Raven.Client;
    using ServiceBus.Management.Infrastructure.Extensions;
    using ServiceBus.Management.Infrastructure.Nancy.Modules;
    using ServiceControl.Operations;

    public class FailedAuditsCountReponse
    {
        public int Count { get; set; }
    }

    public class FailedAuditsModule : BaseModule
    {
        public IBus Bus { get; set; }
        public ImportFailedAudits ImportFailedAudits { get; set; }

        public FailedAuditsModule()
        {
            Get["/failedaudits/count", true] = async (_, token) =>
            {
                using (var session = Store.OpenAsyncSession())
                {
                    RavenQueryStatistics stats;
                    var query =
                        session.Query<FailedAuditImport, FailedAuditImportIndex>().Statistics(out stats);

                    var count = await query.CountAsync();

                    return Negotiate
                        .WithModel(new FailedAuditsCountReponse
                        {
                            Count = count
                        })
                        .WithEtag(stats);
                }
            };

            Post["/failedaudits/import", true] = async (_, token) =>
            {
                var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                await ImportFailedAudits.Run(tokenSource);
                return HttpStatusCode.OK;
            };
        }
    }
}