using Carter;

namespace CarterSample.Modules.OpenApi
{
    public class GetWidgetById : RouteMetaData
    {
        public override string Description { get; } = "Gets a widget by it's id";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = 200, Description = $"A {nameof(Widget)}",
                Response = typeof(Widget)
            },
            new RouteMetaDataResponse
            {
                Code = 404,
                Description = $"{nameof(Widget)} not found"
            }
        };

        public override string Tag { get; } = "Widgets";
    }
}
