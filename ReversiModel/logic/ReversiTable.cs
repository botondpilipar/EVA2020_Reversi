using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace kd417d.eva
{

    using TableData = Dictionary<Dimension<uint>, ReversiDisk>;

    [Serializable]
    public class ReversiTable
    {
        #region Fields
        // Public table data is for testing purposes only
        public TableData TableData { get; }

        private TimeSpan _timeEllapsed;
        private reversi.TableLogic _logic;
        private ReversiColor _currentlyStepping;

        [NonSerialized]
        private Timer _userTime;

        public Dimension<uint> dimension
        {
            get { return dimension; }
            set
            {
                dimension = value;

            }
        }
        #endregion

        #region Constructor
        public ReversiTable(uint horizontal, uint vertical) : this(new Dimension<uint>(horizontal, vertical))
        { }
        public ReversiTable(Dimension<uint> dimension)
        {
            this.dimension = dimension;
            _userTime = new Timer();
            _userTime.Interval = 1000;
            _userTime.Tick += new EventHandler((obj, a) => _timeEllapsed.Add(TimeSpan.FromSeconds(1)));
            _timeEllapsed = TimeSpan.FromSeconds(0);
            _logic = new reversi.TableLogic(TableData, dimension);
            _currentlyStepping = ReversiColor.BLACK;

            // Add 4 starting 
            reversi.TableLogic.GetInitial(dimension.Horizontal, dimension.Vertical)
                                .ForEach(block => 
                                    TableData.Add(new Dimension<uint>(block.Horizontal, block.Vertical), block.Disk));
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
            var stepTo = new Dimension<uint>(horizontal, vertical);
            if(!(_logic.IsPossiblyAllowedStep(stepTo)))
            {
                return;
            }
            var affectedAfterStep = _logic.GetAffectedDisksOnStep(stepTo);
            if(affectedAfterStep.Count() != 0)
            {
                TableData.Add(new Dimension<uint>(horizontal, vertical), new ReversiDisk(_currentlyStepping));
                _currentlyStepping = ReversiColorFunctions.NextColor(_currentlyStepping);
                foreach(var elem in affectedAfterStep)
                {
                    TableData[elem].Flip();
                }
                RaiseTableUpdatedEvent(new TableUpdateEventArgs()
                {
                    currentlyStepping = _currentlyStepping,
                    data = ToReversiBlocks()
                });
            }
        }
        public void NewGame(Dimension<uint> dimension)
        {
            this.dimension = dimension;
            TableData.Clear();
            reversi.TableLogic.GetInitial(dimension.Horizontal, dimension.Vertical)
                                .ForEach(block =>
                                    TableData.Add(new Dimension<uint>(block.Horizontal, block.Vertical), block.Disk));
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
        }
        #endregion

        #region Private Methods
        private List<ReversiBlock> ToReversiBlocks()
        {
            var result = new List<ReversiBlock>(TableData.Count);
            foreach(var kwp in TableData)
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
        #endregion

    }
}
