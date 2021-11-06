using ServerBuilder;

namespace ExampleNetModels.Models
{
    public class GameCreatedModel : BaseModel<ModelType>
    {
        public override ModelType ModelType => ModelType.GameCreated;
        public int GameId { get; set; }
        public int PlayerTeam { get; set; }
        public int Turn { get; set; }
        public string PlayerId { get; set; }
    }
}
