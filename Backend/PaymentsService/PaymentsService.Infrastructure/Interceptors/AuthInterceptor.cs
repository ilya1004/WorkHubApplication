using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace PaymentsService.Infrastructure.Interceptors;

public class AuthInterceptor(IHttpContextAccessor httpContextAccessor) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        
        if (!string.IsNullOrEmpty(token))
        {
            var headers = context.Options.Headers ?? [];
            headers.Add("Authorization", token);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, context.Options.WithHeaders(headers));
        }

        return continuation(request, context);
    }
}