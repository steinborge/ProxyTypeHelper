using WPFEventInter.ViewModel;
using System.Windows.Controls;



namespace WPFEventInter.View
{
    /// <summary>
    /// Interaction logic for CarView.xaml
    /// </summary>
    public partial class GenericDetailsView : UserControl
    {
        public GenericDetailsView()
        {
           /* CarsContext carsContext = new CarsContext();

            Data.GenericRepository<Car> carsRepository = new GenericRepository<Car>(carsContext);
            GenericViewModel<Car, CarViewModel> genericViewModel = new GenericViewModel<Car, CarViewModel>(carsRepository);


            DataContext = genericViewModel;*/
            InitializeComponent();
        }

        public GenericDetailsView(IGenericViewModel genericViewModel)
        {
            DataContext = genericViewModel;
            InitializeComponent();
        }

        public GenericDetailsView(IGenericViewModel genericViewModel, string EntityName):this(genericViewModel)
        {

        }
    
    }
}


/*   DataTemplate dataTemplate = null;
   try
   {


       // FrameworkElementFactory fef = new FrameworkElementFactory(typeof(CarsDetailView));
       FrameworkElementFactory fef = new FrameworkElementFactory(typeof(TextBlock));

       Binding placeBinding = new Binding();
       //fef.SetBinding(CarsDetailView.DataContextProperty, placeBinding);
       fef.SetBinding(TextBlock.TextProperty, placeBinding);

       placeBinding.Path = new PropertyPath("Name");
        dataTemplate = new DataTemplate();

       dataTemplate.VisualTree = fef;

     //  contCtl.ContentTemplate = dataTemplate;
                
       var key = dataTemplate.DataTemplateKey;
     //  Application.Current.Resources.Add("somekey", dataTemplate);

                

   }

   catch (Exception ex)
   {

   }
    // contCtl.ContentTemplate = (DataTemplate)(new System.Linq.SystemCore_EnumerableDebugView(Application.Current.Resources.Values as System.Windows.ResourceDictionary.ResourceValuesCollection)).Items[2]; // dataTemplate; DataTemplateKey(WPFEventInter.ViewModel.CarsDetailViewModel); //dataTemplate;
   */


/*Data.GenericRepository<Car> carsRepository = new GenericRepository<Car>(carsContext);
CarsDetailViewModel genericViewModel = new CarsDetailViewModel (carsRepository);
DataContext = genericViewModel;
*/