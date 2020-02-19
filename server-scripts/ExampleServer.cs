using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.EntitySync;
using AltV.Net.EntitySync.ServerEvent;
using AltV.Net.EntitySync.SpatialPartitions;
using DasNiels.AltV.Streamers;
using System;
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
            Console.WriteLine( " " );
            Console.WriteLine( "| do {id} -> Destroy a dynamic object by ID(IDs start at 0)." );
            Console.WriteLine( "| go {id} -> Get dynamic object data of the specified object ID." );
            Console.WriteLine( " " );
            Console.WriteLine( "| countobj -> Get the amount of created objects." );
            Console.WriteLine( "|--------------------------------------------------------------------|" );
            Console.ResetColor( );
        }

        private void CreateObjects( )
        {
            // Create some objects
            ObjectStreamer.CreateDynamicObject( "bkr_prop_biker_bblock_cor", new Vector3( -859.655f, -803.499f, 25.566f ), new Rotation( 0, 0, 0 ), 0 );
            ObjectStreamer.CreateDynamicObject( "bkr_prop_biker_bowlpin_stand", new Vector3( -959.655f, -903.499f, 25.566f ), new Rotation( 0, 0, 0 ), 0 );
            ObjectStreamer.CreateDynamicObject( "bkr_prop_biker_tube_crn", new Vector3( -909.655f, -953.499f, 25.566f ), new Rotation( 0, 0, 0 ), 0 );
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
                    Vector3 rot = obj.GetRotation( );

                    if( obj.SetRotation( new Vector3( rot.X, rot.Y, rot.Z + 5f ) ) )
                        Console.WriteLine( $"Object rotation increased on Z with +5f" );
                    else
                        Console.WriteLine( $"Object rotation failed for object with ID { objId }." );
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
                    if( obj.SetModel( "lf_house_17_" ) )
                        Console.WriteLine( $"Object changed into a house." );
                    else
                        Console.WriteLine( $"Object model change failed for object ID { objId }." );
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
                    obj.SetPosition( new Vector3( obj.Position.X, obj.Position.Y, obj.Position.Z + 5f ) );
                    Console.WriteLine( $"Object position increased on Z with +5f" );
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
                    var data = obj.GetDynamicObjectData( );
                    Console.WriteLine( $"Object found, data: { data.Model }, { data.EntityType }, { data.RotX }, { data.RotY }, { data.RotZ }!" );
                }
                else
                    Console.WriteLine( $"Couldnt find object with ID { objId }" );
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
