# Keynote demo

## prerequisites

* A [Visual Studio Team Services](https://visualstudio.com) account
* Commited [demo application source code](./Session4-Ten_release_on_every_day-Continuous_integration_and_delivery/Asset3-Demo_application)
* An [Azure](https://azure.microsoft.com) subscription. *MSDN or Free Trial is sufficient*
* Deployed and configured [Linux w/ Docker & .NetCore Build Agent](./Session4-Ten_release_on_every_day-Continuous_integration_and_delivery/Asset1-Ubuntu_VSTS_Build_Agent_Docker_NodeJS_DotNetCore) 
* Deployed and configured [Docker Swarm cluster](./Session4-Ten_release_on_every_day-Continuous_integration_and_delivery/Asset2-Swarm_cluster)

## deploy staging environment

> [deploy-staging.sh](../blob/master/Session4-Ten_release_on_every_day-Continuous_integration_and_delivery/deploy-staging.sh)

```bash
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
```


## deploy production environment

> [deploy-production.sh](../blob/master/Session4-Ten_release_on_every_day-Continuous_integration_and_delivery/deploy-production.sh)

```bash
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
```
