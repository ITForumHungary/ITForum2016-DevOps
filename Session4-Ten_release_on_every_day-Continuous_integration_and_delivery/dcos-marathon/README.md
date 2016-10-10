### DCOS commands
##### First you have to login
```
dcos config set core.dcos_url http://localhost:8080
dcos auth login
```
##### After that you can add applications with this command
```
dcos marathon app add ./mshudx-vote-worker.json
```
##### Check for an application's health with this command
```
dcos marathon app show mshudx-vote-worker | grep tasks
```
