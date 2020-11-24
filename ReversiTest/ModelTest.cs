using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using kd417d.eva;

namespace kd417d.eva.reversi
{
    [TestClass]
    class ModelTest
    {
        private ReversiTable _table;
        [TestInitialize]
        public void Setup()
        {
            _table = new ReversiTable(10, 10);
        }

        [TestCleanup]
        public void TearDown()
        {

        }
    }
}
