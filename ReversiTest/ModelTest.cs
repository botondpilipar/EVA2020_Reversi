using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using kd417d.eva.test.utility;
using Moq;

namespace kd417d.eva.reversi.logic
{
    [TestClass]
    public class ModelTest
    {
        /*  | 0 |   |   |   |   |
         *  |   | 0 | 0 | x |   |
         *  | x | 0 | x |   |   |
         *  | x | x | 0 | x |   |
         *  |   |   |   |   |   |
         *  
         *  Indexed: Horizontal(from left) x Vertical(from top)
         */

        private Dictionary<Dimension<uint>, ReversiDisk> _gameSetup;
        private ReversiTable _table;
        private Mock<IReversiTableLogic> _mockLogic;
        private EventSpy<NewGameEventArgs> _newGameEventSpy;
        private EventSpy<TableUpdateEventArgs> _tableUpdateEventSpy;
        private EventSpy<GameOverEventArgs> _gameOverEventSpy;
        private EventSpy<UserTimeUpdatedEventArgs> _userTimeUpdatedEventSpy;


        [TestInitialize]
        public void Setup()
        {
            _table = new ReversiTable();
            _mockLogic = new Mock<IReversiTableLogic>();
            _table.Logic = _mockLogic.Object;
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
            _newGameEventSpy = new EventSpy<NewGameEventArgs>();
            _tableUpdateEventSpy = new EventSpy<TableUpdateEventArgs>();
            _userTimeUpdatedEventSpy = new EventSpy<UserTimeUpdatedEventArgs>();
            _gameOverEventSpy = new EventSpy<GameOverEventArgs>();

            _table.NewGameEvent += new EventHandler<NewGameEventArgs>(_newGameEventSpy.OnEventRaised);
            _table.TableUpdatedEvent += new EventHandler<TableUpdateEventArgs>(_tableUpdateEventSpy.OnEventRaised);
            _table.GameOverEvent += new EventHandler<GameOverEventArgs>(_gameOverEventSpy.OnEventRaised);
            _table.UserTimeUpdatedEvent += new EventHandler<UserTimeUpdatedEventArgs>(_userTimeUpdatedEventSpy.OnEventRaised);
        }

        [TestCleanup]
        public void TearDown()
        {

        }

        [TestMethod] 
        public void TestNewGameEventRaised()
        {
            var newGameDimension = new Dimension<uint>(10, 10);
            _table.NewGame(newGameDimension);
            Assert.IsTrue(_newGameEventSpy.IsEventRaised());

            Assert.AreEqual(newGameDimension, _newGameEventSpy.CapturedArguments.First().dimensions);
            Assert.AreEqual(4, _newGameEventSpy.CapturedArguments.First().boardUpdate.data.Count);
            Assert.AreEqual(ReversiColor.WHITE, _newGameEventSpy.CapturedArguments.First().boardUpdate.currentlyStepping);
        }

        [TestMethod]
        public void TestInitialBoardUpdate()
        {
            _table.NewGame(new Dimension<uint>(10, 10));
            var expectedInitialDisks = new List<ReversiBlock>()
            {
                new ReversiBlock(5, 5, ReversiColor.WHITE),
                new ReversiBlock(6, 6, ReversiColor.WHITE),
                new ReversiBlock(6, 5, ReversiColor.BLACK),
                new ReversiBlock(5, 6, ReversiColor.BLACK)
            };
            var actualInitialDisks = _newGameEventSpy.CapturedArguments.First().boardUpdate.data;
            Assert.AreEqual(0, expectedInitialDisks.Except(expectedInitialDisks).Count());
        }

        [TestMethod]
        public void TestBoardUpdateOnValidStep()
        {
            _mockLogic.Setup(p => p.IsPossiblyAllowedStep(It.IsAny<Dimension<uint>>())).Returns(true);
            _mockLogic.Setup(p => p.GetAffectedDisksOnStep(It.IsAny<Dimension<uint>>())).Returns(new List<Dimension<uint>>()
            { 
                new Dimension<uint>(5, 5),
                new Dimension<uint>(6, 6),
                new Dimension<uint>(6, 5),
                new Dimension<uint>(5, 6)
            });
            _table.NewGame(new Dimension<uint>(10, 10));
            ReversiColor stepWith = _newGameEventSpy.CapturedArguments.First().boardUpdate.currentlyStepping;
            _table.Step(6, 7);

            // One inserted and others flipped
            var update = _tableUpdateEventSpy.CapturedArguments.First().data;
            Assert.AreEqual(5, update.Count);
            Assert.AreEqual(3, update.Where(p => p.Disk.color == stepWith).Count());
        }

