# Etapa 1: Construcción (Build)
FROM node:18-alpine AS build-step
WORKDIR /app

# Copiamos archivos de dependencias primero para aprovechar el caché de Docker
COPY package*.json ./
RUN npm install

# Copiamos todo el código y construimos
COPY . .
RUN npm run build

# Etapa 2: Servidor Web (Nginx)
FROM nginx:alpine
# IMPORTANTE: Aquí usamos 'ArroyoSeco-FrontEnd-production' o el nombre que venga en tu angular.json
COPY --from=build-step /app/dist/ArroyoSeco-FrontEnd-production /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
