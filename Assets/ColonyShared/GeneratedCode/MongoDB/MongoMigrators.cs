using System;
using MongoDB.Bson;
using GeneratedCode.DeltaObjects;

namespace SharedCode.Utils.BsonSerialization
{
    public class WorldCharacterTestMigrator : BaseMigrator<WorldCharacter>
    {
        public override string FromVersion => "test";

        public override string ToVersion => "7A1A3C67121073FB5161A0E5CFCB41C4A1D45AC4";

        public override BsonDocument Convert(BsonDocument bsonDocument)
        {
            var oldInventoryValue = bsonDocument.GetValue("test");
            bsonDocument.Remove("test");
            bsonDocument.Add("Inventory__Database", oldInventoryValue);
            return bsonDocument;
        }
    }
}