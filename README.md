# Web chat

### What is it?
This project is a simple web service designed for message exchange. It consists of three main components: a web server, a PostgreSQL database, and three clients for message submission and display.

### Project Structure
- **webClientGetHistoryFromDB**: Retrieving and displaying messages over a period of time.
- **webClientReceiveMessage**: Displays messages in real-time via WebSocket.
- **webClientSendMessage**: Sends messages to the server.
- **webServer**: Handles the REST API for message sending and retrieval.
- **PostgreSQL Database**: Stores messages with timestamps and sequence numbers.

### Features

- Each message includes:
  - Text (up to 128 characters)
  - Timestamp (set by the server)
- Real-time message sending from the webClinetSendMessage to webServer using WebSockets.
- Real-time message streaming from the webServer to webClientReceiveMessage using WebSockets.
- webClientGetHistoryFromDB allows users to view the history of messages over a period of time.
- The server provides a REST API with the following endpoints:
  - Send a message.
  - Retrieve a list of messages within a date range.
- Swagger documentation is generated for the REST API.
- Implemented in C#, using an MVC pattern, with a separate Data Access Layer (DAL) without ORM.

### How to Run the Project

1. **Clone the repository**:

   ```bash
   git clone https://github.com/tvojk13/webChat.git
   ```

2. **Build and Start the Application**:

Run the following command to start all components using Docker Compose:

   ```bash
   docker-compose up --build
   ```

3. **Project rebuild or shutdown**:
    
If you need to restart or stop the project, you should run the following command:

```bash
docker-compose down -v
```

### Contact us
If you have any ideas or improvements for this repository, please let me know.<br>
Email for contact: tvojk13@gmail.com
