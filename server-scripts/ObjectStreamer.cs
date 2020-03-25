using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.EntitySync;
using DasNiels.AltV.Streamers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using IEntity = AltV.Net.EntitySync.IEntity;

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

    public static class ObjectStreamer
    {
        // Only change this if you know what you're doing.
        public static ulong ENTITY_TYPE_OBJECT = 0;
        public static ulong ENTITY_TYPE_REMOVED_OBJECT = 1;

        // Dictionary to hold currently moving objects, don't edit this!
        public static Dictionary<ulong, TaskCompletionSource<bool>> MovingObjects { get; set; } = new Dictionary<ulong, TaskCompletionSource<bool>>( );

        /// <summary>
        /// Create a new dynamic object.
        /// </summary>
        /// <param name="model">The object model hash.</param>
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
        /// <param name="type">(Optional): Type of object, only use this if you want to separate certain object types on a custom client-side based object streamer.</param>
        /// <param name="streamRange">(Optional): The range that a player has to be in before the object spawns, default value is 400.</param>
        /// <returns>The newly created dynamic object</returns>
        public static DynamicObject CreateDynamicObject(
            uint model, Vector3 position, Vector3 rotation, int dimension = 0, bool? isDynamic = null, bool? frozen = null, uint? lodDistance = null,
            Rgb lightColor = null, bool? onFire = null, TextureVariation? textureVariation = null, bool? visible = null, uint streamRange = 400
        )
        {
            DynamicObject obj = new DynamicObject( position, dimension, streamRange )
            {
                Rotation = rotation,
                Model = model,
                Dynamic = isDynamic,
                Frozen = frozen,
                LodDistance = lodDistance,
                LightColor = lightColor,
                OnFire = onFire,
                TextureVariation = textureVariation,
                Visible = visible
            };

            AltEntitySync.AddEntity( obj );
            return obj;
        }

        /// <summary>
        /// Create a new dynamic object of given type.
        /// </summary>
        /// <typeparam name="T">The type of dynamic object to return.</typeparam>
        /// <param name="entity">An instance of the class that extends DynamicObject.</param>
        /// <param name="model">The model of the object in string format.</param>
        /// <param name="rotation">The rotation to spawn the object at(degrees).</param>
        /// <param name="isDynamic">(Optional): Set object dynamic or not.</param>
        /// <param name="frozen">(Optional): Set object frozen.</param>
        /// <param name="lodDistance">(Optional): Set LOD distance.</param>
        /// <param name="lightColor">(Optional): set light color.</param>
        /// <param name="onFire">(Optional): set object on fire(DOESN'T WORK PROPERLY YET!)</param>
        /// <param name="textureVariation">(Optional): Set object texture variation.</param>
        /// <param name="visible">(Optional): Set object visibility.</param>
        /// <returns>Newly created dynamic object of given type.</returns>
        public static T CreateDynamicObject<T>(
            IDynamicObject entity, string model, Vector3 rotation, bool? isDynamic = null, bool? frozen = null, uint? lodDistance = null,
            Rgb lightColor = null, bool? onFire = null, TextureVariation? textureVariation = null, bool? visible = null
        )
        {
            return CreateDynamicObject<T>( entity, Alt.Hash( model ), rotation, isDynamic, frozen, lodDistance, lightColor, onFire, textureVariation, visible );
        }

        /// <summary>
        /// Create a new dynamic object of given type.
        /// </summary>
        /// <typeparam name="T">The type of dynamic object to return.</typeparam>
        /// <param name="entity">An instance of the class that extends DynamicObject.</param>
        /// <param name="model">The model of the object in hash format.</param>
        /// <param name="rotation">The rotation to spawn the object at(degrees).</param>
        /// <param name="dimension">The dimension to spawn the object in.</param>
        /// <param name="isDynamic">(Optional): Set object dynamic or not.</param>
        /// <param name="frozen">(Optional): Set object frozen.</param>
        /// <param name="lodDistance">(Optional): Set LOD distance.</param>
        /// <param name="lightColor">(Optional): set light color.</param>
        /// <param name="onFire">(Optional): set object on fire(DOESN'T WORK PROPERLY YET!)</param>
        /// <param name="textureVariation">(Optional): Set object texture variation.</param>
        /// <param name="visible">(Optional): Set object visibility.</param>
        /// <returns>Newly created dynamic object of given type.</returns>
        public static T CreateDynamicObject<T>(
            IDynamicObject entity, uint model, Vector3 rotation, bool? isDynamic = null, bool? frozen = null, uint? lodDistance = null,
            Rgb lightColor = null, bool? onFire = null, TextureVariation? textureVariation = null, bool? visible = null
        )
        {
            if( !( entity is T ) )
            {
                Console.WriteLine( $"[OBJECT-STREAMER] Given object is of type { entity.GetType( ) } but type { typeof( T ) } was expected." );
                return default;
            }

            entity.Model = model;
            entity.Rotation = rotation;
            entity.Dynamic = isDynamic;
            entity.Frozen = frozen;
            entity.LodDistance = lodDistance;
            entity.LightColor = lightColor;
            entity.OnFire = onFire;
            entity.TextureVariation = textureVariation;
            entity.Visible = visible;

            AltEntitySync.AddEntity( entity );
            return ( T ) entity;
        }

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
        /// <param name="type">(Optional): Type of object, only use this if you want to separate certain object types on a custom client-side based object streamer.</param>
        /// <param name="streamRange">(Optional): The range that a player has to be in before the object spawns, default value is 400.</param>
        /// <returns>The newly created dynamic object</returns>
        public static DynamicObject CreateDynamicObject(
            string model, Vector3 position, Vector3 rotation, int dimension = 0, bool? isDynamic = null, bool? frozen = null, uint? lodDistance = null,
            Rgb lightColor = null, bool? onFire = null, TextureVariation? textureVariation = null, bool? visible = null, uint streamRange = 400
        )
        {
            return CreateDynamicObject( Alt.Hash( model ), position, rotation, dimension, isDynamic, frozen, lodDistance, lightColor, onFire, textureVariation, visible, streamRange );
        }

        /// <summary>
        /// Remove an existing gta world object from the map.
        /// </summary>
        /// <param name="model">The model of the object to remove.</param>
        /// <param name="position">The position at which the object is roughly located.</param>
        /// <param name="radius">The radius in which to search for the object from the given position.</param>
        /// <param name="dimension">The dimension to search.</param>
        /// <param name="streamRange">The streamrange at which the object gets deleted.</param>
        /// <returns>The removed object, calling the Restore method will put the object back on the map.</returns>
        public static RemovedWorldObject RemoveWorldObjectOfTypeAtCoords( string model, Vector3 position, uint radius = 5, int dimension = 0, uint streamRange = 400 )
        {
            return RemoveWorldObjectOfTypeAtCoords( Alt.Hash( model ), position, radius, dimension, streamRange );
        }

        /// <summary>
        /// Remove an existing gta world object from the map.
        /// </summary>
        /// <param name="model">The model hash of the object to remove.</param>
        /// <param name="position">The position at which the object is roughly located.</param>
        /// <param name="radius">The radius in which to search for the object from the given position.</param>
        /// <param name="dimension">The dimension to search.</param>
        /// <param name="streamRange">The streamrange at which the object gets deleted.</param>
        /// <returns>The removed object, calling the Restore method will put the object back on the map.</returns>
        public static RemovedWorldObject RemoveWorldObjectOfTypeAtCoords( uint model, Vector3 position, uint radius = 5, int dimension = 0, uint streamRange = 400 )
        {
            RemovedWorldObject obj = new RemovedWorldObject( model, position, dimension, streamRange, radius );
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
            return DestroyDynamicObject<IDynamicObject>( dynamicObjectId );
        }

        /// <summary>
        /// Destroy a dynamic object by it's ID and type.
        /// </summary>
        /// <param name="dynamicObjectId">The ID of the object.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool DestroyDynamicObject<T>( ulong dynamicObjectId )
        {
            T obj = GetDynamicObject<T>( dynamicObjectId );

            if( obj == null )
                return false;

            AltEntitySync.RemoveEntity( ( IEntity ) obj );
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
        public static IDynamicObject GetDynamicObject( ulong dynamicObjectId )
        {
            return GetDynamicObject<IDynamicObject>( dynamicObjectId );
        }

        /// <summary>
        /// Get a dynamic object by it's ID and type.
        /// </summary>
        /// <param name="dynamicObjectId">The ID of the object.</param>
        /// <returns>The dynamic object or null if not found.</returns>
        public static T GetDynamicObject<T>( ulong dynamicObjectId )
        {
            ulong entityType = ENTITY_TYPE_OBJECT;

            // if it's a removed object to be returned
            if( typeof( RemovedWorldObject ).IsAssignableFrom( typeof( T ) ) )
                entityType = ENTITY_TYPE_REMOVED_OBJECT;

            if( !AltEntitySync.TryGetEntity( dynamicObjectId, entityType, out IEntity entity ) )
            {
                Console.WriteLine( $"[OBJECT-STREAMER] [GetDynamicObject] ERROR: Entity with ID { dynamicObjectId } couldn't be found." );
                return default;
            }

            if( !( entity is T ) )
                return default;

            return ( T ) entity;
        }

        /// <summary>
        /// Get a removed world object by ID.
        /// </summary>
        /// <param name="removedObjectId">The ID of the removed object.</param>
        /// <returns>The removed world object or null if not found.</returns>
        public static RemovedWorldObject GetRemovedWorldObjectById( ulong removedObjectId )
        {
            if( !AltEntitySync.TryGetEntity( removedObjectId, ENTITY_TYPE_REMOVED_OBJECT, out IEntity entity ) )
            {
                Console.WriteLine( $"[OBJECT-STREAMER] [GetRemovedWorldObjectById] ERROR: Entity with ID { removedObjectId } couldn't be found." );
                return default;
            }


            return ( RemovedWorldObject ) entity;
        }

        /// <summary>
        /// Destroy all created dynamic objects.
        /// </summary>
        public static void DestroyAllDynamicObjects( )
        {
            DestroyAllDynamicObjects<IDynamicObject>( );
        }

        /// <summary>
        /// Destroy all created dynamic objects of given type.
        /// </summary>
        public static void DestroyAllDynamicObjects<T>( )
        {
            foreach( T obj in GetAllDynamicObjects<T>( ) )
            {
                AltEntitySync.RemoveEntity( ( IEntity ) obj );
            }
        }

        /// <summary>
        /// Get all created dynamic objects.
        /// </summary>
        /// <returns>A list of dynamic objects.</returns>
        public static List<IDynamicObject> GetAllDynamicObjects( )
        {
            return GetAllDynamicObjects<IDynamicObject>( );
        }

        /// <summary>
        /// Get all created dynamic objects of given type.
        /// </summary>
        /// <returns>A list of dynamic objects.</returns>
        public static List<T> GetAllDynamicObjects<T>( )
        {
            List<T> objects = new List<T>( );

            foreach( IEntity entity in AltEntitySync.GetAllEntities( ) )
            {
                T obj = GetDynamicObject<T>( entity.Id );

                if( obj != null )
                    objects.Add( obj );
            }

            return objects;
        }

        /// <summary>
        /// Get closest dynamic object.
        /// </summary>
        /// <param name="pos">The position from which to check.</param>
        /// <returns>The object closest to the given position, or null if none found close enough.</returns>
        public static (IDynamicObject obj, float distance) GetClosestDynamicObject( Vector3 pos )
        {
            return GetClosestDynamicObject<IDynamicObject>( pos );
        }

        /// <summary>
        /// Get the closest dynamic object to the specified position
        /// </summary>
        /// <typeparam name="T">The type of dynamic object to search for, Default is IDynamicObject.</typeparam>
        /// <param name="pos"></param>
        /// <returns>The object or null if not found, and the distance from the object to the specified position.</returns>
        public static (T obj, float distance) GetClosestDynamicObject<T>( Vector3 pos )
        {
            T obj = default;
            float distance = 5000;

            foreach( IEntity o in GetAllDynamicObjects<T>( ) )
            {
                if( !( o is T ) )
                    continue;

                float dist = Vector3.Distance( o.Position, pos );
                if( dist < distance )
                {
                    obj = ( T ) o;
                    distance = dist;
                }
            }

            return (obj, distance);
        }

        /// <summary>
        /// Get the player that's closest to the given object.
        /// </summary>
        /// <param name="dynamicObject">The object to get the closest player to.</param>
        /// <returns>The found player as first parameter or null, and the distance to the player as second parameter.</returns>
        public static (IPlayer, float) GetClosestPlayerToDynamicObject( IDynamicObject dynamicObject )
        {
            IPlayer closestPlayer = null;
            float closestDist = 5000;

            if( Alt.GetAllPlayers( ).Count == 0 )
                return (closestPlayer, closestDist);

            foreach( IPlayer player in Alt.GetAllPlayers( ) )
            {
                float dist = player.Position.Distance( dynamicObject.Position );

                if( dist < closestDist )
                {
                    closestPlayer = player;
                    closestDist = dist;
                }
            }

            return (closestPlayer, closestDist);
        }

        /// <summary>
        /// Move an object to the given position at the specified speed.
        /// </summary>
        /// <param name="dynamicObject">THe object to move.</param>
        /// <param name="newPos">The position to move the object to.</param>
        /// <param name="speed">The speed at which the object should move at.</param>
        /// <returns>Awaitable task.</returns>
        public static async Task MoveDynamicObject( IDynamicObject dynamicObject, Vector3 newPos, float speed )
        {
            // if the object is already moving
            if( dynamicObject.MovingTimer != null )
            {
                Console.WriteLine( $"[OBJECT-STREAMER] [ERROR] Object with ID { dynamicObject.Id } is already moving." );
                return;
            }

            (IPlayer player, float dist) = GetClosestPlayerToDynamicObject( dynamicObject );

            // if there are no players within stream range, just update the position...
            if( player == null || dist > 400 )
            {
                dynamicObject.Position = newPos;
                return;
            }

            Console.WriteLine( $"checks done" );

            // Task completion source which we can assign to the dictionary and set the result once we've recieved an event that the object finished moving.
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>( );

            var timeout = 15000 * ( 1 / speed );

            if( timeout < 5000 )
                timeout = 5000;

            Console.WriteLine( $"timeout: { timeout }ms" );

            dynamicObject.Move = new MoveData { X = newPos.X, Y = newPos.Y, Z = newPos.Z, Speed = speed };

            Console.WriteLine( $"movedata set, setting timer" );

            dynamicObject.MovingTimer = new Timer( async ( stateInfo ) =>
            {
                Console.WriteLine( $"running timer" );
                await AltAsync.Do( async ( ) =>
                {
                    Vector3 currentPos = dynamicObject.Position;

                    Console.WriteLine( $"calculating pos.." );

                    dynamicObject.Position = new Vector3(
                        CalculateNextPosForAxis( currentPos.X, newPos.X, speed ),
                        CalculateNextPosForAxis( currentPos.Y, newPos.Y, speed ),
                        CalculateNextPosForAxis( currentPos.Z, newPos.Z, speed )
                    );

                    Console.WriteLine( $"pos calculated" );

                    if( dynamicObject.Position == newPos )
                    {
                        Console.WriteLine( $"pos is same as finish, updating.." );
                        await dynamicObject.MovingTimer.DisposeAsync( );
                        tcs.TrySetResult( true );
                        Console.WriteLine( $"result set" );
                    }
                } );

            }, null, 500, 500 );

            Console.WriteLine( $"timer set" );

            // wait for the task to complete or timeout
            var completed = await Task.WhenAny( tcs.Task, DelayResult( false, TimeSpan.FromMilliseconds( timeout ) ) );

            Console.WriteLine( $"task completed" );

            // if the task timedout
            if( completed != tcs.Task )
            {
                Console.WriteLine( $"task timedout" );

                if( dynamicObject.MovingTimer != null )
                    await dynamicObject.MovingTimer.DisposeAsync( );

                Console.WriteLine( $"[OBJECT-STREAMER] [ERROR] Moving object { dynamicObject.Id } timedout." );
            }

            // if the task returned false
            else if( !await completed )
            {
                Console.WriteLine( $"task returned false" );

                if( dynamicObject.MovingTimer != null )
                    await dynamicObject.MovingTimer.DisposeAsync( );

                Console.WriteLine( $"[OBJECT-STREAMER] [ERROR] Failed to move object with ID { dynamicObject.Id }." );
            }

            Console.WriteLine( $"wrapping up move data" );
            var moveData = dynamicObject.Move;
            dynamicObject.Move = null;
            dynamicObject.Position = newPos;
            dynamicObject.MovingTimer = null;
        }

        private static float CalculateNextPosForAxis( float currentPos, float nextPos, float step )
        {
            if( currentPos != nextPos )
            {
                if( Math.Abs( currentPos - nextPos ) < step )
                    currentPos = nextPos;
                else if( currentPos < nextPos )
                    currentPos += step;
                else
                    currentPos -= step;
            }

            return currentPos;
        }

        public static async Task<T> DelayResult<T>( T result, TimeSpan delay )
        {
            await Task.Delay( delay );
            return result;
        }
    }
}
