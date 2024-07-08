# MinimalDB

## Overview
A lightweight, file-based database that uses JSON format for data storage. Perfect for small projects, prototyping, or applications needing a straightforward data storage solution.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Features](#features)
- [Examples](#examples)
- [Contributing](#contributing)

## Installation

To get started with MinimalDB, follow these steps:

```bash
# Clone the repository
git clone https://github.com/rahiyansafz/MinimalDB.git

# Navigate to the project directory
cd MinimalDB

# Install dependencies
dotnet restore
```
## Usage

After installing the dependencies, you can build and run the project:

```bash
# Build the project
dotnet build

# Run the project
dotnet run
```

## Features
1. JSON-Based storage: Uses JSON for data persistence, providing a human-readable format.
2. CRUD Operations: Supports full CRUD (Create, Read, Update, Delete) operations. You can easily add, retrieve, update, and delete data using a simple and intuitive API.
3. Batch Operations: Allows inserting, updating and deleting multiple records at once.
4. Flexible querying with LINQ-style predicates
5. Schema Validation: Enforces data integrity through basic schema validation.
6. Dynamic ID generation: Generates and manages unique IDs for int, string and Guid primary keys.
7. Basic Transaction Support: All-or-nothing commit process for multiple operations.

## Examples
### Load Database
```csharp
// Open a database in project's directory(if file doesn't already exist, it creates a new one)
var db = new JsonStore("database.json");

//However, you can specify a custom path for the database
var db = new JsonStore(@"C:\Path\To\database.json");
```

### Create Table
```csharp
public record Employee(int Id, string Name, string Workplace);

// Create a new table
db.CreateTable<Employee>(); //This creates an 'employee' table and generates the schema that will you be used to validate entries into the table
```
### Commit
The Commit method processes all pending changes that have been queued up through various operations (Insert, Update, Delete) and writes them to the underlying JSON file. This method provides a transaction-like behavior, ensuring that all operations either succeed together or fail without partially modifying the database.
```csharp
// Only when Commit is called are the changes actually written to the file.
db.Commit();
```
### Insert one/multiple Elements
```csharp
// Insert one element into a table
db.InsertOne<Employee>(new Employee(1, "Rahiyan", "MPower"));

// Alternatively, the Id property can be omitted and it'll be updated to the correct value (this only works for integer, string, and Guid types)
db.InsertOne<Employee>(new Employee(0, "Rahiyan", "MPower"));

// Insert multiple elements into a table
db.InsertMultiple<Employee>(new List<Employee>
{
    new Employee("Ifty", "Binary Quest"),
    new Employee("Mintu", "IQVIA"),
    new Employee("Rishad", "Doodles"),
    new Employee("Mubasshir", "MPower")
});

db.Commit();
// Employee 'Mubasshir' will have 11 as its Id
```
### Get all elements in a table or only those that satisfy a condition
```csharp
// Find all elements in a table (returns IEnumerable<Employee>)
db.FindAll<Employee>();

// Find elements that satisfy a condition (returns IEnumerable<Employee>)
db.FindByCondition<Employee>(employee => employee.Id == 1);
```
### Update elements satisfying a condition
```csharp
// Update employee where name is Rahiyan to work at Binary Quest
var replacement = new Employee(1, "Rahiyan", "Binary Quest");
// The Id can also be updated as long as another record with that Id does not already exist
var replacement = new Employee(12, "Rahiyan", "Binary Quest");

db.UpdateByCondition<Employee>(employee => employee.Name.Equals("rahiyan", StringComparison.CurrentCultureIgnoreCase), replacement);
db.Commit();

// Before Update: {"id": 1, "name": "Rahiyan", "workplace" = "MPower"}
// After Update: {"id": 1, "name": "Rahiyan", "workplace" = "Binary Quest"}

// Update multiple employees
var replacement = new Employee("Update", "UpdatedPlace");

// Updates all employees
db.UpdateByCondition<Employee>(employee => true, replacement);
db.Commit();

// Before Update: {"id": 1, "name": "Rahiyan", "workplace" = "MPower"}, {"id": 2, "name": "Ifty", "workplace" = "Binary Quest"}... {"id": 11, "name": "Mubasshir", "workplace" = "MPower"}
// After Update: {"id": 1, "name": "Update", "workplace" = "UpdatedPlace"}, {"id": 2, "name": "Update", "workplace" = "UpdatedPlace"}... {"id": 11, "name": "Update", "workplace" = "UpdatedPlace"}
```

### Delete one/Multiple elements
```csharp
// Delete one element with Id 1 (this only works when the class has a property "Id")
db.DeleteOne<Employee>(1);
db.Commit();

// Before Delete: {"id": 1, "name": "Rahiyan", "workplace" = "MPower"}, {"id": 2, "name": "Ifty", "workplace" = "Binary Quest"}... {"id": 11, "name": "Mubasshir", "workplace" = "MPower"}
// After Delete: {"id": 2, "name": "Ifty", "workplace" = "Binary Quest"}... {"id": 11, "name": "Mubasshir", "workplace" = "MPower"}

// Delete all elements that satisfy a condition
db.DeleteByCondition<Employee>(x => x.Id == 1 || x.Id == 2);
db.Commit();

// Before Delete: {"id": 1, "name": "Rahiyan", "workplace" = "MPower"}, {"id": 2, "name": "Ifty", "workplace" = "Binary Quest"}... {"id": 11, "name": "Mubasshir", "workplace" = "MPower"}
// After Delete: {"id": 10, "name": "Mintu", "workplace" = "IQVIA"}, {"id": 11, "name": "Mubasshir", "workplace" = "MPower"}
```

## Contributing

Contributions are welcome! If you have any suggestions, bug reports, or feature requests, please open an issue or submit a pull request.
