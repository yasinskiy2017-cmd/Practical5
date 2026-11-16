namespace MovieCatalogApp.Controls;

public partial class MovieItemView : ContentView
{
    public static readonly BindableProperty ModelProperty =
        BindableProperty.Create(nameof(Model), typeof(object), typeof(MovieItemView), null);

    public object Model
    {
        get => GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }

    public MovieItemView()
    {
        InitializeComponent();
    }
}
