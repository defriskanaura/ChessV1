using Moq;
using ChessLib;
using System.Drawing;
using System.Numerics;

namespace ChessLib.Test
{
    [TestFixture]
    public class Tests
    {
        private GameManager _gameManager1;
        private GameManager _gameManager2;
        private Mock<IBoard> _board;
        [SetUp]
        public void Setup()
        {
            _board = new Mock<IBoard>();
            _gameManager1 = new GameManager(_board.Object);
            _gameManager2 = new GameManager(ColorType.White,_board.Object);
        }

        [TestCase(ColorType.White)]
        [TestCase(ColorType.Black)]
        public void SetCurrentColor_ShouldReturnTrue_PickedColor(ColorType color)
        {
            bool result = _gameManager1.SetCurrentColor(color);
            Assert.IsTrue(result);
        }

        [TestCase(ColorType.None)]
        public void SetCurrentColor_ShouldReturnFalse_PickedColor(ColorType color)
        {
            bool result = _gameManager1.SetCurrentColor(color);
            Assert.IsFalse(result);
        }
        [TestCase(ColorType.White)]
        [TestCase(ColorType.Black)]
        public void AddPlayer_ShouldReturnTrue_PlayerCanBeAdded(ColorType color)
        {
            Mock<IPlayer> player = new Mock<IPlayer>();
            bool result = _gameManager1.AddPlayer(player.Object, color);
            Assert.IsTrue(result);
        }
        [TestCase("Rania", ColorType.White)]
        public void AddPlayer_ShouldReturnTrue_PlayerCanBeAddedByName(string name, ColorType color)
        {
            bool result = _gameManager1.AddPlayer(name, color);
            Assert.IsTrue(result);
        }

        [TestCase(ColorType.White)]
        public void GetCurrentColor_ShouldReturnPlayerCurrentColor_AssignedViaConstructor(ColorType expected)
        {
            ColorType result = _gameManager2.GetCurrentColor();
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(ColorType.Black)]
        public void xGetCurrentColor_ShouldReturnPlayerCurrentColor_AssignedViaConstructor(ColorType expected)
        {
            ColorType result = _gameManager2.GetOpponentColor();
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}