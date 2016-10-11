docker network create --driver overlay --subnet=192.168.1.0/24 vote-staging
docker service create --name postgres --network vote-staging postgres
docker service create --name redis --network vote-staging redis:alpine
docker service create --name mshudx-vote-web --network vote-staging \
    --publish 5000:5000  \
    ersekattila/mshudx.vote.web:94
docker service create --name mshudx-vote-results --network vote-staging \
    --publish 5001:5000 \
    ersekattila/mshudx.vote.results:94
docker service create --name mshudx-vote-worker --network vote-staging \
    ersekattila/mshudx.vote.worker:94

