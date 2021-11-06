# Simple Server Builder

Welcome! Simple Server Builder is small framework built to easily construct and configure game server nodes with UDP, TCP, or Websockets.

### Dependencies
* [LiteNetLib - UDP](https://github.com/RevenantX/LiteNetLib)
* [Telepathy - TCP](https://github.com/vis2k/Telepathy)
* [Nina.WebSockets - Websockets](https://github.com/ninjasource/Ninja.WebSockets)
* [Newtonsoft.Json](https://www.newtonsoft.com/json)
* [YamlDotNet](https://github.com/aaubry/YamlDotNet)

### Solution Structure

* ServerBuilder - this is the main project described in this readme
* TransportLayer - this is a wrapper around the above frameworks
* ExampleGameServer - this is the example game server
* ExampleNetModels - this is a class library used in the example
* example-game-client-reactjs - this is a client example with a react web page

The example is fully functional, run the C# project `ExampleGameServer` then call `npm start` in the `example-game-client-reactjs` directory to try it out

### Implementation

To get started with a server node, create an enum to define your models.
```c#
public enum ModelType
{
    ConnectionState,
    HelloWorld
}
```
Create your ServiceConfig Yaml file, which must include the following properties. [Read more about configuration files here](#config)
```yaml
ConfigName: ServiceConfig
LoggerTitle: Service Config Logger
AssemblyName: Service_Namespace
Servers:
  udp_server:
    Key: somekey
    TransportLayer: 0 #LiteNetLibServer protocol
    MaxUsers: -1
    Port: 8050
    Listener: Service_Namespace.NetListener
    Reconnect: false

```
Create a listener that follows the namespace you defined in your service config. This listener allows you to inject functionality as you need, or can be left empty(Note usage in the ExampleGameServer). You must also define an attribute that inherits `IServerEvent<ModelType>` with your defined model
```c#
class NetListener : ServerEventListener<ModelType>
{
    public override void OnConnectionRequest(string data)
    {
    }
    public override void OnNetworkError(string data)
    {
    }
    public override void OnNetworkLatencyUpdate(int latency)
    {
    }
    public override void OnNetworkReceiveUnconnected(string data)
    {
    }
    public override void OnPeerConnected(TNetPeer peer)
    {
    }
    public override void OnPeerDisconnected(TNetPeer peer, string data)
    {
    }
}
//An attribute to mark your events
public class ServerEvent : Attribute, IServerEvent<ModelType>
{
    public ModelType Type { get; }
    public ServerEvent(NetModels.ModelType type)
    {
        Type = type;
    }
}
```
Then create a basic entry point
```c#
private static EntryPointBuilder<ModelType> _entryPoint;
private const string _serviceConfigPath = "path/to/serviceConfig.yaml";
static void Main(string[] args)
{
    _entryPoint = new ServerBuilder.EntryPoint.EntryPointBuilder<ModelType>(
                _serviceConfigPath);
    _entryPoint.Build();
}
```
If you start the program now it will run, and clients can connect but nothing exciting will happen yet. We need to create some models and events so we can process information! Let's start with some models
```c#
public class ConnectionStateModel : BaseModel<ModelType>
{
    public override ModelType ModelType => ModelType.ConnectionState;
    public bool IsConnected { get; set; }
}
public class HelloWorldModel : BaseModel<ModelType>
{
    public override ModelType ModelType => ModelType.HelloWorld;
    public string Data { get; set; }
}
```
Now we need events that process the data in a model. Note how the events inherit IEventFactory, and are marked with the `ServerEvent` attribute that we defined earlier
```c#
[ServerEvent(ModelType.ConnectionState)]
class ConnectionStateEvent : IEventFactory
{
    public Task Construct(BaseModel baseObj, TNetPeer peer)
    {
        var obj = baseObj.GetJsonObject<ConnectionStateModel>();
        if(obj.IsConnected)
        {
            Console.WriteLine("Connected!");
        }
        else
        {
            Console.WriteLine("Disconnected!");
        }
        return Task.CompletedTask;
    }
}
[ServerEvent(ModelType.HelloWorld)]
class HelloWorldEvent : IEventFactory
{
    public async Task Construct(BaseModel baseObj, TNetPeer peer)
    {
        var obj = baseObj.GetJsonObject<HelloWorldModel>();
        Console.WriteLine($"Client wrote: {obj.Data}");
        peer.Send(obj.JsonSerialize())
        return Task.CompletedTask;
    }
}
```
Et voila! The data from the models gets processed! If your client sends the ConnectionStateModel, the server will write "Connected!" or "Disconnected!" depending on the bool. If the client sends the HelloWorldModel, the server will write the data sent to the console, and send the model back to the client.

From here, when we want to create more events we just need a matching model class that inherits `BaseModel<ModelType>`, a definition in the `ModelType` enum, and an event class.

### Clients

Your game client can be solved either with any of the above frameworks individually, or for consistency you can use the client factory in TransportLayer
```c#
private IClientManager _manager;
void Start()
{
    _manager = TransportLayer.Factory.BuildClient(data.Ip, data.Port, data.Protocol, listener, data.Key, false);
}
void Update()
{
    _manager.PollEvents();
}
```

### Server 2 Server Connections

Your services can also connect to eachother! This is a persistent way to maintain communication between different services. Just include client configurations in your ServiceConfig with the relevent detals; make sure you create a separate NetListener for client side events!
```yaml
Clients:
  udp_client:
    Key: somekey
    TransportLayer: 3
    Ip: localhost
    Port: 8000
    Listener: Service_Namespace.ClientNetListener
    Reconnect: true
```

### <a name="config"></a> Configuration

**ServiceConfig Setup**

The ServiceConfig file is the main configuration used for defining servers. Let's look at one in depth

```yaml
# ConfigName is required in all configs, it's used for uniqueness and easy lookup
ConfigName: ServiceConfig
# This is the title data for the Logger you chose
LoggerTitle: Service Config Logger
# This is the assembly information where it can find Events marked with the attribute
AssemblyName: Service_Namespace
# This is a collection of servers that will be built
Servers:
  # A name for the server, used for referencing at runtime
  udp_server:
    # A key required by the client to make a connection
    Key: somekey
    # The selected Transport Layer
    TransportLayer: 0
    # The maximum number of users that can connect(-1 is int.MaxValue)
    MaxUsers: -1
    # The port to listen on
    Port: 8050
    # Your custom listener(requires fully qualified namespace)
    Listener: Service_Namespace.NetListener
    # Force reconnect option(only useful on clients)
    Reconnect: false
Clients:
  some_udp_client:
    Key: someotherkey
    TransportLayer: 3 # LiteNetLib-Client
    Ip: localhost
    Port: 8000
    Listener: Service_Namespace.ClientNetListener
    Reconnect: true
```

The selected Transport Layer is the setting that defines the used protocol. It accepts an int which is defined by this Enum
```
#       LiteNetLibServer = 0,
#       TelepathyServer = 1,
#       TelepathyClient = 2,
#       LiteNetLibClient = 3,
#       WebSockets = 4
```

Now that we know how to setup a server, lets look at how to configure it with the various types of build options.

**AddConfigOption with custom Option**
```c#
//Custom config class that has a matching Yaml file to serialize from
public class CustomConfig : IConfig
{
    public string ConfigName { get; set; }
    public string MyApiData { get; set; }
}

//Custom build options when adding these options
public class CustomConfigOptions : IConfigOptions
{
    public void Build<T>(EntryPointBuilder<T> entryPointBuilder, ILogger logger, IConfig configuration)
    {
        //We cast IConfig to our custom config and store its data in as a static variable in the Program Class
        Program.MyStoredApiData = ((CustomConfig)configuration).MyApiData;
    }
}

private static EntryPointBuilder<ModelType> _entryPoint;
private const string _serviceConfigPath = "path/to/serviceConfig.yaml";
public static string MyStoredApiData;
static void Main(string[] args)
{
    _entryPoint = new ServerBuilder.EntryPoint.EntryPointBuilder<ModelType>(
                _serviceConfigPath);
    _entryPoint.AddConfigOption<CustomConfig>(new CustomConfigOptions(), "path/to/customConfig.yaml");
    _entryPoint.Build();
}
```

**AddConfigOption without custom Option**
```c#
//Custom config class that has a matching yaml file to serialize from
public class SimpleConfig : IConfig
{
    public string ConfigName { get; set; }
    public int SomeUsefulInt { get; set; }
}

//here we will add it into the previous entry point
private static EntryPointBuilder<ModelType> _entryPoint;
private const string _serviceConfigPath = "path/to/serviceConfig.yaml";
public static string MyStoredApiData;
static void Main(string[] args)
{
    _entryPoint = new ServerBuilder.EntryPoint.EntryPointBuilder<ModelType>(
                _serviceConfigPath);
    _entryPoint.AddConfigOption<CustomConfig>(new CustomConfigOptions(), "path/to/customConfig.yaml");
    _entryPoint.AddConfigOption<SimpleConfig>("path/to/simpleConfig.yaml");
    _entryPoint.Build();
    
    //Now you can access SimpleConfig at a later time by calling ((SimpleConfig)_entryPoint.GetConfig("SimpleConfig"));
    //The string used is the string defined in Yaml as the property `ConfigName`
}
```

**AddSetupOption and AddPostBuildSetupOption**

These options allow you to add a build script without a config file. AddPostBuild happens after `.Build()` is executed
```c#
class Setup : ISetup
{
    public void Invoke()
    {
        //do some kind of setup code here
    }
}
class PostBuildSetup : ISetup
{
    public void Invoke()
    {
        //do some kind of post build setup here
    }
}
static void Main(string[] args)
{
    _entryPoint...
    _entryPoint.AddSetupOption(new Setup());
    _entryPoint.AddPostBuildSetupOption(new PostBuildSetup());
    _entryPoint.Build();
}
```

Note: All configuration must be defined before `.Build()` is called

### Execution Order
This is the basic execution order of events when creating an entry point

* Service is deserialized and set upon creation with the constructor
* All `Config` Options are deserialized and Options are executed when added
* When `.Build()` is called, all `Setup` classes are invoked
* Then the main Service is executed with the base options for starting the service
* After, all `PostBuildSetup` classes are invoked

The execution is important because if your setup requires getting data from the service after it starts, you *must* use Post Build

### History

While developing my mobile online RPG - [Rite of Kings](https://www.riteofkings.com) I ran into a situation where it would be beneficial to be able to quickly change protocols, or listen to different protocals at the same time. This resulted in an excessive amount of boiler plate that quickly became unmanageable. This framework solves that problem in an easy to remember format with very scalable configuration.