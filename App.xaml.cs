using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Windows;
using ShoppingCart.Model;
using ShoppingCart.ViewModel;
using System.Windows.Navigation;
using ShoppingCart.View;


namespace ShoppingCart;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var ordersWindow = new Orders2();
        ordersWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);



        // Cleanup logic if needed
    }
    //public App()
    //{
    //    var services = new ServiceCollection();

    //    // Configure DbContext
    //    services.AddDbContext<AppDbContext>(options =>
    //        options.UseSqlServer("MyDbConnection"));

    //    // Register ViewModels
    //    services.AddSingleton<CustomerViewModel>();

    //    // Register Views
    //    services.AddSingleton<MainWindow>();

    //    _serviceProvider = services.BuildServiceProvider();
    //}

    //protected override void OnStartup(StartupEventArgs e)
    //{
    //    base.OnStartup(e);

    //    // Resolve MainWindow
    //    var mainWindow = _serviceProvider.GetService<MainWindow>();
    //    mainWindow?.Show();

    //}

}

