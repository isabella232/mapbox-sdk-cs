﻿//-----------------------------------------------------------------------
// <copyright file="MapTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System.Drawing;
    using Mapbox.Map;
    using NUnit.Framework;

    [TestFixture]
    internal class MapTest
    {
        private Mono.FileSource fs;

        [SetUp]
        public void SetUp()
        {
            this.fs = new Mono.FileSource();
        }

        [Test]
        public void World()
        {
            var map = new Map<VectorTile>(this.fs);

            map.GeoCoordinateBounds = GeoCoordinateBounds.World();
            map.Zoom = 3;

            var mapObserver = new Utils.VectorMapObserver();
            map.Subscribe(mapObserver);

            this.fs.WaitForAllRequests();

            // TODO: Assert.True(mapObserver.Complete);
            // TODO: Assert.IsNull(mapObserver.Error);
            Assert.AreEqual(64, mapObserver.Tiles.Count);

            map.Unsubscribe(mapObserver);
        }

        [Test]
        public void Helsinki()
        {
            var map = new Map<RasterTile>(this.fs);

            map.Center = new GeoCoordinate(60.163200, 24.937700);
            map.Zoom = 13;

            var mapObserver = new Utils.RasterMapObserver();
            map.Subscribe(mapObserver);

            this.fs.WaitForAllRequests();

            // TODO: Assert.True(mapObserver.Complete);
            // TODO: Assert.IsNull(mapObserver.Error);
            Assert.AreEqual(1, mapObserver.Tiles.Count);
            Assert.AreEqual(new Size(256, 256), mapObserver.Tiles[0].Size);

            map.Unsubscribe(mapObserver);
        }

        [Test]
        public void ChangeSource()
        {
            var map = new Map<RasterTile>(this.fs);

            var mapObserver = new Utils.RasterMapObserver();
            map.Subscribe(mapObserver);

            map.Center = new GeoCoordinate(60.163200, 24.937700);
            map.Zoom = 13;
            map.Source = "invalid";

            this.fs.WaitForAllRequests();
            Assert.AreEqual(0, mapObserver.Tiles.Count);

            map.Source = "mapbox.terrain-rgb";

            this.fs.WaitForAllRequests();
            Assert.AreEqual(1, mapObserver.Tiles.Count);

            map.Source = null; // Use default source.

            this.fs.WaitForAllRequests();
            Assert.AreEqual(2, mapObserver.Tiles.Count);

            // Should have fetched tiles from different sources.
            Assert.AreNotEqual(mapObserver.Tiles[0], mapObserver.Tiles[1]);

            map.Unsubscribe(mapObserver);
        }
    }
}
