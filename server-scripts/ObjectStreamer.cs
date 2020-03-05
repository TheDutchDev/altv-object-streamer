using AltV.Net;
using AltV.Net.EntitySync;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DasNiels.AltV.Streamers
{
    public enum TextureVariation
    {
        Pacific = 0,
        Azure = 1,
        Nautical = 2,
        Continental = 3,
        Battleship = 4,
        Intrepid = 5,
        Uniform = 6,
        Classico = 7,
        Mediterranean = 8,
        Command = 9,
        Mariner = 10,
        Ruby = 11,
        Vintage = 12,
        Pristine = 13,
        Merchant = 14,
        Voyager = 15
    }

    public class MoveData : IWritable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Speed { get; set; }
        public void OnWrite( IMValueWriter writer )
        {
            writer.BeginObject( );
            writer.Name( "X" );
            writer.Value( X );
            writer.Name( "Y" );
            writer.Value( "Y" );
            writer.Name( "Z" );
            writer.Value( Z );
            writer.Name( "Speed" );
            writer.Value( Speed );
            writer.EndObject( );
        }
    }

    public class Rgb : IWritable
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public Rgb( int red, int green, int blue )
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public void OnWrite( IMValueWriter writer )
        {
            writer.BeginObject( );
            writer.Name( "Red" );
            writer.Value( Red );
            writer.Name( "Green" );
            writer.Value( Green );
            writer.Name( "Blue" );
            writer.Value( Blue );
            writer.EndObject( );
        }
    }

    /// <summary>
    /// DynamicObject class that stores all data related to a single object
    /// </summary>
    public class DynamicObject : Entity, IEntity
    {
        public string EntityType
        {
            get
            {
                if( !TryGetData( "entityType", out string type ) )
                    return null;

                return type;
            }
            set
            {
                // No data changed
                if( EntityType == value )
                    return;

                SetData( "entityType", value );
            }
        }

        /// <summary>
        /// Set or get the current object's rotation (in degrees).
        /// </summary>
        public Vector3 Rotation
        {
            get
            {
                if( !TryGetData( "rotation", out Dictionary<string, object> data ) )
                    return default;

                return new Vector3( )
                {
                    X = Convert.ToSingle( data[ "x" ] ),
                    Y = Convert.ToSingle( data[ "y" ] ),
                    Z = Convert.ToSingle( data[ "z" ] ),
                };
            }
            set
            {
                // No data changed
                if( Rotation != null && Rotation.X == value.X && Rotation.Y == value.Y && Rotation.Z == value.Z && value != new Vector3( 0, 0, 0 ) )
                    return;

                Dictionary<string, object> dict = new Dictionary<string, object>( )
                {
                    [ "x" ] = value.X,
                    [ "y" ] = value.Y,
                    [ "z" ] = value.Z,
                };
                SetData( "rotation", dict );
            }
        }

        /// <summary>
        /// Set or get the current object's model.
        /// </summary>
        public string Model
        {
            get
            {
                if( !TryGetData( "model", out string model ) )
                    return null;

                return model;
            }
            set
            {
                // No data changed
                if( Model == value )
                    return;

                SetData( "model", value );
            }
        }

        /// <summary>
        /// Set or get LOD Distance of the object.
        /// </summary>
        public uint? LodDistance
        {
            get
            {
                if( !TryGetData( "lodDistance", out uint lodDist ) )
                    return null;

                return lodDist;
            }
            set
            {
                // if value is set to null, reset the data
                if( value == null )
                {
                    SetData( "lodDistance", null );
                    return;
                }

                // No data changed
                if( LodDistance == value )
                    return;

                SetData( "lodDistance", value );
            }
        }

        /// <summary>
        /// Get or set the current texture variation, use null to reset it to default.
        /// </summary>
        public TextureVariation? TextureVariation
        {
            get
            {
                if( !TryGetData( "textureVariation", out int variation ) )
                    return null;

                return ( TextureVariation ) variation;
            }
            set
            {
                // if value is set to null, reset the data
                if( value == null )
                {
                    SetData( "textureVariation", null );
                    return;
                }

                // No data changed
                if( TextureVariation == value )
                    return;

                SetData( "textureVariation", ( int ) value );
            }
        }

        /// <summary>
        /// Get or set the object's dynamic state. Some objects can be moved around by the player when dynamic is set to true.
        /// </summary>
        public bool? Dynamic
        {
            get
            {
                if( !TryGetData( "dynamic", out bool isDynamic ) )
                    return false;

                return isDynamic;
            }
            set
            {
                // if value is set to null, reset the data
                if( value == null )
                {
                    SetData( "dynamic", null );
                    return;
                }

                // No data changed
                if( Dynamic == value )
                    return;

                SetData( "dynamic", value );
            }
        }

        /// <summary>
        /// Set/get visibility state of object
        /// </summary>
        public bool? Visible
        {
            get
            {
                if( !TryGetData( "visible", out bool visible ) )
                    return false;

                return visible;
            }
            set
            {
                // if value is set to null, reset the data
                if( value == null )
                {
                    SetData( "visible", null );
                    return;
                }

                // No data changed
                if( Visible == value )
                    return;

                SetData( "visible", value );
            }
        }

        /// <summary>
        /// Set/get an object on fire, NOTE: does not work very well as of right now, fire is very small.
        /// </summary>
        public bool? OnFire
        {
            get
            {
                if( !TryGetData( "onFire", out bool onFire ) )
                    return false;

                return onFire;
            }
            set
            {
                // if value is set to null, reset the data
                if( value == null )
                {
                    SetData( "onFire", null );
                    return;
                }

                // No data changed
                if( OnFire == value )
                    return;

                SetData( "onFire", value );
            }
        }

        /// <summary>
        /// Freeze an object into it's current position. or get it's status
        /// </summary>
        public bool? Frozen
        {
            get
            {
                if( !TryGetData( "frozen", out bool frozen ) )
                    return false;

                return frozen;
            }
            set
            {
                // if value is set to null, reset the data
                if( value == null )
                {
                    SetData( "frozen", null );
                    return;
                }

                // No data changed
                if( Frozen == value )
                    return;

                SetData( "frozen", value );
            }
        }

        /// <summary>
        /// Set the light color of the object, use null to reset it to default.
        /// </summary>
        public Rgb LightColor
        {
            get
            {
                if( !TryGetData( "lightColor", out Dictionary<string, object> data ) )
                    return null;

                return new Rgb(
                    Convert.ToInt32( data[ "r" ] ),
                    Convert.ToInt32( data[ "g" ] ),
                    Convert.ToInt32( data[ "b" ] )
                );
            }
            set
            {
                // if value is set to null, reset the data
                if( value == null )
                {
                    SetData( "lightColor", null );
                    return;
                }

                // No data changed
                if( LightColor != null && LightColor.Red == value.Red && LightColor.Green == value.Green && LightColor.Blue == value.Blue )
                    return;

                Dictionary<string, object> dict = new Dictionary<string, object>
                {
                    { "r", value.Red },
                    { "g", value.Green },
                    { "b", value.Blue }
                };
                SetData( "lightColor", dict );
            }
        }

        public DynamicObject( Vector3 position, int dimension, uint range, string entityType ) : base( 0, position, dimension, range )
        {
            EntityType = entityType;
        }

        public void Destroy( )
        {
            AltEntitySync.RemoveEntity( this );
        }
    }


    public static class ObjectStreamer
    {
        /// <summary>
        /// Create a new dynamic object.
        /// </summary>
        /// <param name="model">The object model name.</param>
        /// <param name="position">The position to spawn the object at.</param>
        /// <param name="rotation">The rotation to spawn the object at(degrees).</param>
        /// <param name="dimension">The dimension to spawn the object in.</param>
        /// <param name="isDynamic">(Optional): Set object dynamic or not.</param>
        /// <param name="frozen">(Optional): Set object frozen.</param>
        /// <param name="lodDistance">(Optional): Set LOD distance.</param>
        /// <param name="lightColor">(Optional): set light color.</param>
        /// <param name="onFire">(Optional): set object on fire(DOESN'T WORK PROPERLY YET!)</param>
        /// <param name="textureVariation">(Optional): Set object texture variation.</param>
        /// <param name="visible">(Optional): Set object visibility.</param>
        /// <param name="streamRange">(Optional): The range that a player has to be in before the object spawns, default value is 400.</param>
        /// <returns>The newly created dynamic object.</returns>
        public static DynamicObject CreateDynamicObject(
            string model, Vector3 position, Vector3 rotation, int dimension = 0, bool? isDynamic = null, bool? frozen = null, uint? lodDistance = null,
            Rgb lightColor = null, bool? onFire = null, TextureVariation? textureVariation = null, bool? visible = null, uint streamRange = 400
        )
        {
            DynamicObject obj = new DynamicObject( position, dimension, streamRange, "object" )
            {
                Rotation = rotation,
                Model = model,
                Dynamic = isDynamic ?? null,
                Frozen = frozen ?? null,
                LodDistance = lodDistance ?? null,
                LightColor = lightColor ?? null,
                OnFire = onFire ?? null,
                TextureVariation = textureVariation ?? null,
                Visible = visible ?? null
            };

            AltEntitySync.AddEntity( obj );
            return obj;
        }

        /// <summary>
        /// Destroy a dynamic object by it's ID.
        /// </summary>
        /// <param name="dynamicObjectId">The ID of the object.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool DestroyDynamicObject( ulong dynamicObjectId )
        {
            DynamicObject obj = GetDynamicObject( dynamicObjectId );

            if( obj == null )
                return false;

            AltEntitySync.RemoveEntity( obj );
            return true;
        }

        /// <summary>
        /// Destroy a dynamic object.
        /// </summary>
        /// <param name="obj">The object instance to destroy</param>
        /// <returns></returns>
        public static void DestroyDynamicObject( DynamicObject obj )
        {
            AltEntitySync.RemoveEntity( obj );
        }

        /// <summary>
        /// Get a dynamic object by it's ID.
        /// </summary>
        /// <param name="dynamicObjectId">The ID of the object.</param>
        /// <returns>The dynamic object or null if not found.</returns>
        public static DynamicObject GetDynamicObject( ulong dynamicObjectId )
        {
            if( !AltEntitySync.TryGetEntity( dynamicObjectId, out IEntity entity ) )
            {
                Console.WriteLine( $"[OBJECT-STREAMER] [GetDynamicObject] ERROR: Entity with ID { dynamicObjectId } couldn't be found." );
                return default;
            }

            if( !( entity is DynamicObject ) )
                return default;

            return ( DynamicObject ) entity;
        }

        /// <summary>
        /// Destroy all created dynamic objects.
        /// </summary>
        public static void DestroyAllDynamicObjects( )
        {
            foreach( DynamicObject obj in GetAllDynamicObjects( ) )
            {
                AltEntitySync.RemoveEntity( obj );
            }
        }

        /// <summary>
        /// Get all created dynamic objects.
        /// </summary>
        /// <returns>A list of dynamic objects.</returns>
        public static List<DynamicObject> GetAllDynamicObjects( )
        {
            List<DynamicObject> objects = new List<DynamicObject>( );

            foreach( IEntity entity in AltEntitySync.GetAllEntities( ) )
            {
                DynamicObject obj = GetDynamicObject( entity.Id );

                if( obj != null )
                    objects.Add( obj );
            }

            return objects;
        }

        /// <summary>
        /// Get the dynamic object that's closest to a specified position.
        /// </summary>
        /// <param name="pos">The position from which to check.</param>
        /// <returns>The closest dynamic object to the specified position, or null if none found.</returns>
        public static (DynamicObject obj, float distance) GetClosestDynamicObject( Vector3 pos )
        {
            if( GetAllDynamicObjects( ).Count == 0 )
                return (null, 5000);

            DynamicObject obj = null;
            float distance = 5000;

            foreach( DynamicObject o in GetAllDynamicObjects( ) )
            {
                float dist = Vector3.Distance( o.Position, pos );
                if( dist < distance )
                {
                    obj = o;
                    distance = dist;
                }
            }

            return (obj, distance);
        }
    }
}
