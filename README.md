# Blackbird.io HTTP

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->


The HTTP application provides a set of HTTP actions to be used within Blackbird workflows. It supports various HTTP methods, including GET, GET File, POST, PUT, PATCH, and DELETE. Each action allows you to configure custom headers, query parameters, and JSON payloads (when applicable). The actions include JSON validation to ensure that headers, query parameters, and request bodies are correctly formatted before executing the HTTP request.

## Connecting

1. Navigate to Apps and search for HTTP.
2. Click _Add Connection_.
3. Enter a name for your connection for future reference, e.g. “My HTTP connection”.
4. Fill in the Base URL, the root URL of the API you want to call (for example https://api.example.com/v1).
4. Click _Connect_.

## How connection validation works

When you click **Connect**, the platform runs a quick validation to confirm the host is reachable:

- The app takes the **Base URL** you entered and extracts the **host (domain)** part  
  (e.g. `https://api.example.com/v1` -> `api.example.com`).
- It then performs a **DNS lookup** for that host.
- If DNS resolves to at least one IP address, the connection is considered **valid**.
- If DNS fails (invalid URL, unknown host, DNS error), the connection is marked **invalid** and you’ll see **“Ping failed”**.

> **Note:** This validation does **not** require the base URL to return `200 OK`. Many APIs return `401/403/404` for `/`, so the HTTP app validates the connection by checking that the domain exists rather than requiring a successful API response.

## Actions

- **GET**: Executes an HTTP GET request with optional headers and query parameters.
- **GET File**: Executes an HTTP GET request to download files, integrating with Blackbird’s file management system.
- **POST**: Executes an HTTP POST request with a JSON body. Supports file uploads.
- **PUT**: Executes an HTTP PUT request with support for headers, query parameters, and a JSON body.
- **PATCH**: Executes an HTTP PATCH request with support for headers, query parameters, and a JSON body.
- **DELETE**: Executes an HTTP DELETE request with optional headers and query parameters.

## Events

- **Webhook — On request received**
  - When a bird is published, a unique URL is generated. Requests to this URL trigger the flight.
  - POST requests: the event returns the request body.
  - GET requests: the event returns query parameters as a string (e.g. ?param1=value1&param2=value2).
  - Authentication is mandatory: set a token in the event parameters and include the same value in the `Authorization` header of requests to the URL to avoid unauthorized triggers.
  - Example header: Authorization: my-secret-token

## Use cases

- Use this app for simple API interactions where advanced configurations, complex authentication schemes, or detailed error handling are not required.
- For complex cases or heavy-duty HTTP interactions, it is recommended to use specialized HTTP libraries or custom implementations.


## Key features

- **Simplicity:** Ideal for straightforward, light-weight HTTP requests without the need for complex logic.
- **Quick Integration:** Easily integrates into your workflows, allowing you to automate routine tasks without extensive development.
- **Prototyping and Testing:** Perfect for testing endpoints or handling temporary, simple API calls.


## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
