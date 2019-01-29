using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace RoutingBasics
{
    public class Startup
    {
        private readonly IList<Widget> _widgets;

        public Startup()
        {
            _widgets = new List<Widget>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRouting(cfg =>
            {
                cfg.MapGet("/widgets", ctx => ctx.Response.WriteAsync(JsonConvert.SerializeObject(_widgets)));
                cfg.MapGet("/widgets/{id:int}", ctx =>
                {
                    var routeValue = ctx.GetRouteValue("id") as string;
                    var id = Int32.Parse(routeValue);

                    try
                    {
                        var widget = _widgets.First(x => x.Id == id);
                        return ctx.Response.WriteAsync(JsonConvert.SerializeObject(widget));
                    }
                    catch (InvalidOperationException)
                    {
                        ctx.Response.StatusCode = 404;
                        return Task.CompletedTask;
                    }
                });
                cfg.MapPost("/widgets", ctx =>
                {
                    using (var reader = new StreamReader(ctx.Request.Body))
                    {
                        var body = reader.ReadToEnd();
                        var widget = JsonConvert.DeserializeObject<Widget>(body);

                        if (string.IsNullOrEmpty(widget.Description))
                        {
                            ctx.Response.StatusCode = 422;
                            return ctx.Response.WriteAsync("Description is required");
                        }

                        if (_widgets.Any())
                        {
                            var maxId = _widgets.Max(w => w.Id);
                            widget.Id = maxId + 1;
                        }
                        else
                        {
                            widget.Id = 1;
                        }

                        _widgets.Add(widget);

                        var uriBuilder = new UriBuilder(
                            ctx.Request.Scheme, 
                            ctx.Request.Host.Host, 
                            ctx.Request.Host.Port.Value,
                            $"{ctx.Request.Path}/{widget.Id}");
                        
                        ctx.Response.Headers.Add("Location", uriBuilder.Uri.AbsoluteUri);
                        ctx.Response.StatusCode = 201;
                        return ctx.Response.WriteAsync(JsonConvert.SerializeObject(widget));
                    }
                });
                cfg.MapDelete("/widgets/{id:int}", ctx =>
                {
                    var routeValue = ctx.GetRouteValue("id") as string;
                    var id = Int32.Parse(routeValue);

                    try
                    {
                        var widget = _widgets.First(x => x.Id == id);
                        _widgets.Remove(widget);
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    ctx.Response.StatusCode = 204;
                    return Task.CompletedTask;
                });
            });
        }
    }
}