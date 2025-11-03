# Build stage
FROM node:20-alpine AS build

WORKDIR /app
COPY package*.json ./
RUN npm install

# Copy source files
COPY . .

# Create dummy certificate files to skip certificate generation
RUN mkdir -p /root/.aspnet/https/
RUN echo "dummy" > /root/.aspnet/https/resumehxapp.client.pem
RUN echo "dummy" > /root/.aspnet/https/resumehxapp.client.key

ENV NODE_ENV=production
RUN npm run build

# Production stage
FROM nginx:alpine

# Copy the built assets to nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx configuration if needed
# COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 3000

# Use custom nginx configuration to handle SPA routing and API proxy
RUN echo 'server { \
    listen 3000; \
    \
    location / { \
        root /usr/share/nginx/html; \
        try_files $uri $uri/ /index.html; \
    } \
    \
    location /api/ { \
        proxy_pass http://backend:8080/api/; \
        proxy_http_version 1.1; \
        proxy_set_header Upgrade $http_upgrade; \
        proxy_set_header Connection "upgrade"; \
        proxy_set_header Host $host; \
        proxy_cache_bypass $http_upgrade; \
    } \
    \
    location /weatherforecast { \
        proxy_pass http://backend:8080; \
        proxy_http_version 1.1; \
        proxy_set_header Upgrade $http_upgrade; \
        proxy_set_header Connection "upgrade"; \
        proxy_set_header Host $host; \
        proxy_cache_bypass $http_upgrade; \
    } \
}' > /etc/nginx/conf.d/default.conf

CMD ["nginx", "-g", "daemon off;"]