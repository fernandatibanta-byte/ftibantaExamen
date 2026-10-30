namespace ftibantaExamen.Views;

public partial class vAcercaDe : ContentPage
{
    public vAcercaDe()
    {
        InitializeComponent();
    }

    private async void btnRegresar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
