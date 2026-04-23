import http from 'k6/http';
import { check, sleep } from 'k6';

// Configuración de la prueba de carga
export const options = {
  vus: 10, // 10 Usuarios Virtuales
  duration: '10s', // Durante 10 segundos
  thresholds: {
    http_req_failed: ['rate<0.01'], // Menos del 1% de errores permitidos
    http_req_duration: ['p(95)<500'], // El 95% de las peticiones deben tardar menos de 500ms
  },
};

export default function () {
  // En GitHub Actions inyectaremos la URL del contenedor (ej: http://localhost:8081)
  const targetUrl = __ENV.TARGET_URL || 'http://localhost';

  // Realizamos la petición GET a la raíz
  const res = http.get(targetUrl);

  // Verificamos que responda HTTP 200 OK
  check(res, {
    'status es 200': (r) => r.status === 200,
    // Puedes agregar más validaciones aquí, por ejemplo si pruebas un endpoint de tu backend:
    // 'body contiene algo': (r) => r.body.includes('texto esperado'),
  });

  sleep(1); // Esperar 1 segundo entre peticiones del mismo VU
}
