# Changelog
## [1.0.6]
### Fix
- Remove unnecessary frame, fixing Material Design In XAML demo

## [1.0.5]
### Change
- Now element whose XamlDisplayer.DisplayCode is false will have left alignment and more top margin

## [1.0.4]
### Added
- Added `IsControlPanelDisplayed` static property to XamlDisplayerPanel (which is set to false by default)  
  - So that client can choose whether to display the control panel or not 
- Added `DisplayCode` attached property for XamlDisplayer
  - So that client can control which element should display code

### Change
- Rename property `IsCodeDisplayed` to `IsCodeDisplayingPanelExpanded` (of XamlDisplayer)

## [1.0.3]
### Added
- Added control panel, which contain the following components
  - Search bar
  - Toggles (for toggling visibility of code displaying panel)
