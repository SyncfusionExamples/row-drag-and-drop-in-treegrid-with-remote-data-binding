# row-drag-and-drop-in-treegrid-with-remote-data-binding

## Repository Description
This repository demonstrates remote data binding for ASP.NET Core TreeGrid using url adaptor. It showcases fetching data dynamically and enabling server-side Row drag and drop action.

## Overview
Remote data binding using a url adaptor allows the TreeGrid to fetch and display data on demand, reducing memory usage and supporting scalable handling of large datasets.

## Features
- url adaptor: connect to url adaptor endpoints
- Dynamic Binding: fetch from remote sources
- Server-Side drag and drop: reordered data


## Prerequisites
- .NET 6.0 or higher
- Visual Studio or VS Code
- ASP.NET Core SDK
- C# knowledge
- REST endpoint knowledge

## Installation
1. Clone repository
2. Navigate to project directory
3. Run `dotnet restore`
4. Execute `dotnet build`
5. Run `dotnet run`

## Usage
Implement remote data binding:
1. Create url adaptor returning JSON
2. Configure treegrid with url adaptor
3. Set drag and drop handling in server end.

## Configuration
- API Endpoint: url adaptor
- Response Format: JSON
- DragDrop Parameters: Reordered data

## Documentation
For detailed information and configuration options:

https://ej2.syncfusion.com/aspnetcore/documentation/tree-grid/row/row-drag-and-drop#drag-and-drop-within-tree-grid

https://ej2.syncfusion.com/aspnetcore/documentation/tree-grid/data-binding/remote-data#loadchildondemand


