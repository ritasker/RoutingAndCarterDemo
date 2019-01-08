using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace RoutingBasics
{
    public class Startup
    {
        private IList<Widget> _widgets;

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
            var routeBuilder = new RouteBuilder(app);

            routeBuilder.MapGet("/", context => context.Response.WriteAsync("Hi, from AspNet Core Routing!"));
            
            routeBuilder.MapGet("/widgets", context => context.Response.WriteAsync(JsonConvert.SerializeObject(_widgets)));
            routeBuilder.MapGet("/widgets/{id:int}", context =>
            {
                var routeValue = context.GetRouteValue("id") as string;
                var id = Int32.Parse(routeValue);

                try
                {
                    var widget = _widgets.First(x => x.Id == id);
                    return context.Response.WriteAsync(JsonConvert.SerializeObject(widget));
                }
                catch (InvalidOperationException e)
                {
                    context.Response.StatusCode = 404;
                    return context.Response.WriteAsync("Widget Not Found");
                }
            });
            
            routeBuilder.MapPost("/widgets", context =>
            {
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var body = reader.ReadToEnd();
                    var widget = JsonConvert.DeserializeObject<Widget>(body);

                    if (_widgets.Any())
                    {
                        var maxId = _widgets.Max(w => w.Id);
                        widget.Id = maxId+1;
                    }
                    else
                    {
                        widget.Id =1;
                    }
                    
                    _widgets.Add(widget);

                    context.Response.Headers.Add("Location", new Uri($"{context.Request.Scheme}://{context.Request.Host}/{context.Request.Path}/{widget.Id}").AbsoluteUri);
                    context.Response.StatusCode = (int)HttpStatusCode.Created;
                    return context.Response.WriteAsync(JsonConvert.SerializeObject(widget));
                }
            });
            
            
            
            
            var routes = routeBuilder.Build();
            app.UseRouter(routes);
        }
    }

    public class Widget
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
