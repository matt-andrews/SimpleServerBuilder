using ServerBuilder;

namespace ExampleNetModels.Models
{
    public class UpdateGameStateModel : BaseModel<ModelType>
    {
        public override ModelType ModelType => ModelType.UpdateGameState;
        public int Turn { get; set; }
        public string[] Squares { get; set; }
        public int Winner { get; set; }
        public bool Complete { get; set; }
    }
}
