/*
    Developed by DasNiels/Niels/DingoDongBlueBalls
    Async Models loading by Micaww
*/

import * as alt from 'alt';

import { objStreamer } from "./object-streamer";

// when an object is streamed in
alt.onServer( "entitySync:create", entity => {
    if( entity.data ) {
        let data = entity.data;

        if( data && data.entityType === "object" ) {

            alt.log( 'data: ', JSON.stringify( data ) );

            objStreamer.addObject(
                +entity.id, data.model, data.entityType,
                entity.position, data.rotation,
                data.lodDistance, data.textureVariation, data.dynamic,
                data.visible, data.onFire, data.frozen, data.lightColor
            );
        }
    }
} );

// when an object is streamed out
alt.onServer( "entitySync:remove", entityId => {
    objStreamer.removeObject( +entityId );
} );

// when a streamed in object changes position data
alt.onServer( "entitySync:updatePosition", ( entityId, position ) => {
    let obj = objStreamer.getObject( +entityId );

    if( obj === null )
        return;

    objStreamer.setPosition( obj, position );
} );

// when a streamed in object changes data
alt.onServer( "entitySync:updateData", ( entityId, newData ) => {

    alt.log( "entity update: ", JSON.stringify( newData ) );

    let obj = objStreamer.getObject( +entityId );

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
} );

// when a streamed in object needs to be removed
alt.onServer( "entitySync:clearCache", entityId => {
    objStreamer.clearObject( +entityId );
} );