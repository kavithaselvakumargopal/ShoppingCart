using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using ShoppingCart.Data;
using ShoppingCart.Model;
using ShoppingCart.View;

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


        public class OrdersViewModel2 : ViewModelBase, IDataErrorInfo
        {
            private readonly CustomerService _customerService;
            private CustomerOrder _selectedOrder;
            private string _searchText;
            private string _phone;
            private string _amount;
            private string _qty;
            private string _productid;
            private string _customerName;
            private readonly Dictionary<string, string> _validationErrors = new();

            public RelayCommand AddOrderCommand { get; }
            public RelayCommand RemoveOrderCommand { get; }
            public ICommand SearchOrderCommand { get; }

            public OrdersViewModel2()
            {
                _customerService = new CustomerService();
                AddOrderCommand = new RelayCommand(AddOrder, CanAddOrder);
                RemoveOrderCommand = new RelayCommand(RemoveOrder, CanRemoveOrder);
                SearchOrderCommand = new RelayCommand<string>(ExecuteSearchOrder);
                Orders = new ObservableCollection<CustomerOrder>();
                LoadOrders();
            }

            public ObservableCollection<CustomerOrder> Orders { get; set; }

            public CustomerOrder SelectedOrder
            {
                get => _selectedOrder;
                set
                {
                    _selectedOrder = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }

            public string SearchText
            {
                get => _searchText;
                set { _searchText = value; OnPropertyChanged(); }
            }

            public string Phone
            {
                get => _phone;
                set
                {
                    _phone = value;
                    OnPropertyChanged();
                    Validate(nameof(Phone));
                    LoadCustomerName();
                    RefreshCommands();
                }
            }

            public string CustomerName
            {
                get => _customerName;
                set
                {
                    if (_customerName != value)
                    {
                        _customerName = value;
                        OnPropertyChanged();
                        Validate(nameof(CustomerName));
                        RefreshCommands();
                    }
                }
            }

            public string TotalAmount
            {
                get => _amount;
                set
                {
                    _amount = value;
                    OnPropertyChanged();
                    Validate(nameof(TotalAmount));
                    RefreshCommands();
                }
            }

            public string Quantity
            {
                get => _qty;
                set
                {
                    _qty = value;
                    OnPropertyChanged();
                    Validate(nameof(Quantity));
                    RefreshCommands();
                }
            }

            public string ProductId
            {
                get => _productid;
                set
                {
                    _productid = value;
                    OnPropertyChanged();
                    Validate(nameof(ProductId));
                    RefreshCommands();
                }
            }

            private void LoadOrders()
            {
                using (var context = new AppDbContext())
                {
                    var orderList = context.Orders.ToList();
                    Orders.Clear();
                    foreach (var order in orderList)
                    {
                        Orders.Add(order);
                    }
                }
            }

            private void LoadCustomerName()
            {
                var customer = _customerService.GetCustomerByPhone(Phone);
                string newCustomerName = customer != null ? customer.Name : "Not Found";

                if (_customerName != newCustomerName)
                {
                    _customerName = newCustomerName;
                    OnPropertyChanged(nameof(CustomerName));

                    if (_customerName != "Not Found")
                    {
                        _validationErrors.Remove(nameof(CustomerName));
                    }
                    else
                    {
                        _validationErrors[nameof(CustomerName)] = "Customer not found. Please enter a valid phone number.";
                    }

                    OnPropertyChanged(nameof(Error));
                    OnPropertyChanged(nameof(CustomerName));
                    RefreshCommands();
                }
            }
        {
            var customer = _customerService.GetCustomerByPhone(Phone);
            string newCustomerName = customer != null ? customer.Name : "Not Found";
            _customerName = newCustomerName;
            OnPropertyChanged(nameof(CustomerName));
            Validate(nameof(CustomerName));
            RefreshCommands();
        }

        private void AddOrder()
            {
                if (!CanAddOrder()) return;

                using (var context = new AppDbContext())
                {
                    if (context.Customers.Any(c => c.Phone == Phone))
                    {
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
                    else
                    {
                        MessageBox.Show("Phone number not found in customer records. Order rejected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            private bool CanAddOrder()
            {
                return !string.IsNullOrWhiteSpace(Phone) &&
                       Regex.IsMatch(Phone, "^[0-9]+$") &&
                       decimal.TryParse(TotalAmount, out decimal amount) && amount > 0 &&
                       int.TryParse(Quantity, out int qty) && qty > 0 &&
                       !string.IsNullOrWhiteSpace(ProductId) &&
                       !string.IsNullOrWhiteSpace(CustomerName) && CustomerName != "Not Found";
            }
        {
            return !string.IsNullOrWhiteSpace(Phone) &&
                   Regex.IsMatch(Phone, "^[0-9]+$") &&
                   decimal.TryParse(TotalAmount, out decimal amount) && amount > 0 &&
                   int.TryParse(Quantity, out int qty) && qty > 0 &&
                   !string.IsNullOrWhiteSpace(ProductId) &&
                   !string.IsNullOrWhiteSpace(CustomerName) && CustomerName != "Not Found";
        }

    private void RemoveOrder()
    {
        if (SelectedOrder == null) return;

        using (var context = new AppDbContext())
        {
            context.Orders.Remove(SelectedOrder);
            context.SaveChanges();
            Orders.Remove(SelectedOrder);
        }
    }

    private bool CanRemoveOrder() => SelectedOrder != null;

    private void ExecuteSearchOrder(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return;
        var filteredOrders = Orders.Where(o => o.Phone.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
        Orders.Clear();
        foreach (var order in filteredOrders)
        {
            Orders.Add(order);
        }
    }

    private void RefreshCommands()
    {
        CommandManager.InvalidateRequerySuggested();
        OnPropertyChanged(nameof(AddOrderCommand));
    } { CommandManager.InvalidateRequerySuggested(); }

private void Validate(string propertyName)
{
    string error = propertyName switch
    {
        nameof(Phone) => string.IsNullOrWhiteSpace(Phone) ? "Phone number is required." :
                         (!Regex.IsMatch(Phone, "^[0-9]+$") ? "Phone number must contain only digits." : null),
        nameof(TotalAmount) => !decimal.TryParse(TotalAmount, out decimal amount) || amount <= 0 ? "Total Amount must be a valid positive number." : null,
        nameof(Quantity) => !int.TryParse(Quantity, out int qty) || qty <= 0 ? "Quantity must be a valid positive number." : null,
        nameof(ProductId) => string.IsNullOrWhiteSpace(ProductId) ? "Product ID is required." : null,
        nameof(CustomerName) => string.IsNullOrWhiteSpace(CustomerName) || CustomerName == "Not Found" ? "Customer not found. Please enter a valid phone number." : null,
        _ => null
    };

    if (error != null)
    {
        _validationErrors[propertyName] = error;
    }
    else
    {
        _validationErrors.Remove(propertyName);
    }

    OnPropertyChanged(nameof(Error));
    OnPropertyChanged($"[{propertyName}]");
}

public string Error => _validationErrors.Any() ? "Some fields have errors" : null;
public string this[string columnName] => _validationErrors.ContainsKey(columnName) ? _validationErrors[columnName] : null;
    }
}
