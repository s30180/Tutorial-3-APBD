# Tutorial 3 - APBD

Console application in C# (.NET 9.0) for simulating loading containers onto a container ship.

## Description

The application includes three types of containers:

- LiquidContainer – can hold hazardous and non-hazardous liquids
- GasContainer – includes pressure and keeps 5% of cargo when unloaded
- RefrigeratedContainer – has temperature and product type

Each container has basic properties like serial number, height, depth, tare weight, max payload, etc.

There's also a ContainerShip class that can load/unload containers and check limits.

## Features

- Serial number generation
- Capacity checks and exceptions
- Hazard notifications
- Cargo unloading rules
- Console output with ship and container info


## Author

Student ID: s30180  
Tutorial 3 – APBD (2025)
