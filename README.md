# How to run as a container
```CMD
git clone https://github.com/voltwu/C-Sharp-Web-VoltTools.git

cd C-Sharp-Web-VoltTools

docker build --tag volttools --file voltTools/Dockerfile .

docker run --publish 5000:80 --detach --name volttools volttools:latest
```
