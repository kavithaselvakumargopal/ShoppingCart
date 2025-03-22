using ShoppingCart.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShoppingCart.ViewModel
{
    public class NavigationService
    {
        public void ShowSecondView()
        {
            SecondView secondView = new SecondView();
            secondView.Show();

            // Close MainView
            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }


}
