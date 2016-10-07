docker network create --driver overlay --subnet=192.168.0.0/24 vote-prod
docker service create --name postgres-prod --network vote-prod postgres
docker service create --name redis-prod --network vote-prod redis:alpine
docker service create --name web-prod --network vote-prod \
    --env ConnectionStrings__redis="redis-prod" \
    --publish 80:5000  \
    ersekattila/mshudx.vote.web:94
docker service create --name results-prod --network vote-prod \
    --env ConnectionStrings__pgsql="Server=postgres-prod;Username=postgres;" \
    --publish 8080:5000 \
    ersekattila/mshudx.vote.results:94
docker service create --name worker-prod --network vote-prod \
    --env ConnectionStrings__pgsql="Server=postgres-prod;Username=postgres;" \
    --env ConnectionStrings__redis="redis-prod" \
    ersekattila/mshudx.vote.worker:94
