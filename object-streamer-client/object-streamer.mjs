/*
    Developed by DasNiels/Niels/DingoDongBlueBalls
*/

import * as alt from 'alt';
import * as natives from 'natives';

import { asyncModel }  from "./async-models";

class ObjectStreamer {
    constructor( ) {
        this.objects = [];
    }

    async add( entityId, model, entityType, x, y, z, rx, ry, rz ) {

        // clear the object incase it still exists.
        this.remove( entityId );
        this.clear( entityId );

        let handle = await this.spawnObject( +entityId, model, x, y, z, rx, ry, rz );

        this.objects.push( {
            handle: handle, entityId: +entityId, model: model, entityType: entityType,
            x: x, y: y, z: z, rx: rx, ry: ry, rz: rz
        } );
    }

    async spawnObject( entityId, model, x, y, z, rx, ry, rz ) {
        await asyncModel.load( entityId, model );

        let handle = natives.createObject( natives.getHashKey( model ), x, y, z, true, true, true );
        natives.setEntityRotation( handle, Math.PI * rx / 180, Math.PI * ry / 180, Math.PI * rz / 180, 1, true );

        return handle;
    }

    async updatePosition( entityId, x, y, z ) {
        let idx = this.objects.findIndex( o => +o.entityId === +entityId );

        if( idx === -1 )
            return;

        if( this.objects[ idx ].handle !== null )
            natives.deleteObject( +this.objects[ idx ].handle );

        this.objects[ idx ].handle = await this.spawnObject( +entityId, this.objects[ idx ].model, x, y, z, this.objects[ idx ].rx, this.objects[ idx ].ry, this.objects[ idx ].rz );
    }

    async updateData( entityId, data ) {
        let idx = this.objects.findIndex( o => +o.entityId === +entityId );

        if( idx === -1 )
            return;

        for( const key in data ) {
            if( data.hasOwnProperty( key ) )
                this.objects[ idx ][ key ] = data[ key ];
        }

        // delete existing object
        if( this.objects[ idx ].handle !== null )
            natives.deleteObject( +this.objects[ idx ].handle );

        this.objects[ idx ].handle = await this.spawnObject(
            +entityId,
            this.objects[ idx ].model,
            this.objects[ idx ].x, this.objects[ idx ].y, this.objects[ idx ].z,
            this.objects[ idx ].rx, this.objects[ idx ].ry, this.objects[ idx ].rz
        );
    }

    remove( entityId ) {
        let idx = this.objects.findIndex( o => +o.entityId === +entityId );

        if( idx === -1 )
            return;

        asyncModel.cancel( +entityId );

        natives.deleteObject( +this.objects[ idx ].handle );
        this.objects[ idx ].handle = null;
    }

    clear( entityId ) {
        let idx = this.objects.findIndex( o => +o.entityId === +entityId );

        if( idx === -1 )
            return;

        this.objects.splice( idx, 1 );
    }
}

export const objStreamer = new ObjectStreamer();
