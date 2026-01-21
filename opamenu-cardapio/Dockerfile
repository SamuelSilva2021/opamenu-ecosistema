# Build
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY opamenu-cardapio/package*.json ./opamenu-cardapio/

WORKDIR /app/opamenu-cardapio

RUN npm install

# Copy source code
COPY opamenu-cardapio/ .

RUN npm run build

# Runtime
FROM nginx:alpine

# Copia build
COPY --from=build /app/opamenu-cardapio/dist /usr/share/nginx/html

# Template do nginx
COPY opamenu-cardapio/nginx.conf.template /etc/nginx/templates/default.conf.template

# Railway injeta PORT automaticamente
ENV NGINX_ENVSUBST_OUTPUT_DIR=/etc/nginx/conf.d

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]