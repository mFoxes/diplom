using System.Net;
using DTO;
using FluentValidation;
using MediatR;

namespace GrandmaApi.Mediatr.Pipelines;

public class BrokerMessagesValidation<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : BrokerCommandResponse, new()

{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public BrokerMessagesValidation(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationResults =
            await Task.WhenAll(_validators.Select(async v => await v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .SelectMany(v => v.Errors)
            .Where(v => v != null)
            .Select(v => new ErrorDto()
            {
                FieldName = v.PropertyName,
                Message = v.ErrorMessage
            }).ToList();
        if (failures.Any())
        {
            return new TResponse()
            {
                IsSucceed = false,
                Errors = failures
            };
        }

        return await next();
    }
}