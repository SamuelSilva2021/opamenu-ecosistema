# Build
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Runtime
FROM nginx:alpine

# Copia build
COPY --from=build /app/dist /usr/share/nginx/html

# Template do nginx
COPY nginx.conf.template /etc/nginx/templates/default.conf.template

# Railway injeta PORT automaticamente
ENV NGINX_ENVSUBST_OUTPUT_DIR=/etc/nginx/conf.d

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]
