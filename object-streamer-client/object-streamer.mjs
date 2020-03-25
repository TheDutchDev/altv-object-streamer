/*
    Developed by DasNiels/Niels/DingoDongBlueBalls
*/

import * as alt from 'alt';
import * as natives from 'natives';

import { asyncModel }  from "./async-models";

let OBJECT_TYPES = {
    OBJECT: 0,
    RWO: 1
};


class ObjectStreamer {
    constructor( ) {
        this.objects = [];
    }

    async addObject( entityId, model, entityType, pos, rot, lodDistance, textureVariation, dynamic, visible, onFire, frozen, lightColor  ) {
        // clear the object incase it still exists.
        this.removeObject( entityId, entityType );
        this.clearObject( entityId, entityType );

        if( !await asyncModel.load( +entityId, model ) )
            return alt.log( `[OBJECT-STREAMER] Couldn't create object with model ${ model }.` );

        let handle = natives.createObjectNoOffset( model, pos.x, pos.y, pos.z, true, true, false );

        let obj = { handle: handle, entityId: +entityId, model: model, entityType: entityType, position: pos, move: null };
        this.objects.push( obj );

        this.setRotation( obj, rot );
        this.setLodDistance( obj, lodDistance );
        this.setTextureVariation( obj, textureVariation );
        this.setDynamic( obj, dynamic );
        this.setVisible( obj, visible );
        this.setOnFire( obj, onFire );
        this.setFrozen( obj, frozen );
        this.setLightColor( obj, lightColor );
    }

    removeWorldObject( entityId, position, model, radius, entityType ) {

        let handle = natives.getClosestObjectOfType( position.x, position.y, position.z, +radius, model, false, true, true );

        alt.log( `remove world object: ${ entityId }, ${ model }, ${ handle }` );

        if( handle !== 0 )
        {
            alt.log( "handle is valid" );
            natives.setEntityCollision( +handle, false, false );
            natives.setEntityVisible( +handle, false, false );
        }

        this.objects.push( { handle: handle, entityId: +entityId, model: model, entityType: entityType, radius: radius, position: position } );
    }

    getObject( entityId, entityType ) {
        let obj = this.objects.find( o => +o.entityId === +entityId && +o.entityType === +entityType );

        if( !obj )
            return null;

        return obj;
    }

    async restoreObject( entityId, entityType ) {
        let obj = this.getObject( entityId, entityType );

        if( obj === null )
            return;

        if( entityType === OBJECT_TYPES.RWO )
        {
            natives.setEntityCollision( +obj.handle, false, false );
            natives.setEntityVisible( +obj.handle, false, false );
            return;
        }

        if( !await asyncModel.load( +entityId, obj.model ) )
            return alt.log( `[OBJECT-STREAMER] Couldn't create object with model ${ obj.model }.` );

        obj.handle = natives.createObjectNoOffset( natives.getHashKey( obj.model ), obj.position.x, obj.position.y, obj.position.z, true, true, false );

        this.setRotation( obj, obj.rotation );
        this.setLodDistance( obj, obj.lodDistance );
        this.setTextureVariation( obj, obj.textureVariation );
        this.setDynamic( obj, obj.dynamic );
        this.setVisible( obj, obj.visible );
        this.setOnFire( obj, obj.onFire );
        this.setFrozen( obj, obj.frozen );
        this.setLightColor( obj, obj.lightColor );
    }

    removeObject( entityId, entityType ) {
        let obj = this.getObject( entityId, entityType );

        if( obj === null )
            return;

        if( entityType === OBJECT_TYPES.RWO )
        {
            natives.setEntityCollision( +obj.handle, true, true );
            natives.setEntityVisible( +obj.handle, true, false );
            return;
        }

        asyncModel.cancel( +entityId );

        natives.deleteObject( +obj.handle );
        obj.handle = null;
    }

    clearObject( entityId, entityType ) {
        let idx = this.objects.findIndex( o => +o.entityId === +entityId && +o.entityType === +entityType );

        if( idx === -1 )
            return;

        this.objects.splice( idx, 1 );
    }

    setRotation( obj, rot ) {
        natives.setEntityRotation( +obj.handle, rot.x, rot.y, rot.z, 2, false );
        obj.rotation = rot;
    }

    setPosition( obj, pos ) {
        if( obj.move !== null )
            return;

        natives.setEntityCoordsNoOffset( +obj.handle, pos.x, pos.y, pos.z, true, true, true );
        obj.position = pos;
    }

