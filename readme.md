# Custom Identity Provider with Azure Entra ID Trust

This README describes how to use a custom Identity Provider that has a trust relationship with an Entra ID tenant to exchange short-lived tokens for an Entra Auth token, and use it to access Azure resources with Managed Identity permissions.

## Prerequisites

- You need access to a Managed Identity with at least `Reader` permissions on the subscription.
- A working custom Identity Provider that issues short-lived tokens.
- A trust relationship established between your custom Identity Provider and your Entra ID tenant.

---

## 1. Retrieve a Short-Lived Token from the Custom Identity Provider

First, you retrieve a short-lived token from the custom Identity Provider.

```bash
curl -X GET "https://federationtest-azcagddggmawbwf6.westeurope-01.azurewebsites.net/token"
```

## 2. Exchange the Token for an Entra Auth Token

Once you have the short-lived token, you can exchange it for an Entra Auth token by making the following request:

```bash
curl -X POST "https://login.microsoftonline.com/49a79654-5983-4e18-a5d4-ef31e4742287/oauth2/v2.0/token" \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -d "client_id=96fe28a0-a855-4fc4-8024-d3bc78bf7cb8" \
     -d "grant_type=client_credentials" \
     -d "scope=https://management.azure.com/.default" \
     -d "client_assertion=<token-von-meinem-identitiy-provider>" \
     -d "client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer"
```

client_id: The client ID of the App Registration.
grant_type: client_credentials.
scope: https://management.azure.com/.default (to access Azure resources).
client_assertion: The short-lived token retrieved from your Identity Provider.
client_assertion_type: Set to urn:ietf:params:oauth:client-assertion-type:jwt-bearer.

## 3. Use the Entra Auth Token to Access Azure Resources

Once you have the Entra Auth token, you can use it to interact with Azure resources, just as the Managed Identity would. For example, to list subscriptions:

```bash
curl -X GET "https://management.azure.com/subscriptions?api-version=2022-12-01" \
     -H "Authorization: Bearer <token-vom-entra-id>" \
     -H "Content-Type: application/json":w
```

## Notes
The Managed Identity should have the necessary permissions (e.g., Reader) to access the resources in Azure.

