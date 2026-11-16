using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MovieCatalogApp.Models;
using MovieCatalogApp.Services;

namespace MovieCatalogApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly MovieService _movieService;
        private bool _isLoading;

        public ObservableCollection<Movie> Movies { get; } = new();

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadMoreCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainViewModel(MovieService movieService)
        {
            _movieService = movieService;
            LoadMoreCommand = new Command(async () => await LoadMoviesAsync(false));
            RefreshCommand = new Command(async () => await LoadMoviesAsync(true));

            _ = LoadMoviesAsync(true);
        }

        public async Task LoadMoviesAsync(bool reset = false)
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                var newItems = await _movieService.GetMoviesAsync(reset);

                if (reset)
                {
                    Movies.Clear();
                }

                foreach (var item in newItems)
                {
                    Movies.Add(item);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
