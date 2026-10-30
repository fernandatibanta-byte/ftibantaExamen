namespace ftibantaExamen.Views;

public partial class vResumen : ContentPage
{
    string usuario = "";

    public vResumen(string usuarioConectado, string nombre, string apellido, int edad, string fecha,
        string plan, bool casillero, double montoInicial, double pagoMensual, double pagoTotal)
    {
        InitializeComponent();
        usuario = usuarioConectado;
        lblUsuario.Text = "Usuario conectado: " + usuario;

        lblNombre.Text = nombre;
        lblApellido.Text = apellido;
        lblEdad.Text = edad.ToString();
        lblFechaInicio.Text = fecha;
        lblPlan.Text = plan;
        lblCasillero.Text = casillero ? "Si" : "No";
        lblMontoInicial.Text = montoInicial.ToString("0.00");
        lblPagoMensual.Text = pagoMensual.ToString("0.00");
        lblPagoTotal.Text = pagoTotal.ToString("0.00");
    }

    private async void btnExportarTXT_Clicked(object sender, EventArgs e)
    {
        try
        {
            string mensaje = $"REGISTRO DE SOCIO - GIMNASIO \n" +
                $" Usuario conectado: {usuario} \n" +
                $" Fecha de registro: {DateTime.Now:dd/MM/yyyy HH:mm} \n" +
                $" ------------------------------ \n" +
                $" Nombre: {lblNombre.Text} \n" +
                $" Apellido: {lblApellido.Text} \n" +
                $" Edad: {lblEdad.Text} \n" +
                $" Fecha de inicio: {lblFechaInicio.Text} \n" +
                $" Plan: {lblPlan.Text} \n" +
                $" Casillero: {lblCasillero.Text} \n" +
                $" Monto Inicial: {lblMontoInicial.Text} \n" +
                $" Pago mensual (5 cuotas): {lblPagoMensual.Text} \n" +
                $" Pago Total: {lblPagoTotal.Text}";

            string nombreArchivo = $"Socio_{lblNombre.Text}_{lblApellido.Text}_{DateTime.Now:yyyyMMddHHmmss}.txt"
                .Replace(" ", "_");

            string carpeta;
#if ANDROID
            //Carpeta Descargas del dispositivo
            carpeta = "/storage/emulated/0/Download";
#else
            carpeta = FileSystem.AppDataDirectory;
#endif
            string path = Path.Combine(carpeta, nombreArchivo);

            try
            {
                File.WriteAllText(path, mensaje);
            }
            catch (Exception)
            {
                //Si no hay permiso en Descargas, se guarda en la carpeta de la aplicacion
                path = Path.Combine(FileSystem.AppDataDirectory, nombreArchivo);
                File.WriteAllText(path, mensaje);
            }

            await DisplayAlertAsync("Archivo TXT generado",
                $"Nombre del archivo: {nombreArchivo}\n\nRuta: {path}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No se pudo generar el archivo: " + ex.Message, "OK");
        }
    }

    private async void btnNuevoRegistro_Clicked(object sender, EventArgs e)
    {
        //Se reemplaza el Registro anterior por uno nuevo con los campos limpios
        var paginas = Navigation.NavigationStack.ToList();
        Page registroAnterior = paginas[paginas.Count - 2];

        Navigation.InsertPageBefore(new vRegistro(usuario), this);
        Navigation.RemovePage(registroAnterior);
        await Navigation.PopAsync();
    }

    private async void btnCerrarSesion_Clicked(object sender, EventArgs e)
    {
        //Regresa al Login (primera ventana)
        await Navigation.PopToRootAsync();
    }
}
