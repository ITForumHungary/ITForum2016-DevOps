docker service create --name postgres postgres
docker service create --name redis redis:alpine
docker service create --name mshudx-vote-web \
    --publish 5000:5000  \
    ersekattila/mshudx.vote.web:94
docker service create --name mshudx-vote-results \
    --publish 5001:5000 \
    ersekattila/mshudx.vote.results:94
docker service create --name mshudx-vote-worker \
    ersekattila/mshudx.vote.worker:94

