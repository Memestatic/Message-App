# Message App

Message App is a web application designed to facilitate real-time messaging between users. The project leverages various technologies to provide a robust and secure messaging platform.

## Technologies Used

### Frontend
- **HTML**: Markup language to structure the web pages.
- **CSS**: Styling the HTML elements.
- **JavaScript**: Adding interactivity to the web pages.

### Backend
- **C#**: Primary programming language used for server-side logic.
- **ASP.NET Core**: Framework for building web applications and services.
- **SignalR**: Library for adding real-time web functionality, allowing server-side code to push content to clients instantly.

### Database
- **SQL Server**: Relational database management system for storing application data.

### Authentication
- **ASP.NET Core Identity**: Used for managing user authentication and authorization.

## Features

- **Real-Time Messaging**: Powered by SignalR, allows users to send and receive messages instantly.
- **User Authentication**: Managed by ASP.NET Core Identity to ensure secure access to the application.
- **Database Integration**: Uses SQL Server for efficient data management and storage.

## Getting Started

To get started with the project, follow these steps:

1. **Clone the repository**:
    ```bash
    git clone https://github.com/Memestatic/Message-App.git
    ```

2. **Navigate to the project directory**:
    ```bash
    cd Message-App
    ```

3. **Set up the database**:
   Ensure you have SQL Server installed and configured. Create new migration using app context.
   Update the connection string in `dataSources.xml` if necessary.
   Finally update the SQL Server database.

5. **Run the application**:
    ```bash
    dotnet run
    ```

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests for any enhancements or bug fixes.

## License

This project is licensed under the MIT License.
