using System.Linq;
using System.Windows;
using Data;
using Model;
using ProxyHelper;            
using WPFEventInter.ViewModel;

namespace WPFEventInter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Lauches the entry form on startup
        /// </summary>
        /// <param name="e">Arguments of the startup event</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
         
            CarsContext carsContext = new CarsContext();
            
           //store lookup values. These are used when dynamically assigning lookups to view models
           TypeStuff.SetLookupData("BrandsLookup", typeof(Brand), carsContext.Brands.ToList());
           TypeStuff.SetLookupData("ColoursLookup", typeof(Colour), carsContext.Colours.ToList());
           TypeStuff.SetLookupData("CarTypes", typeof(CarType), carsContext.CarTypes.ToList());


           var manager = new Core.WPF.Infrastructure.DataTemplateManager();

           //store data templates associated with view models
           manager.RegisterDataTemplate(typeof(GenericViewModel<Car, WPFEventInter.ViewModel.CarViewModel>), typeof(WPFEventInter.UserControls.CarsDetailView));
           manager.RegisterDataTemplate(typeof(GenericViewModel<Brand, WPFEventInter.ViewModel.BrandViewModel>), typeof(WPFEventInter.UserControls.BrandsDetailView));
           manager.RegisterDataTemplate(typeof(GenericViewModel<CarType, WPFEventInter.ViewModel.CarTypeViewModel>), typeof(WPFEventInter.UserControls.CarTypesView));
           manager.RegisterDataTemplate(typeof(GenericViewModel<Colour, WPFEventInter.ViewModel.ColourViewModel>), typeof(WPFEventInter.UserControls.ColoursDetailView));

            //initialize proxy types - go through all assembilies and find any associated proxy objects
            TypeStuff.InitializeProxyTypes();

            //add some properties and proxy object to PersonViewModel
            PersonViewModel.AddProperty("Name", typeof(string));
            PersonViewModel.AddProxyObject(typeof(Person));


        }


    }
}
