using ServerBuilder;

namespace ExampleNetModels.Models
{
    public class PlaceMarkerModel : BaseModel<ModelType>
    {
        public override ModelType ModelType => ModelType.PlaceMarker;
        public long GameId { get; set; }
        public string PlayerId { get; set; }
        public int Place { get; set; }
    }
}
