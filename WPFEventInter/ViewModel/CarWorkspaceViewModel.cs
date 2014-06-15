
using Model;
using Data;
using ProxyHelper;
using System.Collections.Generic;

namespace WPFEventInter.ViewModel
{
    public class CarWorkspaceViewModel : ViewModelValidated<CarWorkspaceViewModel>
    {

        GenericViewModel<Car, CarViewModel> carDetailsVM;
        GenericViewModel<CarType, CarTypeViewModel> carTypeDetailsVM;
        GenericViewModel<Colour, ColourViewModel> colourDetailsVM;
        GenericViewModel<Brand, BrandViewModel> brandDetailsVM;
        PersonViewModel persons;


        public CarWorkspaceViewModel()
        {
            initPersons();

           CarsContext carsContext = new CarsContext();
           Data.GenericRepository<Car> carsRepository = new GenericRepository<Car>(carsContext);

           carDetailsVM = new GenericViewModel<Car, CarViewModel>(new GenericRepository<Car>(carsContext));
           carTypeDetailsVM = new GenericViewModel<CarType, CarTypeViewModel>(new GenericRepository<CarType>(carsContext));
           colourDetailsVM = new GenericViewModel<Colour, ColourViewModel>(new GenericRepository<Colour>(carsContext));
           brandDetailsVM = new GenericViewModel<Brand, BrandViewModel>(new GenericRepository<Brand>(carsContext));
           
        }


        void initPersons()
        {

            persons = new PersonViewModel();

            Person person = new Person();
            person.FirstName = "Fred";
            person.SurName = "Smith";
            person.Height = 180;
            person.ContactDetails = new List<ContactDetail>();
            person.ContactDetails.Add(new ContactDetail(1, "Fred"));
            person.ContactDetails.Add(new ContactDetail(2, "Joe"));
            person.ContactDetails.Add(new ContactDetail(3, "Hans"));

            persons.SetValue(typeof(Person), person);


        }


        public GenericViewModel<Car, CarViewModel> CarDetails { get { return carDetailsVM; }  }
        public GenericViewModel<Brand, BrandViewModel> BrandDetails { get { return brandDetailsVM; } }
        public GenericViewModel<CarType, CarTypeViewModel> CarTypeDetails { get { return carTypeDetailsVM; } }
        public GenericViewModel<Colour, ColourViewModel> ColourDetails { get { return colourDetailsVM; } }
        public PersonViewModel Persons { get { return persons; } }
    }
}
