# CSharp Web Exam

Clean C# Web template project to be completed as a technical exam for hiring purposes.

## Software stack

Tools needed to work on:

- **Operative System**:
  - Microsoft Windows 10 or newer
- **Framework**:
  - [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- ***IDE***:
  - [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)
- **Build & Package**:
  - Built-in .NET CLI tools (`dotnet build`, `dotnet run`)

## Statement

1. Import project to a personal git-based storage.
2. Create a branch to make changes:
   - Following the [technical statement](#technical-statement).
   - Meeting [requirements](requirements.md).
3. Compile and test.
4. Create a merge request (or pull request) for:
   - Describing changes
   - Reviewing changes
5. Share your solution.

## Technical statement

It is necessary to develop three minimal components for the ecosystem (solution):

- **A relational database** (with at least two related tables)
- **An application programming interface (*API*)**
- **A graphical user interface (*GUI*)**

Functionality requirements:

- *API* must permit:
  - Create, read, update and delete data through *ORM*.
- *GUI* must permit:
  - In form view:
    - *CRUD* (Create, read, update & delete) data through *API*.
  - In report view:
    - Paginate data (in case of a lot of rows).
    - Group data under some criteria.
- The entire solution must log what happens in the methods, to track all kinds of events (information, warnings, errors & debug).

## Documentation

Documentation must be completed inside the `Docs/` directory.

> Additional folders or documents may be added if needed, but the goal is to fully complete what is explicitly requested in each section.
