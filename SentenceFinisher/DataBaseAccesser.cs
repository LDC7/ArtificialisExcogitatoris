namespace SentenceFinisher
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.Common;
  using System.Data.SqlClient;
  using AppSettings;

  public static class DataBaseAccesser
  {
    private static readonly string connectionString = AppSettings.Get<string>("DEFAULT_CONNECTION_STRING"); 

    public static void AddExcludedPattern(string pattern)
    {
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "AddWord";
          command.Parameters.AddParameter("Pattern", pattern);

          connection.Open();
          command.ExecuteNonQuery();
          connection.Close();
        }
      }
    }

    public static void AddExcludedWord(string word)
    {
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "AddExcludedWord";
          command.Parameters.AddParameter("Word", word);

          connection.Open();
          command.ExecuteNonQuery();
          connection.Close();
        }
      }
    }

    public static Tuple<long, long> AddRelation(string word, string relatedWord, int position)
    {
      Tuple<long, long> ids;

      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "AddRelation";
          command.Parameters.AddParameter("Word", word);
          command.Parameters.AddParameter("RelatedWord", relatedWord);
          command.Parameters.AddParameter("Type", position);

          connection.Open();
          using (IDataReader reader = command.ExecuteReader())
          {
            reader.Read();
            ids = new Tuple<long, long>((long)reader[0], (long)reader[1]);
          }
          connection.Close();
        }
      }

      return ids;
    }

    public static long AddWord(string word)
    {
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "AddWord";
          command.Parameters.AddParameter("Word", word);

          connection.Open();
          return (long)command.ExecuteScalar();
        }
      }
    }

    public static bool CheckExcludedWord(string word)
    {
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "CheckExcludedWord";
          command.Parameters.AddParameter("Word", word);

          connection.Open();
          return (bool)command.ExecuteScalar();
        }
      }
    }

    public static bool CheckWordExists(long id)
    {
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "CheckWordExists";
          command.Parameters.AddParameter("Id", id);

          connection.Open();
          return (bool)command.ExecuteScalar();
        }
      }
    }

    public static Dictionary<long, DateTime> GetTimeTable()
    {
      Dictionary<long, DateTime> timeTable = new Dictionary<long, DateTime>();

      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "GetTimeTable";

          connection.Open();
          using (IDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              timeTable.Add((long)reader["DiscordId"], (DateTime)reader["ActualDateTime"]);
            }
          }
          connection.Close();
        }
      }

      return timeTable;
    }

    public static string GetWord(long id)
    {
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "GetWord";
          command.Parameters.AddParameter("Id", id);

          connection.Open();
          return (string)command.ExecuteScalar();
        }
      }
    }

    public static IEnumerable<long> GetWordsWithoutType()
    {
      List<long> result = new List<long>();
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "GetWordsWithoutType";

          connection.Open();
          using (IDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              result.Add((long)reader["PK_WordId"]);
            }
          }
          connection.Close();
        }
      }
      return result;
    }

    public static void SetTime(long id, DateTime dateTime)
    {
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "SetTime";
          command.Parameters.AddParameter("ChannalId", id);
          command.Parameters.AddParameter("DateTime", dateTime);

          connection.Open();
          command.ExecuteNonQuery();
          connection.Close();
        }
      }
    }

    public static void SetWordType(long id, byte type)
    {
      using (var connection = new SqlConnection())
      {
        connection.ConnectionString = connectionString;
        using (var command = connection.CreateCommand())
        {
          command.CommandType = CommandType.StoredProcedure;
          command.CommandText = "SetWordType";
          command.Parameters.AddParameter("WordId", id);
          command.Parameters.AddParameter("Type", type);

          connection.Open();
          command.ExecuteNonQuery();
          connection.Close();
        }
      }
    }

    private static void AddParameter<T>(this DbParameterCollection collection, string parameterName, T parameter)
    {
      object value = DBNull.Value;
      if (parameter != null)
      {
        value = parameter;
      }

      collection.Add(new SqlParameter(parameterName, value));
    }
  }
}
