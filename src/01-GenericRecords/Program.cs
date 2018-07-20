using System;
using System.IO;
using Avro;
using Avro.Generic;
using Avro.IO;

namespace _01_GenericRecords
{
    class Program
    {
        /// <summary>
        /// Parsed Avro schema. This schema object is required to create, serialize and
        /// deserialize records.
        /// </summary>
        private static readonly RecordSchema UserLoginEventSchema
            = (RecordSchema)Schema.Parse(File.ReadAllText("UserLoginEvent.avsc"));

        static void Main(string[] args)
        {
            GenericRecord originalEvent;
            GenericRecord deserializedEvent;
            var timestamp = DateTime.UtcNow;

            // Create a UserLoginEvent using GenericRecord
            originalEvent = new GenericRecord(UserLoginEventSchema);
            originalEvent.Add("timestamp", DateTimeToAvroTimestampMillis(timestamp));
            originalEvent.Add("userID", "blachniet");
            originalEvent.Add("wasSuccessful", true);

            using (var memoryStream = new MemoryStream())
            {
                // Write the record to a memory stream
                {
                    var binaryEncoder = new BinaryEncoder(memoryStream);
                    var defaultWriter = new DefaultWriter(UserLoginEventSchema);

                    defaultWriter.Write(originalEvent, binaryEncoder);
                    binaryEncoder.Flush();
                }

                // Reset the stream position before we read it
                memoryStream.Position = 0;

                // Read the record from the memory stream
                {
                    var binaryDecoder = new BinaryDecoder(memoryStream);
                    var defaultReader = new DefaultReader(UserLoginEventSchema, UserLoginEventSchema);

                    deserializedEvent = defaultReader.Read<GenericRecord>(null, binaryDecoder);
                }
            }

            Console.WriteLine($@"
Original Event:
    timestamp     : {originalEvent["timestamp"]}
    userID        : {originalEvent["userID"]}
    wasSuccessful : {originalEvent["wasSuccessful"]}
Deserialized Event:
    timestamp     : {deserializedEvent["timestamp"]}
    userID        : {deserializedEvent["userID"]}
    wasSuccessful : {deserializedEvent["wasSuccessful"]}

Press 'Enter' to exit.
".TrimStart());
            Console.ReadLine();
        }

        static long DateTimeToAvroTimestampMillis(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Only UTC DateTime is accepted", nameof(dateTime));
            }

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)dateTime.Subtract(unixEpoch).TotalMilliseconds;
        }
    }
}
