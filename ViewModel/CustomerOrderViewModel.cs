using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
//using CommunityToolkit.Mvvm.Input;
using ShoppingCart.Data;
using ShoppingCart.Model;
using ShoppingCart.View;

namespace ShoppingCart.ViewModel
{
    public class CustomerOrderViewModel : ViewModelBase
    {

        private bool CanSearch(object parameter) => true;

        private CustomerOrder _selectedOrder;
        public CustomerOrder SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                ((RelayCommand)RemoveOrderCommand).RaiseCanExecuteChanged();
            }
        }

        private ObservableCollection<CustomerOrder> _orders;
        public ObservableCollection<CustomerOrder> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));  // Notify UI that Orders changed
            }
        }

        public ICommand AddOrderCommand { get; }
        public ICommand RemoveOrderCommand { get; }
        public ICommand SearchOrderCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }
        private string _phone;
        private decimal _amount;
        private int _qty;
        private string _productid;
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public decimal TotalAmount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => _qty;
            set { _qty = value; OnPropertyChanged(); }
        }
        public string ProductId
        {
            get => _productid;
            set { _productid = value; OnPropertyChanged(); }
        }


        public CustomerOrderViewModel()
        {

            AddOrderCommand = new RelayCommand(AddOrder);
            RemoveOrderCommand = new RelayCommand(RemoveOrder);
            SearchOrderCommand = new RelayCommand<string>(SearchOrder);
            LoadOrders();
          
        }

       


        private void AddOrder()
        {

            using (var context = new AppDbContext())
            {
                bool isValidPhone = context.Customers.Any(c => c.Phone == Phone);
                if (isValidPhone)
                {
                    var Neworder = new CustomerOrder
                    {
                        Phone = Phone,
                        TotalAmount = TotalAmount,
                        Quantity = Quantity,
                        ProductId = ProductId
                    };

                    context.Orders.Add(Neworder);
                    context.SaveChanges(); // Saves changes to the database
                    Orders.Add(Neworder); // LoadOrders();
                    MessageBox.Show("Order placed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Phone number not found in customer records. Order rejected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }


        }

       


        public void LoadOrders()
        {
          
            using (var context = new AppDbContext())
            {
                var orderList = context.Orders.ToList();

                if (Orders == null)
                {
                    Orders = new ObservableCollection<CustomerOrder>(orderList);
                }
                else
                {
                    Orders.Clear();
                    foreach (var order in orderList)
                    {
                        Orders.Add(order);
                    }
                }
            }


        }
        private bool CanRemoveOrder()
        {
            return SelectedOrder != null; // Ensure this returns true when needed
        }

        private void RemoveOrder()
        {
            if (SelectedOrder != null)
            {
                using (var context = new AppDbContext())
                {
                    context.Orders.Remove(SelectedOrder);
                    context.SaveChanges();
                    SelectedOrder = null;
                    LoadOrders();
                }
            }
          
        }

        private void SearchOrder(string Phone)
        {
            var result = Orders.Where(o => o.Phone.Contains(Phone, StringComparison.OrdinalIgnoreCase)).ToList();

            if (result.Any())
            {
                Orders.Clear();
                foreach (var order in result)
                {
                    Orders.Add(order);
                }
            }
        }

    }
}


