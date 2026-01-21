# Build
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY opamenu-painel/package*.json ./opamenu-painel/

WORKDIR /app/opamenu-painel

RUN npm install

# Copy source code
COPY opamenu-painel/ .

RUN npm run build

# Runtime
FROM nginx:alpine

# Copia build
COPY --from=build /app/opamenu-painel/dist /usr/share/nginx/html

# Template do nginx
COPY opamenu-painel/nginx.conf.template /etc/nginx/templates/default.conf.template

# Railway injeta PORT automaticamente
ENV NGINX_ENVSUBST_OUTPUT_DIR=/etc/nginx/conf.d

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]