        [TestMethod]
        public void TestBoardUpdateOnInvalidStep()
        {
            _mockLogic.Setup(p => p.IsPossiblyAllowedStep(It.IsAny<Dimension<uint>>())).Returns(false);
            _table.NewGame(new Dimension<uint>(10, 10));
            _table.Step(4, 4);

            Assert.IsTrue(_tableUpdateEventSpy.IsEmpty());
        }

        [TestMethod]
        public void TestBoardUpdateOnOccupiedStep()
        {
            _mockLogic.Setup(p => p.IsPossiblyAllowedStep(It.IsAny<Dimension<uint>>())).Returns(true);
            _mockLogic.Setup(p => p.IsAlreadyOnTable(It.IsAny<Dimension<uint>>())).Returns(true);

            _table.NewGame(new Dimension<uint>(10, 10));
            _table.Step(3, 3);

            Assert.IsTrue(_tableUpdateEventSpy.IsEmpty());
        }
        
        [TestMethod]
        public void TestGameEventRaised()
        {
            _mockLogic.Setup(p => p.IsGameOverScenario()).Returns(true);
            _mockLogic.Setup(p => p.IsPossiblyAllowedStep(It.IsAny<Dimension<uint>>())).Returns(true);
            _mockLogic.Setup(p => p.GetAffectedDisksOnStep(It.IsAny<Dimension<uint>>())).Returns(new List<Dimension<uint>>()
            {
                new Dimension<uint>(5, 5),
            });
            _mockLogic.Setup(p => p.IsAlreadyOnTable(It.IsAny<Dimension<uint>>())).Returns(false);

            _table.NewGame(new Dimension<uint>(10, 10));
            _table.Step(4, 4);

            ReversiColor firstToStep = _newGameEventSpy.CapturedArguments.First().boardUpdate.currentlyStepping;
            Assert.IsTrue(_gameOverEventSpy.IsEventRaised());
            Assert.AreEqual(firstToStep, _gameOverEventSpy.CapturedArguments.First().winningSide);
        }

        [TestMethod]
        public void TestUserTimeEllapsed()
        {
            _table.NewGame(new Dimension<uint>(10, 10));
            Thread.Sleep(1200);
            var userTimeEventSpyRef = _userTimeUpdatedEventSpy;
            Assert.IsTrue(_userTimeUpdatedEventSpy.IsEventRaised());
        }

        [TestMethod]
        public void TestPauseGame()
        {
            _table.NewGame(new Dimension<uint>(10, 10));
            _table.PauseGame();
            _userTimeUpdatedEventSpy.CapturedArguments.Clear();
            _tableUpdateEventSpy.CapturedArguments.Clear();

            Thread.Sleep(1000);
            _table.Step(3, 4);

            Assert.IsTrue(_userTimeUpdatedEventSpy.IsEmpty());
            Assert.IsTrue(_tableUpdateEventSpy.IsEmpty());
        }

        [TestMethod]
        public void TestContinueGame()
        {
            _mockLogic.Setup(p => p.IsGameOverScenario()).Returns(true);
            _mockLogic.Setup(p => p.IsPossiblyAllowedStep(It.IsAny<Dimension<uint>>())).Returns(true);
            _mockLogic.Setup(p => p.GetAffectedDisksOnStep(It.IsAny<Dimension<uint>>())).Returns(new List<Dimension<uint>>()
            {
                new Dimension<uint>(5, 5),
            });

            _table.NewGame(new Dimension<uint>(10, 10));
            _table.PauseGame();
            _userTimeUpdatedEventSpy.CapturedArguments.Clear();
            _tableUpdateEventSpy.CapturedArguments.Clear();

            _table.ContinueGame();

            Thread.Sleep(1200);

            _table.Step(2, 3);

            Assert.IsTrue(_userTimeUpdatedEventSpy.IsEventRaised());
            Assert.IsTrue(_tableUpdateEventSpy.IsEventRaised());
        }
    }
}
