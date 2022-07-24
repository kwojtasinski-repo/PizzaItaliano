# get the base node image
FROM node:18.6.0-alpine3.15 as builder

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
COPY ./default.conf ./default.conf.template /etc/nginx/conf.d

RUN chown -R app:app /etc/nginx/conf.d

WORKDIR /usr/share/nginx/html
COPY ./env.sh .env.production ./
RUN chmod +x env.sh

USER app:app

EXPOSE 80
CMD ["/bin/sh", "-c", "/usr/share/nginx/html/env.sh && envsubst '${API_DNS}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g \"daemon off;\""]