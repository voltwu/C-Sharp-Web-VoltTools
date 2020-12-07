# Use the offical image
FROM nginx:latest

# copy the necessary files from your host to your container
COPY index.html /usr/share/nginx/html
COPY linux.png /usr/share/nginx/html

# Inform Docker that the container is listening on the specified port at runtime.
EXPOSE 80 443

# Specify the run command within the container
CMD ["nginx", "-g", "daemon off;"]
