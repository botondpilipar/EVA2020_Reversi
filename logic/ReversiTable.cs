using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace kd417d.eva
{

    using TableData = Dictionary<Dimension<uint>, ReversiDisk>;

    [Serializable]
    public class ReversiTable
    {
        #region Fields
        private TableData _data;
        private Timer _userTime;
        private TimeSpan _timeEllapsed;
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

            // Add 4 starting 
            reversi.TableLogic.GetInitial(dimension.Horizontal, dimension.Vertical)
                                .ForEach(block => 
                                    _data.Add(new Dimension<uint>(block.Horizontal, block.Vertical), block.Disk));
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

        }
        public void NewGame(Dimension<uint> dimension)
        {
            this.dimension = dimension;
            _data.Clear();
            reversi.TableLogic.GetInitial(dimension.Horizontal, dimension.Vertical)
                                .ForEach(block =>
                                    _data.Add(new Dimension<uint>(block.Horizontal, block.Vertical), block.Disk));
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
            var result = new List<ReversiBlock>(_data.Count);
            foreach(var kwp in _data)
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
