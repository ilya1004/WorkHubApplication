using Grpc.Core;
using Grpc.Core.Interceptors;
using ProjectsService.Application.Exceptions;

namespace ProjectsService.API.Interceptors;

public class ErrorHandlingInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                BadRequestException => StatusCode.InvalidArgument,
                AlreadyExistsException => StatusCode.AlreadyExists,
                NotFoundException => StatusCode.NotFound,
                UnauthorizedException => StatusCode.Unauthenticated,
                ForbiddenException => StatusCode.PermissionDenied,
                _ => StatusCode.Internal
            };
            
            var status = new Status(statusCode, ex.Message, ex.InnerException);
            
            throw new RpcException(status);
        }
    }
}