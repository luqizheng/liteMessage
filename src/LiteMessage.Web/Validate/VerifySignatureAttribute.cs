using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LiteMessage.Web.Controllers
{
    public class SignResponseAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var r = context.HttpContext.Items["signatureResult"];
            if (r == null)
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(new { code = "-19", Message = "api接口需要签名" });
            }
            else if (!(bool)r)
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(new { code = "-22", Message = "签名出错" });
            }
            base.OnActionExecuting(context);
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {

            //创建一个标记，告诉middleware需要填充这个数值
            base.OnActionExecuted(context);
            if (!context.HttpContext.Response.Headers.ContainsKey("signature"))
                context.HttpContext.Response.Headers.Add("signature", "");
        }
    }
}