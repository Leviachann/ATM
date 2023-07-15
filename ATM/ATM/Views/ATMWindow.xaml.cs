using ATMApp.ViewModels;
using System.Windows;

namespace ATM.Views
{
    public partial class ATMWindow : Window
    {
        public ATMWindow()
        {
            InitializeComponent();
            DataContext = new ATMViewModel();
        }
    }
}
