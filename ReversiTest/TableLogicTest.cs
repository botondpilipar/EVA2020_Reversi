using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using kd417d.eva;
using System.Collections.Generic;
using System.Linq;
using kd417d.eva.reversi.logic;

namespace kd417d.eva.reversi
{
    [TestClass]
    public class TableLogicTest
    {
        private logic.TableLogic _logic;
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
        private Dictionary<Dimension<uint>, ReversiDisk> _gameSetup;
            
        private Dimension<uint> tableTopLeft = new Dimension<uint>(1, 1);
        private Dimension<uint> tableBottomRight = new Dimension<uint>(5, 5);
        private Dimension<uint> tableCentral = new Dimension<uint>(3, 3);

        ISet<T> ToSet<T>(IEnumerable<T> param) { return new HashSet<T>(param); }

        [TestInitialize]
        public void Setup()
        {
            _gameSetup = 
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
            Assert.AreEqual((uint)1, ITableLogic.GetHorizontalDistance(_dimA, _dimB));
            Assert.AreEqual((uint)7, ITableLogic.GetHorizontalDistance(_dimB, _dimC));
            Assert.AreEqual((uint)8, ITableLogic.GetHorizontalDistance(_dimA, _dimC));
        }

        [TestMethod]
        public void TestVerticalDistance()
        {
            Assert.AreEqual((uint)1, ITableLogic.GetVerticalDistance(_dimA, _dimB));
            Assert.AreEqual((uint)0, ITableLogic.GetVerticalDistance(_dimB, _dimC));
            Assert.AreEqual ((uint)1, ITableLogic.GetVerticalDistance(_dimA, _dimC));
        }

        [TestMethod]
        public void TestGetLargerDistance()
        {
            var verticallyGreaterDistance_A = new Dimension<uint>(3, 12);
            var verticallyGreaterDistance_B = new Dimension<uint>(2, 2);
            Assert.AreEqual((uint)1, ITableLogic.GetLargerDistance(_dimA, _dimB));
            Assert.AreEqual((uint)7,ITableLogic.GetLargerDistance (_dimB, _dimC));
            Assert.AreEqual((uint)8, ITableLogic.GetLargerDistance(_dimC, _dimA));
            Assert.AreEqual((uint)10, ITableLogic.GetLargerDistance(verticallyGreaterDistance_B
                , verticallyGreaterDistance_A));
        }

        [TestMethod]
        public void TestIsHorizontallyAlignedWith()
        {
             Assert.IsTrue(ITableLogic.IsHorizontallyAlignedWith(_dimB, _dimC));
        }

        [TestMethod]
        public void TestIsVerticallyAlignedWith()
        {
           
            Assert.IsTrue(ITableLogic.IsVerticallyAlignedWith(_dimA, _dimD));
            Assert.IsTrue(ITableLogic.IsVerticallyAlignedWith(
                new Dimension<uint>(34, 43),
                new Dimension<uint>(34, 45)));
        }

        [TestMethod]
        public void TestIsDiagonallyAlignedWith()
        {
            Assert.IsTrue(ITableLogic.IsDiagonallyAlignedWith(
                _dimD,
                new Dimension<uint>(4, 4)));
        }

        [TestMethod]
        public void TestTableTopSideFromDimension()
        {
            var aboveTop = ITableLogic.TopSideFrom(tableTopLeft, _gameSetup.Keys);
            var aboveBottom = ITableLogic.TopSideFrom(tableBottomRight, _gameSetup.Keys);
            var aboveMiddle = ITableLogic.TopSideFrom(tableCentral, _gameSetup.Keys);

            Assert.AreEqual(0, aboveTop.Count());
            Assert.AreEqual(_gameSetup.Keys.Count, aboveBottom.Count());
            Assert.AreEqual(4, aboveMiddle.Count());
        }

