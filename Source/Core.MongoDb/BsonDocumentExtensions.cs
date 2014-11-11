using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace IdentityServer.Core.MongoDb
{
    public static class BsonDocumentExtensions
    {
        public static void SetIfNotNull(this BsonDocument doc, string name, string value)
        {
            if (value != null)
                doc[name] = value;
        }
        public static void SetIfNotNull(this BsonDocument doc, string name, Uri value)
        {
            if (value != null)
                doc[name] = value.ToString();
        }

        public static int GetValueOrDefault(this BsonDocument doc, string name, int @default)
        {
            if (doc.Contains(name) && doc[name].IsInt32)
            {
                return doc[name].AsInt32;
            }
            return @default;
        }

        public static bool GetValueOrDefault(this BsonDocument doc, string name, bool @default)
        {
            if (doc.Contains(name) && doc[name].IsBoolean)
            {
                return doc[name].AsBoolean;
            }
            return @default;
        }

        public static DateTime GetValueOrDefault(this BsonDocument doc, string name, DateTime @default)
        {
            if (doc.Contains(name) && doc[name].IsValidDateTime)
            {
                return doc[name].ToUniversalTime();
            }
            return @default;
        }

        public static string GetValueOrDefault(this BsonDocument doc, string name, string @default)
        {
            if (doc.Contains(name) && doc[name].IsString)
            {
                return doc[name].AsString;
            }
            return @default;
        }

        public static Uri GetValueOrDefault(this BsonDocument doc, string name, Uri @default)
        {
            if (doc.Contains(name) && doc[name].IsString && Uri.IsWellFormedUriString(doc[name].AsString, UriKind.Absolute))
            {
                return new Uri(doc[name].AsString);
            }
            return @default;
        }


        public static T GetValueOrDefault<T>(this BsonDocument doc, string name, T @default)
            where T : struct
        {
            T value;
            if (doc.Contains(name)
                && doc[name].IsString
                && Enum.TryParse(doc[name].AsString, out value))
            {
                return value;
            }
            return @default;
        }

        public static IEnumerable<T> GetValueOrDefault<T>(
            this BsonDocument doc,
            string name,
            Func<BsonDocument, T> reader,
            IEnumerable<T> @default)
            where T : class
        {
            if (doc.Contains(name) && doc[name].IsBsonArray)
            {
                var values = doc[name].AsBsonArray;
                return values
                    .Where(x => x.IsBsonDocument)
                    .Select(x => x.AsBsonDocument)
                    .Select(reader).Where(x => x != null);
            }
            return @default;
        }

        public static IEnumerable<string> GetValueOrDefault(
          this BsonDocument doc,
          string name,
          IEnumerable<string> @default)
        {
            if (doc.Contains(name) && doc[name].IsBsonArray)
            {
                var values = doc[name].AsBsonArray;
                return values
                    .Where(x => x.IsString)
                    .Select(x => x.AsString);
            }
            return @default;
        }
    }
}