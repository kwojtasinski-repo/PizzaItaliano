# get the base node image
FROM node:alpine as builder

# set the working dir for container
WORKDIR /app

# copy the json file first
COPY package.json /app

# install npm dependencies
RUN npm install

COPY . .

# build the folder
RUN npm run build

# Handle Nginx
FROM nginx:1.21.6
COPY --from=builder /app/build /usr/share/nginx/html

RUN useradd -u 5000 app \
    && mkdir -p /var/run/nginx /var/tmp/nginx \
    && chown -R app:app /usr/share/nginx /var/run/nginx /var/tmp/nginx

COPY ./nginx.conf /etc/nginx/nginx.conf
COPY ./default.conf /etc/nginx/conf.d/default.conf

ARG API_DNS=test:1234
RUN sed -i 's/{API_DNS}/'"$API_DNS"'/g' /etc/nginx/conf.d/default.conf

WORKDIR /usr/share/nginx/html
COPY ./env.sh .env.production ./
RUN chmod +x env.sh

USER app:app

EXPOSE 80
CMD ["/bin/sh", "-c", "/usr/share/nginx/html/env.sh && nginx -g \"daemon off;\""]