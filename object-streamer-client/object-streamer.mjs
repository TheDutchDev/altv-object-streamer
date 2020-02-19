/*
    Developed by DasNiels/Niels/DingoDongBlueBalls
*/

import * as alt from 'alt';
import * as natives from 'natives';

// array to store the created objects
let objects;

export class ObjectStreamer {
    constructor( ) {
        if( objects === undefined ) {
            objects = this;
            objects.list = [];
        }

        return objects;
    }

    add( entityId, model, entityType, x, y, z, rx, ry, rz ) {

        try { // clear the object incase it still exists.
            objects.remove( entityId );
            objects.clear( entityId );

            let handle = objects.spawnObject( model, x, y, z, rx, ry, rz );

            objects.list.push( {
                handle: handle, entityId: +entityId, model: model, entityType: entityType,
                x: x, y: y, z: z, rx: rx, ry: ry, rz: rz
            } );
        } catch( e ) {
        }
    }

    spawnObject( model, x, y, z, rx, ry, rz ) {
        try {
            let handle = natives.createObject( natives.getHashKey( model ), x, y, z, true, true, true );
            natives.setEntityRotation( handle, Math.PI * rx / 180, Math.PI * ry / 180, Math.PI * rz / 180, 1, true );

            return handle;
        } catch( e ) {
            return null;
        }
    }

    updatePosition( entityId, x, y, z ) {
        try {
            let idx = objects.list.findIndex( o => +o.entityId === +entityId );

            if( idx === -1 )
                return;

            if( objects.list[ idx ].handle !== null )
                natives.deleteObject( +objects.list[ idx ].handle );

            objects.list[ idx ].handle = null;
            objects.list[ idx ].handle = objects.spawnObject( objects.list[ idx ].model, x, y, z, objects.list[ idx ].rx, objects.list[ idx ].ry, objects.list[ idx ].rz );
        } catch( e ) {
        }
    }

    updateData( entityId, data ) {
        try {
            let idx = objects.list.findIndex( o => +o.entityId === +entityId );

            if( idx === -1 )
                return;

            for( const key in data ) {
                if( data.hasOwnProperty( key ) )
                    objects.list[ idx ][ key ] = data[ key ];
            }

            // delete existing object
            if( objects.list[ idx ].handle !== null )
                natives.deleteObject( +objects.list[ idx ].handle );

            objects.list[ idx ].handle = null;
            objects.list[ idx ].handle = objects.spawnObject(
                objects.list[ idx ].model,
                objects.list[ idx ].x, objects.list[ idx ].y, objects.list[ idx ].z,
                objects.list[ idx ].rx, objects.list[ idx ].ry, objects.list[ idx ].rz
            );
        } catch( e ) {
        }
    }

    remove( entityId ) {
        try {
            let idx = objects.list.findIndex( o => +o.entityId === +entityId );

            if( idx === -1 )
                return;

            natives.deleteObject( +objects.list[ idx ].handle );
            objects.list[ idx ].handle = null;
            alt.log( "objects in list after remove: ", JSON.stringify( objects.list ) );
        } catch( e ) {
        }
    }

    clear( entityId ) {
        try {
            let idx = objects.list.findIndex( o => +o.entityId === +entityId );

            if( idx === -1 )
                return;

            objects.list.splice( idx, 1 );
            alt.log( "objects in list after clear: ", JSON.stringify( objects.list ) );
        } catch( e ) {
        }
    }
}