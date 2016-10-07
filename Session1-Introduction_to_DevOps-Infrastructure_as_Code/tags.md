# Tags

```json
"resources": [    
  {
    "type": "Microsoft.Compute/virtualMachines",
    "apiVersion": "2015-06-15",
    "name": "SimpleWindowsVM",
    "location": "[resourceGroup().location]",
    "tags": {
        "costCenter": "Finance"
    },
    ...
  }
]
```

> List resources by tag

```bash
azure resource list -t costCenter=Finance --json
```

> List tags in resource group

```bash
azure group show -n tag-demo-group
azure group show -n tag-demo-group --json | jq ".tags"
```

> List resource names with tags in a group

```bash
azure resource list --json | jq ".[] | select(.tags.Dept == \"Finance\") | .name"
```
