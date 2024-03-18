using NUnit.Framework;
using UnityEngine;
using System.IO;
using LegendaryTools;

[TestFixture]
public class UnityFilePathTest
{
    [Test]
    public void ResolvesFileName_WithExtension()
    {
        UnityFilePath path = new UnityFilePath
        {
            RootPathType = UnityFilePathType.Data,
            FileName = "testFile",
            Extension = "txt"
        };

        Assert.AreEqual(Application.dataPath + "/testFile.txt", path.Path);
    }

    [Test]
    public void ResolvesFileName_DefaultUnnamed()
    {
        UnityFilePath path = new UnityFilePath
        {
            RootPathType = UnityFilePathType.Data,
            Extension = "txt"
        };

        Assert.AreEqual(Application.dataPath + "/UNNAMED_FILE.txt", path.Path);
    }

    [Test]
    public void FormatsPathWithForwardSlashes()
    {
        UnityFilePath path = new UnityFilePath
        {
            UseBackwardsSlash = false,
            RootPathType = UnityFilePathType.Data, // Assume returns "DataPath" for testing
            PostRootPath = "Folder",
            FileName = "file",
            Extension = "ext"
        };
        
        Assert.AreEqual(Application.dataPath + "/Folder/file.ext", path.Path);
    }

    [Test]
    public void FormatsPathWithBackwardSlashes()
    {
        UnityFilePath path = new UnityFilePath
        {
            UseBackwardsSlash = true,
            RootPathType = UnityFilePathType.Data, // Assume returns "DataPath" for testing
            PostRootPath = "Folder",
            FileName = "file",
            Extension = "ext"
        };
        
        Assert.AreEqual(Application.dataPath.Replace('/', '\\') + "\\Folder\\file.ext", path.Path);
    }
}