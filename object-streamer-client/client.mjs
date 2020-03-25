/*
    Developed by DasNiels/Niels/DingoDongBlueBalls
    Async Models loading by Micaww
*/

import * as alt from 'alt';

import { objStreamer } from "./object-streamer";

// when an object is streamed in
alt.onServer( "entitySync:create", ( entityId, entityType, position, entityData ) => {
    alt.log( 'obj streamin: ', JSON.stringify( entityData ) );

    if( entityData ) {
        if( +entityType === 0 ) {

            objStreamer.addObject(
                +entityId, +entityData.model, +entityType,
                position, entityData.rotation,
                entityData.lodDistance, entityData.textureVariation, entityData.dynamic,
                entityData.visible, entityData.onFire, entityData.frozen, entityData.lightColor
            );
        }

        if( +entityType === 1 ) {
            objStreamer.removeWorldObject( +entityId, position, +entityData.model, entityData.radius, entityType );
        }
    }
    // this entity has streamed in before
    else {
        objStreamer.restoreObject( +entityId, +entityType );
    }
} );

// when an object is streamed out
alt.onServer( "entitySync:remove", ( entityId, entityType ) => {

    alt.log( 'streamout: ', entityId );
    objStreamer.removeObject( +entityId, +entityType );
} );

// when a streamed in object changes position data
alt.onServer( "entitySync:updatePosition", ( entityId, entityType, position ) => {
    let obj = objStreamer.getObject( +entityId, +entityType );

    if( obj === null )
        return;

    objStreamer.setPosition( obj, position );
} );

// when a streamed in object changes data
alt.onServer( "entitySync:updateData", ( entityId, entityType, newData ) => {
    let obj = objStreamer.getObject( +entityId, +entityType );

    if( obj === null )
        return;

    if( newData.hasOwnProperty( "rotation" ) )
        objStreamer.setRotation( obj, newData.rotation );

    if( newData.hasOwnProperty( "model" ) )
        objStreamer.setModel( obj, newData.model );

    if( newData.hasOwnProperty( "lodDistance" ) )
        objStreamer.setLodDistance( obj, newData.lodDistance );

    if( newData.hasOwnProperty( "textureVariation" ) )
        objStreamer.setTextureVariation( obj, newData.textureVariation );

    if( newData.hasOwnProperty( "dynamic" ) )
        objStreamer.setDynamic( obj, newData.dynamic );

    if( newData.hasOwnProperty( "visible" ) )
        objStreamer.setVisible( obj, newData.visible );

    if( newData.hasOwnProperty( "onFire" ) )
        objStreamer.setOnFire( obj, newData.onFire );

    if( newData.hasOwnProperty( "frozen" ) )
        objStreamer.setFrozen( obj, newData.frozen );

    if( newData.hasOwnProperty( "lightColor" ) )
        objStreamer.setLightColor( obj, newData.lightColor );

    if( newData.hasOwnProperty( "move" ) )
        objStreamer.moveObject( obj, newData.move );

    if( newData.hasOwnProperty( "radius" ) )
        objStreamer.setRadius( obj, newData.radius );
} );

// when a streamed in object needs to be removed
alt.onServer( "entitySync:clearCache", ( entityId, entityType ) => {
    objStreamer.clearObject( +entityId, +entityType );
} );