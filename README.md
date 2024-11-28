# Sentimatrix

A sentiment analysis application with a WPF frontend and Python backend.

## Project Structure

```
Sentimatrix/
├── frontend/    # WPF Application
└── backend/     # Python Backend Server
```

## Prerequisites

### Frontend Requirements
- Visual Studio 2019 or later
- .NET Framework 4.7.2 or later
- Windows OS

### Backend Requirements
- Python 3.8 or later
- pip (Python package manager)

## Setup Instructions

### Backend Setup
1. Navigate to the backend directory:
   ```bash
   cd backend
   ```

2. Create a virtual environment (recommended):
   ```bash
   python -m venv venv
   ```

3. Activate the virtual environment:
   - Windows (CMD):
     ```bash
     venv\Scripts\activate
     ```
   - Windows (PowerShell):
     ```powershell
     .\venv\Scripts\Activate.ps1
     ```

4. Install required packages:
   ```bash
   pip install -r requirements.txt
   ```

### Frontend Setup
1. Open the solution in Visual Studio:
   - Navigate to `frontend` folder
   - Open `WpfSidebarApp.csproj`

2. Restore NuGet packages:
   - Right-click on the solution in Solution Explorer
   - Select "Restore NuGet Packages"

## Running the Application

### Start the Backend Server
1. Ensure you're in the backend directory with the virtual environment activated
2. Run the Flask server:
   ```bash
   python app.py
   ```
   The server will start on `http://localhost:5000`

### Start the Frontend Application
1. In Visual Studio, set the WPF project as the startup project
2. Press F5 or click the "Start" button to run the application
3. The WPF application will launch and connect to the backend server

## Development

### Backend Development
- API endpoints are defined in `app.py`
- Model training and prediction code is in the respective modules
- Configuration can be modified in `config.py`

### Frontend Development
- Main application window is in `MainWindow.xaml`
- Business logic is in the corresponding `.cs` files
- UI styling and templates are defined in XAML

## Troubleshooting

1. Backend Connection Issues:
   - Ensure the Flask server is running
   - Check if the port 5000 is not in use
   - Verify firewall settings

2. Frontend Build Issues:
   - Clean and rebuild the solution
   - Restore NuGet packages
   - Check Visual Studio output window for detailed errors

3. Package Installation Issues:
   - Ensure you're using the correct Python version
   - Update pip: `python -m pip install --upgrade pip`
   - Check for any conflicting dependencies

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details
