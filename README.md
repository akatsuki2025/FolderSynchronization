# Folder Synchronization Tool

## Overview
This is a command-line folder synchronization utility that continuously synchronizes a source folder to a replica folder. It copies new and modified files, replicates the folder structure, and removes files/folders that don't exist in the source directory.

## Features
- One-way synchronization from source to destination folder
- Configurable synchronization interval
- MD5 checksum comparison to validate file content
- Logs all operations to a specified log file
- Safe cleanup of incomplete copies on error

## Usage
```
FolderSynchronization <sourceFolderPath> <destinationFolderPath> <synchronizationIntervalSeconds> <logFolderPath>
```

### Parameters
- **sourceFolderPath**: Path to the source folder to synchronize from
- **destinationFolderPath**: Path to the parent directory where the replica will be created
- **synchronizationIntervalSeconds**: Time between synchronization operations in seconds
- **logFolderPath**: Directory where log files will be stored. Log files are automatically created with timestamps (format: log_YYYYMMDD.txt) and limited to 100MB each.

## Synchronization Process
1. Creates destination folder if it doesn't exist
2. Copies new and modified files from source to destination
3. Creates any missing subdirectories
4. Removes files and directories from the destination that don't exist in source
5. Logs all operations
6. Waits for the configured interval and repeats

## Building the Project
This is a .NET 8.0 console application. Build using Visual Studio or the .NET CLI:

```
dotnet build
```

## Example
```
FolderSynchronization C:\SourceFolder C:\Destination 60 C:\Logs
```
This will create a folder called "SourceFolder_copy" in C:\Destination and synchronize with C:\SourceFolder every 60 seconds. All operations will be logged to automatically generated files in C:\Logs directory (e.g., log_20250607.txt).

## Notes
- Pressing Ctrl+C will terminate the application
- The application will prompt to delete incomplete copies if synchronization fails
