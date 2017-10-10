# Changelog
## [1.0.4]
### Added
- Added `IsControlPanelDisplayed` static property to XamlDisplayerPanel (which is set to false by default)
-- So that client can choose whether to display the control panel or not 
- Added `DisplayCode` attached property for XamlDisplayer
-- So that client can control which element should display code

### Change
- Rename property `IsCodeDisplayed` to `IsCodeDisplayingPanelExpanded` (of XamlDisplayer)
