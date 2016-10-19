﻿//-----------------------------------------------------------------------
// <copyright file="GeoCoordinateTest.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.UnitTest
{
    using System;
    using Mapbox;
    using NUnit.Framework;

    [TestFixture]
    internal class GeoCoordinateTest
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void NullIsland()
        {
            var lngLat = new GeoCoordinate(0, 0);
            Assert.AreEqual("0.00000,0.00000", lngLat.ToString());
        }

        [Test]
        public void DC()
        {
            var lngLat = new GeoCoordinate(38.9165, -77.0295);
            Assert.AreEqual("-77.02950,38.91650", lngLat.ToString());
        }
    }
}