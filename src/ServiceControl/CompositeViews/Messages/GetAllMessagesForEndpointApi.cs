namespace ServiceControl.CompositeViews.Messages
{
    using System.Threading.Tasks;
    using Nancy;
    using Raven.Client;
    using Raven.Client.Linq;
    using ServiceControl.Infrastructure.Extensions;

    public class GetAllMessagesForEndpointApi : ScatterGatherApi<string>
    {
        public override async Task<QueryResult> LocalQuery(Request request, string input)
        {
            using (var session = Store.OpenAsyncSession())
            {
                RavenQueryStatistics stats;

                var results = await session.Query<MessagesViewIndex.SortAndFilterOptions, MessagesViewIndex>()
                    .IncludeSystemMessagesWhere(request)
                    .Where(m => m.ReceivingEndpointName == input)
                    .Statistics(out stats)
                    .Sort(request)
                    .Paging(request)
                    .TransformWith<MessagesViewTransformer, MessagesView>()
                    .ToListAsync()
                    .ConfigureAwait(false);

                return Results(results, stats);
            }

        }
    }
}