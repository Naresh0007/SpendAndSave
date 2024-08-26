using System;
using SpendAndSave.Models;
using SQLite;
namespace SpendAndSave.Services
{
	public class DatabaseService
	{
        //Retrieved and learnt the db code from https://learn.microsoft.com/en-sg/answers/questions/1656950/can-not-find-path-for-sqlite-db-in-maui-app //
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LoginRequestModel>().Wait();
            _database.CreateTableAsync<ExpenseData>().Wait();
            _database.CreateTableAsync<CategoryData>().Wait();
            _database.CreateTableAsync<GoalData>().Wait();
            _database.CreateTableAsync<SavingData>().Wait();
        }

        public async Task<int> SaveUserAsync(LoginRequestModel user)
        {
            //return _database.InsertOrReplaceAsync(user);
            if (user.Id != 0)
            {
                return await _database.UpdateAsync(user);
            }
            else
            {
                return await _database.InsertAsync(user);
            }

        }

        public Task<LoginRequestModel> GetUserAsync(int id)
        {
            return _database.Table<LoginRequestModel>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> DeleteUserAsync(LoginRequestModel user)
        {
            return _database.DeleteAsync(user);
        }

        public Task<LoginRequestModel> GetUserAsync(string username, string password)
        {
            return _database.Table<LoginRequestModel>()
                            .Where(u => u.UserName == username && u.Password == password)
                            .FirstOrDefaultAsync();
        } 
        
        public Task<LoginRequestModel> GetUserDetails(string username)
        {
            return _database.Table<LoginRequestModel>()
                            .Where(u => u.UserName == username)
                            .FirstOrDefaultAsync();
        }

        public Task<List<LoginRequestModel>> GetUsersAsync()
        {
            return _database.Table<LoginRequestModel>().ToListAsync();
        }

        public Task<LoginRequestModel> GetUserByUsernameAsync(string username)
        {
            return _database.Table<LoginRequestModel>()
                            .Where(u => u.UserName == username)
                            .FirstOrDefaultAsync();
        }

        public Task<LoginRequestModel> GetLoggedInUserAsync(int loggedInUserId)
        {
            return _database.Table<LoginRequestModel>().Where(u => u.Id == loggedInUserId).FirstOrDefaultAsync();
        }

    }
}

