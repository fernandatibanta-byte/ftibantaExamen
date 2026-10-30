namespace ftibantaExamen.Views;

public partial class vLogin : ContentPage
{
    //Matriz 4 x 2 con usuarios y contrasenias
    string[,] usuarios =
    {
        { "recepcion",  "gym2026"  },
        { "entrenador", "fit_01"   },
        { "uisrael",    "2026"     },
        { "gerente",    "admin123" }
    };

    public vLogin()
    {
        InitializeComponent();
    }

    //Al volver al Login (cerrar sesion) se limpian los campos
    protected override void OnAppearing()
    {
        base.OnAppearing();
        txtUsuario.Text = "";
        txtContrasenia.Text = "";
    }

    private async void btnIniciarSesion_Clicked(object sender, EventArgs e)
    {
        try
        {
            string usuarioIngresado = txtUsuario.Text?.Trim() ?? "";
            string contraseniaIngresada = txtContrasenia.Text ?? "";

            //Validar campos vacios
            if (string.IsNullOrWhiteSpace(usuarioIngresado) ||
                string.IsNullOrWhiteSpace(contraseniaIngresada))
            {
                await DisplayAlertAsync("ERROR", "Ingrese usuario y contraseña", "Cerrar");
                return;
            }

            //Recorrer la matriz para validar las credenciales
            bool acceso = false;
            for (int i = 0; i < usuarios.GetLength(0); i++)
            {
                if (usuarios[i, 0] == usuarioIngresado && usuarios[i, 1] == contraseniaIngresada)
                {
                    acceso = true;
                    break;
                }
            }

            if (acceso)
            {
                await Navigation.PushAsync(new vRegistro(usuarioIngresado));
            }
            else
            {
                await DisplayAlertAsync("ERROR", "Dato incorrecto: usuario o contraseña no válidos", "Cerrar");
                txtContrasenia.Text = "";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("ERROR", ex.Message, "Cerrar");
        }
    }

    private async void btnAcercaDe_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new vAcercaDe());
    }
}
