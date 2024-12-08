# Sentimatrix

A real-time email sentiment analysis application built for the IIT Bombay TechFest Datamatics Hackathon. The solution combines RPA using TruBot for email processing with a modern WPF client and .NET Core backend for sentiment analysis.

## Features

- Automated email processing using TruBot RPA
- Real-time email sentiment analysis
- Desktop client with modern WPF interface
- SignalR for real-time updates
- Swagger API documentation
- Email content processing and analysis

## Tech Stack

### RPA Automation
- TruBot Designer for automation development
- TruBot Cockpit Personal for bot execution
- Email extraction and processing capabilities
- Seamless integration with backend API

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
- TruBot Designer (for RPA development)
- TruBot Cockpit Personal (for bot execution)

## Setup Instructions

### TruBot Setup
1. Install TruBot Designer and TruBot Cockpit Personal
2. Import the provided bot project:
   - Open TruBot Designer
   - File > Import Project
   - Select the `EmailProcessor` bot project
3. Configure email settings:
   - Update email server configurations
   - Set credentials in secure parameters
4. Test the bot:
   - Run in TruBot Designer for development
   - Deploy to TruBot Cockpit for production

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

### RPA Components
```
TruBot/
├── EmailProcessor/    # Main bot project
├── Objects/          # Reusable automation objects
└── Workflows/        # Email processing workflows
```

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

## Workflow

1. TruBot RPA Process:
   - Bot monitors email inbox
   - Extracts email content and metadata
   - Processes attachments if present
   - Sends data to backend API

2. Backend Processing:
   - Receives email data from RPA bot
   - Performs sentiment analysis
   - Broadcasts results via SignalR
   - Stores processed data

3. Frontend Display:
   - Receives real-time updates
   - Displays sentiment analysis results
   - Provides interactive dashboard
   - Shows historical data

## API Endpoints

The backend provides several API endpoints through its controllers:
- Swagger UI provides detailed API documentation
- Real-time updates through SignalR hub
- Email processing and sentiment analysis endpoints

## Development

### RPA Development
1. Open TruBot Designer
2. Modify email processing workflows
3. Test changes in development mode
4. Deploy to TruBot Cockpit when ready

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

1. RPA Issues:
   - Verify TruBot services are running
   - Check email server connectivity
   - Validate credentials and permissions
   - Review bot execution logs

2. Backend Issues:
   - Check if ports 7777 or 5000 are available
   - Ensure all NuGet packages are restored
   - Check Swagger UI for API documentation

3. Frontend Issues:
   - Verify backend is running and accessible
   - Check SignalR connection status
   - Ensure .NET 6.0 runtime is installed

4. Build Issues:
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
