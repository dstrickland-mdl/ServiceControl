namespace ServiceControl.CompositeViews.Messages
{
    using System.Threading.Tasks;
    using Nancy;
    using Raven.Client;
    using ServiceControl.Infrastructure.Extensions;

    public class GetAllMessagesApi : ScatterGatherApi<NoInput>
    {
        public override async Task<QueryResult> LocalQuery(Request request, NoInput input)
        {
            using (var session = Store.OpenAsyncSession())
            {
                RavenQueryStatistics stats;

                var results = await session.Query<MessagesViewIndex.SortAndFilterOptions, MessagesViewIndex>()
                    .IncludeSystemMessagesWhere(request)
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