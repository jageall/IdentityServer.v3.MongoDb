using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace IdentityServer.Core.MongoDb
{
    static class BsonDocumentExtensions
    {
        public static void SetIfNotNull(this BsonDocument doc, string name, string value)
        {
            if (value != null)
                doc[name] = value;
        }

        public static void SetIfNotNull(this BsonDocument doc, string name, DateTimeOffset? value)
        {
            if (value != null)
                doc[name] = value.Value.ToBsonDateTime();
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

        public static DateTimeOffset GetValueOrDefault(this BsonDocument doc, string name, DateTimeOffset @default)
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

        public static DateTimeOffset? GetValueOrDefault(this BsonDocument doc, string name, DateTimeOffset? @default)
        {
            if (doc.Contains(name) && doc[name].IsValidDateTime)
            {
                return doc[name].ToUniversalTime();
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
                BsonArray values = doc[name].AsBsonArray;
                return values
                    .Where(x => x.IsBsonDocument)
                    .Select(x => x.AsBsonDocument)
                    .Select(reader).Where(x => x != null);
            }
            return @default;
        }

        public static IEnumerable<T> GetNestedValueOrDefault<T>(
            this BsonDocument doc,
            string name,
            Func<BsonDocument, IEnumerable<T>> reader,
            IEnumerable<T> @default)
            where T : class
        {
            if (doc.Contains(name) && doc[name].IsBsonDocument)
            {
                var value = doc[name].AsBsonDocument;
                return reader(value);
            }
            return @default;
        }

        public static T GetNestedValueOrDefault<T>(
            this BsonDocument doc,
            string name,
            Func<BsonDocument, T> reader,
            T @default)
            where T : class
        {
            if (doc.Contains(name) && doc[name].IsBsonDocument)
            {
                var value = doc[name].AsBsonDocument;
                return reader(value);
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
                BsonArray values = doc[name].AsBsonArray;
                return values
                    .Where(x => x.IsString)
                    .Select(x => x.AsString);
            }
            return @default;
        }

        public static BsonValue ToBsonDateTime(this DateTimeOffset dateTime)
        {
            return BsonTypeMapper.MapToBsonValue(dateTime, BsonType.DateTime);
        }
    }
}