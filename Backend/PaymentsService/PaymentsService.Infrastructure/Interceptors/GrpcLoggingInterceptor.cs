using Grpc.Core;
using Grpc.Core.Interceptors;

namespace PaymentsService.Infrastructure.Interceptors;

public class GrpcLoggingInterceptor(ILogger<GrpcLoggingInterceptor> logger) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        logger.LogInformation("Starting gRPC call. Method: {MethodType} {MethodName}", 
            context.Method.Type, context.Method.Name);

        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleResponse(call.ResponseAsync, context),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    private async Task<TResponse> HandleResponse<TRequest, TResponse>(Task<TResponse> responseTask, ClientInterceptorContext<TRequest, TResponse> context) 
        where TRequest : class where TResponse : class
    {
        var response = await responseTask;
            
        logger.LogInformation("gRPC call completed successfully. Method: {MethodName}", 
            context.Method.Name);
            
        return response;
    }
}