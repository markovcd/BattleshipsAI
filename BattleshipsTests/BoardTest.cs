using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ships;

namespace BattleshipsTests
{
    
    
    [TestClass]
    public class BoardTest
    {
        [TestMethod]
        public void TestSurroundingSpace()
        {
            var s = new[]
            {
                "----------",
                "----------",
                "----------",
                "-----m----",
                "--m--hm---",
                "--m--h----",
                "-m---h----",
                "-----d----",
                "----------",
                "----------",
            };

            var board = new Board(s);

            foreach (var hit in board.Hits())
            {
                hit.SurroundingSpace(board);
            }

            var actual = board.Hits()
                              .Select(h => h.SurroundingSpace(board))
                              .OrderBy(i => i)
                              .Select(i => i.ToString())
                              .Aggregate((a, b) => a + " " + b);

            var expected = "(2, Horizontal) (6, Horizontal) (7, Horizontal)";

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void TestSurroundingSpace2()
        {
            var s = new[]
            {
                "----------",
                "----------",
                "----------",
                "-----m----",
                "-----hm---",
                "--m--h----",
                "-m---h----",
                "----------",
                "----------",
                "----------",
            };

            var board = new Board(s);

            foreach (var hit in board.Hits())
            {
                hit.SurroundingSpace(board);
            }

            var actual = board.Hits()
                              .Select(h => h.SurroundingSpace(board))
                              .OrderBy(i => i)
                              .Select(i => i.ToString())
                              .Aggregate((a, b) => a + " " + b);

            var expected = "(3, Vertical) (5, Horizontal) (6, Horizontal) (7, Horizontal)";

            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void TestSurroundingSpace3()
        {
            var s = new[]
            {
                "----------",
                "----------",
                "-----m----",
                "----------",
                "-----h----",
                "----------",
                "----------",
                "----------",
                "----------",
                "----------",
            };

            var board = new Board(s);

            foreach (var hit in board.Hits())
            {
                hit.SurroundingSpace(board);
            }


            var actual = board.Hits().Distinct()
                              .Select(h => h.SurroundingSpace(board))
                              .OrderBy(i => i)
                              .Select(i => i.ToString())
                              .Aggregate((a, b) => a + " " + b);

            var expected = "(9, Horizontal)";

            Assert.AreEqual(expected, actual);

        }
    }
}
