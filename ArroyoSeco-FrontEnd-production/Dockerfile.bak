# Etapa 1: Construcción
FROM node:18-alpine AS build-step
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Etapa 2: Servidor Nginx
FROM nginx:alpine
# REVISA: Cambia 'arroyo-seco-frontend' por el nombre en tu angular.json si es distinto
COPY --from=build-step /app/dist/arroyo-seco-frontend /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
