# ALT:V MP Server-side Object Streamer
A server-side C# implementation of an object streamer for ALT:V MP.

## Installation
- This resource makes use of the ``AltV.Net.EntitySync`` and ``AltV.Net.EntitySync.ServerEvent`` nuget package, make sure to install those prior to using this resource.
- Copy ``server-scripts/ObjectStreamer.cs`` to your gamemode.
- Make sure to add the following code to your gamemode's OnStart() method(the object streamer won't work without it!):
```csharp
// Documentation: https://fabianterhorst.github.io/coreclr-module/articles/entity-sync.html
AltEntitySync.Init( 1, 100,
   repository => new ServerEventNetworkLayer( repository ),
   ( ) => new LimitedGrid3( 50_000, 50_000, 100, 10_000, 10_000, 600 ),
   new IdProvider( )
);
```
- Copy ``object-streamer-client`` to your ``server-root/resources`` directory.
- Add ``"object-streamer-client"`` to your server config resources list.

## Usage
The following global methods are available:
```csharp
// Create a new object on the map, returns the created object.
DynamicObject CreateDynamicObject( 
    string model, Vector3 position, Vector3 rotation, int dimension = 0, bool? isDynamic = null, bool? frozen = null, uint? lodDistance = null, 
    Rgb lightColor = null, bool? onFire = null, TextureVariation? textureVariation = null, bool? visible = null, string type = "object", uint streamRange = 400 
);

// Destroy an object by it's ID or object instance. returns true if successful.
bool DestroyDynamicObject( ulong dynamicObjectId );
bool DestroyDynamicObject( DynamicObject obj );

// Get an object by it's ID. returns the object if successful or null if not.
DynamicObject GetDynamicObject( ulong dynamicObjectId );

// Destroy all created objects.
void DestroyAllDynamicObjects( );

// Get a list of all created objects.
List<DynamicObject> GetAllDynamicObjects( );
```

Each object has it's own set of methods that can be used to change properties:
```csharp
// Get/set object's rotation
Vector3 Rotation { get; set; }

// Get/set object's position
Vector3 Position { get; set; }

// Get/set object's model
string Model { get; set; }

// Get/set LOD Distance (default null)
uint? LodDistance { get; set; }

// Get/set object's texture variation (default null)
TextureVariation? TextureVariation { get; set; }

// Get/set object's dynamic state (default null)
bool? Dynamic { get; set; }

// Get/set object's visibility state (default null) 
bool? Visible { get; set; }

// Get/set object's on fire state (default null) (don't use this as of right now, it does create a fire but it's very small. requires further native testing).
bool? OnFire { get; set; }

// Get/set object's frozen state (default null)
bool? Frozen { get; set; }

// Get/set object's light color (default null)
Rgb LightColor { get; set; }
```

## Examples
```csharp
// Create an object.
DynamicObject obj = ObjectStreamer.CreateDynamicObject( "bkr_prop_biker_bblock_cor", new Vector3( -859.655f, -803.499f, 25.566f ), new Rotation( 0, 0, 0 ), 0 );

// Change object into a house.
obj.Model = "lf_house_17_";

// Change position.
obj.Position = new Position( 300f, 500f, 25f ); // Accepts both Vector3 and Position types.

// Change rotation.
obj.Rotation = new Rotation( 0f, 0f, 25f ); // Accepts both Vector3 and Rotation types.

// Hide the object
obj.Visible = false;

// Set an object's texture variation
obj.TextureVariation = TextureVariation.Nautical;

// Set an object's light color
obj.LightColor = new Rgb( 25, 49, 120 ); // random

// Freeze an object
obj.Frozen = true;

// Destroy the object
ObjectStreamer.DestroyDynamicObject( obj ); // has an overload method that accepts an ID instead of object instance.
```

Furthermore, there's an example C# file included in the package, the example file can be found at ``server-scripts/ExampleServer.cs``.

## Future plans
- ``MoveDynamicObject( ulong objectId, Vector3 newPosition );``
- ``DestroyWorldPropAtCoords( string model, Vector3 position, float range )``