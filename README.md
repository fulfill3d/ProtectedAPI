
# ProtectedAPI

ProtectedAPI is a secure API service built using Azure Functions and .NET, designed to provide protected endpoints with JWT authentication. It integrates with Azure AD B2C for user authentication and authorization, ensuring secure access to API resources.

## Table of Contents

1. [Introduction](#introduction)
2. [Features](#features)
3. [Tech Stack](#tech-stack)
4. [Usage](#usage)
5. [Configuration](#configuration)

## Introduction

ProtectedAPI provides a secure interface for handling requests that require user authentication and authorization. It uses JWT tokens to validate and authorize users, ensuring that only authenticated users can access protected resources.

## Features

- **JWT Authentication:** Secure endpoints with JSON Web Token (JWT) authentication.
- **Azure AD B2C Integration:** Integrates with Azure AD B2C for identity management.
- **Scope-Based Authorization:** Authorize users based on predefined scopes.

## Tech Stack

- **Backend:** .NET 8 (Isolated Worker), Azure Functions v4
- **Authentication:** Azure AD B2C, JWT tokens
- **Configuration & Secrets Management:** Azure App Configuration, Azure Key Vault
- **Dependency Injection:** Used for service registrations and configurations

## Usage

1. **Deploy the functions to Azure Functions.**
2. **Configure Azure App Configuration and Key Vault with necessary secrets.**
3. **Ensure Azure AD B2C is correctly set up and integrated.**

## Configuration

### TokenValidationOptions

- **MetadataUrl:** URL to the metadata document used for token validation.
- **ClientId:** The client ID for the Azure AD B2C application.
- **Issuer:** The expected issuer for the tokens.

```csharp
public class TokenValidationOptions
{
    public string MetadataUrl { get; set; }
    public string ClientId { get; set; }
    public string Issuer { get; set; }
}
```

### AuthorizationScope

- **TestScope:** The scope required for accessing the protected endpoint.

```csharp
public class AuthorizationScope
{
    public string TestScope { get; set; }
}
```
