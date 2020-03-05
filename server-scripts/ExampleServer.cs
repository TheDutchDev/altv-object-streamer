using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.EntitySync;
using AltV.Net.EntitySync.ServerEvent;
using AltV.Net.EntitySync.SpatialPartitions;
using DasNiels.AltV.Streamers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace TestServer
{
    public class ExampleServer : AsyncResource
    {
        public override void OnStart( )
        {
            // YOU MUST ADD THIS IN THE ONSTART OF YOUR GAMEMODE, OBJECTSTREAMER WONT WORK WITHOUT IT!
            AltEntitySync.Init( 1, 100,
               repository => new ServerEventNetworkLayer( repository ),
               ( ) => new LimitedGrid3( 50_000, 50_000, 100, 10_000, 10_000, 600 ),
               new IdProvider( )
            );
            //////////////////////////

            AltAsync.OnPlayerConnect += OnPlayerConnect;
            AltAsync.OnConsoleCommand += OnConsoleCommand;

            // Spawn objects
            CreateObjects( );

            // Display commands in console
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine( "|---------------------AVAILABLE CONSOLE COMMANDS:---------------------|" );
            Console.WriteLine( "| dao -> Destroy all created objects." );
            Console.WriteLine( "| cao -> Create all objects defined in the CreateObjects method." );
            Console.WriteLine( " " );
            Console.WriteLine( "| cp {id} -> Move a specified object by 5 units on the Z axis(height)." );
            Console.WriteLine( "| cr {id} -> Rotate a specified object by 5 units on the Z axis(yaw)." );
            Console.WriteLine( "| cm {id} -> Change model of the specified object." );
            Console.WriteLine( "| cld {id} -> Change LOD Distance of the specified object." );
            Console.WriteLine( "| ctv {id} -> Change texture variation of the specified object." );
            Console.WriteLine( "| cd {id} -> Change dynamic of the specified object." );
            Console.WriteLine( "| cv {id} -> Change visibility of the specified object." );
            Console.WriteLine( "| cof {id} -> Change on fire of the specified object." );
            Console.WriteLine( "| cf {id} -> Change frozen of the specified object." );
            Console.WriteLine( "| clc {id} -> Change light color of the specified object." );
            Console.WriteLine( " " );
            Console.WriteLine( "| do {id} -> Destroy a dynamic object by ID(IDs start at 0)." );
            Console.WriteLine( "| go {id} -> Get dynamic object data of the specified object ID." );
            Console.WriteLine( "| gc -> Get the dynamic object closest to player 1." );
            Console.WriteLine( " " );
            Console.WriteLine( "| countobj -> Get the amount of created objects." );
            Console.WriteLine( "|--------------------------------------------------------------------|" );
            Console.ResetColor( );
        }

        private void CreateObjects( )
        {
            // Create some objects
            ObjectStreamer.CreateDynamicObject( "port_xr_lifeboat", new Vector3( -859.655f, -803.499f, 25.566f ), new Vector3( 0, 0, 0 ), 0 );
            ObjectStreamer.CreateDynamicObject( "bkr_prop_biker_bowlpin_stand", new Vector3( -959.655f, -903.499f, 25.566f ), new Vector3( 0, 0, 0 ), 0 );
            ObjectStreamer.CreateDynamicObject( "bkr_prop_biker_tube_crn", new Vector3( -909.655f, -953.499f, 25.566f ), new Vector3( 0, 0, 0 ), 0 );
        }

        private async Task OnConsoleCommand( string name, string[ ] args )
        {
            // destroy all objects
            if( name == "dao" )
            {
                ObjectStreamer.DestroyAllDynamicObjects( );
                Console.WriteLine( $"all objects destroyed." );
            }

            // create all objects
            if( name == "cao" )
            {
                ObjectStreamer.DestroyAllDynamicObjects( );
                CreateObjects( );
            }

            // destroy object
            if( name == "do" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                if( ObjectStreamer.DestroyDynamicObject( objId ) )
                {
                    Console.WriteLine( $"Object with ID { objId } deleted!" );
                }
            }

            // change rotation
            if( name == "cr" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    Vector3 rot = obj.Rotation;
                    obj.Rotation = new Vector3( rot.X, rot.Y, rot.Z + 5f );
                    Console.WriteLine( $"Object rotation increased on Z with +5f" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change visible
            if( name == "cv" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    obj.Visible = !obj.Visible;
                    Console.WriteLine( $"Object visibility set to { obj.Visible }" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change lod distance
            if( name == "cld" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    obj.LodDistance += 100;
                    Console.WriteLine( $"Object LOD Dist increased by 100" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change texture variation
            if( name == "ctv" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    var variations = Enum.GetValues( typeof( TextureVariation ) );

                    obj.TextureVariation = ( TextureVariation ) variations.GetValue( new Random( ).Next( variations.Length ) );
                    Console.WriteLine( $"Object texture variation changed to a random variation" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change dynamic
            if( name == "cd" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    obj.Dynamic = !obj.Dynamic;
                    Console.WriteLine( $"Object dynamic changed to: { obj.Dynamic }" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change on fire(EXPERIMENTAL, DOESNT WORK VERY WELL AS OF RIGHT NOW!)
            if( name == "cof" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    obj.OnFire = !obj.OnFire;
                    Console.WriteLine( $"Object on fire changed to: { obj.OnFire }" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change frozen
            if( name == "cf" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    obj.Frozen = !obj.Frozen;
                    Console.WriteLine( $"Object frozen changed to: { obj.Frozen }" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change light color
            if( name == "clc" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    Random r = new Random( );
                    obj.LightColor = new Rgb( r.Next( 0, 256 ), r.Next( 0, 256 ), r.Next( 0, 256 ) );
                    Console.WriteLine( $"Object lightcolor changed to random value" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change model
            if( name == "cm" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    // change object into a house
                    obj.Model = "lf_house_17_";
                    Console.WriteLine( $"Object changed into a house." );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // change pos
            if( name == "cp" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    Console.WriteLine( $"obj pos: { obj.Position.Z }" );

                    obj.Position += new Vector3( 0, 0, 5 );
                    Console.WriteLine( $"Object position increased on Z with +5f { obj.Position.Z }" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // get object by ID
            if( name == "go" )
            {
                if( args.Length == 0 )
                    return;

                ulong objId = Convert.ToUInt64( args[ 0 ] );
                var obj = ObjectStreamer.GetDynamicObject( objId );
                if( obj != null )
                {
                    Console.WriteLine( $"Object found, data: { obj.Model }, { obj.Rotation.X }, { obj.Rotation.Y }, { obj.Rotation.Z }, { obj.Frozen }, ...!" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
            }

            // get closest object
            if( name == "gc" )
            {
                IPlayer player = Alt.GetAllPlayers( ).First( );

                if( player != null )
                {
                    (DynamicObject obj, float distance) = ObjectStreamer.GetClosestDynamicObject( player.Position );

                    if( obj == null )
                    {
                        Console.WriteLine( "Couldn't find any object near player." );
                        return;
                    }

                    Console.WriteLine( $"Closest object ID is { obj.Id } at a distance of { distance }." );
                }
                else
                    Console.WriteLine( $"Couldnt find any players." );
            }

            // count objects
            if( name == "countobj" )
            {
                Console.WriteLine( $"total objects created: { ObjectStreamer.GetAllDynamicObjects( ).Count }" );
            }
        }

        private async Task OnPlayerConnect( IPlayer player, string reason )
        {
            Console.WriteLine( $"{ player.Name } connected!" );
            player.Model = ( uint ) AltV.Net.Enums.PedModel.FreemodeMale01;
            player.Spawn( new Position( -889.655f, -853.499f, 20.566f ), 0 );
        }

        public override void OnStop( )
        {
            ObjectStreamer.DestroyAllDynamicObjects( );
            Console.WriteLine( $"Server stopped." );
        }
    }
}
