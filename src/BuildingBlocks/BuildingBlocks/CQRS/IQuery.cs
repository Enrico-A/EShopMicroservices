using MediatR;

namespace BuildingBlocks.CQRS
{
    // IQuery<TResponse> is a generic interface that represents a query in the CQRS pattern, which returns a response of type TResponse.
    // It is used to encapsulate a request to retrieve data or information from the system without modifying its state.
    public interface IQuery<out TResponse> : IRequest<TResponse> 
        where TResponse : notnull
    {
    }
}
