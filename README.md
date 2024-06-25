# Synapse Orders Project

## Overview

The Synapse Orders project is designed to manage order processing for medical equipment. It fetches orders from an API, processes them to identify delivered items, sends delivery alerts, and updates order records accordingly.

## Project Structure

- **Synapse.Orders**: Main project containing the application logic.
- **Synapse.Orders.Tests**: Unit tests for testing various functionalities of the application.
- **db.json File**
I used this JSON File for Data. Each time the project runs, it fetches data from db.json. If an order's status is "delivered," it dispatches a notification alert with the updated notification value. The updated data is then written to the file in the updated array.

## Frameworks and Tools

- **Framework**: .NET 8.0
- **Unit Testing**: xUnit
- **Mock API**: JSON Server

## Setup

### Install JSON Server

To install JSON Server, run the following command in the package manager console:

```bash
npm install -g json-server
```

Start JSON Server using this command
```bash
npx json-server --watch -p 3000 db.json
```

