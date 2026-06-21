# Noots

A lightweight desktop note-taking application built with **C#**, **.NET 8**, **Windows Forms**, and **SQL Server**.

Noots helps users create, organize, search, and manage personal notes with labels and image attachments.

## Features

- User registration and sign-in
- SHA-256 password hashing
- Persistent login using a locally stored user ID
- Create notes with a title, content, labels, and image attachments
- Optional title and content fields
- Auto-save for note title and content changes
- Multiple labels and images per note
- Duplicate label prevention
- Search notes by title and content
- Filter notes by labels
- Label management from the sidebar
- Delete notes with related image records and label relationships
- Local image storage with relative paths
- GUID-based identifiers

## Technology Stack

- C#
- .NET 8
- Windows Forms
- SQL Server
- Microsoft.Data.SqlClient
- ADO.NET
- System.Text.Json
- SHA-256 password hashing

## Project Structure

```text
Noots
│
├── DataAccess
│   ├── DatabaseManager.cs
│   ├── NoteRepository.cs
|   ├── LabelRepository.cs
│   └── UserRepository.cs
│
├── Models
│   ├── Note.cs
│   ├── NoteInfo.cs
│   ├── NoteLabel.cs
│   ├── User.cs
│   ├── NoteImage.cs
│   ├── UserLabel.cs
│   └── LabelModel.cs
│
├── Forms
│   ├── MainForm.cs
│   ├── CreateNoteForm.cs
│   ├── NoteDetailForm.cs
│   ├── SigninForm.cs
│   └── SignupForm.cs
│
├── Session
│   └── UserSession.cs
│
├── Utilities
│   ├── ImageStorageHelper.cs
│   ├── LoginStorage.cs
│   └── PasswordHelper.cs
│
└── UserControls
    └── NoteCardControl.cs
```

## Authentication

Passwords are not stored as plain text. The application hashes passwords with SHA-256 before storing them in SQL Server.

```csharp
PasswordHelper.HashPassword(password);
PasswordHelper.VerifyPassword(inputPassword, hashFromDatabase);
```

## Persistent Login

After a successful sign-in, Noots stores only the current user's `UserId` locally:

```text
%AppData%\Noots\login.json
```

No password or password hash is stored in this file. The saved login is removed when the user logs out.

## Notes and Labels

A note can contain a title, content, one or more labels, and multiple image attachments.

- Notes can be searched by title and content.
- Labels are trimmed and compared case-insensitively to prevent duplicates.
- A note can have multiple labels, and a label can be used across multiple notes.
- Removing a label does not delete the user's notes.

## Auto-Save

Changes to note titles and content are automatically saved in `NoteDetailForm`.

The application waits briefly after the user stops typing before saving, which avoids unnecessary database updates for every typed character. Pending changes are also saved before the note form closes.

## Image Storage

Images are stored locally under:

```text
%AppData%\Noots\Images
```

Each note has a dedicated folder based on its ID. Only relative image paths are saved in the database.

```text
Images\[NoteId]\[ImageFileName].jpg
```

## Database

Database name:

```text
Noots
```

Required tables:

```text
Users
Notes
Labels
UserLabels
NoteLabels
NoteImages
```

## Requirements

- .NET 8 SDK
- Visual Studio 2022 or later
- SQL Server
- `Microsoft.Data.SqlClient` NuGet package

Install the package if needed:

```bash
dotnet add package Microsoft.Data.SqlClient
```

## Configuration

Configure the connection string in `DatabaseManager.cs`.

```csharp
private const string ConnectionString =
    "Server=.;Database=Noots;Trusted_Connection=True;TrustServerCertificate=True;";
```

## Running the Application

1. Open the solution in Visual Studio.
2. Create the `Noots` database and required tables in SQL Server.
3. Configure the connection string in `DatabaseManager.cs`.
4. Restore NuGet packages and build the project.
5. Run the application.
6. Create an account or sign in to start managing notes.
