using ChessLib;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for GameOverMenu.xaml
    /// </summary>
    public partial class GameOverMenu : UserControl
    {
        public event Action<Option> OptionSelected;

        public GameOverMenu(GameManager gameController)
        {
            InitializeComponent();

            WinnerText.Text = GetWinnerText(gameController.GetWinner());
            ReasonText.Text = GetReasonText(gameController.GetReason(), gameController.GetCurrentColor());
        }

        private static string GetWinnerText(ColorType winner)
        {
            return winner switch
            {
                ColorType.White => "WHITE WINS!",
                ColorType.Black => "BLACK WINS!",
                _ => "IT'S A DRAW"
            };
        }

        private static string PlayerString(ColorType playerColor)
        {
            return playerColor switch
            {
                ColorType.White => "WHITE",
                ColorType.Black => "BLACK",
                _ => ""
            };
        }

        private static string GetReasonText(EndReason reason, ColorType playerColor)
        {
            return reason switch
            {
                EndReason.Stalemate => $"STALEMATE - {PlayerString(playerColor)} CAN'T MOVE",
                EndReason.Checkmate => $"CHECKMATE - {PlayerString(playerColor)} CAN'T MOVE",
                EndReason.FiftyMoveRule => "FIFTY-MOVE RULE",
                EndReason.InsufficientMaterial => "INSUFFICIENT MATERIAL",
                _ => ""
            };
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Restart);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Exit);
        }
    }
}
