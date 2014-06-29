using System;
using ProxyHelper;
using Data;
using System.Collections.ObjectModel;
using Core.WPF.Infrastructure;

namespace WPFEventInter.ViewModel
{

    public interface IGenericViewModel
    {
        bool CanSave{get;}
        bool  IsRecordDirty{get;set;}
        bool ValidateAllData();
        string EntityName { get; set; }
    }
    
    /// <summary>
    /// Generic view model
    /// </summary>
    /// <typeparam name="T">Model class</typeparam>
    /// <typeparam name="U">View model class</typeparam>
    public class GenericViewModel<T, U> : ViewModelValidated<GenericViewModel<T, U>>, IGenericViewModel
        where T : class,new()
        where U : ICustomObjectProxyObjects,IIsValid, new()
    {

        static GenericViewModel()
        {
            AddProperty("AllRecords", typeof(ObservableCollection<U>));
        }

        IGenericRepository<T> _repository;

        public bool CanDelete {
            get { return ValidateAllData() && IsRecordDirty && CurrentRecord!=null; } 
        }


        bool _IsRecordDirty;
        public bool IsRecordDirty
        {
            get { return _IsRecordDirty; }
            set {
                if (_IsRecordDirty != value)
                {
                    _IsRecordDirty = value;
                }

                RaisePropertyChanged("CanDelete");
                RaisePropertyChanged("CanSave");
            }
        }

        string _EntityName;

        public string EntityName
        {
            get { return _EntityName; }
            set
            {
                if (_EntityName != value)
                {
                    _EntityName = value;
                }
            }
        }


        public bool CanSave
        {
            get { return ValidateAllData() && IsRecordDirty; }
        }


        public bool ValidateAllData()
        {
            foreach (U value in allRecords)
            {
                if (!value.IsValid())
                    return false; 
            }

            return true;
        }

        ObservableCollection<U> allRecords = null;

        public GenericViewModel()
        {
            //TODO set entity value

        }

        public GenericViewModel(IGenericRepository<T> repository)
        {
            _repository = repository;
            allRecords = new ObservableCollection<U>();


            //load up all records
            foreach (T value in repository.GetAll())
            {
                //create Viewmodel
                U record = new U();
                //assign proxy model record to viewmodel object
                record.SetValue(typeof(T), value);
                //wire up event
                ((System.ComponentModel.INotifyPropertyChanged)record).PropertyChanged += _PropertyChanged;
                allRecords.Add(record);
            }

            allRecords.CollectionChanged += allRecords_CollectionChanged;
            PropertyValues["AllRecords"].Value = allRecords;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void allRecords_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
    
            //if items being remove from collection remove entity records
            if (e.OldItems!=null &&  e.OldItems.Count > 0)
            {
                foreach (U record in e.OldItems)
                {
                    ((System.ComponentModel.INotifyPropertyChanged)record).PropertyChanged -= _PropertyChanged;
                    _repository.Remove(record.GetProxyObjects()[typeof(T).FullName] as T);
                }

            } else //add any new records
                if (e.NewItems != null && e.NewItems.Count > 0)
                {
                    foreach (U record in e.NewItems)
                    {
                        T underlyingRecord = new T();

                        _repository.Insert(underlyingRecord);
                        //assign to view associated viewmodel
                        record.SetValue(typeof(T), underlyingRecord);
                        ((System.ComponentModel.INotifyPropertyChanged)record).PropertyChanged += _PropertyChanged;
                    }
                } 
        }

        [LinkToCommand("SaveChangesCommand")]

        void SaveChanges()
        {
            try
            {
                _repository.SaveChanges();
            }
            //TODO: show an error message 
            catch (Exception ex)
            {

            }
        }

        [LinkToCommand("AddRecordCommand")]
        void AddRecord()
        {
            allRecords.Add(new U());
        }

        [LinkToCommand("DeleteRecordCommand")]
        void DeleteRecord()
        {
            if (CurrentRecord != null)
            {
                allRecords.Remove(CurrentRecord);
            }
        }

        U currentRecord;

        [PropertyChangedEvent]
        void PropertyChangedEvt(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CanDelete" && e.PropertyName != "CanSave" ) 
                IsRecordDirty = true; 
        }

        public U CurrentRecord
        {
            set
            {
                currentRecord = value;
            }

            get
            {
                return currentRecord;
            }
        }
    }
}
