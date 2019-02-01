using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace WebAppIISNginx.Middleware
{
    public class RequestCacheViewMiddleware
    {
        private readonly RequestDelegate _next;
        public IConfiguration Configuration { get; }

        public RequestCacheViewMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            Configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var hasExpire = int.TryParse(Configuration["html_cache_expire_time"], out var expire);
            if (hasExpire && expire > 0)
            {
                var url = context.Request.Path.ToString();
                var th = new MD5CryptoServiceProvider();
                //对特定的请求的url做缓存,特殊字符用md5处理一下了,这样相同的请求url就会返回相同的静态文件里的内容。
                var data = th.ComputeHash(Encoding.Unicode.GetBytes(url));
                var key = Convert.ToBase64String(data, Base64FormattingOptions.None);
                var path = HttpUtility.UrlEncode(key);
                //算法在超时时间内都能产生相同的文件名，判断是否存在这个文件。
                var timeTicks = new DateTime(DateTime.Now.Ticks / 10000000 / expire * 10000000 * expire);

                const string filePath = "static/cache/";
                var fileName = path + "." + timeTicks.ToString("yyyyMMddHHmmss") + ".html";
                var fullPath = Path.Combine(filePath, fileName);

                if (File.Exists(fullPath))
                {
                    await context.Response.SendFileAsync(fullPath);
                }
                else
                {
                    //获取响应体的引用
                    var originalBody = context.Response.Body;

                    try
                    {
                        //因为响应体实例是只读的，需要创建一个内存流实例，用来获取响应流内容
                        using (var memStream = new MemoryStream())
                        {
                            //把内存流的引用设置到Response.Body，假装是真的响应体接收数据
                            context.Response.Body = memStream;
                            //然后执行下一个中间件，等待有响应返回
                            // Call the next delegate/middleware in the pipeline
                            await _next(context);
                            //需要判断响应是否正确，总不能把不正确的内容缓存起来吧
                            if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                            {
                                //检测文件存放目录
                                if (!Directory.Exists(filePath))
                                    Directory.CreateDirectory(filePath);
                                //把内存流的当前操作位置设为0，因为响应写入的过程中位置会在末尾。
                                memStream.Position = 0;
                                //读取内存流的内容，转换为字符串
                                var responseBody = new StreamReader(memStream).ReadToEnd();
                                //把字符串写入文件，这里还稍微压缩了一下
                                await File.WriteAllTextAsync(fullPath, Regex.Replace(responseBody, "\\n+\\s+", string.Empty));
                                //在此把内存流的当前操作位置设为0
                                memStream.Position = 0;
                                //还需要把流复制到之前引用的响应体实例
                                await memStream.CopyToAsync(originalBody);
                            }
                        }
                    }
                    finally
                    {
                        //把响应体实例引用重新设置到响应体
                        context.Response.Body = originalBody;
                    }
                }
            }
        }
    }
}
