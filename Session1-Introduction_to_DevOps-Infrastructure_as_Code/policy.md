# Policy

## Policy structure

```json
{
  "if" : {
      <condition> | <logical operator>
  },
  "then" : {
      "effect" : "deny | audit | append"
  }
}
```

## Logical operators

| Operator Name     | Syntax                                                                            |
| :-------------    | :-------------                                                                    |
| Not               | "not" :  &lt;condition  or operator &gt;                                          |
| And               | "allOf" : [ {&lt;condition  or operator &gt;},{&lt;condition  or operator &gt;}]  |
| Or                | "anyOf" : [ {&lt;condition  or operator &gt;},{&lt;condition  or operator &gt;}]  |

## Conditions

| Condition Name    | Syntax                                                    |
| :-------------    | :-------------                                            |
| Equals            | "equals" : "&lt;value&gt;"                                |
| Like              | "like" : "&lt;value&gt;"                                  |
| Contains          | "contains" : "&lt;value&gt;"                              |
| In                | "in" : [ "&lt;value1&gt;","&lt;value2&gt;" ]              |
| ContainsKey       | "containsKey" : "&lt;keyName&gt;"                         |
| Exists            | "exists" : "&lt;bool&gt;"                                 |

## Fields

The following fields and sources are supported:

Fields: **name**, **kind**, **type**, **location**, **tags**, **tags.***, and **property alias**. 

## Effect

Policy supports three types of effect - **deny**, **audit**, and **append**. 

- Deny generates an event in the audit log and fails the request
- Audit generates an event in audit log but does not fail the request
- Append adds the defined set of fields to the request 

## Examples

The below policy denies all requests which don’t have a tag containing
"costCenter" key.

```json
    {
      "if": {
        "not" : {
          "field" : "tags",
          "containsKey" : "costCenter"
        }
      },
      "then" : {
        "effect" : "deny"
      }
    }
```

The below example shows a policy which will deny all requests where location is not North Europe or West Europe.

```json
    {
      "if" : {
        "not" : {
          "field" : "location",
          "in" : ["northeurope" , "westeurope"]
        }
      },
      "then" : {
        "effect" : "deny"
      }
    }
```

## Create a policy definition

```bash
azure policy definition create --name regionPolicyDefinition --description "description" --policy "path-to-policy-json-on-disk"
```

## Assign a policy definition

```bash
azure policy assignment create --name regionPolicyAssignment \
    --policy-definition-id /subscriptions/########-####-####-####-############/providers/Microsoft.Authorization/policyDefinitions/<policy-name> \
    --scope /subscriptions/########-####-####-####-############/resourceGroups/<resource-group-name>
```