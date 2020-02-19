/*
    Developed by DasNiels/Niels/DingoDongBlueBalls
*/

import * as alt from 'alt';

import { ObjectStreamer } from "./object-streamer";

let objStreamer = new ObjectStreamer();

// when an object is streamed in
alt.onServer( "entitySync:create", entity => {
    try {
        if( entity.data ) {
            let data = entity.data.entityData;

            if( data && data.EntityType === "object" ) {
                objStreamer.add(
                    +entity.id, data.Model, data.EntityType,
                    entity.position.x, entity.position.y, entity.position.z,
                    data.RotX, data.RotY, data.RotZ
                );
            }
        }
    } catch( e ) {
    }
} );

// when an object is streamed out
alt.onServer( "entitySync:remove", entityId => {
    try {
        objStreamer.remove( +entityId );
    } catch( e ) {
    }
} );

// when a streamed in object changes position data
alt.onServer( "entitySync:updatePosition", ( entityId, { x, y, z } ) => {
    try {
        objStreamer.updatePosition( entityId, x, y, z );
    } catch( e ) {
    }
} );

// when a streamed in object changes data
alt.onServer( "entitySync:updateData", ( entityId, { entityData } ) => {
    try {
        objStreamer.updateData(
            +entityId,
            {
                model: entityData.Model,
                rx: entityData.RotX,
                ry: entityData.RotY,
                rz: entityData.RotZ,
                entityType: entityData.EntityType
            }
        );
    } catch( e ) {
    }
} );

// when a streamed in object needs to be removed
alt.onServer( "entitySync:clearCache", entityId => {
    try {
        objStreamer.clear( +entityId );
    } catch( e ) {
    }
} );