# OPTEN Umbraco Localization

[![NuGet release](https://img.shields.io/nuget/v/Opten.Umbraco.Localization.svg)](https://www.nuget.org/packages/Opten.Umbraco.Localization/)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/backoffice-extensions/op10-localization-multilingual-properties/)

Multilingual properties with Umbraco

## Installation

### Nuget Package

[Complete Package](https://www.nuget.org/packages/Opten.Umbraco.Localization/): **Install-Package Opten.Umbraco.Localization**

[Binaries only](https://www.nuget.org/packages/Opten.Umbraco.Localization.Core/): **Install-Package Opten.Umbraco.Localization.Core**

### [Umbraco Package](https://our.umbraco.org/projects/backoffice-extensions/op10-localization-multilingual-properties/)

## Using with other Umbraco Packages

### Nested Content
> /App_Plugins/NestedContent/Views/nestedcontent.editor.html

``` HTML
<umb-property 
    property="property"
    ng-repeat="property in tab.properties"
    data-opten-language-show="property.alias"> <!-- <== this attribute here -->
    <umb-editor model="property"></umb-editor>
</umb-property>
```

### DocTypeGridEditor
> /App_Plugins/DocTypeGridEditor/Views/doctypegrideditor.dialog.html

``` HTML
<umb-property property="property"
    ng-repeat="property in tab.properties"
    data-opten-language-show="property.alias"> <!-- <== this attribute here -->
    <umb-editor model="property"></umb-editor> 
</umb-property>
```