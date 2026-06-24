using MediatR;

namespace BuildingBlocks.CQRS
{
    // ICommand is an interface that represents a command in the CQRS pattern. It is used to encapsulate a request to perform an action or change the
    // state of the system.
    // It does not return a response, as commands are typically used for operations that modify the system's state. 
    public interface ICommand : ICommand<Unit>
    {
    }

    // ICommand<TResponse> is a generic interface that represents a command in the CQRS pattern, which returns a response of type TResponse.
    // It is used to encapsulate a request to perform an action or change the state of the system and return a result.
    // The TResponse type parameter allows for flexibility in defining the expected response type for different commands. 
    // The 'out' keyword indicates that TResponse is covariant, meaning it can be used as a return type in derived interfaces or classes.
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
