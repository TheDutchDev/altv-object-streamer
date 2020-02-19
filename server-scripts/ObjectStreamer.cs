using AltV.Net;
using AltV.Net.EntitySync;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DasNiels.AltV.Streamers
{
    /// <summary>
    /// Class to hold the object data
    /// </summary>
    public class DynamicObjectData : IWritable
    {
        public string Model { get; set; } = "";
        public float RotX { get; set; }
        public float RotY { get; set; }
        public float RotZ { get; set; }
        public string EntityType { get; set; }

        public void OnWrite( IMValueWriter writer )
        {
            writer.BeginObject( );
            writer.Name( "EntityType" );
            writer.Value( EntityType );
            writer.Name( "Model" );
            writer.Value( Model );
            writer.Name( "RotX" );
            writer.Value( RotX );
            writer.Name( "RotY" );
            writer.Value( RotY );
            writer.Name( "RotZ" );
            writer.Value( RotZ );
            writer.EndObject( );
        }
    }

    /// <summary>
    /// DynamicObject class that stores all data related to a single object
    /// </summary>
    public class DynamicObject : Entity, IEntity
    {
        public DynamicObject( Vector3 position, int dimension, uint range, IDictionary<string, object> data ) : base( 0, position, dimension, range, data )
        {
        }   

        /// <summary>
        /// Get all the object's data.
        /// </summary>
        /// <returns>An object containing all the object's data</returns>
        public DynamicObjectData GetDynamicObjectData( )
        {
           if( !TryGetData( "entityData", out DynamicObjectData data ) )
                return default;

            return data;
        }

        /// <summary>
        /// Get the object's current rotation
        /// </summary>
        /// <returns>A vector3 containing the rotation XYZ values in degrees.</returns>
        public Vector3 GetRotation( )
        {
            DynamicObjectData data = GetDynamicObjectData( );

            if( data == null )
                return new Vector3( 0 );

            return new Vector3( data.RotX, data.RotY, data.RotZ );
        }

        /// <summary>
        /// Set the rotation of the object.
        /// </summary>
        /// <param name="newRot">The new rotation in degrees.</param>
        /// <returns>True if rotation was changed successfully.</returns>
        public bool SetRotation( Vector3 newRot )
        {
            DynamicObjectData data = GetDynamicObjectData( );

            if( data == null )
                return false;

            data.RotX = newRot.X;
            data.RotY = newRot.Y;
            data.RotZ = newRot.Z;

            SetDynamicObjectData( data );
            return true;
        }

        /// <summary>
        /// Change an object's model.
        /// </summary>
        /// <param name="newModel">The new model.</param>
        /// <returns>True if operation succeeded, false otherwise.</returns>
        public bool SetModel( string newModel )
        {
            DynamicObjectData data = GetDynamicObjectData( );

            if( data == null )
                return false;

            data.Model = newModel;

            SetDynamicObjectData( data );
            return true;
        }

        /// <summary>
        /// Set the dynamic object's data, only use this if you know what you're doing!
        /// </summary>
        /// <param name="data">The new data</param>
        public void SetDynamicObjectData( DynamicObjectData data )
        {
            SetData( "entityData", data );
        }

        /// <summary>
        /// Set the position of the object.
        /// </summary>
        /// <param name="newPos">The new position of the object.</param>
        public void SetPosition( Vector3 newPos )
        {
            Position = newPos;
        }
    }

    public static class ObjectStreamer
    {
        /// <summary>
        /// Create a dynamic object.
        /// </summary>
        /// <param name="model">The object model name.</param>
        /// <param name="position">The position to spawn the object at.</param>
        /// <param name="rotation">The rotation to spawn the object at(degrees).</param>
        /// <param name="dimension">The dimension to spawn the object in.</param>
        /// <param name="type">(Optional): Type of object, only use this if you want to separate certain object types on a custom client-side based object streamer.</param>
        /// <param name="streamRange">(Optional): The range that a player has to be in before the object spawns, default value is 400.</param>
        /// <returns>The created object</returns>
        public static DynamicObject CreateDynamicObject( string model, Vector3 position, Vector3 rotation, int dimension = 0, string type = "object", uint streamRange = 400 )
        {
            Dictionary<string, object> data = new Dictionary<string, object>( );
            data.Add( "entityData", new DynamicObjectData
            {
                EntityType = type,
                Model = model,
                RotX = rotation.X,
                RotY = rotation.Y,
                RotZ = rotation.Z
            } );

            DynamicObject obj = new DynamicObject( position, dimension, streamRange, data );
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
        public static bool DestroyDynamicObject( DynamicObject obj )
        {
            AltEntitySync.RemoveEntity( obj );
            return true;
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
                return null;
            }

            if( !( entity is DynamicObject ) )
            {
                Console.WriteLine( $"[OBJECT-STREAMER] [GetDynamicObject] ERROR: Entity with ID { dynamicObjectId } is not of type DynamicObject." );
                return null;
            }

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
    }
}
