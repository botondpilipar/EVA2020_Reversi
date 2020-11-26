using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using kd417d.eva.reversi.logic;

namespace kd417d.eva
{

    using TableData = Dictionary<Dimension<uint>, ReversiDisk>;

    [Serializable]
    public class ReversiTable
    {
        #region Fields
        // Properties for testing purposes;
        public TableData Data { get; set; }
        public Dimension<uint> Dimension { get; set; }

        private TimeSpan _timeEllapsed;
        private ReversiColor _currentlyStepping;
        private ReversiBlock _stepRollback;

        [NonSerialized]
        private IReversiTableLogic _logic;
        public IReversiTableLogic Logic
        {
            set { _logic = value; }
        }

        [NonSerialized]
        private Timer _userTime;

        #endregion

        #region Constructor
        public ReversiTable()
        {
            _userTime = new Timer();
            _userTime.Interval = 1000;
            _userTime.Tick += new EventHandler((obj, a) => _timeEllapsed.Add(TimeSpan.FromSeconds(1)));
            _timeEllapsed = TimeSpan.FromSeconds(0);
            Data = new TableData();
            _currentlyStepping = ReversiTableSettings.FirstSteppingColor;
            Dimension = ReversiTableSettings.Dimension;
        }
        #endregion
        #region Events
        public event EventHandler<TableUpdateEventArgs> TableUpdatedEvent;
        public event EventHandler<NewGameEventArgs> NewGameEvent;
        public event EventHandler<GameOverEventArgs> GameOverEvent;
        public event EventHandler<UserTimeUpdatedEventArgs> UserTimeUpdatedEvent;
        #endregion

        #region EventHandlers

        #endregion

        #region Public Methods
        public void Step(uint horizontal, uint vertical)
        {
            var playerStepping = _currentlyStepping;
            var stepTo = new Dimension<uint>(horizontal, vertical);
            if(!(_logic.IsPossiblyAllowedStep(stepTo)) || _logic.IsAlreadyOnTable(stepTo))
            {
                return;
            }
            
            DoStep(stepTo);
            var affectedAfterStep = _logic.GetAffectedDisksOnStep(stepTo).ToList();

            if(affectedAfterStep.Count != 0)
            {
                foreach(var elem in affectedAfterStep)
                {
                    Data[elem].Flip();
                }
                RaiseTableUpdatedEvent(new TableUpdateEventArgs()
                {
                    currentlyStepping = _currentlyStepping,
                    data = ToReversiBlocks()
                });

                if( _logic.IsGameOverScenario())
                {
                    RaiseGameOverEvent(new GameOverEventArgs() { winningSide = playerStepping });
                }
            }
            else
            {
                Rollback();
            }
        }
        public void NewGame(Dimension<uint> dimension)
        {
            this.Dimension = dimension;
            Data.Clear();
            _logic.Dimension = dimension;
            IReversiTableLogic.GetInitial(dimension.Horizontal, dimension.Vertical)
                                .ForEach(block =>
                                    Data.Add(new Dimension<uint>(block.Horizontal, block.Vertical), block.Disk));
            _timeEllapsed = TimeSpan.FromSeconds(0);
            RaiseNewGameEvent(new NewGameEventArgs
            {
                boardUpdate = new TableUpdateEventArgs
                {
                    data = this.ToReversiBlocks(),
                    currentlyStepping = ReversiColor.WHITE
                },
                dimensions = dimension
            });
            _userTime.Start();
        }

        public void PauseGame()
        {
            if(_userTime.Enabled)
            {
                _userTime.Stop();
            }
        }
        public void ContinueGame()
        {
            if(!_userTime.Enabled)
            {
                _userTime.Start();
            }
        }
        #endregion

        #region Private Methods
        private List<ReversiBlock> ToReversiBlocks()
        {
            var result = new List<ReversiBlock>(Data.Count);
            foreach(var kwp in Data)
            {
                var key = kwp.Key;
                result.Add(new ReversiBlock(kwp.Key, kwp.Value.color));
            }
            return result;
        }
        private void RaiseTableUpdatedEvent(TableUpdateEventArgs args)
        {
            if(TableUpdatedEvent != null)
            {
                TableUpdatedEvent(this, args);
            }
        }
        private void RaiseNewGameEvent(NewGameEventArgs args)
        {
            if (NewGameEvent != null)
            {
                NewGameEvent(this, args);
            }
        }
        private void RaiseGameOverEvent(GameOverEventArgs args)
        {
            if (GameOverEvent != null)
            {
                GameOverEvent(this, args);
            }
        }
        private void RaiseUserTimeUpdatedEvent(UserTimeUpdatedEventArgs args)
        {
            if (UserTimeUpdatedEvent != null)
            {
                UserTimeUpdatedEvent(this, args);
            }
        }
        private void DoStep(Dimension<uint> dim)
        {
            _stepRollback = new ReversiBlock(dim, _currentlyStepping);
            Data.Add(dim, new ReversiDisk(_currentlyStepping));
            _currentlyStepping = ReversiColorFunctions.NextColor(_currentlyStepping);
        }
        private void Rollback()
        {
            if(_stepRollback != null)
            {
                var dim = _stepRollback.Dimension;
                Data.Remove(dim);
                _stepRollback = null;
            }

        }
        #endregion

    }
}
