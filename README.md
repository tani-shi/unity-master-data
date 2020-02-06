# UnityMasterData

UnityMasterData is a tool that works just on Unity, to provide a workflow of operating master data for developing quickly/serviceability with using "**Excel**" for your project.

The diagram following here is described how works UnityMasterData.

![Diagram](https://github.com/tani-shi/unity-master-data/blob/master/Documents/How%20works%20UnityMasterData.png)

## Getting started

### Append lines in Package/manifest.json

```
{
  "dependencies": {
    "com.tani-shi.unity-excel": "https://github.com/tani-shi/unity-excel.git#1.0.0",
    "com.tani-shi.unity-master-data": "https://github.com/tani-shi/unity-master-data.git#1.0.0",
    ...
    }
}
```

## Ways to use

### 1. Create/Update excel files in your project.

The excel files to be master data must be format as following below.

||A|B...||
|---|---|---|---|
|1|Key Name *1|Field Name|__<= Not allow empty__|
|2|Comment|Comment|__<= Nullable__|
|3||Enum Definition *2|__<= Nullable__|
|4|Key Type *1|Field Type *2|__<= Not allow empty__|

- *1 : Commonly, the key name will be `id`, and also that type will be `int` or `uint`.
- *2 : If the enum definition is non-empty, that field type will be the enum name, then defines automatically in `MasterDataType.cs`.

The sample excel file is [here](https://github.com/tani-shi/unity-master-data/blob/master/Assets/UnityMasterDataDemo/Excels/Sample.xlsx?raw=true).

### 2. Generate/Update scripts to manipulate master data by MasterDataClassGenerator.

Invoke `MasterDataClassGenerator.GenerateAllDataScripts` from your project, then generate scripts.

```
- {0}/MasterData/DAO/Generated/{1}/{2}DAO.cs
- {0}/MasterData/DTO/Generated/{1}/{2}DTO.cs
- {0}/MasterData/VO/Generated/{1}/{2}VO.cs
- {0}/MasterData/Type/Generated/MasterDataType.cs
- {0}/MasterData/Collection/Generated/MasterDataAccessorObjectCollection.cs
- {0}/MasterData/Editor/Exporter/Generated/{1}Exporter.cs

{0} = The specified path you set.
{1} = A name of the specified excel file without the extension.
{2} = A name of the specified sheet of the specified excel file.
```

### 3. Export master data assets as ScriptableObject.

Invoke `MasterDataExporter.Export` from your project, then export assets.

```
- {0}/MasterData/{1}/{2}.asset

{0} = The specified path you set.
{1} = A name of the specified excel file without the extension.
{2} = A name of the specified sheet of the specified excel file.
```

### 4. Create a class to define how to load assets that is extended MasterDataManagerBase.

The sample script is [here](https://github.com/tani-shi/unity-master-data/blob/master/Assets/UnityMasterDataDemo/Scripts/DemoMasterDataManager.cs).

### 5. Implement for your project.

The sample script is [here](https://github.com/tani-shi/unity-master-data/blob/master/Assets/UnityMasterDataDemo/Scripts/DemoScene.cs).

## Contact developer

- [shintaro.tanikawa@gmail.com](mailto:shintaro.tanikawa@gmail.com)
