using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    public class DataProcessor
    {
        private List<ImportedObject> _importedObjects = new List<ImportedObject>();
        IEnumerable<ImportedObject> ImportedObjects { get { 
                                                                return _importedObjects; 
                                                            }
                                                        set { 
                                                                _importedObjects = value.ToList(); 
                                                            } 
                                                    }

        public DataProcessor()
        {
            ImportedObjects = new List<ImportedObject>();
        }

        public void ImportData(string fileToImport)
        {
            var streamReader = new StreamReader(fileToImport);

            var importedLines = new List<string>();
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                importedLines.Add(line);
            }

            ImportedObjects = MapToImportedObject(importedLines);
            CorrectImportedObjects();
            AssignNumberOfChildren();
        }

        public void PrintData() =>
            _importedObjects
                .Where(database => database.Type == "DATABASE")
                .ToList()
                .ForEach(database => { 
                                        Console.WriteLine($"Database '{database.Name}' ({database.NumberOfChildren} tables)");
                                        PrintDatabaseTables(database.Type, database.Name);
                                     });
                                     
        private void AssignNumberOfChildren() =>
            _importedObjects.ForEach(parent => 
                    parent.NumberOfChildren = _importedObjects.Where(child => child.ParentType == parent.Type && child.ParentName == parent.Name).Count());

        private void CorrectImportedObjects() =>
            _importedObjects.ForEach(importedObject =>
            {
                importedObject.Type = importedObject.Type.Trim().Replace(" ", "").Replace(Environment.NewLine, "").ToUpper();
                importedObject.Name = importedObject.Name.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.Schema = importedObject.Schema.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentName = importedObject.ParentName.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
                importedObject.ParentType = importedObject.ParentType.Trim().Replace(" ", "").Replace(Environment.NewLine, "");
            });

        private List<ImportedObject> MapToImportedObject(List<string> importedLines) =>
            importedLines.Select(line => line.Split(';'))
                        .Where(values => values.Length == 7)
                        .Select(values => new ImportedObject()
                                        { //Type;Name;Schema;ParentName;ParentType;DataType;IsNullable
                                            Type = values[0],
                                            Name = values[1],
                                            Schema = values[2],
                                            ParentName = values[3],
                                            ParentType = values[4],
                                            DataType = values[5],
                                            IsNullable = values[6],
                                        })
                        .ToList();
        
        private void PrintTablesColumns(string tableType, string tableName) =>
            _importedObjects
                .Where(col => col.ParentType.ToUpper() == tableType && col.ParentName == tableName)
                .ToList()
                .ForEach(col => Console.WriteLine($"\t\tColumn '{col.Name}' with {col.DataType} data type {(col.IsNullable == "1" ? "accepts nulls" : "with no nulls")}"));

        private void PrintDatabaseTables(string databaseType, string databaseName) =>
            _importedObjects
                .Where(table => table.ParentType.ToUpper() == databaseType && table.ParentName == databaseName)
                .ToList()
                .ForEach(table => {
                                    Console.WriteLine($"\tTable '{table.Schema}.{table.Name}' ({table.NumberOfChildren} columns)");
                                    PrintTablesColumns(table.Type, table.Name);
                                    });
    }
}