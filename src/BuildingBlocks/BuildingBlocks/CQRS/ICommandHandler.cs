using MediatR;

namespace BuildingBlocks.CQRS
{
    // ICommandHandler<TCommand, Unit> is an interface that defines a handler for commands that do not return a value.
    // It inherits from IRequestHandler<TCommand, Unit> and is used to handle commands that implement the ICommand<Unit> interface.
    // TCommand is the type of the command that the handler will process, and it must implement the ICommand<Unit> interface.
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
        where TCommand : ICommand<Unit>
    {
    }
    // ICommandHandler is an interface that defines a contract for handling commands in a CQRS (Command Query Responsibility Segregation) architecture.
    // It extends the IRequestHandler interface from MediatR, which is a library for implementing the Mediator pattern in .NET applications.
    // The ICommandHandler interface is generic and takes two type parameters: TCommand and TResponse.
    // TCommand represents the type of command that the handler will process, and it must implement the ICommand<TResponse> interface.
    // TResponse represents the type of response that the handler will return after processing the command, and it must be a non-nullable type.
    // <in TCommand> indicates that the TCommand type parameter is contravariant, meaning it can accept a more derived type than originally specified. This allows for greater flexibility when implementing command handlers.
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> 
        where TCommand : ICommand<TResponse> 
        where TResponse : notnull
    {
    }
}