        [TestMethod]
        public void TestTableBottomSideFromDimension()
        {
            var belowBottom = ITableLogic.BottomSideFrom(tableBottomRight, _gameSetup.Keys);
            var belowTop = ITableLogic.BottomSideFrom(tableTopLeft, _gameSetup.Keys);
            var belowMiddle = ITableLogic.BottomSideFrom(tableCentral, _gameSetup.Keys);

            Assert.AreEqual(0, belowBottom.Count());
            Assert.AreEqual(10, belowTop.Count());
            Assert.AreEqual(4, belowMiddle.Count());
        }

        [TestMethod]
        public void TestTableLeftSideFromDimension()
        {
            var leftFromLeftSide = ITableLogic.LeftSideFrom(tableTopLeft, _gameSetup.Keys);
            var leftFromRighSide = ITableLogic.LeftSideFrom(tableBottomRight, _gameSetup.Keys);
            var leftFromMiddle = ITableLogic.LeftSideFrom(tableCentral, _gameSetup.Keys);

            Assert.AreEqual(0, leftFromLeftSide.Count());
            Assert.AreEqual(11, leftFromRighSide.Count());
            Assert.AreEqual(6, leftFromMiddle.Count());
        }

        [TestMethod]
        public void TestTableRighSideFromDimension()
        {
            var rightFromRightSide = ITableLogic.RightSideFrom(tableBottomRight, _gameSetup.Keys);
            var rightFromLeftSide = ITableLogic.RightSideFrom(tableTopLeft, _gameSetup.Keys);
            var rightFromMiddle = ITableLogic.RightSideFrom(tableCentral, _gameSetup.Keys);

            Assert.AreEqual(0, rightFromRightSide.Count());
            Assert.AreEqual(8, rightFromLeftSide.Count());
            Assert.AreEqual(2, rightFromMiddle.Count());
        }

        [TestMethod]
        public void TestGetNeighboringPositionsOnCentralPoint()
        {
            var neighboringCentral = ITableLogic.GetNeighBoringPositions(tableCentral);

            const int expectedValidNeighbours = 7; // 8 - 1, one is empty in the game setup

            Assert.AreEqual(expectedValidNeighbours, neighboringCentral
                .Where(k => _gameSetup.ContainsKey(k))
                .Count());
        }

        [TestMethod]
        public void TestGetNeighboringPositionsOnTopLeftPoint()
        {
            var neighboringTopLeft = ITableLogic.GetNeighBoringPositions(tableTopLeft);

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

            var neighboringRandom = ITableLogic.GetNeighBoringPositions(randomDimension);

            Assert.AreNotEqual(0, neighboringRandom.Where(k => _gameSetup.ContainsKey(k)).Count());
        }

        [TestMethod]
        public void TestGetSurroundedByDiagonally()
        {
            // Testing regardless or disk color
            var surroundedInMainDiagonal = _logic.GetSurroundedBy(tableTopLeft, tableBottomRight);
            var surroundedInReverseMainDiagonal = _logic.GetSurroundedBy(tableBottomRight, tableTopLeft);

            Assert.AreEqual(3, surroundedInMainDiagonal.Count());
            var diagonal = surroundedInMainDiagonal.ToList();
            var reverseDiagonal = surroundedInReverseMainDiagonal.ToList();

            // Difference of the two sets are empty set
            Assert.AreEqual(0, diagonal.Except(reverseDiagonal).Count());
        }

        [TestMethod]
        public void TestGetSurroundedByVertically()
        {
            // Testing regardless of disk color
            var dim1 = new Dimension<uint>(4, 1);
            var dim2 = new Dimension<uint>(4, 5);
            var surroundedVertically = _logic.GetSurroundedBy(dim2, dim1).ToList();
            var surroundedVerticallyReverse = _logic.GetSurroundedBy(dim1, dim2).ToList();

            Assert.AreEqual(2, surroundedVertically.Count());
            Assert.AreEqual(0, surroundedVertically.Except(surroundedVerticallyReverse).Count());
        }

