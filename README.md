# row-drag-and-drop-in-treegrid-with-remote-data-binding

## Repository Description
This repository demonstrates remote data binding for ASP.NET Core TreeGrid using Url adaptor. It showcases fetching data dynamically and enabling server-side Row drag and drop action.

## Overview
Remote data binding with URL API efficiently loads large datasets without requiring all data in memory, enabling real-time updates.

## Features
- URL API: connect to URL endpoints
- Dynamic Binding: fetch from remote sources
- Server-Side drag and drop: reordered data
- Real-Time Updates: dynamic content

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
1. Create URL API endpoint returning JSON
2. Configure treegrid with API URL
3. Set drag and drop handling in server end.

## Configuration
- API Endpoint: API URL
- Response Format: JSON
- DragDrop Parameters: Reordered data

## Documentation
For detailed information and configuration options:

https://ej2.syncfusion.com/aspnetcore/documentation/tree-grid/row/row-drag-and-drop#drag-and-drop-within-tree-grid

https://ej2.syncfusion.com/aspnetcore/documentation/tree-grid/data-binding/remote-data#loadchildondemand


