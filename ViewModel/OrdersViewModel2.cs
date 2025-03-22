using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShoppingCart.Data;
using ShoppingCart.Model;
using ShoppingCart.View;
using ShoppingCart.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ShoppingCart.ViewModel
{

    public class CustomerService
    {
        public Customer GetCustomerByPhone(string phone)
        {
            var context = new AppDbContext();
            List<Customer> customers = context.Customers.ToList();
            return customers.FirstOrDefault(c => c.Phone == phone);
        }
    }

    // OrdersViewModel2.cs

    public class OrdersViewModel2 : ViewModelBase, IDataErrorInfo
    {
        private readonly CustomerService _customerService = new();
        private CustomerOrder _selectedOrder;
        private string _searchText, _phone, _customerName, _totalAmount, _quantity, _productId;
        public ObservableCollection<CustomerOrder> Orders { get; } = new();

        private bool _phoneTouched = false;
        private bool _totalAmountTouched = false;
        private bool _quantityTouched = false;
        private bool _productIdTouched = false;
        private bool CanRemoveOrder() => SelectedOrder != null;

        public RelayCommand AddOrderCommand { get; }
        public RelayCommand RemoveOrderCommand { get; }
        public RelayCommand<string> SearchOrderCommand { get; }

        public OrdersViewModel2()
        {
            Orders = new ObservableCollection<CustomerOrder>();

            AddOrderCommand = new RelayCommand(AddOrder, CanAddOrder);
            RemoveOrderCommand = new RelayCommand(RemoveOrder);
            SearchOrderCommand = new RelayCommand<string>(ExecuteSearchOrder);

            LoadOrders();
        }

        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(); }
        }

      
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; _phoneTouched = true; OnPropertyChanged(); LoadCustomerName(); }
        }

        public string TotalAmount
        {
            get => _totalAmount;
            set { _totalAmount = value; _totalAmountTouched = true; OnPropertyChanged(); RefreshCommands(); }
        }

        public string Quantity
        {
            get => _quantity;
            set { _quantity = value; _quantityTouched = true; OnPropertyChanged(); RefreshCommands(); }
        }

        public string ProductId
        {
            get => _productId;
            set { _productId = value; _productIdTouched = true; OnPropertyChanged(); RefreshCommands(); }
        }




        private void LoadOrders()
        {
            using var context = new AppDbContext();
            Orders.Clear();
            foreach (var order in context.Orders.ToList())
                Orders.Add(order);
        }


        private void LoadCustomerName()
        {
            if (string.IsNullOrWhiteSpace(Phone))
            {
                CustomerName = "";
            }
            else
            {
                var customer = _customerService.GetCustomerByPhone(Phone);
                CustomerName = customer?.Name ?? "Not Found";
            }

            OnPropertyChanged(nameof(CustomerName));
            OnPropertyChanged("Item[]");  // Critical to re-trigger validation
            RefreshCommands();
        }

        private void AddOrder()
        {
            using var context = new AppDbContext();
            var newOrder = new CustomerOrder
            {
                Phone = Phone,
                TotalAmount = decimal.Parse(TotalAmount),
                Quantity = int.Parse(Quantity),
                ProductId = ProductId
            };
            context.Orders.Add(newOrder);
            context.SaveChanges();
            Orders.Add(newOrder);
            MessageBox.Show("Order placed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CanAddOrder() =>
            string.IsNullOrWhiteSpace(this[nameof(Phone)]) &&
            string.IsNullOrWhiteSpace(this[nameof(CustomerName)]) &&
            string.IsNullOrWhiteSpace(this[nameof(TotalAmount)]) &&
            string.IsNullOrWhiteSpace(this[nameof(Quantity)]) &&
            string.IsNullOrWhiteSpace(this[nameof(ProductId)]);

        private void RemoveOrder()
        {
            if (SelectedOrder == null) return;

            using var context = new AppDbContext();

            var orderToRemove = context.Orders
                .FirstOrDefault(o => o.OrderId == SelectedOrder.OrderId); // ensure correct property here

            if (orderToRemove != null)
            {
                context.Orders.Remove(orderToRemove);
                context.SaveChanges();
                Orders.Remove(SelectedOrder);
                SelectedOrder = null;
            }

            RefreshCommands();
        }
                
        private void ExecuteSearchOrder(string SearchText)
        {
            using var context = new AppDbContext();

            var filteredOrders = string.IsNullOrWhiteSpace(SearchText)
                ? context.Orders.ToList()
                : context.Orders.Where(o => o.Phone.Contains(SearchText)).ToList();

            Orders.Clear();
            foreach (var order in filteredOrders)
                Orders.Add(order);

        }






        public CustomerOrder SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                RefreshCommands(); // CRITICAL: explicitly calls command requery
            }
        }


        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Phone):
                        if (!_phoneTouched) return null; // No error until touched
                        if (string.IsNullOrWhiteSpace(Phone))
                            return "Phone required.";
                        if (!Regex.IsMatch(Phone, @"^\d+$"))
                            return "Digits only.";
                        break;

                    case nameof(CustomerName):
                        if (!_phoneTouched || string.IsNullOrWhiteSpace(Phone)) return null;
                        if (CustomerName == "Not Found")
                            return "Customer not found.";
                        break;

                    case nameof(TotalAmount):
                        if (!_totalAmountTouched) return null;
                        if (string.IsNullOrWhiteSpace(TotalAmount))
                            return "Amount required.";
                        if (!decimal.TryParse(TotalAmount, out var amt) || amt <= 0)
                            return "Invalid amount.";
                        break;

                    case nameof(Quantity):
                        if (!_quantityTouched) return null;
                        if (string.IsNullOrWhiteSpace(Quantity))
                            return "Quantity required.";
                        if (!int.TryParse(Quantity, out var qty) || qty <= 0)
                            return "Invalid quantity.";
                        break;

                    case nameof(ProductId):
                        if (!_productIdTouched) return null;
                        if (string.IsNullOrWhiteSpace(ProductId))
                            return "Product ID required.";
                        break;
                }
                return null;
            }
        }

        
        private void RefreshCommands() => CommandManager.InvalidateRequerySuggested();
    }

}