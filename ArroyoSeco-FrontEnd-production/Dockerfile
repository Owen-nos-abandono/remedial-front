# Etapa 1: Build
FROM node:18-alpine AS build-step
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Etapa 2: Nginx
FROM nginx:alpine
COPY --from=build-step /app/dist/ArroyoSeco-FrontEnd-production /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
