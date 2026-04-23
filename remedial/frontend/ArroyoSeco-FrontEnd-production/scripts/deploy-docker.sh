#!/bin/bash
# scripts/deploy-docker.sh
# Este script se ejecuta en la VM de Docker.
# Recibe la imagen de Docker a desplegar como argumento.

set -e

IMAGE=$1

if [ -z "$IMAGE" ]; then
  echo "Error: Se debe proporcionar la imagen de Docker como argumento."
  exit 1
fi

echo "Autenticando y descargando la nueva imagen: $IMAGE"
# La autenticación de GHCR se realiza asumiendo que el runner de GH Actions 
# ya hizo `docker login` o el entorno tiene los permisos.
docker pull $IMAGE

# Identificar el entorno activo
# Comprobamos si el contenedor "blue" está corriendo
IS_BLUE_RUNNING=$(docker ps -q -f name=frontend-blue)

if [ -n "$IS_BLUE_RUNNING" ]; then
  echo "Entorno actual es BLUE. Desplegando en GREEN..."
  NEW_ENV="green"
  OLD_ENV="blue"
  NEW_PORT=8082
else
  echo "Entorno actual es GREEN (o ninguno). Desplegando en BLUE..."
  NEW_ENV="blue"
  OLD_ENV="green"
  NEW_PORT=8081
fi

NEW_CONTAINER_NAME="frontend-$NEW_ENV"
OLD_CONTAINER_NAME="frontend-$OLD_ENV"

# Detener y eliminar el contenedor inactivo si existía
if [ -n "$(docker ps -aq -f name=$NEW_CONTAINER_NAME)" ]; then
  echo "Limpiando contenedor anterior $NEW_CONTAINER_NAME..."
  docker stop $NEW_CONTAINER_NAME || true
  docker rm $NEW_CONTAINER_NAME || true
fi

# Levantar el nuevo contenedor
echo "Iniciando contenedor $NEW_CONTAINER_NAME en el puerto $NEW_PORT..."
docker run -d --name $NEW_CONTAINER_NAME -p $NEW_PORT:80 --restart unless-stopped $IMAGE

# Esperar a que esté sano
echo "Esperando a que el contenedor $NEW_CONTAINER_NAME esté listo..."
sleep 5

# Verificación simple con curl
if curl -s -f http://localhost:$NEW_PORT > /dev/null; then
  echo "✅ El contenedor $NEW_CONTAINER_NAME respondió correctamente."
else
  echo "❌ Error: El contenedor $NEW_CONTAINER_NAME no responde."
  echo "Revirtiendo (deteniendo contenedor fallido)..."
  docker stop $NEW_CONTAINER_NAME
  docker rm $NEW_CONTAINER_NAME
  exit 1
fi

echo "Despliegue de $NEW_ENV completado con éxito. Se debe cambiar el Nginx ahora."
# Imprimimos el nuevo entorno para que el pipeline lo lea
echo "::set-output name=active_env::$NEW_ENV"
echo "NEW_ENV=$NEW_ENV" > /tmp/deploy_status.txt
