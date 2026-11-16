using MovieCatalogApp.ViewModels;

namespace MovieCatalogApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width > height)
            {
                MoviesGridLayout.Span = 2;
            }
            else
            {
                MoviesGridLayout.Span = 1;
            }
        }
    }
}
