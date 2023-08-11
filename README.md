# CSharp Web Exam

Clean C# Web template project for beign completed as an exam to apply for hiring.

## Software stack

Tools needed to work on:

- **Operative System**:
  - Microsoft Windows
- **Framework**:
  - [.NET Core 2.1 (or greater)](https://dotnet.microsoft.com/es-es/download/dotnet)
  - [.NET Framework 4.7.2 (or greater)](https://dotnet.microsoft.com/es-es/download/dotnet-framework)
- ***IDE***:
  - [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- **Build & Package**:
  - [Microsoft Build Tools 2015](https://www.microsoft.com/es-mx/download/details.aspx?id=48159)

## Statement

1. Import project to a personal git-based storage.
1. Create a branch to make changes:
   - Following the [technical statement](#technical-statement).
   - Meeting [requirements](requirements.md).
1. Compile and test.
1. Create a merge request (or pull request) for:
   - Describing changes
   - Reviewing changes
1. Share your solution.

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
- All entire solution must log that happened in the methods, to track all kinds of events (information, warnings, errors & debug).
