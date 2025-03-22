using ShoppingCart.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ShoppingCart.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;

        public MainViewModel()
        {
            _navigationService = new NavigationService();
            OpenSecondViewCommand = new RelayCommand(OpenSecondView);
        }

        public ICommand OpenSecondViewCommand { get; }

        private void OpenSecondView()
        {
            _navigationService.ShowSecondView();
        }
    }
}
