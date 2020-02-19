/*
    Developed by DasNiels/Niels/DingoDongBlueBalls
*/

import * as alt from 'alt';

import { objStreamer } from "./object-streamer";

// when an object is streamed in
alt.onServer( "entitySync:create", entity => {
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
} );

// when an object is streamed out
alt.onServer( "entitySync:remove", entityId => {
    objStreamer.remove( +entityId );
} );

// when a streamed in object changes position data
alt.onServer( "entitySync:updatePosition", ( entityId, { x, y, z } ) => {
    objStreamer.updatePosition( entityId, x, y, z );
} );

// when a streamed in object changes data
alt.onServer( "entitySync:updateData", ( entityId, { entityData } ) => {
    objStreamer.updateData(
        +entityId,
        { model: entityData.Model, rx: entityData.RotX, ry: entityData.RotY, rz: entityData.RotZ, entityType: entityData.EntityType }
    );
} );

// when a streamed in object needs to be removed
alt.onServer( "entitySync:clearCache", entityId => {
    objStreamer.clear( +entityId );
} );