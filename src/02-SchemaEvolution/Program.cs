using Avro;
using Avro.Generic;
using Avro.IO;
using System;
using System.IO;

namespace _02_SchemaEvolution
{
    class Program
    {
        private static readonly RecordSchema EntryV1Schema
            = (RecordSchema)Schema.Parse(File.ReadAllText("Entry.v1.avsc"));

        private static readonly RecordSchema EntryV2Schema
            = (RecordSchema)Schema.Parse(File.ReadAllText("Entry.v2.avsc"));

        static void Main(string[] args)
        {
            WriteV1ReadV2();
            WriteV2ReadV1();

            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Write a record using the v1 schema which doesn't have the 'fieldB' field. Read that
        /// data using the v2 schema, which has 'fieldB' with 'bravo' as the default value.
        /// </summary>
        static void WriteV1ReadV2()
        {
            GenericRecord originalEntry;
            GenericRecord deserializedEntry;

            // Create an entry using the v1 schema
            originalEntry = new GenericRecord(EntryV1Schema);
            originalEntry.Add("fieldA", "Hello");

            using (var memoryStream = new MemoryStream())
            {
                // Write the record using the v1 schema
                {
                    var binaryEncoder = new BinaryEncoder(memoryStream);
                    var writer = new DefaultWriter(EntryV1Schema);

                    writer.Write(originalEntry, binaryEncoder);
                }

                memoryStream.Position = 0;

                // Read the record using the v2 schema
                {
                    var binaryDecoder = new BinaryDecoder(memoryStream);
                    var defaultReader = new DefaultReader(EntryV1Schema, EntryV2Schema);

                    deserializedEntry = defaultReader.Read<GenericRecord>(null, binaryDecoder);
                }
            }

            Console.WriteLine($@"
V1 --> V2
---------------------------------------
Original Entry (v1):
    fieldA: {originalEntry["fieldA"]}
    fieldB: <does not exist in v1>
Deserialized Entry (v2):
    fieldA: {deserializedEntry["fieldA"]}
    fieldB: {deserializedEntry["fieldB"]}
".TrimStart());
        }

        /// <summary>
        /// Write a record using the v2 schema. Read that data using the v1 schema, which doesn't
        /// have the 'fieldB' field.
        /// </summary>
        static void WriteV2ReadV1()
        {
            GenericRecord originalEntry;
            GenericRecord deserializedEntry;

            originalEntry = new GenericRecord(EntryV2Schema);
            originalEntry.Add("fieldA", "Hello");
            originalEntry.Add("fieldB", "World");

            using (var memoryStream = new MemoryStream())
            {

                {
                    var binaryEncoder = new BinaryEncoder(memoryStream);
                    var writer = new DefaultWriter(EntryV2Schema);

                    writer.Write(originalEntry, binaryEncoder);
                }

                memoryStream.Position = 0;

                {
                    var binaryDecoder = new BinaryDecoder(memoryStream);
                    var defaultReader = new DefaultReader(EntryV2Schema, EntryV1Schema);

                    deserializedEntry = defaultReader.Read<GenericRecord>(null, binaryDecoder);
                }
            }

            Console.WriteLine($@"
V2 --> V1
---------------------------------------
Original Entry (v2):
    fieldA: {originalEntry["fieldA"]}
    fieldB: {originalEntry["fieldB"]}
Deserialized Entry (v1):
    fieldA: {deserializedEntry["fieldA"]}
    fieldB: <does not exist in v1>
".TrimStart());
        }
    }
}
