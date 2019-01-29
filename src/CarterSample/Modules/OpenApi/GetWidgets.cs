using System.Collections.Generic;
using Carter;

namespace CarterSample.Modules.OpenApi
{
    public class GetWidgets : RouteMetaData
    {
        public override string Description { get; } = "Returns a list of widgets";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = 200,
                Description = $"A list of {nameof(Widget)}s",
                Response = typeof(IEnumerable<Widget>)
            }
        };

        public override string Tag { get; } = "Widgets";
    }
}
