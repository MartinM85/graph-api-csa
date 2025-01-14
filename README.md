# Graph C# SDK and security attributes

Manage custom security attributes via Graph C# SDK

## Introduction

This sample demonstrates how to 

- manage attribute sets
- manage custom security attributes 
- assign custom security attributes to users
- read custom security attributes from users
- assign custom security attributes to service principals
- read custom security attributes from service principals

via Graph C# SDK.

## Prerequisites

Register Entra ID applications

- Single tenant application with client secret and the `CustoSecAttributeDefinition.ReadWrite.All` application permission to manage attribute sets and custom security attributes
- Single tenant application with client secret and the `User.Read.All`, `CustomSecAttributeAssignment.ReadWrite.All` application permissions to assign security attributes to the users and read assigned attributes
- Single tenant application with client secret and the `Application.Read.All`, `CustomSecAttributeAssignment.ReadWrite.All` application permissions to assign security attributes to the service principals and read assigned attributes

You can of course combine these permissions into a single application registration.

**Be aware that attribute sets and custom security attributes cannot be removed**