using SQLite;

namespace LogWork.IServices
{
    public interface ILocalDbService
    {
        SQLiteConnection GetDbConnection();

        SQLiteConnection GetDbConnection(SQLiteOpenFlags openFlags);

        SQLiteConnection GetDbConnection(string dbPath);

        SQLiteConnection GetDbConnection(string dbPath, SQLiteOpenFlags openFlags);

        SQLiteAsyncConnection GetDbAsyncConnection();

        SQLiteAsyncConnection GetDbAsyncConnection(SQLiteOpenFlags openFlags);

        SQLiteAsyncConnection GetDbAsyncConnection(string dbPath);

        SQLiteAsyncConnection GetDbAsyncConnection(string dbPath, SQLiteOpenFlags openFlags);
    }
}