using HiveLib.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HiveDisplay
{
    /// <summary>
    /// Interaction logic for NewGameWindows.xaml
    /// </summary>
    public partial class NewGameWindow : Window
    {
        public NewGameWindow()
        {
            InitializeComponent();
        }

        private NewGameParameters _newGameParameters;
        public NewGameParameters Parameters
        {
            get { return _newGameParameters; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _newGameParameters = new NewGameParameters();
            _newGameParameters.player1 = ((PlayerChoice)player1ComboBox.SelectedValue).AI;
            _newGameParameters.player2 = ((PlayerChoice)player2ComboBox.SelectedValue).AI;
            _newGameParameters.player1Name = player1Name.Text;
            _newGameParameters.player2Name = player2Name.Text;
            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
