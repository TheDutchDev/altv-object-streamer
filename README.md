# ALT:V MP Server-side Object Streamer
A server-side C# implementation of an object streamer for ALT:V MP.

## Installation
- This resource makes use of the ``AltV.Net.EntitySync`` and ``AltV.Net.EntitySync.ServerEvent`` nuget package, make sure to install those prior to using this resource.
- Copy ``server-scripts/ObjectStreamer.cs`` to your gamemode.
- Make sure to add the following code to your gamemode's OnStart() method(the object streamer won't work without it!):
```csharp
AltEntitySync.Init( 1, 100,
   repository => new ServerEventNetworkLayer( repository ),
   ( ) => new LimitedGrid3( 50_000, 50_000, 100, 10_000, 10_000, 600 ),
   new IdProvider( ) // Documentation: https://fabianterhorst.github.io/coreclr-module/articles/entity-sync.html
);
```
- Copy ``object-streamer-client`` to your ``server-root/resources`` directory.
- Add ``"object-streamer-client"`` to your server config resources list.

## Usage
The following global methods are available:
```csharp
// Create a new object on the map, returns the created object.
DynamicObject CreateDynamicObject( string model, Vector3 position, Vector3 rotation, int dimension = 0, string type = "object", uint streamRange = 400 );

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
// Get object's current rotation in degrees.
Vector3 GetRotation( );

// Set the object's rotation, returns true if successful.
bool SetRotation( Vector3 newRot );

// Change an object's model, returns true if successful.
bool SetModel( string newModel );

// Change the object's position.
void SetPosition( Vector3 newPos );
```

## Examples
```csharp
// Create an object.
DynamicObject obj = ObjectStreamer.CreateDynamicObject( "bkr_prop_biker_bblock_cor", new Vector3( -859.655f, -803.499f, 25.566f ), new Rotation( 0, 0, 0 ), 0 );

// Change object into a house.
obj.SetModel( "lf_house_17_" );

// Change position.
obj.SetPosition( new Position( 300f, 500f, 25f ) ); // Accepts both Vector3 and Position types.

// Change rotation.
obj.SetRotation( new Rotation( 0f, 0f, 25f ) ); // Accepts both Vector3 and Rotation types.

// Destroy the object
ObjectStreamer.DestroyDynamicObject( obj ); // has an overload method that accepts an ID instead of object instance.
```

Furthermore, there's an example C# file included in the package, the example file can be found at ``server-scripts/ExampleServer.cs``.

## Future plans
- ``MoveDynamicObject( ulong objectId, Vector3 newPosition );``
- ``DestroyWorldPropAtCoords( string model, Vector3 position, float range )``