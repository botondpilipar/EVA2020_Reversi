using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using kd417d.eva;
using System.Collections.Generic;
using System.Linq;

namespace kd417d.eva.reversi
{
    [TestClass]
    public class TableLogicTest
    {
        private TableLogic _logic;
        private Dimension<uint> _logicDimension;
        private Dimension<uint> _dimA;
        private Dimension<uint> _dimB;
        private Dimension<uint> _dimC;
        private Dimension<uint> _dimD;

        private const int whiteDisks = 5;
        private const int blackDisk = 6;


        /*  | 0 |   |   |   |   |
         *  |   | 0 | 0 | x |   |
         *  | x | 0 | x |   |   |
         *  | x | x | 0 | x |   |
         *  |   |   |   |   |   |
         *  
         *  Indexed: Horizontal(from left) x Vertical(from top)
         */
        private Dictionary<Dimension<uint>, ReversiDisk> _gameSetup =
            new Dictionary<Dimension<uint>, ReversiDisk>()
            {
                { new Dimension<uint>(1, 1), new ReversiDisk(ReversiColor.WHITE) },
                { new Dimension<uint>(2, 2), new ReversiDisk(ReversiColor.WHITE) },
                { new Dimension<uint>(2, 3), new ReversiDisk(ReversiColor.WHITE) },
                { new Dimension<uint>(3, 2), new ReversiDisk(ReversiColor.WHITE) },
                { new Dimension<uint>(3, 4), new ReversiDisk(ReversiColor.WHITE) },
                { new Dimension<uint>(1, 3), new ReversiDisk(ReversiColor.BLACK) },
                { new Dimension<uint>(1, 4), new ReversiDisk(ReversiColor.BLACK) },
                { new Dimension<uint>(2, 4), new ReversiDisk(ReversiColor.BLACK) },
                { new Dimension<uint>(3, 3), new ReversiDisk(ReversiColor.BLACK) },
                { new Dimension<uint>(4, 4), new ReversiDisk(ReversiColor.BLACK) },
                { new Dimension<uint>(4, 2), new ReversiDisk(ReversiColor.BLACK) }
            };
        private Dimension<uint> tableTopLeft = new Dimension<uint>(1, 1);
        private Dimension<uint> tableBottomRight = new Dimension<uint>(5, 5);
        private Dimension<uint> tableCentral = new Dimension<uint>(3, 3);

        [TestInitialize]
        public void Setup()
        {
            _logicDimension = new Dimension<uint>(5, 5);
            _logic = new TableLogic(_gameSetup, _logicDimension);
            _dimA = new Dimension<uint>(2, 3);
            _dimB = new Dimension<uint>(3, 4);
            _dimC = new Dimension<uint>(10, 4);
            _dimD = new Dimension<uint>(2, 2);
        }

        [TestCleanup]
        public void CleanUp()
        {
        }

        [TestMethod]
        public void TestHorizontalDistance()
        {
            Assert.AreEqual((uint)1, TableLogic.GetHorizontalDistance(_dimA, _dimB));
            Assert.AreEqual((uint)7, TableLogic.GetHorizontalDistance(_dimB, _dimC));
            Assert.AreEqual((uint)8, TableLogic.GetHorizontalDistance(_dimA, _dimC));
        }

        [TestMethod]
        public void TestVerticalDistance()
        {
            Assert.AreEqual((uint)1, TableLogic.GetVerticalDistance(_dimA, _dimB));
            Assert.AreEqual((uint)0, TableLogic.GetVerticalDistance(_dimB, _dimC));
            Assert.AreEqual ((uint)1, TableLogic.GetVerticalDistance(_dimA, _dimC));
        }

        [TestMethod]
        public void TestGetLargerDistance()
        {
            var verticallyGreaterDistance_A = new Dimension<uint>(3, 12);
            var verticallyGreaterDistance_B = new Dimension<uint>(2, 2);
            Assert.AreEqual((uint)1, TableLogic.GetLargerDistance(_dimA, _dimB));
            Assert.AreEqual((uint)7,TableLogic.GetLargerDistance (_dimB, _dimC));
            Assert.AreEqual((uint)8, TableLogic.GetLargerDistance(_dimC, _dimA));
            Assert.AreEqual((uint)10, TableLogic.GetLargerDistance(verticallyGreaterDistance_B
                , verticallyGreaterDistance_A));
        }

