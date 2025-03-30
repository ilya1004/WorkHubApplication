using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ProjectsService.API.Interceptors;

public class GrpcLoggingInterceptor(ILogger<GrpcLoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var response = await continuation(request, context);
        
        stopwatch.Stop();

        logger.LogInformation("Completed gRPC call {MethodName} in {ElapsedMs}ms with status {StatusCode}",
            context.Method, stopwatch.ElapsedMilliseconds, context.Status.StatusCode);

        return response;
    }
}