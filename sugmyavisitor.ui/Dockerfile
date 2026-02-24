# ---------- Stage 1 : Build Angular ----------
FROM node:18-alpine AS build

WORKDIR /app

COPY package*.json ./

# dependency conflict ignore
RUN npm config set legacy-peer-deps true

# install deps
RUN npm install

RUN npm install @angular/localize@15 --save

COPY . .

# angular build (production)
RUN npm run build -- --configuration production


# ---------- Stage 2 : Nginx Serve ----------
FROM nginx:alpine

RUN rm -rf /usr/share/nginx/html/*

# dist/project-name hota hai → isliye * use
COPY --from=build /app/dist/* /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

