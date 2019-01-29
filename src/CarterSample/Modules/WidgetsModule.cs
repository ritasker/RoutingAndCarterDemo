using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Carter;
using Carter.ModelBinding;
using Carter.Request;
using Carter.Response;
using CarterSample.Modules.OpenApi;

namespace CarterSample.Modules
{
    public class WidgetsModule : CarterModule
    {
        private static readonly List<Widget> Widgets = new List<Widget>();
        
        public WidgetsModule()
        {
            Get<GetWidgets>("/widgets", (req, res, routeData) => res.Negotiate(Widgets));
            
            Get<GetWidgetById>("/widgets/{id:int}", (req, res, routeData) =>
            {
                var id = routeData.As<int>("id");

                try
                {
                    var widget = Widgets.First(x => x.Id == id);
                    return res.Negotiate(widget);
                }
                catch (InvalidOperationException)
                {
                    res.StatusCode = 404;
                    return Task.CompletedTask;
                }
            });
            Post<AddWidget>("/widgets", (req, res, routeData) =>
            {
                var result = req.BindAndValidate<Widget>();

                if (result.ValidationResult.IsValid)
                {
                    var widget = result.Data;
                    if (Widgets.Any())
                    {
                        var maxId = Widgets.Max(w => w.Id);
                        widget.Id = maxId + 1;
                    }
                    else
                    {
                        widget.Id = 1;
                    }

                    Widgets.Add(widget);

                    var uriBuilder = new UriBuilder(
                        req.Scheme, 
                        req.Host.Host, 
                        req.Host.Port.Value,
                        $"{req.Path}/{widget.Id}");
                        
                    res.Headers.Add("Location", uriBuilder.Uri.AbsoluteUri);
                    res.StatusCode = 201;
                    return res.Negotiate(widget);
                }

                res.StatusCode = (int) HttpStatusCode.UnprocessableEntity;
                return res.Negotiate(result.ValidationResult.GetFormattedErrors());
            });
        }
    }
}