using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using hmcts_test.model;

namespace hmctsBackend.Tests.Helpers
{
    public class TestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;

        public TaskContext Context { get; }

        public TestDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<TaskContext>()
                .UseSqlite(_connection)
                .Options;

            Context = new TaskContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
            _connection.Close();
        }
    }
}
