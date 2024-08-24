using Npgsql;
using webChat.Models;

namespace webChat
{
    public class DAL
    {
        private readonly string _connectionString;
        private readonly ILogger<DAL> _logger;

        public DAL(string connectionString, ILogger<DAL> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task AddMessageAsync(MessageModel message)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    _logger.LogInformation("[DAL] Opened database connection.");

                    using (var command = new NpgsqlCommand("INSERT INTO messages (text, timestamp) VALUES (@Text, @Timestamp)", connection))
                    {
                        command.Parameters.AddWithValue("Text", message.Text);
                        command.Parameters.AddWithValue("Timestamp", message.Timestamp);

                        await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("[DAL] Added message: {Text} at {Timestamp}", message.Text, message.Timestamp);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DAL] An error occurred while adding a message.");
                throw;
            }
        }

        public async Task<IEnumerable<MessageModel>> GetMessagesAsync(DateTime start, DateTime end)
        {
            var messages = new List<MessageModel>();

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    _logger.LogInformation("[DAL] Opened database connection.");

                    const string sql = "SELECT * FROM messages WHERE timestamp BETWEEN @Start AND @End";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("Start", start);
                        command.Parameters.AddWithValue("End", end);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                MessageModel message = new MessageModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Text = reader.GetString(reader.GetOrdinal("text")),
                                    Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp")),
                                };
                                messages.Add(message);
                                _logger.LogInformation("[DAL] Retrieved message: {Text} at {Timestamp}", message.Text, message.Timestamp);
                            }

                            _logger.LogInformation("[DAL] Total messages retrieved: {MessageCount}", messages.Count);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DAL] An error occurred while retrieving messages.");
                throw;
            }

            return messages;
        }
    }
}
