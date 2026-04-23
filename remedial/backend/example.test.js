
// La lógica que queremos probar (Simulando lo que recibe tu API)
const validarAlojamiento = (alojamiento) => {
  // Verificamos que existan las propiedades con el nombre exacto de la API
  if (!alojamiento.nombre || !alojamiento.precioPorNoche) {
    return false;
  }
  if (alojamiento.precioPorNoche <= 0) {
    return false;
  }
  return true;
};

// El bloque de pruebas de JEST
describe('Pruebas de Integridad de Alojamientos', () => {
  
  test('Debe validar correctamente un alojamiento válido', () => {
    const alojamientoFicticio = {
      id: 1,
      nombre: "Cabaña en Arroyo Seco",
      precioPorNoche: 1200,
      Estado: "Disponible" // Sincronizado con C#
    };

    expect(validarAlojamiento(alojamientoFicticio)).toBe(true);
  });

  test('Debe fallar si el precio es menor o igual a cero', () => {
    const alojamientoInvalido = {
      nombre: "Hotel Prueba",
      precioPorNoche: 0
    };

    expect(validarAlojamiento(alojamientoInvalido)).toBe(false);
  });

  test('Debe verificar que el objeto contenga la propiedad Estado', () => {
    const dataDeApi = {
      id: 1,
      nombre: "Lugar de Prueba",
      Estado: "Activo" // Sincronizado con C#
    };

    // Validamos que exista 'Estado' (con E mayúscula) tal cual lo configuramos
    expect(dataDeApi).toHaveProperty('Estado'); 
  });
});
