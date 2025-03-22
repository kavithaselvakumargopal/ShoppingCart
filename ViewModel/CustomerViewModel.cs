using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Model;
using ShoppingCart.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Navigation;



namespace ShoppingCart.ViewModel
{
    public class CustomerViewModel: INotifyPropertyChanged
    {
        private string _name;
        private string _phone;

        public ObservableCollection<Customer> Customers { get; set; }
        public ICommand AddCustomerCommand { get; set; }
        public ICommand TestConnectionCommand { get; set; }

        private readonly AppDbContext _context;

        private DatabaseService _databaseService;


        private readonly NavigationService _navigationService;

        public ICommand CloseWindowCommand { get; }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _customerNames;

        public ObservableCollection<string> CustomerNames
        {
            get => _customerNames;
            set
            {
                _customerNames = value;
                OnPropertyChanged();
            }
        }

        private void LoadCustomerNames()
        {
            using (var context = new AppDbContext())
            {
                CustomerNames = new ObservableCollection<string>(
                    context.Customers
                        .OrderBy(c => c.Name)  // Order by Name
                        .Select(c => c.Name)
                        .ToList()
                );
            }
        }

        public void AddCustomer()
        {
            TestConnectionCommand = new RelayCommand(TestDatabaseConnection);
            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Phone))
            {
                Customers.Add(new Customer { Id = Customers.Count + 1, Name = Name, Phone = Phone });
              //  Name = string.Empty;
              //  Phone = string.Empty;
            }
         
            using (var context = new AppDbContext())
            {
                var customer = new Customer
                {
                    Name = Name,
                    Phone = Phone,
                 
                };

                context.Customers.Add(customer);
                context.SaveChanges(); // Saves changes to the database
                LoadCustomerNames();
            }


        }

        private void AddCustomer(object obj)
        {
            Customers.Add(new Customer { Id = Customers.Count + 1, Name = "New Customer", Phone = "98949981" });
        }

        public CustomerViewModel()
        {
            TestConnectionCommand = new RelayCommand(TestDatabaseConnection);
            Customers = new ObservableCollection<Customer>();
            // AddCustomerCommand = new RelayCommand(AddCustomer);
            AddCustomerCommand = new RelayCommand(() => AddCustomer());
            LoadCustomerNames();
          
        }

       
        private void TestDatabaseConnection()
        {

            _databaseService = new DatabaseService();
            string connString = _databaseService.GetConnectionString();
           
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    MessageBox.Show("Database connection successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


   
      



    }

    //public class RelayCommand : ICommand
    //{
    //    private readonly Action _execute;
    //    private readonly Func<bool> _canExecute;

    //    public RelayCommand(Action execute, Func<bool> canExecute = null)
    //    {
    //        _execute = execute;
    //        _canExecute = canExecute;
    //    }
    //    public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
    //    public void Execute(object parameter) => _execute();

    //    public event EventHandler CanExecuteChanged
    //    {
    //        add { CommandManager.RequerySuggested += value; }
    //        remove { CommandManager.RequerySuggested -= value; }
    //    }


    //}
}
