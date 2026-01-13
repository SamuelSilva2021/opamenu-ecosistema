# Stage 1: Build
FROM node:20-alpine AS builder

# Build Arguments (Environment variables passed from Railway)
ARG VITE_API_BASE_URL
ARG VITE_APP_TITLE
ARG VITE_APP_VERSION
ARG VITE_DEBUG_MODE
ARG VITE_LOG_LEVEL

# Set Environment Variables
ENV VITE_API_BASE_URL=$VITE_API_BASE_URL
ENV VITE_APP_TITLE=$VITE_APP_TITLE
ENV VITE_APP_VERSION=$VITE_APP_VERSION
ENV VITE_DEBUG_MODE=$VITE_DEBUG_MODE
ENV VITE_LOG_LEVEL=$VITE_LOG_LEVEL

WORKDIR /app

# Copy package files (adjusting for monorepo root context)
COPY access-control-web/package*.json ./access-control-web/

WORKDIR /app/access-control-web

# Install dependencies
RUN npm ci

# Copy source code
COPY access-control-web/ .

# Build
RUN npm run build

# Stage 2: Serve
FROM nginx:alpine

# Copy nginx config
COPY access-control-web/nginx.conf /etc/nginx/conf.d/default.conf

# Copy build artifacts
COPY --from=builder /app/access-control-web/dist /usr/share/nginx/html

ENV PORT=80

CMD ["/bin/sh", "-c", "sed -i 's/__PORT__/'\"$PORT\"'/g' /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
