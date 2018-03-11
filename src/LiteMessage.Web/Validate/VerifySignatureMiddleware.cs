using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace LiteMessage.Web.Controllers
{
    public class VerifySignatureMiddleware
    {
        private readonly ILogger<VerifySignatureMiddleware> _logger;
        private readonly IConfiguration configuration;
        private readonly RequestDelegate _next;


        public VerifySignatureMiddleware(RequestDelegate next,
            ILogger<VerifySignatureMiddleware> logger, IConfiguration configuration)
        {
            _next = next;

            _logger = logger;
            this.configuration = configuration;
        }

        public Task Invoke(HttpContext context)
        {

            if (!context.Request.Headers.ContainsKey("signature"))
            {
                _next(context);
                return Task.CompletedTask;
            }
            try
            {
                ReturnMessage exception;
                if (!validateRequestSignature(context, out exception))
                {
                    context.Response.StatusCode = 400;
                    context.Response.WriteAsync(JsonConvert.SerializeObject(exception));
                    return Task.CompletedTask;
                }


                //处理验证。替换一个可以随意 seek的 memoryStream，代替只能向前的 RessonseStream对象。
                var bodyStream = context.Response.Body;
                var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;


                // Call the next delegate/middleware in the pipeline
                var result = _next(context);
                //处理返回值签名
                //if (context.Response.Headers.ContainsKey("signature"))
                //{
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = new StreamReader(responseBodyStream).ReadToEnd();

                BuildResponse(context, responseBody);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                responseBodyStream.CopyTo(bodyStream);
                return result;
            }

            catch (Exception nottdException)
            {
                context.Response.StatusCode = 500;
                context.Response.WriteAsync(JsonConvert.SerializeObject(nottdException));
                return Task.CompletedTask;
            }
        }

        private void BuildResponse(HttpContext context, string responseBody)
        {
            var headers = context.Response.Headers;
            if (headers.ContainsKey("signature"))
                headers["signature"] = BuildSignature(context, responseBody);
            else
                headers.Add("signature", BuildSignature(context, responseBody));
            //}
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="SignatureException"></exception>
        private bool validateRequestSignature(HttpContext context, out ReturnMessage returnMessage)
        {
            returnMessage = null;
            var verifySignature = context.Request.Headers["signature"];
            var requestBody = GetRequestContent(context);
            _logger.LogDebug("接收到数据 {0}", requestBody);
            var validResult = BuildSignature(context, requestBody);
            _logger.LogDebug("接收到数据-服务器签名 {0}，客户端提交签名{1}", validResult, verifySignature);
            if (validResult != verifySignature)
            {

                returnMessage = new ReturnMessage
                {
                    Code = "-20",
                    Message = "sign fail."
                };
                return false;
            }
            context.Items["signatureResult"] = true;
            return true;

        }

        private string BuildSignature(HttpContext context, string signatureData)
        {
            //Todo hard code here.
            return Md5(signatureData + configuration["appKey"]);

        }
        private string Md5(string inputValue)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }

        private string GetRequestContent(HttpContext ctx)
        {
            var request = ctx.Request;
            if (request.Method.ToLower() != "get")
            {
                var buffer = new MemoryStream();

                request.Body.CopyTo(buffer);
                buffer.Position = 0;
                var result = "";
                var reaer = new StreamReader(buffer);
                result = reaer.ReadToEnd();
                buffer.Position = 0;
                request.Body = buffer;
                if (result.Length != 0)
                    return result;
            }

            return request.GetUri().ToString();
        }

        public class ReturnMessage
        {
            public string Code { get; set; }
            public string Message { get; set; }
        }
    }
}