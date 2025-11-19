# BeerMachineApi

BeerMachineApi is a C# Web API for managing and monitoring a beer machine. It provides endpoints to manage batches, and to collect basic metrics for monitoring. 
The project is makes use of C# entity frame work and the Opc.UaFx libary and is intended to be run as a standalone Web API.

## Prerequisites

- 

## Quick Start (local)

1. Clone the repository

   git clone https://github.com/GustavSkou/BeerMachineApi.git

   cd BeerMachineApi

3. Restore dependencies and build

   dotnet restore
   dotnet build

4. Run the API

   dotnet run --launch-profile http

   The API will be available at http://localhost:5107

5. Explore the API
   
   /status/batch

   /status/inventory

   /status/queue

   /status/machine
   

   /command
   
## API Overview
Use .http for a overview of how get and post methods works
please install some http testing extension for this, like "httpYac - Rest Client"

