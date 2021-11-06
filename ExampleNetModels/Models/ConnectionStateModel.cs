using ServerBuilder;

namespace ExampleNetModels.Models
{
    public class ConnectionStateModel : BaseModel<ModelType>
    {
        public override ModelType ModelType => ModelType.ConnectionState;
        public bool IsConnected { get; set; }
    }
}
