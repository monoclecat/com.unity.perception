# Contribution Guidelines

Thank you for your interest in contributing to Perception! We are incredibly excited to see how members of our community will use and extend the Perception tools. To facilitate your contributions, we've outlined a brief set of guidelines to ensure that your extensions can be easily integrated.

## Communication

First, please read through our
[code of conduct](CODE_OF_CONDUCT.md),
as we expect all our contributors to follow it.

Second, before starting on a project that you intend to contribute to the Perception package we **strongly** recommend posting on our [Issues page](https://github.com/monoclecat/com.unity.perception/issues) and briefly outlining the changes you plan to make. This will enable us to provide
some context that may be helpful for you, including advice on how to optimally perform your changes or even reasons for not doing it.

Lastly, if you're looking for input on what to contribute, feel free to browse the GitHub issues.

## Git Branches

The `main` branch corresponds to the most recent version of the project. Note
that this may be newer than the
[latest release](https://github.com/monoclecat/com.unity.perception/releases/tag/latest_release).

When contributing to the project, please make sure that your Pull Request (PR)
contains the following:

- Detailed description of the changes performed
- Corresponding changes to documentation and unit tests
- Summary of the tests performed to validate your changes
- Issue numbers that the PR resolves (if any)

## Continuous Integration (CI)

Tests are not run as part of the CI, as this requires a costly Unity license. 
Still, all tests must be passing before the PR is merged. 

To run the tests locally:
* Open `Samples~/PerceptionHDRP` in Unity
* From the menu bar, open Window -> General -> Test Runner
* Select 'PlayMode'
* Click 'Run All'
* Check for failed tests
* When finished, select 'EditMode'
* Click 'Run All'
* Check for failed tests

## Local development
The repository includes a test project for local development located at `Samples~/PerceptionHDRP`.

### Suggested IDE Setup
For closest standards conformity and best experience overall, JetBrains Rider or Visual Studio w/f JetBrains Resharper are suggested. For optimal experience, allow navigating to code in all packages included in your project. To do so, in your Unity Editor, navigate to `Preferences` → `External Tools` and check `Generate all .csproj files.`
