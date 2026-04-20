# Etapa 1: Construcción
FROM node:18-alpine as build-step
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Etapa 2: Servidor Nginx
FROM nginx:alpine
# Ajusta "arroyo-seco-frontend" por el nombre que viene en tu angular.json
COPY --from=build-step /app/dist/arroyo-seco-frontend /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
#Versión Final
