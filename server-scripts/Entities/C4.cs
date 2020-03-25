using AltV.Net.Elements.Entities;
using System.Numerics;

namespace DasNiels.AltV.Streamers.Entities
{
    /// <summary>
    /// C4 class that stores all data related to a single c4 object
    /// </summary>
    public class C4 : DynamicObject, IDynamicObject
    {
        public IPlayer Owner { get; set; }

        public C4( Vector3 position, int dimension, uint range ) : base( position, dimension, range )
        {
        }
    }
}
