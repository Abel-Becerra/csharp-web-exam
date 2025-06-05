# Requirements

## General

### Software concepts

- Functionality
- Reliability
- Usability
- Stability
- Efficiency
- Maintainability

### Architecture

- Transparency and coherence
- Clear route definitions
- Proper data design and structure
- Use of ORM for data access
- Use of MVC patterns
- Proper use of access modifiers (public, private, internal)

## Specific

### Solution

- **Exception handling**: Catch errors, handle unexpected conditions, and support graceful user interruptions.
- **Non-hardcoded code**: Use constants or configuration files instead of magic values.
- **Test use cases (or unit tests)**: Provide evidence of tests performed to ensure stability.
- **Logging**: Log relevant events during logic execution for debugging and traceability.
- **Security**: Properly scope access to attributes and methods.
- **Naming**: Consistent and meaningful naming throughout the codebase.

### Documentation

- **User**: Describe what the end user can do with the application.
- **Implementation**: Describe how the system is deployed and executed locally.
- **Code**: Document logic inside the code through structure, naming, and minimal comments when necessary.
- **Tests**: Describe the test coverage and expected input/output behavior.

### Technical

- [Apache Log4net](https://logging.apache.org/log4net/)

## Workflow

- At least 8 commits.
- Each commit must be atomic and focused on a single change.
- Each commit message must clearly describe the change made.
