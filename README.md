# Blackbird.io HTTP

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->


The HTTP application provides a set of HTTP actions to be used within Blackbird workflows. It supports various HTTP methods, including GET, GET File, POST, PUT, PATCH, and DELETE. Each action allows you to configure custom headers, query parameters, and JSON payloads (when applicable). The actions include JSON validation to ensure that headers, query parameters, and request bodies are correctly formatted before executing the HTTP request.

## Actions

- **GET**: Executes an HTTP GET request with optional headers and query parameters.
- **GET File**: Executes an HTTP GET request to download files, integrating with Blackbird’s file management system.
- **POST**: Executes an HTTP POST request with a JSON body.
- **PUT**: Executes an HTTP PUT request with support for headers, query parameters, and a JSON body.
- **PATCH**: Executes an HTTP PATCH request with support for headers, query parameters, and a JSON body.
- **DELETE**: Executes an HTTP DELETE request with optional headers and query parameters.

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
