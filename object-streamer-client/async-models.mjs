/*
    Developed by Micaww
*/

import * as alt from 'alt';
import * as natives from 'natives';

class AsyncModel {
    constructor( ) {
        this.loadingModels = new Set( );
    }

    cancel( model ) {
        if( typeof model === 'string' )
            model = natives.getHashKey( model );

        this.loadingModels.delete( model );
    }

    async load( model ) {
        return new Promise( resolve => {
            if( typeof model === 'string' )
                model = natives.getHashKey( model );

            this.loadingModels.add( model );

            natives.requestModel( model );

            const interval = alt.setInterval( () => {
                if( !this.loadingModels.has( model ) ) {
                    return done( !!natives.hasModelLoaded( model ) );
                }

                if( natives.hasModelLoaded( model ) ) {
                    return done( true );
                }
            }, 0 );

            const done = result => {
                if( typeof interval !== 'undefined' )
                    alt.clearInterval( interval );

                this.loadingModels.delete( model );
                resolve( result );
            };

            if( !natives.isModelValid( model ) )
                return done( false );

            if( natives.hasModelLoaded( model ) )
                return done( true );
        } );
    }
}

export const asyncModel = new AsyncModel();