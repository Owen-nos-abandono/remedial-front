# Etapa 1: Construcción (Build)
FROM node:20-alpine AS build

WORKDIR /app

# Copiar archivos de configuración de paquetes
COPY package*.json ./

# Instalar dependencias
RUN npm ci

# Copiar el resto del código
COPY . .

# Construir la aplicación para producción
RUN npm run build --configuration=production

# Etapa 2: Servir con Nginx (Producción)
FROM nginx:1.25-alpine

# Eliminar el sitio por defecto de Nginx
RUN rm -rf /usr/share/nginx/html/*

# Copiar configuración personalizada
COPY nginx-custom.conf /etc/nginx/conf.d/default.conf

# Copiar los archivos compilados de Angular
# El builder "application" de Angular 17+ guarda en dist/{proyecto}/browser
# Si falla, puede que esté en dist/arroyo-seco.
COPY --from=build /app/dist/arroyo-seco/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