        [TestMethod]
        public void TestGetSurroundedByHorizontally()
        {
            var dim1 = new Dimension<uint>(1, 2);
            var dim2 = new Dimension<uint>(5, 2);
            var surroundedHorizontally = _logic.GetSurroundedBy(dim1, dim2);
            var surroundedHorizontallyReverse = _logic.GetSurroundedBy(dim2, dim1);

            Assert.AreEqual(3, surroundedHorizontally.Count());
            Assert.AreEqual(0, surroundedHorizontally.Except(surroundedHorizontallyReverse).Count());
        }

        [TestMethod]
        public void TestGetClosesNeighboursOfSameTypeForBlackColor()
        {
            var testDimA = new Dimension<uint>(1, 4);
            var testDimB = new Dimension<uint>(3, 3);

            var expectedNeighborsForA = new List<Dimension<uint>>() {
                new Dimension<uint>(1, 3),
                new Dimension<uint>(2, 4)
            };
            var expectedNeighborsForB = new List<Dimension<uint>>()
            {
                new Dimension<uint>(4, 4),
                new Dimension<uint>(4, 2),
                new Dimension<uint>(2, 4),
                new Dimension<uint>(1, 3)
            };

            var actualNeighborsForA = _logic.GetClosestNeighBoursOfSameType(testDimA);
            var actualNeighborsForB = _logic.GetClosestNeighBoursOfSameType(testDimB);

            Assert.AreEqual(expectedNeighborsForA.Count(), actualNeighborsForA.Count());
            Assert.AreEqual(expectedNeighborsForB.Count(), actualNeighborsForB.Count());
            Assert.AreEqual(0, expectedNeighborsForA.Except(actualNeighborsForA).Count());
            Assert.AreEqual(0, expectedNeighborsForB.Except(actualNeighborsForB).Count());
        }

        [TestMethod]
        public void TestGetClosesNeighborsOfSameTypeForWhiteColor()
        {
            var testDim = new Dimension<uint>(2, 2);
            var expectedNeighbors = new List<Dimension<uint>>() {
                new Dimension<uint>(1, 1),
                new Dimension<uint>(2, 3),
                new Dimension<uint>(3, 2)
            };

            var actualNeighbors = _logic.GetClosestNeighBoursOfSameType(testDim);

            Assert.AreEqual(3, actualNeighbors.Count());
            Assert.AreEqual(0, expectedNeighbors.Except(actualNeighbors).Count());
        }

        [TestMethod]
        public void TestGetAffectedDisksOnStepForWhiteColor()
        {
            // Step with white to surround vertically
            var newStep = new Dimension<uint>(2, 5);
            _gameSetup.Add(newStep, new ReversiDisk(ReversiColor.WHITE));
            var expectToBeAffected = new List<Dimension<uint>>()
            {
                new Dimension<uint>(2, 4)
            };

            var actualAffected = _logic.GetAffectedDisksOnStep(newStep);

            Assert.AreEqual(expectToBeAffected.Count(), actualAffected.Count());
            Assert.AreEqual(0, expectToBeAffected.Except(actualAffected).Count());

            // Step with white to surround diagonally
            var newStepDiag = new Dimension<uint>(5, 5);
            _gameSetup.Add(newStepDiag, new ReversiDisk(ReversiColor.WHITE));
            var expectToBeAffectedDiagonally = new List<Dimension<uint>>()
            {
                new Dimension<uint>(3, 3),
                new Dimension<uint>(4, 4)
            };
            var closesNeightbor = _logic.GetClosestNeighBoursOfSameType(newStepDiag);
            var actualAffectedDiagonally = _logic.GetAffectedDisksOnStep(newStepDiag);

            Assert.AreEqual(expectToBeAffectedDiagonally.Count(), actualAffectedDiagonally.Count());
            Assert.AreEqual(0, expectToBeAffectedDiagonally.Except(actualAffectedDiagonally).Count());
        }
    }
}
