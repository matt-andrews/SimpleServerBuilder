using ServerBuilder;

namespace ExampleNetModels.Models
{
    public class CreateGameModel : BaseModel<ModelType>
    {
        public override ModelType ModelType => ModelType.CreateGame;
        public bool IsAi { get; set; }
    }
}