    async setModel( obj, model ) {

        if( obj.entityType !== OBJECT_TYPES.RWO )
        {
            if( !await asyncModel.load( +obj.entityId, model ) )
                return alt.log( `[OBJECT-STREAMER] Couldn't load model ${ model }.` );

            natives.createModelSwap( obj.position.x, obj.position.y, obj.position.z, 2, natives.getHashKey( obj.model ), natives.getHashKey( model ), true );
        }

        obj.model = model;
    }

    setLodDistance( obj, lodDistance ) {
        if( lodDistance === null )
            return;

        natives.setEntityLodDist( +obj.handle, +lodDistance );
        obj.lodDistance = lodDistance;
    }

    setRadius( obj, radius ) {
        if( radius === null )
            return;

        obj.radius = radius;
    }

    setTextureVariation( obj, textureVariation ) {
        if( textureVariation === null )
        {
            if( obj.textureVariation !== null )
            {
                natives.setObjectTextureVariation( +obj.handle, +textureVariation );
                obj.textureVariation = null;
            }
            return;
        }

        natives.setObjectTextureVariation( +obj.handle, +textureVariation );
        obj.textureVariation = textureVariation;
    }

    setDynamic( obj, dynamic ) {
        if( dynamic === null )
            return;

        natives.setEntityDynamic( +obj.handle, !!dynamic );
        obj.dynamic = !!dynamic;
    }

    setVisible( obj, visible ) {
        if( visible === null )
            return;

        natives.setEntityVisible( +obj.handle, !!visible, false );
        obj.textureVariation = !!visible;
    }

    setOnFire( obj, onFire ) {
        if( onFire === null )
            return;

        if( !!onFire )
        {
            obj.fireHandle = natives.startScriptFire( obj.position.x, obj.position.y, obj.position.z, 1, true );
        }
        else
        {
            if( obj.fireHandle !== null )
            {
                natives.removeScriptFire( +obj.fireHandle );
                obj.fireHandle = null;
            }
        }

        obj.onFire = !!onFire;
    }

    setFrozen( obj, frozen ) {
        if( frozen === null )
            return;

        natives.freezeEntityPosition( +obj.handle, !!frozen );
        obj.frozen = !!frozen;
    }

    setLightColor( obj, lightColor ) {
        if( lightColor === null )
            natives.setObjectLightColor( +obj.handle, false, 0, 0, 0 );
        else
            natives.setObjectLightColor( +obj.handle, true, +lightColor.r, +lightColor.g, +lightColor.b );

        obj.lightColor = lightColor;
    }

    moveObject( obj, data ) {
        alt.log( 'data recieved: ', JSON.stringify( data ) );
        if( data === null )
            obj.move = null;
        else
            obj.move = { ...data, speed: data.speed / 100 };
    }
}

function calculateNextPosForAxis( currentPos, nextPos, step ) {
    if( currentPos !== nextPos )
    {
        if( Math.abs( currentPos - nextPos ) < step )
            currentPos = nextPos;
        else if( currentPos < nextPos )
            currentPos += step;
        else
            currentPos -= step;
    }

    return currentPos;
}

export const objStreamer = new ObjectStreamer();

alt.on( "resourceStop", ( ) => {
    objStreamer.objects.forEach( ( obj ) => {
        objStreamer.removeObject( +obj.entityId, +obj.entityType );
        objStreamer.clearObject( +obj.entityId, +obj.entityType );
    } );
} );

alt.setInterval( ( ) => {
    objStreamer.objects.filter( o => o.move !== null && o.entityType !== OBJECT_TYPES.RWO ).forEach( ( obj ) => {
        let pos = {
            x: calculateNextPosForAxis( obj.position.x, obj.move.x, obj.move.speed ),
            y: calculateNextPosForAxis( obj.position.y, obj.move.y, obj.move.speed ),
            z: calculateNextPosForAxis( obj.position.z, obj.move.z, obj.move.speed ),
        };

        // objStreamer.setPosition( obj, pos );

        natives.setEntityCoordsNoOffset( +obj.handle, pos.x, pos.y, pos.z, true, true, true );
        obj.position = pos;

        if( pos.x === obj.move.x && pos.y === obj.move.y && pos.z === obj.move.z )
        {
            // obj.move = null;
            // alt.log( JSON.stringify( obj ) );
            alt.log( 'object moved!' );
            // alt.emitServer( "OnDynamicObjectMoveFinished", +obj.entityId );
        }
    } );
}, 1 );