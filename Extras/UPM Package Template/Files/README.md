# Extension-Template
A template to create a new UPM package


## New Extensions Ground Rules



### Rules

Ensure compliance with the following rules:

1. **Namespace**:
   - All classes should be scoped under the namespace `Company.<my-extension-name>` or its sub-namespaces.

2. **Test Organization**:
   - Place all editor tests/folders within the `Tests/Editor` folder.
   - Place all runtime tests/folders within the `Tests/Runtime` folder.

3. **Folder Structure**:
   - The extension should be contained in a single folder, ideally including `Scripts`, `Editor`, `Tests/Editor`, and `Tests/Runtime`.


### Common Errors

#### Extension Naming
If for some reason, some of the naming in your extension isn't right, these are the steps that performs automatically:
   - Rename `Editor/Company.NewExtension.Editor.asmdef` to `Editor/Company.<my-extension-name>.Editor.asmdef`.
     In the asmdef, change the "name" variable from `Company.NewExtension.Editor` to `Company.<my-extension-name>.Editor`
   - Rename `Scripts/Company.NewExtension.asmdef` to `Scripts/Company.<my-extension-name>.asmdef`.
     In the asmdef, change the "name" variable from `Company.NewExtension` to `Company.<my-extension-name>`
   - Rename `Tests/Editor/Company.NewExtension.Editor.Tests.asmdef` to `Tests/Editor/Company.<my-extension-name>.Editor.Tests`.
     In the asmdef, change the "name" variable from `Company.NewExtension.Editor.Tests` to `Company.<my-extension-name>.Editor.Tests`
   - Rename `Tests/Runtime/Company.NewExtension.Tests.asmdef` to `Tests/Runtime/Company.<my-extension-name>.Tests`.
     In the asmdef, change the "name" variable from `Company.NewExtension.Tests` to `Company.<my-extension-name>.Tests`
