#!/bin/bash
# scripts/switch-nginx.sh
# Este script se ejecuta en la VM de Nginx.
# Requerimientos: Se necesitan permisos de sudo sin contraseña para nginx o ejecutar como root.

set -e

ACTIVE_ENV=$1
DOCKER_VM_IP=$2

if [ -z "$ACTIVE_ENV" ] || [ -z "$DOCKER_VM_IP" ]; then
  echo "Uso: $0 <active_env: blue|green> <docker_vm_ip>"
  exit 1
fi

if [ "$ACTIVE_ENV" == "blue" ]; then
  ACTIVE_PORT=8081
elif [ "$ACTIVE_ENV" == "green" ]; then
  ACTIVE_PORT=8082
else
  echo "Entorno inválido. Debe ser 'blue' o 'green'."
  exit 1
fi

NGINX_CONF="/etc/nginx/conf.d/frontend.conf"

echo "Configurando Nginx para apuntar al entorno $ACTIVE_ENV (Puerto: $ACTIVE_PORT en $DOCKER_VM_IP)..."

# Crear o sobreescribir la configuración del proxy inverso en Nginx
sudo bash -c "cat > $NGINX_CONF <<EOF
server {
    listen 80;
    server_name _; # Cambia esto por tu dominio real si tienes uno

    location / {
        proxy_pass http://$DOCKER_VM_IP:$ACTIVE_PORT;
        proxy_set_header Host \\\$host;
        proxy_set_header X-Real-IP \\\$remote_addr;
        proxy_set_header X-Forwarded-For \\\$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \\\$scheme;
    }
}
EOF"

echo "Verificando sintaxis de Nginx..."
sudo nginx -t

echo "Recargando Nginx..."
sudo systemctl reload nginx

echo "✅ Tráfico redirigido exitosamente al entorno $ACTIVE_ENV."
