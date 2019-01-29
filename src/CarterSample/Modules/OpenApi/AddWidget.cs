using System;
using Carter;

namespace CarterSample.Modules.OpenApi
{
    public class AddWidget : RouteMetaData
    {
        public override string Description { get; } = "Create a widget in the system";

        public override Type Request { get; } = typeof(Widget);

        public override RouteMetaDataResponse[] Responses { get; } = { new RouteMetaDataResponse { Code = 201, Description = "Created Widget" } };

        public override string Tag { get; } = "Widgets";
    }
}
