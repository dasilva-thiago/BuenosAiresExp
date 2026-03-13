using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BuenosAiresExp.Models;
using BuenosAiresExp.Services;

namespace BuenosAiresExp.ViewModels
{
    public class LocationViewModel : INotifyPropertyChanged
    {
        private readonly LocationService _service;

        public ObservableCollection<Location> Locations { get; set; }

        private string _newName = string.Empty;
        public string NewName
        {
            get => _newName;
            set
            {
                _newName = value;
                OnPropertyChanged(nameof(NewName));
            }
        }

        private string _newCategory = string.Empty;
        public string NewCategory
        {
            get => _newCategory;
            set
            {
                _newCategory = value;
                OnPropertyChanged(nameof(NewCategory));
            }
        }

        public LocationViewModel()
        {
            _service = new LocationService();
            Locations = new ObservableCollection<Location>(_service.GetAll());
        }

        public void AddLocation()
        {
            var location = new Location
            {
                Name = NewName,
                Category = NewCategory
            };

            _service.Add(location);
            Locations.Add(location);

            NewName = string.Empty;
            NewCategory = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}