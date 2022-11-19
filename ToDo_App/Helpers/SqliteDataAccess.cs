using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using ToDo_App.Models;

namespace ToDo_App.Helpers
{
    public class SqliteDataAccess
    {
        public static void DeleteToDoListItem(ToDoListItem toDoListItem)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("DELETE FROM ToDoListTable WHERE Id = @Id", toDoListItem);
            }
        }

        public static List<ToDoListItem>? LoadToDoListTable()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<ToDoListItem>("SELECT * FROM ToDoListTable", new DynamicParameters());
                return output.ToList();
            }
        }
        public static void SaveToDoListItem(ToDoListItem toDoListItem)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("INSERT INTO ToDoListTable (Contents, CreatedDateTime, Date, Title) VALUES (@Contents, @CreatedDateTime, @Date, @Title)", toDoListItem);
            }
        }

        public static void UpdateToDoListItem(ToDoListItem toDoListItem)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute(
                    "UPDATE ToDoListTable SET " +
                    "Contents = @Contents, " +
                    "Date = @Date, " +
                    "Title = @Title " +
                    "WHERE Id = @Id",
                    toDoListItem);
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}