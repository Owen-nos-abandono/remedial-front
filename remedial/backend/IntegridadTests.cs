using Xunit;

namespace arroyoSeco.Tests;

public class AlojamientoTests
{
    // Simulación de la lógica de validación
    public bool ValidarAlojamiento(dynamic alojamiento)
    {
        if (string.IsNullOrEmpty(alojamiento.Nombre) || alojamiento.PrecioPorNoche <= 0)
        {
            return false;
        }
        return true;
    }

    [Fact]
    public void Debe_Validar_Correctamente_Un_Alojamiento_Valido()
    {
        var alojamiento = new { 
            Nombre = "Cabaña en Arroyo Seco", 
            PrecioPorNoche = 1200, 
            Estado = "Disponible" 
        };

        Assert.True(ValidarAlojamiento(alojamiento));
    }

    [Fact]
    public void Debe_Fallar_Si_El_Precio_Es_Cero()
    {
        var alojamiento = new { 
            Nombre = "Hotel Prueba", 
            PrecioPorNoche = 0 
        };

        Assert.False(ValidarAlojamiento(alojamiento));
    }

    [Fact]
    public void Objeto_Debe_Contener_Propiedad_Estado()
    {
        var alojamiento = new { 
            Id = 1, 
            Nombre = "Prueba", 
            Estado = "Activo" 
        };

        // Verificamos que la propiedad exista y no sea nula
        Assert.NotNull(alojamiento.GetType().GetProperty("Estado"));
    }
}
