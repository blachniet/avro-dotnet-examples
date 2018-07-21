# Avro .NET Examples

This repository provides example usages of the Avro .NET library. These projects use the [blachniet.Avro](https://www.nuget.org/packages/blachniet.Avro/) NuGet package. This NuGet package is created from the [github.com/blachniet/avro](github.com/blachniet/avro) which is a fork of the main Avro project.

## 01-GenericRecords

This application shows the simplest example of serializing and deserializing a record using the `GenericRecord` class.

## 02-SchemaEvolution

This application shows how you can read serialized data using a different version of the schema than was used to write the data. In v2 we added a new field, `fieldB`. We have examples of writing with v1 and reading with v2, as well as writing with v2 and reading with v1.
