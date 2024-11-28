# Sentimatrix

A real-time email sentiment analysis application with a WPF desktop client and .NET Core backend. The application processes emails and analyzes their sentiment using advanced natural language processing.

## Features

- Real-time email sentiment analysis
- Desktop client with modern WPF interface
- SignalR for real-time updates
- Swagger API documentation
- Email content processing and analysis

## Tech Stack

### Backend (.NET 6.0)
- ASP.NET Core Web API
- SignalR for real-time communication
- Swagger/OpenAPI for API documentation
- HtmlAgilityPack for HTML processing
- Newtonsoft.Json for JSON handling

### Frontend (WPF .NET 6.0)
- WPF (Windows Presentation Foundation)
- SignalR Client for real-time updates
- Modern UI with sidebar design
- Newtonsoft.Json for JSON handling

## Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or later (recommended)
- Windows 10/11 for running the WPF client

## Setup Instructions

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

The API will start at `https://localhost:7777` and `http://localhost:5000`
- Swagger UI will be available at `https://localhost:7777/swagger`

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the WPF application:
   ```bash
   dotnet run
   ```

## Project Structure

### Backend
```
backend/
├── Controllers/     # API endpoints
├── Hubs/           # SignalR hubs for real-time communication
├── Models/         # Data models and DTOs
├── Services/       # Business logic and services
├── Program.cs      # Application entry point and configuration
└── *.json          # Sample email data files
```

### Frontend
```
frontend/
├── App.xaml        # Application resources and startup
├── MainWindow.xaml # Main application window UI
└── *.cs           # Code-behind files
```

## API Endpoints

The backend provides several API endpoints through its controllers:
- Swagger UI provides detailed API documentation
- Real-time updates through SignalR hub
- Email processing and sentiment analysis endpoints

## Development

### Backend Development
1. Open `SentimatrixAPI.csproj` in Visual Studio or your preferred IDE
2. API endpoints are defined in the Controllers directory
3. Real-time communication is handled through SignalR hubs
4. Services directory contains the business logic

### Frontend Development
1. Open `WpfSidebarApp.csproj` in Visual Studio
2. UI is defined in XAML files
3. Code-behind files contain the UI logic
4. SignalR client handles real-time updates

## Troubleshooting

1. Backend Issues:
   - Check if ports 7777 or 5000 are available
   - Ensure all NuGet packages are restored
   - Check Swagger UI for API documentation

2. Frontend Issues:
   - Verify backend is running and accessible
   - Check SignalR connection status
   - Ensure .NET 6.0 runtime is installed

3. Build Issues:
   - Clean solution and rebuild
   - Delete bin and obj folders
   - Restore NuGet packages

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.
