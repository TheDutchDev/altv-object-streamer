using AltV.Net.EntitySync;
using DasNiels.AltV.Streamers;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DasNiels.AltV.Streamers.Entities
{
    /// <summary>
    /// C4 class that stores all data related to a single c4 object
    /// </summary>
    public class RemovedWorldObject : Entity, IEntity
    {
        /// <summary>
        /// The model of the object to search for
        /// </summary>
        public uint Model
        {
            get
            {
                if( !TryGetData( "model", out uint model ) )
                    return 0;

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
        /// The radius in which to search for the object to be deleted
        /// </summary>
        public uint? Radius
        {
            get
            {
                if( !TryGetData( "radius", out uint radius ) )
                    return null;

                return radius;
            }
            set
            {
                // if value is set to null, reset the data
                if( value == null )
                {
                    SetData( "radius", null );
                    return;
                }

                // No data changed
                if( Radius == value )
                    return;

                SetData( "radius", value );
            }
        }

        public RemovedWorldObject( uint model, Vector3 position, int dimension, uint range, uint radius ) : base( ObjectStreamer.ENTITY_TYPE_REMOVED_OBJECT, position, dimension, range )
        {
            Model = model;
            Radius = radius;
        }

        /// <summary>
        /// Restore the deleted world object.
        /// </summary>
        public void Restore( )
        {
            AltEntitySync.RemoveEntity( this );
        }
    }
}
