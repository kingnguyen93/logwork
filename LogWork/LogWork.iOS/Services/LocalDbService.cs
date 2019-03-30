using LogWork.iOS.Services;
using LogWork.IServices;
using SQLite;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(LocalDbService))]

namespace LogWork.iOS.Services
{
    public class LocalDbService : ILocalDbService
    {
        private readonly string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
        private readonly string dbName = "organilog.db";
        private readonly string dbPassword = "Tuyendaica1993@";

        public SQLiteConnection GetDbConnection()
        {
            return new SQLiteConnection(Path.Combine(dbPath, dbName), openFlags: SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create, storeDateTimeAsTicks: true, key: dbPassword);
        }

        public SQLiteConnection GetDbConnection(SQLiteOpenFlags openFlags)
        {
            return new SQLiteConnection(Path.Combine(dbPath, dbName), openFlags, storeDateTimeAsTicks: true, key: dbPassword);
        }

        public SQLiteConnection GetDbConnection(string dbPath)
        {
            return new SQLiteConnection(Path.Combine(dbPath, dbName), openFlags: SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create, storeDateTimeAsTicks: true, key: dbPassword);
        }

        public SQLiteConnection GetDbConnection(string dbPath, SQLiteOpenFlags openFlags)
        {
            return new SQLiteConnection(Path.Combine(dbPath, dbName), openFlags, storeDateTimeAsTicks: true, key: dbPassword);
        }

        public SQLiteAsyncConnection GetDbAsyncConnection()
        {
            return new SQLiteAsyncConnection(Path.Combine(dbPath, dbName), openFlags: SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create, storeDateTimeAsTicks: true, key: dbPassword);
        }

        public SQLiteAsyncConnection GetDbAsyncConnection(SQLiteOpenFlags openFlags)
        {
            return new SQLiteAsyncConnection(Path.Combine(dbPath, dbName), openFlags, storeDateTimeAsTicks: true, key: dbPassword);
        }

        public SQLiteAsyncConnection GetDbAsyncConnection(string dbPath)
        {
            return new SQLiteAsyncConnection(Path.Combine(dbPath, dbName), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache | SQLiteOpenFlags.FullMutex, storeDateTimeAsTicks: true, key: dbPassword);
        }

        public SQLiteAsyncConnection GetDbAsyncConnection(string dbPath, SQLiteOpenFlags openFlags)
        {
            return new SQLiteAsyncConnection(Path.Combine(dbPath, dbName), openFlags, storeDateTimeAsTicks: true, key: dbPassword);
        }
    }
}