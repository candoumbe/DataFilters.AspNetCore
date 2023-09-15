# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### 🚨 Breaking changes

- Dropped `net5.0` support ([#119](https://github.com/candoumbe/DataFilters.AspNetCore/issues/119))

### 🧹 Housekeeping

- Replaced Moq with Nsubstitute ([#117](https://github.com/candoumbe/DataFilters.AspNetCore/issues/117))
- Fixed broken status badges ([#118](https://github.com/candoumbe/DataFilters.AspNetCore/issues/118))
- Updated CI/CD to use [Candoumbe.Pipelines](https://nuget.org/packages/Candoumbe.Pipelines)

## [0.4.0] / 2022-08-17
- Moved `MinimalAttribute` to a dedicated [`DataFilters.AspNetCore.Attributes`](https://nuget.org/packages/DataFilters.AspNetCore.Attributes) nuget package
- Added `IDataFilterService.Compute<T>(string, FilterOptions)` overload

## [0.3.0] / 2022-08-14
- Added `MinimalAttribute` to support `Prefer: return=minimal` HTTP header ([#47](https://github.com/candoumbe/DataFilters.AspNetCore/issues/47)) 

## [0.2.0] / 2022-03-29
- Bumped [`Candoumbe.MiscUtilities`](https://nuget.org/packages/Candoumbe.MiscUtilities) to [`0.6.3`](https://nuget.org/packages/Candoumbe.DataFilters/0.8.0)
- Bumped [`DataFilters`](https://nuget.org/packages/DataFilters) to [`0.11.0`](https://nuget.org/packages/Candoumbe/DataFilters/0.8.0)
- Dropped `FluentAssertions` dependency
- Improved overall documentation

## [0.1.0] / 2021-05-16
- Initial release

[Unreleased]: https://github.com/candoumbe/DataFilters.AspNetCore/compare/0.4.0...HEAD
[0.4.0]: https://github.com/candoumbe/DataFilters.AspNetCore/compare/0.3.0...0.4.0
[0.3.0]: https://github.com/candoumbe/DataFilters.AspNetCore/compare/0.2.0...0.3.0
[0.2.0]: https://github.com/candoumbe/DataFilters.AspNetCore/compare/0.1.0...0.2.0
[0.1.0]: https://github.com/candoumbe/DataFilters.AspNetCore/tree/0.1.0

