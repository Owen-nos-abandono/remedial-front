# Configuración del Despliegue Blue/Green (Google Cloud)

Este proyecto cuenta con un flujo automatizado en GitHub Actions para realizar despliegues Blue/Green utilizando dos Máquinas Virtuales (VMs) en Google Cloud:
- **VM de Docker**: Donde se ejecutan los contenedores del frontend.
- **VM de Nginx**: Donde Nginx funciona como proxy inverso y balanceador hacia la VM de Docker.

## 1. Secrets de GitHub Actions
Para que el pipeline funcione, debes ir a la configuración de tu repositorio en GitHub (`Settings` > `Secrets and variables` > `Actions`) y crear los siguientes **Repository secrets**:

| Nombre del Secret | Descripción |
| --- | --- |
| `DOCKER_VM_IP` | La dirección IP pública de tu VM de Docker. |
| `DOCKER_VM_USER` | Tu usuario SSH para la VM de Docker (ej. `ubuntu`). |
| `NGINX_VM_IP` | La dirección IP pública de tu VM de Nginx. |
| `NGINX_VM_USER` | Tu usuario SSH para la VM de Nginx (ej. `ubuntu`). |
| `VM_SSH_PRIVATE_KEY` | El contenido de tu llave privada SSH (`~/.ssh/id_rsa` o similar) que tiene acceso a ambas VMs. *Nota: Asegúrate de añadir la llave pública a `~/.ssh/authorized_keys` en ambas VMs.* |
| `DOCKER_VM_INTERNAL_IP` | *(Opcional pero recomendado)* La IP interna (VPC) de la VM de Docker en Google Cloud. Esto hace que la comunicación entre Nginx y Docker sea por la red privada. |

## 2. Permisos del Repositorio (GitHub Packages)
El pipeline utiliza **GitHub Container Registry (GHCR)** para almacenar las imágenes de Docker. 
Asegúrate de que en los ajustes del repositorio (`Settings` > `Actions` > `General`) bajo "Workflow permissions", esté seleccionado **"Read and write permissions"** para que el token automático pueda subir imágenes a GHCR.

## 3. Preparación de las VMs
### En la VM de Docker
- Debes tener `docker` instalado y tu usuario debe pertenecer al grupo `docker`.
- Abre los puertos `8081` y `8082` en el Firewall de Google Cloud para esta VM (especialmente para la IP interna de la VM de Nginx).

### En la VM de Nginx
- Debes tener `nginx` instalado.
- Tu usuario debe poder ejecutar `sudo nginx -t` y `sudo systemctl reload nginx` sin contraseña (o debes ejecutar como root). Puedes configurarlo editando el archivo `/etc/sudoers`.

## 4. ¿Cómo Funciona el Despliegue?
1. Al hacer un `push` a la rama `main` en los archivos del frontend, se dispara el GitHub Action.
2. Construye la imagen usando el `Dockerfile` y la sube a GHCR.
3. Se conecta a la **VM de Docker**, descarga la imagen, revisa si está corriendo el contenedor `blue` o `green` y lanza el nuevo en el puerto inactivo (8081 o 8082).
4. Se conecta a la **VM de Nginx** y actualiza el proxy para que apunte al nuevo puerto activo y recarga la configuración.
