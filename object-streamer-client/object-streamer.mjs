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

    async addObject( entityId, model, entityType, pos, rot, lodDistance, textureVariation, dynamic, visible, onFire, frozen, lightColor  ) {
        // clear the object incase it still exists.
        this.removeObject( entityId );
        this.clearObject( entityId );

        if( !await asyncModel.load( +entityId, model ) )
            return alt.log( `[OBJECT-STREAMER] Couldn't create object with model ${ model }.` );

        let handle = natives.createObjectNoOffset( natives.getHashKey( model ), pos.x, pos.y, pos.z, true, true, false );

        let obj = { handle: handle, entityId: +entityId, model: model, entityType: entityType, position: pos };
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

    toRadian( angle ) {
        return Math.PI * angle / 180;
    }

    getObject( entityId ) {
        let obj = this.objects.find( o => +o.entityId === +entityId );

        if( !obj )
            return null;

        return obj;
    }

    removeObject( entityId ) {
        let obj = this.getObject( entityId );

        if( obj === null )
            return;

        asyncModel.cancel( +entityId );

        natives.deleteObject( +obj.handle );
        obj.handle = null;
    }

    clearObject( entityId ) {
        let idx = this.objects.findIndex( o => +o.entityId === +entityId );

        if( idx === -1 )
            return;

        this.objects.splice( idx, 1 );
    }

    setRotation( obj, rot ) {
        natives.setEntityRotation( +obj.handle, this.toRadian( rot.x ), this.toRadian( rot.y ), this.toRadian( rot.z ), 1, true );
        obj.rotation = rot;
    }

    setPosition( obj, pos ) {
        natives.setEntityCoordsNoOffset( +obj.handle, pos.x, pos.y, pos.z, true, true, true );
        obj.position = pos;
    }

    async setModel( obj, model ) {

        if( !await asyncModel.load( +obj.entityId, model ) )
            return alt.log( `[OBJECT-STREAMER] Couldn't create object with model ${ model }.` );

        natives.createModelSwap( obj.position.x, obj.position.y, obj.position.z, 2, natives.getHashKey( obj.model ), natives.getHashKey( model ), true );
        obj.model = model;
    }

    setLodDistance( obj, lodDistance ) {
        if( lodDistance === null )
            return;

        natives.setEntityLodDist( +obj.handle, +lodDistance );
        obj.lodDistance = lodDistance;
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
}

export const objStreamer = new ObjectStreamer();
