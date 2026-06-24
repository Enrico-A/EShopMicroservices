using MediatR;

namespace BuildingBlocks.CQRS
{
    // IQueryHandler is an interface that defines a handler for queries in the CQRS pattern. It extends the IRequestHandler interface from MediatR,
    // allowing it to handle queries of type TQuery and return a response of type TResponse.
    // TQuery represents the type of query that the handler will process, and it must implement the IQuery<TResponse> interface.
    // TResponse represents the type of response that the handler will return after processing the query, and it must be a non-nullable type.
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : notnull
    {
    }
}
