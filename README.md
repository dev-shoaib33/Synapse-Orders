Overview
The Synapse Orders project is designed to manage order processing for medical equipment. It fetches orders from an API, processes them to identify delivered items, sends delivery alerts, and updates order records accordingly.

The project is structured as follows
Synapse.Orders Main project containing the application logic.
Synapse.Orders.Tests Unit tests for testing various functionalities of the application.

Framework .NET 8
Unit Testing xUnit

Mock API (JSON Server)
Setup Install JSON Server 
Start JSON Server Create a db.json file with mock data and run JSON Server
json-server db.json --port port_no --start listening on port port_no 

Run --dotnet Synapse.Orders 
Test--dotnet test


