# Yaml Format

Tests are defined in YAML following the same guidelines as Power FX does. To find out more about the Power Fx YAML formula grammar, view [this](https://docs.microsoft.com/en-us/power-platform/power-fx/yaml-formula-grammar).

View the [samples](../../samples/) folder for detailed examples.

## YAML schema definition

| Property | Description |
| -- | -- |
| [test](./test.md) | Defines one test, the steps needed for the test and configuration specific to the test |
| [testSettings](./testSettings.md) | Defines settings for the test that are reused across multiple tests |
| [environmentVariables](./environmentVariables.md) | Defines variables that could potentially change as the app is ported across different environments |