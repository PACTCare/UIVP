namespace Kvk.Api.Wrapper.Services
{
  using System;
  using System.Data.SQLite;
  using System.IO;

  public static class PublicKeyResolver
  {
    public static void Add(string kvkNumber, string publicKey)
    {
      InitDatabase();

      using (var connection = new SQLiteConnection("Data Source=publickeys.sqlite;Version=3;"))
      {
        connection.Open();

        using (var command = new SQLiteCommand($"INSERT OR REPLACE INTO PublicKeys (KvkNumber, PublicKey) VALUES ('{kvkNumber}', '{publicKey}')", connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }

    public static string GetPublicKey(string kvkNumber)
    {
      InitDatabase();

      using (var connection = new SQLiteConnection("Data Source=publickeys.sqlite;Version=3;"))
      {
        connection.Open();

        using (var command = new SQLiteCommand($"SELECT PublicKey FROM PublicKeys WHERE KvkNumber='{kvkNumber}'", connection))
        {
          var result = command.ExecuteScalar();
          return result == null ? string.Empty : result.ToString();
        }
      }
    }

    private static void InitDatabase()
    {
      if (File.Exists("publickeys.sqlite"))
      {
        return;
      }

      SQLiteConnection.CreateFile("publickeys.sqlite");

      using (var connection = new SQLiteConnection("Data Source=publickeys.sqlite;Version=3;"))
      {
        connection.Open();

        using (var command = new SQLiteCommand("CREATE TABLE PublicKeys (KvkNumber TEXT NOT NULL PRIMARY KEY, PublicKey TEXT NOT NULL)", connection))
        {
          command.ExecuteNonQuery();
        }
      }
    }
  }
}