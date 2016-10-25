﻿//-----------------------------------------------------------------------
// <copyright file="Tile.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Map
{
    using System;

    /// <summary>
    ///    A Map tile, a square with vector or raster data representing a geographic
    ///    bounding box. More info <see href="https://en.wikipedia.org/wiki/Tiled_web_map">
    ///    here </see>.
    /// </summary>
    public abstract class Tile
    {
        private CanonicalTileId id;
        private string error;
        private bool loaded = false;

        private IAsyncRequest request;
        private Action callback;

        /// <summary> Gets the canonical tile identifier. </summary>
        /// <value> The canonical tile identifier. </value>
        public CanonicalTileId Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary> Gets the error message if any. </summary>
        /// <value> The error string. </value>
        public string Error
        {
            get
            {
                return this.error;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the tile is loaded. A loaded
        ///     tile doesn't necessarily contain data, as it could have error'ed.
        /// </summary>
        /// <value> True if loaded, false otherwise. </value>
        public bool Loaded
        {
            get
            {
                return this.loaded;
            }
        }

        /// <summary>
        ///     Initializes the <see cref="T:Mapbox.Map.Tile"/> object. It will
        ///     start a network request and fire the callback when completed.
        /// </summary>
        /// <param name="param"> Initialization parameters. </param>
        /// <param name="callback"> The completion callback. </param>
        public void Initialize(Parameters param, Action callback)
        {
            if (this.request != null)
            {
                this.request.Cancel();
            }

            this.id = param.Id;
            this.request = param.Fs.Request(this.MakeTileResource(param.Source).GetUrl(), this.HandleTileResponse);
            this.callback = callback;
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String"/> that represents the current
        ///     <see cref="T:Mapbox.Map.Tile"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String"/> that represents the current
        ///     <see cref="T:Mapbox.Map.Tile"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Id.ToString();
        }

        internal void Cancel()
        {
            if (this.request != null)
            {
                this.request.Cancel();
                this.request = null;
            }
        }

        // Get the tile resource (raster/vector/etc).
        internal abstract TileResource MakeTileResource(string source);

        // Decode the tile.
        internal abstract bool ParseTileData(byte[] data);

        // TODO: Currently the tile decoding is done on the main thread. We must implement
        // a Worker class to abstract this, so on platforms that support threads (like Unity
        // on the desktop, Android, etc) we can use worker threads and when building for
        // the browser, we keep it single-threaded.
        private void HandleTileResponse(Response response)
        {
            if (response.Error != null)
            {
                this.error = response.Error;
            } 
            else if (this.ParseTileData(response.Data) == false)
            {
                this.error = "ParseError";
            }

            this.loaded = true;
            this.callback();
        }

        /// <summary>
        ///    Parameters for initializing a Tile object.
        /// </summary>
        public struct Parameters
        {
            /// <summary> The tile id. </summary>
            public CanonicalTileId Id;

            /// <summary> The tile source, will use the default if not set. </summary>
            public string Source;

            /// <summary> The data source abstraction. </summary>
            public IFileSource Fs;
        }
    }
}
