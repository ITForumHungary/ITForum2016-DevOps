docker network create --driver bridge vote-dev
docker run -d --name redis --network vote-dev redis:alpine
docker run -d --name postgres --network vote-dev postgres