        [TestMethod]
        public void TestIsHorizontallyAlignedWith()
        {
            Assert.IsTrue(TableLogic.IsHorizontallyAlignedWith(_dimA, _dimD));
            Assert.IsTrue(TableLogic.IsHorizontallyAlignedWith(
                new Dimension<uint>(34, 43),
                new Dimension<uint>(34, 45)));
        }

        [TestMethod]
        public void TestIsVerticallyAlignedWith()
        {
            Assert.IsTrue(TableLogic.IsVerticallyAlignedWith(_dimB, _dimC));
        }

        [TestMethod]
        public void TestIsDiagonallyAlignedWith()
        {
            Assert.IsTrue(TableLogic.IsDiagonallyAlignedWith(
                _dimD,
                new Dimension<uint>(4, 4)));
        }

        [TestMethod]
        public void TestTableTopSideFromDimension()
        {
            var aboveTop = TableLogic.TopSideFrom(tableTopLeft, _gameSetup.Keys);
            var aboveBottom = TableLogic.TopSideFrom(tableBottomRight, _gameSetup.Keys);
            var aboveMiddle = TableLogic.TopSideFrom(tableCentral, _gameSetup.Keys);

            Assert.AreEqual(0, aboveTop.Count());
            Assert.AreEqual(_gameSetup.Keys.Count, aboveBottom.Count());
            Assert.AreEqual(4, aboveMiddle.Count());
        }

        [TestMethod]
        public void TestTableBottomSideFromDimension()
        {
            var belowBottom = TableLogic.BottomSideFrom(tableBottomRight, _gameSetup.Keys);
            var belowTop = TableLogic.BottomSideFrom(tableTopLeft, _gameSetup.Keys);
            var belowMiddle = TableLogic.BottomSideFrom(tableCentral, _gameSetup.Keys);

            Assert.AreEqual(0, belowBottom.Count());
            Assert.AreEqual(10, belowTop.Count());
            Assert.AreEqual(4, belowMiddle.Count());
        }

        [TestMethod]
        public void TestTableLeftSideFromDimension()
        {
            var leftFromLeftSide = TableLogic.LeftSideFrom(tableTopLeft, _gameSetup.Keys);
            var leftFromRighSide = TableLogic.LeftSideFrom(tableBottomRight, _gameSetup.Keys);
            var leftFromMiddle = TableLogic.LeftSideFrom(tableCentral, _gameSetup.Keys);

            Assert.AreEqual(0, leftFromLeftSide.Count());
            Assert.AreEqual(11, leftFromRighSide.Count());
            Assert.AreEqual(6, leftFromMiddle.Count());
        }

        [TestMethod]
        public void TestTableRighSideFromDimension()
        {
            var rightFromRightSide = TableLogic.RightSideFrom(tableBottomRight, _gameSetup.Keys);
            var rightFromLeftSide = TableLogic.RightSideFrom(tableTopLeft, _gameSetup.Keys);
            var rightFromMiddle = TableLogic.RightSideFrom(tableCentral, _gameSetup.Keys);

            Assert.AreEqual(0, rightFromRightSide.Count());
            Assert.AreEqual(8, rightFromLeftSide.Count());
            Assert.AreEqual(2, rightFromMiddle.Count());
        }

        [TestMethod]
        public void TestGetNeighboringPositionsOnCentralPoint()
        {
            var neighboringCentral = TableLogic.GetNeighBoringPositions(tableCentral);

            const int expectedValidNeighbours = 7; // 8 - 1, one is empty in the game setup

            Assert.AreEqual(expectedValidNeighbours, neighboringCentral
                .Where(k => _gameSetup.ContainsKey(k))
                .Count());
        }

        [TestMethod]
        public void TestGetNeighboringPositionsOnTopLeftPoint()
        {
            var neighboringTopLeft = TableLogic.GetNeighBoringPositions(tableTopLeft);

            const int expectedValidNeighbours = 1;

            Assert.AreEqual(expectedValidNeighbours, neighboringTopLeft
                .Where(k => _gameSetup.ContainsKey(k))
                .Count());
        }

        [TestMethod]
        public void TestGetNeighboringPositionsOnRandomPosition()
        {
            Random rand = new Random();
            int randomHorizontal = rand.Next(1, 6);
            int randomVertical = rand.Next(1, 6);
            Dimension<uint> randomDimension = new Dimension<uint>(
                (uint)randomHorizontal,
                (uint)randomVertical);

            var neighboringRandom = TableLogic.GetNeighBoringPositions(randomDimension);

            Assert.AreNotEqual(0, neighboringRandom.Where(k => _gameSetup.ContainsKey(k)).Count());
        }
    }
}
