using System.Globalization;

namespace ftibantaExamen.Views;

public partial class vRegistro : ContentPage
{
    //Datos de la membresia
    const double COSTO_MEMBRESIA = 1200;
    const int NUMERO_CUOTAS = 5;
    const double PORCENTAJE_CUOTA = 0.02;   // 2 % del costo por cuota
    const double VALOR_CASILLERO = 10;      // adicional por cuota
    const double MONTO_MINIMO = 120;        // 10 % de 1200
    const double MONTO_MAXIMO = 600;        // 50 % de 1200

    string usuario = "";

    public vRegistro(string usuarioConectado)
    {
        InitializeComponent();
        usuario = usuarioConectado;
        lblUsuario.Text = "Usuario conectado: " + usuario;

        //No permitir fechas pasadas
        dpFechaInicio.MinimumDate = DateTime.Today;
        dpFechaInicio.Date = DateTime.Today;
    }

    //Edad: solo se permiten numeros
    private void txtEdad_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue) && !e.NewTextValue.All(char.IsDigit))
        {
            txtEdad.Text = e.OldTextValue;
        }
    }

    //Si cambia el monto inicial se debe volver a calcular
    private void txtMontoInicial_TextChanged(object sender, TextChangedEventArgs e)
    {
        txtPagoMensual.Text = "";
    }

    //Si cambia el casillero se debe volver a calcular
    private void swCasillero_Toggled(object sender, ToggledEventArgs e)
    {
        txtPagoMensual.Text = "";
    }

    //Validar todos los campos del formulario
    private async Task<bool> ValidarDatos()
    {
        string nombre = txtNombre.Text?.Trim() ?? "";
        string apellido = txtApellido.Text?.Trim() ?? "";
        string edadTexto = txtEdad.Text?.Trim() ?? "";
        string montoTexto = txtMontoInicial.Text?.Trim() ?? "";

        //Nombre
        if (string.IsNullOrWhiteSpace(nombre))
        {
            await DisplayAlertAsync("Error", "El campo nombre no debe ser vacio", "OK");
            return false;
        }
        if (!nombre.All(c => char.IsLetter(c) || c == ' '))
        {
            await DisplayAlertAsync("Error", "El nombre solo debe contener letras", "OK");
            return false;
        }

        //Apellido
        if (string.IsNullOrWhiteSpace(apellido))
        {
            await DisplayAlertAsync("Error", "El campo apellido no debe ser vacio", "OK");
            return false;
        }
        if (!apellido.All(c => char.IsLetter(c) || c == ' '))
        {
            await DisplayAlertAsync("Error", "El apellido solo debe contener letras", "OK");
            return false;
        }

        //Edad: solo numeros y mayor o igual a 16
        if (string.IsNullOrWhiteSpace(edadTexto))
        {
            await DisplayAlertAsync("Error", "El campo edad no debe ser vacío", "OK");
            return false;
        }
        if (!edadTexto.All(char.IsDigit) || !int.TryParse(edadTexto, out int edad))
        {
            await DisplayAlertAsync("Error", "La edad debe ser un número entero", "OK");
            return false;
        }
        if (edad < 16)
        {
            await DisplayAlertAsync("Error", "La edad debe ser mayor o igual a 16 años", "OK");
            return false;
        }
        if (edad > 100)
        {
            await DisplayAlertAsync("Error", "Ingrese una edad válida (maximo 100)", "OK");
            return false;
        }

        //Fecha de inicio: no se permiten fechas pasadas
        DateTime fecha = dpFechaInicio.Date ?? DateTime.Today;
        if (fecha.Date < DateTime.Today)
        {
            await DisplayAlertAsync("Error", "La fecha de inicio no puede ser una fecha pasada", "OK");
            return false;
        }

        //Plan
        if (pkPlan.SelectedIndex == -1)
        {
            await DisplayAlertAsync("Error", "Seleccione un plan", "OK");
            return false;
        }

        //Monto inicial: numerico y entre 120 y 600
        if (string.IsNullOrWhiteSpace(montoTexto))
        {
            await DisplayAlertAsync("Error", "El campo monto inicial no debe ser vacío", "OK");
            return false;
        }
        if (!double.TryParse(montoTexto.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out double monto))
        {
            await DisplayAlertAsync("Error", "El monto inicial debe ser un valor numérico", "OK");
            return false;
        }
        if (monto < MONTO_MINIMO || monto > MONTO_MAXIMO)
        {
            await DisplayAlertAsync("Error", "El monto inicial debe estar entre 120 y 600", "OK");
            return false;
        }

        return true;
    }

    //Convierte el monto inicial aceptando punto o coma decimal
    private double LeerMonto()
    {
        string texto = (txtMontoInicial.Text ?? "0").Trim().Replace(',', '.');
        return double.Parse(texto, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    //Calculo de la cuota mensual
    private double CalcularPagoMensual(double montoInicial, bool casillero)
    {
        double saldo = COSTO_MEMBRESIA - montoInicial;
        double cuota = (saldo / NUMERO_CUOTAS) + (COSTO_MEMBRESIA * PORCENTAJE_CUOTA);

        if (casillero)
            cuota = cuota + VALOR_CASILLERO;

        return cuota;
    }

    private async void btnCalcular_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await ValidarDatos())
                return;

            double montoInicial = LeerMonto();
            double pagoMensual = CalcularPagoMensual(montoInicial, swCasillero.IsToggled);

            txtPagoMensual.Text = pagoMensual.ToString("0.00");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private async void btnVerResumen_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await ValidarDatos())
                return;

            //Validar que se haya calculado el pago mensual
            if (string.IsNullOrWhiteSpace(txtPagoMensual.Text))
            {
                await DisplayAlertAsync("Error", "Primero presione Calcular Pago mensual", "OK");
                return;
            }

            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            int edad = Convert.ToInt32(txtEdad.Text);
            string fecha = (dpFechaInicio.Date ?? DateTime.Today).ToString("dd/MM/yyyy");
            string plan = pkPlan.SelectedItem?.ToString() ?? "";
            bool casillero = swCasillero.IsToggled;
            double montoInicial = LeerMonto();
            double pagoMensual = CalcularPagoMensual(montoInicial, casillero);
            double pagoTotal = montoInicial + (pagoMensual * NUMERO_CUOTAS);

            //Enviar datos a la ventana Resumen
            await Navigation.PushAsync(new vResumen(usuario, nombre, apellido, edad, fecha,
                plan, casillero, montoInicial, pagoMensual, pagoTotal));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}
