using AltV.Net.EntitySync;
using DasNiels.AltV.Streamers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;

namespace DasNiels.AltV.Streamers.Entities
{
    public interface IDynamicObject : IEntity
    {
        Timer MovingTimer { get; set; }

        Vector3 Rotation { get; set; }
        uint Model { get; set; }
        string ModelName { set; }
        uint? LodDistance { get; set; }
        TextureVariation? TextureVariation { get; set; }
        bool? Dynamic { get; set; }
        bool? Visible { get; set; }
        bool? OnFire { get; set; }
        bool? Frozen { get; set; }
        MoveData Move { get; set; }
        Rgb LightColor { get; set; }

        void Destroy( );
    }
}
