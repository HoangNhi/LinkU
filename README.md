# LinkU

LinkU is a real-time chat application built with .NET 8, featuring a service-based architecture with separate frontend and backend services. It leverages SignalR for real-time communication, Redis for caching, and a SQL Server database for data persistence. The entire application is containerized using Docker for easy setup and deployment.

## Features

*   **Real-time Chat:** Instant messaging between users and in groups.
*   **User Authentication:** Secure user login and registration with JWT.
*   **Friendship Management:** Send, accept, and reject friend requests.
*   **Group Conversations:** Create and manage chat groups.
*   **Message Reactions:** React to messages with emojis.
*   **Media Sharing:** Share files and media in conversations.
*   **User Profiles:** View and manage user profiles.

## Technologies Used

### Backend

*   **.NET 8:** The core framework for the backend API.
*   **ASP.NET Core:** For building the web API.
*   **SignalR:** For real-time messaging.
*   **Entity Framework Core:** For data access and database migrations.
*   **SQL Server:** As the primary database.
*   **Redis:** For caching and improving performance.
*   **AutoMapper:** For object-to-object mapping.
*   **JWT (JSON Web Tokens):** For secure authentication.
*   **Azure Blob Storage:** For storing media files.
*   **MailKit:** For sending emails (e.g., for OTP).

### Frontend

*   **.NET 8:** The core framework for the frontend application.
*   **ASP.NET Core MVC:** For building the user interface.
*   **Bootstrap:** For styling and responsive design.
*   **jQuery:** For client-side scripting.
*   **SignalR Client:** To connect to the real-time messaging hub.

### Database

*   **Microsoft SQL Server:** The relational database management system.
*   **Redis:** In-memory data store for caching.

## Getting Started

To get the application up and running, you will need to have Docker and Docker Compose installed on your machine.

1.  **Clone the repository:**

    ```bash
    git clone https://github.com/your-username/LinkU.git
    ```

2.  **Navigate to the project directory:**

    ```bash
    cd LinkU
    ```

3.  **Run the application with Docker Compose:**

    ```bash
    docker-compose up -d
    ```

This will build the Docker images for the frontend, backend, and database services and run them in detached mode.

*   The frontend will be accessible at `http://localhost:80`.
*   The backend API will be accessible at `http://localhost:8081`.
*   The SQL Server database will be running on port `1433`.

## API Endpoints

The backend API provides the following endpoints:

*   `/api/Conversation`: Manage chat conversations.
*   `/api/FriendRequest`: Manage friend requests.
*   `/api/Friendship`: Manage friendships.
*   `/api/Group`: Manage chat groups.
*   `/api/GroupMember`: Manage group members.
*   `/api/GroupRequest`: Manage group join requests.
*   `/api/MediaFile`: Handle file uploads and downloads.
*   `/api/Message`: Send and receive messages.
*   `/api/MessageList`: Get message history.
*   `/api/MessageReaction`: Manage message reactions.
*   `/api/ReactionType`: Get available reaction types.
*   `/api/User`: Manage user accounts and profiles.

## Database

The project uses a SQL Server database to store application data. The database schema is managed using Entity Framework Core migrations. The `docker-compose.yml` file sets up a SQL Server container and restores a database backup from `DATABASE/SQLServer/LINKU.bak`.

Redis is used for caching frequently accessed data, which helps to reduce the load on the database and improve the application's performance.

## Project Structure

The project is organized into the following main directories:

*   `BE`: The backend .NET 8 project.
*   `FE`: The frontend .NET 8 project.
*   `ENTITIES`: Contains the database entity models and `DbContext`.
*   `MODELS`: Contains the data transfer objects (DTOs) and request/response models.
*   `DATABASE`: Contains the Dockerfile and configuration for the SQL Server and Redis databases.
*   `.github`: Contains GitHub Actions workflows for CI/CD.
*   `docker-compose.yml`: The Docker Compose file for orchestrating the services.
