using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BiographyWebApp.Services.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var task = base.OnExceptionAsync(context);
            return task.ContinueWith(t =>
            {
                string infoAction = context.ActionDescriptor.DisplayName;
                string infoStack = context.Exception.StackTrace;
                string infoException = context.Exception.Message;
                string info = $"Method {infoAction} cracked with {infoException}\n{infoStack}";

                Console.WriteLine(info);

                context.Result = new ViewResult()
                {
                    ViewName = "Error"
                };
            });
        }
    }
}
