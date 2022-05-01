using Elastic.Apm.Api;
using MediatR;
using System.Diagnostics;
using System.Reflection;

namespace ECom.Expansion.Service.Utils
{
    public class ApmTraceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        [DebuggerStepThrough]
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            MemberInfo memberInfo = typeof(TRequest);

            var hasApmTraceAttrib = memberInfo
                .GetCustomAttributes(typeof(ApmTraceAttribute), false)
                .Length > 0;

            var commandName = typeof(TRequest).Name;
            ISpan commandSpan = null;

            if (hasApmTraceAttrib)
            {
                var span = Elastic.Apm.Agent.Tracer.CurrentSpan;
                if (span != null)
                {
                    commandSpan = span.StartSpan(commandName, "span");
                }
                else
                {
                    var transaction = Elastic.Apm.Agent.Tracer.CurrentTransaction;
                    if (transaction != null)
                    {
                        commandSpan = transaction.StartSpan(commandName, "span");
                    }
                }
            }

            try
            {
                return await next();
            }
            finally
            {
                commandSpan?.End();
            }
        }
    }
}
