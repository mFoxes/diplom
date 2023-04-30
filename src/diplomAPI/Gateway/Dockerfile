FROM nginx
RUN apt-get update && apt-get install apache2-utils -y
RUN htpasswd -b -c /etc/nginx/.htpasswd admin 'XIst&S9$WE4!'

COPY nginx.conf /etc/nginx/
